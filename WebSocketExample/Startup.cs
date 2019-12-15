using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using System.Net.WebSockets;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Collections.Concurrent;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;
using System.ComponentModel.Composition;

namespace WebSocketExample
{
    public class Startup
    {

        public ConcurrentBag<WebSocket> BrowserClients { get; set; }

        public ConcurrentBag<ClientSocket> CurrentClients { get; set; }

        public string rootPath { get; set; }
        public AggregateCatalog Catalog { get; private set; }
        public CompositionContainer Container { get; private set; }

        [ImportMany]
        public ICollection<IWebAdapterAble> WebAdapters { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            this.WebAdapters = new List<IWebAdapterAble>();
            this.CurrentClients = new ConcurrentBag<ClientSocket>();
            this.BrowserClients = new ConcurrentBag<WebSocket>();
            this.rootPath = env.WebRootPath;
            WebfileFactory.IndexPath = Path.Combine(this.rootPath, "index2.html");
            WebfileFactory.ScriptPath = Path.Combine(this.rootPath, "script2.js");

            WebfileFactory.GenerateFiles(this.CurrentClients);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseFileServer(enableDirectoryBrowsing: true);
            app.UseWebSockets(); // Only for Kestrel

            app.Map("/ws", builder =>
            {
                builder.Use(async (context, next) =>
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        var client = new ClientSocket(webSocket, Guid.NewGuid().ToString());
                        this.CurrentClients.Add(client);
                        await this.ClientDevice(client);

                        // Device disconected -> remove and update
                        this.CurrentClients.TryTake(out client);
                        return;
                    }

                    await next();
                });
            });

            this.LoadAdapterAssemblies();
        }

        private async Task ClientDevice(ClientSocket clientSocket)
        {
            byte[] buffer = new byte[1024 * 4];
            var result = await clientSocket.Socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            var protoObj = new ProtocollObject(this.DecodeByteArray(buffer, result.Count));

            // Set serverside Identifier
            protoObj.Identifier = clientSocket.UniqueID;
            clientSocket.Name = protoObj.Name;

            IWebAdapterAble foundAdapter = null;

            foreach (var adapt in this.WebAdapters)
            {
                if (adapt.AdapterName == protoObj.Adapter)
                {
                    foundAdapter = adapt;
                    break;
                }
            }

            if (foundAdapter == null)
            {
                clientSocket.Socket.CloseAsync(WebSocketCloseStatus.ProtocolError, "No matching adapter found", CancellationToken.None).Wait(5000);
                return;
            }
            else
            {
                clientSocket.Adapter = foundAdapter;
            }

            WebfileFactory.GenerateFiles(this.CurrentClients);

            var message = protoObj.BuildProtocollMessage();

            // Inform device about its number
            await clientSocket.Socket.SendAsync(new ArraySegment<byte>(this.EncodeToByteArray(message), 0, message.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {

                result = await clientSocket.Socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                Console.WriteLine(this.DecodeByteArray(buffer, result.Count) + "++++++++++++++++++++++++++++++++++++++++++++");

                // Create new ProtocollObject from the clients message
                protoObj = new ProtocollObject(this.DecodeByteArray(buffer, result.Count));
                // Set serverside Identifier
                protoObj.Identifier = clientSocket.UniqueID;
                
                await this.DistributeToBrowserClients(protoObj).ConfigureAwait(false);

                //await clientSocket.Socket.SendAsync(new ArraySegment<byte>(this.EncodeToByteArray(message), 0, message.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);

            }

            await clientSocket.Socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }

        private async Task DistributeToBrowserClients(ProtocollObject protoObj)
        {
            ////await Task.Delay(5000);
            var message = protoObj.BuildProtocollMessage();

            foreach (var bc in this.BrowserClients)
            {
                await bc.SendAsync(new ArraySegment<byte>(this.EncodeToByteArray(message), 0, message.Length), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        private async Task BrowserDevices(WebSocket webSocket)
        {
            byte[] buffer = new byte[1024 * 4];
            WebSocketReceiveResult result;

            do
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                var protoObj = new ProtocollObject(this.DecodeByteArray(buffer, result.Count));
                
            }
            while (!result.CloseStatus.HasValue);

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }

        private async Task InformCorrespondingClient(ProtocollObject protoObj)
        {
            var message = protoObj.BuildProtocollMessage();

            foreach (var c in this.CurrentClients)
            {
                if (protoObj.Identifier == c.UniqueID)
                {
                    await c.Socket.SendAsync(new ArraySegment<byte>(this.EncodeToByteArray(message), 0, message.Length), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }


        private void LoadAdapterAssemblies()
        {
            this.Catalog = new AggregateCatalog();

            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            this.Catalog.Catalogs.Add(new DirectoryCatalog(path));

            this.Catalog.Catalogs.Add(new AssemblyCatalog(typeof(Startup).Assembly));

            path = Path.Combine(this.rootPath, "adapters");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            this.Catalog.Catalogs.Add(new DirectoryCatalog(path, "*.dll"));
            this.Catalog.Catalogs.Add(new DirectoryCatalog(path, "*.exe"));

            this.Container = new CompositionContainer(this.Catalog);
            this.Container.ComposeParts(this);
        }

        private string DecodeByteArray(byte[] input, int length)
        {
            return Encoding.UTF8.GetString(input, 0, length);
        }

        private byte[] EncodeToByteArray(string message)
        {
            return Encoding.UTF8.GetBytes(message);
        }
    }
}

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
using System.Linq;

namespace WebSocketExample
{
    public class Startup
    {

        public ConcurrentBag<WebSocket> BrowserClients { get; set; }

        public ConcurrentBag<WebSocket> PiplineSockets { get; set; }

        public ConcurrentBag<ClientSocket> CurrentClients { get; set; }

        public string rootPath { get; set; }
        public AggregateCatalog Catalog { get; private set; }
        public CompositionContainer Container { get; private set; }

        [ImportMany]
        public ICollection<IWebAdapterAble> WebAdapters { get; set; }
        public ConcurrentBag<Pipeline> Pipelines { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            this.PiplineSockets = new ConcurrentBag<WebSocket>();
            this.Pipelines = new ConcurrentBag<Pipeline>();
            this.WebAdapters = new List<IWebAdapterAble>();
            this.CurrentClients = new ConcurrentBag<ClientSocket>();
            this.BrowserClients = new ConcurrentBag<WebSocket>();
            this.rootPath = env.WebRootPath;
            WebfileFactory.IndexPath = Path.Combine(this.rootPath, "index.html");
            WebfileFactory.ScriptPath = Path.Combine(this.rootPath, "script.js");


            WebfileFactory.GenerateFiles(this.CurrentClients, this.Pipelines);

            var testcollection = new ConcurrentBag<object>();
            object obj = new object();
            testcollection.Add(obj);
            testcollection.Add(obj);
            testcollection.Add(obj);

            //Task.Run(() =>
            //{
            //    while (true)
            //    {
            //        foreach (var item in testcollection)
            //        {
            //            item.ToString();
            //        }
            //    }
            //});

            //Task.Run(() =>
            //{
            //    while (true)
            //    {
            //        Task.Delay(5000);
            //        testcollection.TryTake(out obj);
            //    }
            //});


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

                        var client = new ClientSocket(webSocket, Guid.NewGuid().ToString().Replace('-', 'x'));
                        this.CurrentClients.Add(client);
                        await this.ClientDevice(client);

                        // Device disconected -> remove and update
                        this.CurrentClients.TryTake(out client);

                        // Remove old DeviceRepresentation -> override old outdated index and script files
                        WebfileFactory.GenerateFiles(this.CurrentClients, this.Pipelines);
                        return;
                    }

                    await next();
                });
            });

            app.Map("/bs", builder =>
            {
                builder.Use(async (context, next) =>
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        this.BrowserClients.Add(webSocket);

                        //Task.Run( async () =>
                        //{
                        //    await Task.Delay(5000);
                        //    await webSocket.CloseAsync(WebSocketCloseStatus.Empty,"ForcesClose",CancellationToken.None);
                        //    this.BrowserClients.TryTake(out webSocket);
                        //});

                        await this.BrowserDevices(webSocket);

                        // Device disconected -> remove and update
                        this.BrowserClients.TryTake(out webSocket);
                        return;
                    }

                    await next();
                });
            });

            app.Map("/ps", builder =>
            {
                builder.Use(async (context, next) =>
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        this.PiplineSockets.Add(webSocket);
                        await this.ListenPipelines(webSocket);

                        // Device disconected -> remove and update
                        this.PiplineSockets.TryTake(out webSocket);
                        return;
                    }

                    await next();
                });
            });

            this.LoadAdapterAssemblies();
        }

        private void CheckForInactiveSockets(int duration, ClientSocket con)
        {
            Task.Run(() =>
            {
                while (true)
                {
                    Task.Delay(duration);

                    var currentTime = DateTime.UtcNow;
                    var oldConnections = new List<ClientSocket>();

                    foreach(var client in this.CurrentClients)
                    {
                        if ((currentTime - client.LastTimeStamp).TotalMinutes >= 1) //5.5
                        {
                            client.Socket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Closed for inactivity", CancellationToken.None);
                            //this.CurrentClients.TryTake(out cl);
                            this.CurrentClients = new ConcurrentBag<ClientSocket>(this.CurrentClients.Except(new[] { client }));

                            // Remove old DeviceRepresentation
                            WebfileFactory.GenerateFiles(this.CurrentClients, this.Pipelines);
                            //return;
                        }
                    }

                    return;
                }
            });
        }

        //private void CheckForInactiveSockets(int duration, ClientSocket con)
        //{
        //    Task.Run(() =>
        //    {
        //        while (true)
        //        {
        //            Task.Delay(duration);

        //            var currentTime = DateTime.UtcNow;
        //            var oldConnections = new List<ClientSocket>();


        //            if ((currentTime - con.LastTimeStamp).TotalMinutes > 5.5)
        //            {
        //                con.Socket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Closed for inactivity", CancellationToken.None);
        //                this.CurrentClients.TryTake(out con);

        //                // Remove old DeviceRepresentation
        //                WebfileFactory.GenerateFiles(this.CurrentClients, this.Pipelines);
        //                return;
        //            }
        //        }
        //    });
        //}

        private async Task ClientDevice(ClientSocket clientSocket)
        {
            byte[] buffer = new byte[1024 * 4];
            var result = await clientSocket.Socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            var protoObj = new ProtocollObject(this.DecodeByteArray(buffer, result.Count));

            
            // do only it is the reconnecting device
            //"identifier:" + identifier + ";adapter:"+adapter+";name:"+adapterName+";;reconnection:"+identifier+";";

            try
            {
                var clientToReplace = this.CurrentClients
                    .Where(client => client.LastProtoObj != null && (client.LastProtoObj.ParamObjects
                    .Where(paramObj => paramObj.ParamName == "reconnection")
                    .FirstOrDefault() != null))
                    .FirstOrDefault();
            
                if(clientToReplace != null)
                {
                    string id = clientToReplace.LastProtoObj.ParamObjects
                        .Where(a => a.ParamName == "reconnection")
                        .First().Value;

                    // security issue

                    if(protoObj.Identifier == id && protoObj.ParamObjects.Where(paramObj => paramObj.ParamName == "reconnection").FirstOrDefault() != null)
                    {
                        clientSocket.UniqueID = id;
                    }

                    //if (protoObj.ParamObjects.Where(paramObj => paramObj.ParamName == "reconnection").FirstOrDefault() != null)
                    //{
                        //clientSocket.UniqueID = id;
                    //}

                    try
                    {
                        // ClientSocket clientToRemove = this.CurrentClients.Where(a => a.LastProtoObj.ParamObjects.Where(a => a.ParamName == "deleteConn").FirstOrDefault() != null).FirstOrDefault();
                        this.CurrentClients = new ConcurrentBag<ClientSocket>(this.CurrentClients.Except(new[] { clientToReplace }));

                        WebfileFactory.GenerateFiles(this.CurrentClients, this.Pipelines);

                        this.CheckForInactiveSockets(5000, clientSocket);

                        await clientToReplace.Socket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Closed for inactivity", CancellationToken.None);
                    }
                    catch
                    {

                    }
                }

            }
            catch(Exception e)
            {
                //
            }

            // Set serverside Identifier
            protoObj.Identifier = clientSocket.UniqueID;
            clientSocket.Name = protoObj.Name;
            clientSocket.LastProtoObj = protoObj;

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

            WebfileFactory.GenerateFiles(this.CurrentClients, this.Pipelines);

            var message = protoObj.BuildProtocollMessage();

            // Inform device about its number
            await clientSocket.Socket.SendAsync(new ArraySegment<byte>(this.EncodeToByteArray(message), 0, message.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);

            //Start watcher
            this.CheckForInactiveSockets(5000, clientSocket);

            while (!result.CloseStatus.HasValue)
            {
                result = await clientSocket.Socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                // Refresh last recieved message
                clientSocket.LastTimeStamp = DateTime.UtcNow;
                //this.CheckForInactiveSockets(5000, clientSocket);

                Console.WriteLine(this.DecodeByteArray(buffer, result.Count) + "++++++++++++++++++++++++++++++++++++++++++++");

                // Create new ProtocollObject from the clients message
                protoObj = new ProtocollObject(this.DecodeByteArray(buffer, result.Count));

                // Set serverside Identifier
                protoObj.Identifier = clientSocket.UniqueID;
                clientSocket.LastProtoObj = protoObj;
                message = protoObj.BuildProtocollMessage();

                //try
                //{
                //    var objc = protoObj.ParamObjects.First(a => a.ParamName == "deleteConn");

                //    ClientSocket clientToRemove = this.CurrentClients.Where(a => a.LastProtoObj.ParamObjects.Where(a => a.ParamName == "deleteConn").FirstOrDefault() != null).FirstOrDefault();
                //    this.CurrentClients = new ConcurrentBag<ClientSocket>(this.CurrentClients.Except(new[] { clientToRemove }));

                //    WebfileFactory.GenerateFiles(this.CurrentClients, this.Pipelines);

                //    this.CheckForInactiveSockets(5000, clientSocket);

                //    await clientToRemove.Socket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Closed for inactivity", CancellationToken.None);
                //}
                //catch
                //{
                        //
                //}

                // Update value of webpage and inform pipeline targets
                await this.DistributeToBrowserClients(protoObj).ConfigureAwait(false);
                await this.UsePipelines(protoObj);

                await clientSocket.Socket.SendAsync(new ArraySegment<byte>(this.EncodeToByteArray(message), 0, message.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);
            }

            await clientSocket.Socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }

        private async Task UsePipelines(ProtocollObject protoObj)
        {
            foreach (Pipeline pipe in this.Pipelines)
            {
                if (pipe.FromId == protoObj.Identifier)
                {
                    foreach (var c in this.CurrentClients)
                    {
                        if (c.UniqueID == pipe.ToId)
                        {
                            var paramsToSend = protoObj.ParamObjects;
                            c.LastProtoObj.ParamObjects = paramsToSend;
                            var message = c.LastProtoObj.BuildProtocollMessage() + pipe.AdditionalParams;

                            await c.Socket.SendAsync(new ArraySegment<byte>(this.EncodeToByteArray(message), 0, message.Length), WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                    }
                }
            }
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

            foreach (var c in this.CurrentClients)
            {
                var message = c.LastProtoObj.BuildProtocollMessage();
                await webSocket.SendAsync(new ArraySegment<byte>(this.EncodeToByteArray(message), 0, message.Length), WebSocketMessageType.Text, true, CancellationToken.None);
            }

            do
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                var protoObj = new ProtocollObject(this.DecodeByteArray(buffer, result.Count));

                var message = protoObj.BuildProtocollMessage();
                //await clientSocket.Socket.SendAsync(new ArraySegment<byte>(this.EncodeToByteArray(message), 0, message.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);
                //await this.CurrentClients.Where(a => a.LastProtoObj == protoObj).First().Socket.SendAsync(new ArraySegment<byte>(this.EncodeToByteArray(message), 0, message.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);

                await this.InformCorrespondingClient(protoObj);
            }
            while (!result.CloseStatus.HasValue);

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }

        private async Task ListenPipelines(WebSocket webSocket)
        {
            byte[] buffer = new byte[1024 * 4];
            WebSocketReceiveResult result;

            //foreach (var p in this.Pipelines)
            //{
            //    var message = $"add:{p.FromId}-->{p.ToId}";
            //    await webSocket.SendAsync(new ArraySegment<byte>(this.EncodeToByteArray(message), 0, message.Length), WebSocketMessageType.Text, true, CancellationToken.None);
            //}

            do
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                var pipeObj = this.DecodeByteArray(buffer, result.Count).Split(";;");

                var pipeParts = pipeObj[0].Split(":");
                if (pipeParts.Length > 1)
                {
                    var ids = pipeParts[1].Split("-->");

                    if (ids.Length > 1)
                    {
                        var tempL = new List<ClientSocket>(this.CurrentClients);
                        if (tempL.Where(x => x.UniqueID == ids[0] || x.UniqueID == ids[1]).Count() == 2)
                        {
                            if (pipeParts[0] == "add")
                            {
                                var additionalParams = string.Empty;

                                if (pipeObj.Length > 1)
                                {
                                    additionalParams = pipeObj[1];
                                }

                                this.Pipelines.Add(new Pipeline(ids[0], ids[1], additionalParams));

                                WebfileFactory.GenerateFiles(this.CurrentClients, this.Pipelines);
                            }
                            if (pipeParts[0] == "delete")
                            {
                                Pipeline foundpipeline = null;
                                foreach (var p in this.Pipelines)
                                {
                                    if (p.FromId == ids[0] && p.ToId == ids[1])
                                    {
                                        foundpipeline = p;
                                        break;
                                    }
                                }
                                if (foundpipeline != null)
                                {
                                    //// Inform everyone and yourself to delete the pipeline
                                    //foreach (var ps in this.PiplineSockets)
                                    //{
                                    //    await ps.SendAsync(new ArraySegment<byte>(this.EncodeToByteArray(pipeObj), 0, pipeObj.Length), WebSocketMessageType.Text, true, CancellationToken.None);
                                    //}
                                    this.Pipelines.TryTake(out foundpipeline);

                                    WebfileFactory.GenerateFiles(this.CurrentClients, this.Pipelines);
                                }
                            }
                        }
                        else
                        {
                            var errormsg = "error:No matching ids found";
                            await webSocket.SendAsync(new ArraySegment<byte>(this.EncodeToByteArray(errormsg), 0, errormsg.Length), WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                    }
                }
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

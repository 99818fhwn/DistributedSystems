using System.ComponentModel.Composition;

namespace WebSocketExample
{
    [InheritedExport]
    public interface IWebAdapterAble
    {
        /// <summary>
        /// The name which will be case sensitive and should be all lowercase to be used for requesting a gven controller at the start of a new connection to the server.
        /// </summary>
        public string AdapterName { get; set; }

        /// <summary>
        /// MethodAll(inid){if(id == inid){getcode}} -> every adapter can be asure that the connection used is named "socket" and the message is in "msg" complete:
        /// </summary>
        /// <param name="identifier">The identifier wich will make every element and the functionname distinct.</param>
        /// <returns></returns>
        string GenerateScriptcodeGet(string identifier);

        /// <summary>
        /// The code that sends the text message to the server with all package data information. 
        /// </summary>
        /// <param name="identifier">The identifier wich will make every element and the functionname distinct.</param>
        /// <returns></returns>
        string GenerateScriptCodeSet(string identifier);

        /// <summary>
        /// The html code that will be visible on the index page if a divice hoocks up with the adapter.
        /// </summary>
        /// <param name="identifier">The identifier which will make every element and the functionname distinct, in this case every id attibute or used function and so on.</param>
        /// <returns></returns>
        string GenerateHTMLCode(string identifier);
    }
}

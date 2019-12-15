using System.ComponentModel.Composition;

namespace WebSocketExample
{
    [InheritedExport]
    public interface IWebAdapterAble
    {

        public string AdapterName { get; set; }

        /// <summary>
        /// MethodAll(inid){if(id == inid){getcode}} -> every adapter can be asure that the connection used is named "socket"
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        string GenerateScripcodeGet(string identifier);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        string GenerateScripCodeSet(string identifier);

        string GenerateHTMLCode(string identifier);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebSocketExample
{
    public static class StringManipulation
    {
        public static string[] Partioning(string message, bool header)
        {
            var msgMainParts = message.Trim().Split(";;");

            if (header)
            {
                return message.Trim().Split(";;")[0].Split(";");
            }
            else
            {
                if (msgMainParts.Length > 1)
                {
                    return msgMainParts[1].Split(";");
                }
                else
                {
                    return null;
                }
            }
        }

        internal static string GetParameter(string msgPart)
        {
            return msgPart.Split(":").FirstOrDefault();
        }

        internal static string GetValue(string msgPart)
        {
            var parts = msgPart.Split(":");
            if (parts.Length > 1)
            {
                return parts[1];
            }
            else
            {
                return null;
            }
        }

        public static string GetName(string message)
        {
            var possibleMsg = Partioning(message, true).Where(x => x.Contains("name:")).FirstOrDefault();
            if (possibleMsg != null)
            {
                var posibleId = possibleMsg.Split(":");
                return posibleId.Length > 1 ? posibleId[1] : null;
            }
            else
            {
                return null;
            }
        }

        public static string GetIdentifier(string message)
        {
            var possibleMsg = Partioning(message, true).Where(x => x.Contains("identifier:")).FirstOrDefault();
            if (possibleMsg != null)
            {
                var posibleId = possibleMsg.Split(":");
                return posibleId.Length > 1 ? posibleId[1] : null;
            }
            else
            {
                return null;
            }
        }

        public static string GetAdapter(string message)
        {
            var possibleMsg = Partioning(message, true).Where(x => x.Contains("adapter:")).FirstOrDefault();
            if (possibleMsg != null)
            {
                var posibleId = possibleMsg.Split(":");
                return posibleId.Length > 1 ? posibleId[1] : null;
            }
            else
            {
                return null;
            }
        }
    }
}

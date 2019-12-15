namespace WebSocketExample
{
    public class ParamObject
    {
        public string ParamName { get; set; }

        public string Value { get; set; }

        public ParamObject(string msgPart)
        {
            this.ParamName = StringManipulation.GetParameter(msgPart);
            this.Value = StringManipulation.GetValue(msgPart);
        }
    }
}
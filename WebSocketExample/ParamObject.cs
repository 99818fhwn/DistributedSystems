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

        public ParamObject(string paramName, string value)
        {
            this.Value = value;
            this.ParamName = paramName;
        }
    }
}
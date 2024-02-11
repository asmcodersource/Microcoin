﻿namespace NodeNet.Message
{
    // Mean one full received message
    // Can be broadcast, or personal
    [Serializable]
    internal class Message
    {
        public MessageInfo Info { get; protected set; }
        public String Data { get; protected set; } = "";
        public string MessageSign { get; protected set; } = "";
        public int TimeToLive { get; protected set; } = 128;

        public Message(MessageInfo info, string data, string messageSign = "")
        {
            Info = info;
            Data = data;
            MessageSign = messageSign;
        }

        public void SetMessageSign(String sign)
        {
            // should be valid...
            MessageSign = sign;
        }
    }
}

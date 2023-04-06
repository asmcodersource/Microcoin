using Microcoin.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Microcoin.Data
{
    [Serializable]
    public class Message: ISignable
    {
        public DateTime SendingTime { get; set; }
        public MessageType MessageType { get; set; }
        public string ReceiverPublicKey { get; set; }
        public string SenderPublicKey { get; set; }
        public string Signature { get; set; }
        public object MessageObject { get; set; }

        public byte[] Serialize()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Message));
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Serialize(stream, this);
                return stream.ToArray();
            }
        }

        public static Message Deserialize(byte[] packetBytes)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Message));
            using (MemoryStream stream = new MemoryStream(packetBytes))
            {
                object obj = serializer.Deserialize(stream);
                return (Message)obj;
            }
        }
    }

    [Serializable]
    public enum MessageType
    {
        NopeMessage,
        NewBlockMined,
        CreateNewTransaction
    }
}

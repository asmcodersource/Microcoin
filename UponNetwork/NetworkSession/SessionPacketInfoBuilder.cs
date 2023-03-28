using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using TcpNetwork;

namespace UponNetwork.NetworkSession
{
    public class SessionPacketInfoBuilder : ITcpPacketInfoBuilder
    {

        public SessionPacketInfoBuilder()
        {
            PacketInfoSize = SessionPacketInfo.PacketInfoSize;
        }

        public override ITcpPacketInfo DeserializePacketInfo(byte[] bytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(bytes, 0, bytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            return (SessionPacketInfo)binForm.Deserialize(memStream);
        }

        public override ITcpPacketInfo CreatePacketInfo()
        {
            return new SessionPacketInfo();
        }
    }
}

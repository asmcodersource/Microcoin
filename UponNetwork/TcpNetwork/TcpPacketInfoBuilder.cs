using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using TcpNetwork;

namespace TcpNetwork
{
    public class ITcpPacketInfoBuilder
    {
        public long PacketInfoSize { get; set; }

        public virtual ITcpPacketInfo CreatePacketInfo()
        {
            return null;
        }

        public virtual ITcpPacketInfo DeserializePacketInfo(byte[] bytes) { 
            return null; 
        }
    }

    public class TcpPacketInfoBuilder: ITcpPacketInfoBuilder
    {

        public TcpPacketInfoBuilder() {
            PacketInfoSize = TcpPacketInfo.PacketInfoSize;
        }

        public override ITcpPacketInfo DeserializePacketInfo(byte[] bytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(bytes, 0, bytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            return (TcpPacketInfo)binForm.Deserialize(memStream);
        }

        public override ITcpPacketInfo CreatePacketInfo()
        {
            return new TcpPacketInfo();
        }
    }
}

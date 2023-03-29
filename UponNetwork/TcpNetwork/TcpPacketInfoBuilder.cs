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

        public virtual Task<ITcpPacketInfo> DeserializePacketInfo(Stream stream) { 
            return null; 
        }
    }

    public class TcpPacketInfoBuilder: ITcpPacketInfoBuilder
    {

        public TcpPacketInfoBuilder() {
            PacketInfoSize = TcpPacketInfo.PacketInfoSize;
        }

        public async override Task<ITcpPacketInfo> DeserializePacketInfo(Stream stream)
        {
            BinaryFormatter binForm = new BinaryFormatter();
            return (TcpPacketInfo)binForm.Deserialize(stream);
        }

        public override ITcpPacketInfo CreatePacketInfo()
        {
            return new TcpPacketInfo();
        }
    }
}

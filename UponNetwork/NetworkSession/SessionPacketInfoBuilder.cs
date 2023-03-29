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

        public async override Task<ITcpPacketInfo> DeserializePacketInfo(Stream stream)
        {
            await Task.Yield();
            BinaryFormatter binForm = new BinaryFormatter();
            return (SessionPacketInfo)binForm.Deserialize(stream);
        }

        public override ITcpPacketInfo CreatePacketInfo()
        {
            return new SessionPacketInfo();
        }
    }
}

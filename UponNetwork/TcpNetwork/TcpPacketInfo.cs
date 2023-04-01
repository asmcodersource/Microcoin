using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization.Formatters.Binary;

namespace TcpNetwork
{
    public record ReceivedPacket
    {
        public ITcpPacketInfo? Info { get; set; }
        public byte[]? Data { get; set; }
    }


    public interface ITcpPacketInfo
    {
        public long PacketSize { get; set; }
        public int GetHashCode();
    }


    [Serializable]
    public class TcpPacketInfo: ITcpPacketInfo
    {
        public long PacketSize { get; set; }
        public static long PacketInfoSize { get; protected set; } 
        
       static TcpPacketInfo()
       {
            TcpPacketInfo tcpPacketInfo = new TcpPacketInfo();

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, tcpPacketInfo);

            PacketInfoSize = ms.ToArray().Length;
        }
        
    }
}

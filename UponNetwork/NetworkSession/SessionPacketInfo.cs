using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using TcpNetwork;

namespace UponNetwork.NetworkSession
{

    [Serializable]
    public class SessionPacketInfo : ITcpPacketInfo
    {
        public long PacketSize { get; set; }
        public static long PacketInfoSize { get; protected set; }
        public int NodePathLength = 0;
        public int MaximumNodePathLength = 128;
        public byte[] MessageSender = new byte[64];
        public byte[] MessageSign = new byte[64];


        public SessionPacketInfo( int MaximumNodePathLength = 128 ) 
        { 
            this.NodePathLength = MaximumNodePathLength;
        }

        static SessionPacketInfo()
        {
            ITcpPacketInfo tcpPacketInfo = new SessionPacketInfo();

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, tcpPacketInfo);

            PacketInfoSize = ms.ToArray().Length;
        }
    }
}

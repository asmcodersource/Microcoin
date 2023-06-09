﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using TcpNetwork;

using System.Security.Cryptography;
using System.Net;

namespace Microcoin.UponNetwork.NetworkSession
{

    [Serializable]
    public class SessionPacketInfo : ITcpPacketInfo
    {
        public static long PacketInfoSize { get; protected set; }
        public long PacketSize { get; set; }
        public byte[] MessageSign { get; set; }
        public string MessageSenderPublicKey { get; set; }
        public bool IsTehnicalPacket { get; set; }
        public int PeersToPass { get; set; }


        static SessionPacketInfo()
        {
            ITcpPacketInfo tcpPacketInfo = new SessionPacketInfo();

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, tcpPacketInfo);

            PacketInfoSize = ms.ToArray().Length;
        }


        public override int GetHashCode()
        {
            return HashCode.Combine(MessageSenderPublicKey, PacketSize, Convert.ToBase64String(MessageSign));
        }
    }
}

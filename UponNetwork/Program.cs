using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TcpNetwork;
using UponNetwork.NetworkSession;
using System.Threading;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using UponNetwork.NetworkServer;


// Create server side of node
NodeServer nodeServer = new NodeServer();
await nodeServer.StartSpecificListener("127.0.0.1", 1300);

// Create client side of node
TcpConnection connection = new TcpConnection();
connection.PacketInfoBuilder = new SessionPacketInfoBuilder();
await connection.ConnectToAsync("127.0.0.1", 1300);
await connection.VerifyConnection();

// Send some type of data to server side
string msg = "Hello world!";
SessionPacketInfo packetInfo = new SessionPacketInfo();
connection.SendDataPacket(Encoding.UTF8.GetBytes(msg), packetInfo);
connection.SendDataPacket(Encoding.UTF8.GetBytes(msg), packetInfo);
connection.SendDataPacket(Encoding.UTF8.GetBytes(msg), packetInfo);

// Await for press any key
Console.ReadKey();

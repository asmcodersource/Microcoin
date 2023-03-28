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

ITcpServer tcpServer = new TcpServer();
tcpServer.PacketInfoBuilder = new SessionPacketInfoBuilder();
tcpServer.ConnectionBuilder = new TcpConnectionBuilder();
tcpServer.NewConnectionAccepted += (object sender, ITcpConnection connection) => Console.WriteLine("New connection accepted!");
tcpServer.NewConnectionRejected += (object sender, ITcpConnection connection) => Console.WriteLine("New connection rejected!");
tcpServer.SetupListening("127.0.0.1", 1300);
var listenTaks = tcpServer.StartListeningAsync();


ITcpConnection tcpClient = new TcpConnection();
tcpClient.PacketInfoBuilder = new SessionPacketInfoBuilder();
await tcpClient.ConnectToAsync("127.0.0.1", 1300);
await tcpClient.VerifyConnection();


string msg = "Hello world!";
var packetInfo = new SessionPacketInfo();
packetInfo.MessageSender[0] = 0xCC;
tcpClient.SendDataPacket(Encoding.UTF8.GetBytes(msg), packetInfo);

foreach(var connection in tcpServer.Connections)
{
    await Task.Delay(100);
    var packet = connection.ReceiveDataPacket();
    Console.WriteLine(Encoding.UTF8.GetString(packet?.Data));
}

Console.ReadKey();

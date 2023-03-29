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


Node node = new Node();
Console.Write("Initialize node keys...");
node.PrepareNodeCrypto();
Console.WriteLine("Ok");
Console.Write("Starting node server...");
node.PrepareNodeServer();
Console.WriteLine("Ok");

// Await for press any key
Console.ReadKey();

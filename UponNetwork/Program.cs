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


var node1 = CreateNewNode(1300);
Console.WriteLine("______________________________");
var node2 = CreateNewNode(1301);
Console.WriteLine("______________________________");

var success = await node2.ConnectToNode("127.0.0.1", 1300);
Console.WriteLine("Connect node2 to node1...{0}", success ? "Ok" : "Fail");
node2.SendMessage(Encoding.UTF8.GetBytes("HELLO WORLD!"));

Console.WriteLine("______________________________");

// Await for press any key
Console.ReadKey();


static Node CreateNewNode(int port)
{
    Node node = new Node();
    Console.Write("Initialize node keys...");
    node.PrepareNodeCrypto();
    Console.WriteLine("Ok");
    Console.Write("Starting node server...");
    node.PrepareNodeServer(port);
    Console.WriteLine("Ok");
    return node;
}
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
using UponNetwork.NetworkNode;
using Microcoin.Settings;

class Program
{
    static Node? node;
    static object locked = new object();

    static async Task Main(string[] args)
    {
        var settings = LoadSettingsFromFile("Settings.xml");
        node = CreateNewNode(settings.PeerNetworkSettings.ListeningPort, settings.PeerNetworkSettings.PeerKeysFileName);
        node.NodeReceivedTechnicalMessage += AnyMessageHandler;
        node.NodeReceivedMessage += AnyMessageHandler;
        await node.CreateSessionsByPeersStorage();
        await node.NodeDiscovery.SendDiscoveryRequest();
        node.NodeDiscovery.StartBeacon();

        await node.ConnectToNode("192.168.0.101", 1300);
        await node.NodeDiscovery.SendDiscoveryRequest();

        await Task.Delay(-1);
    }


    static void AnyMessageHandler(object sender, ReceivedPacket packet)
    {
        lock (locked)
        {
            SessionPacketInfo sessionPacketInfo = (SessionPacketInfo)packet.Info;
            Console.WriteLine($"\nNew message received from peer network");
            Console.WriteLine($"Peer: {sessionPacketInfo.MessageSenderPublicKey}");
            Console.WriteLine($"Message size: {sessionPacketInfo.PacketSize}");
            Console.WriteLine($"Technical: {sessionPacketInfo.IsTehnicalPacket.ToString()}");
            Console.WriteLine($"Date: {DateTime.UtcNow.ToString()}");
            Console.WriteLine(Encoding.UTF8.GetString(packet.Data));
        }
    }


    public static Settings LoadSettingsFromFile(string filePath)
    {
        Settings settings;
        var dir = System.AppDomain.CurrentDomain.BaseDirectory;
        string fullFilePath = Path.Combine(dir, filePath);
        if(!File.Exists(fullFilePath))
        {
            settings = new Settings();
            settings.SaveSettingsToFile(fullFilePath);
            return settings;
        } else
        {
            settings = Settings.LoadSettingsFromFile(fullFilePath);
            return settings;
        }  
    }

    static Node CreateNewNode(int port, string keyFileName)
    {
        Node node = new Node();
        Console.Write("Initialize node keys...");
        node.PrepareNodeCrypto(keyFileName + ".keys");
        Console.WriteLine("Ok");
        Console.Write("Starting node server...");
        node.PrepareNodeServer(port);
        node.LoadPeersFromFile(keyFileName + ".peers");
        Console.WriteLine("Ok");
        return node;
    }
}
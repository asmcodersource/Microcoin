using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TcpNetwork;
using System.Threading;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using Microcoin.Settings;
using Microcoin.UponNetwork.NetworkNode;
using Microcoin.UponNetwork.NetworkSession;

class Program
{
    static async Task Main(string[] args)
    {

        var peer = new Microcoin.Peer.Peer();
        var settings = Settings.LoadOrCreateSettingsFile("Settings.xml");
        await peer.InitializeNode(settings);
        await peer.Node.ConnectToNode("192.168.0.101", settings.PeerNetworkSettings.ListeningPort);
        Console.WriteLine("Peer created");

        while (true)
        {
            Microcoin.Peer.Message message = new Microcoin.Peer.Message();
            message.MessageType = Microcoin.Peer.MessageType.NopeMessage;
            message.SendingTime = DateTime.UtcNow;
            message.ReceiverPublicKey = null;

            peer?.Node?.SendMessage(message.Serialize());
            await Task.Delay(2000);
        }
    }
}
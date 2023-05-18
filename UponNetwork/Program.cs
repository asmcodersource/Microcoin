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
using Microcoin.Crypto;
using Microcoin.Data;

class Program
{
    static async Task Main(string[] args)
    {
        var settings = Settings.LoadOrCreateSettingsFile("Settings.xml");

        var peer = new Microcoin.Peer.Peer();
        await peer.InitializePeer(settings);
        await peer.Node.ConnectToNode("192.168.0.101", settings.PeerNetworkSettings.ListeningPort);
        await Task.Delay(1000);

        await Task.Run(() =>
        {
            while (true)
            {
                Message echo = new Microcoin.Data.Message();
                echo.MessageType = MessageType.NopeMessage;
                echo.SendingTime = DateTime.UtcNow;


                peer?.Node?.SendMessage(echo.Serialize());
                peer?.SendCoins((decimal)0.01, "sdasdasd");
                Thread.Sleep(5000);
            }
        });

        await Task.Delay(-1);
    }
}
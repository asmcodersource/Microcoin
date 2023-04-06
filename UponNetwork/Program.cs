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
using Microcoin.Data.Transaction;

class Program
{
    static async Task Main(string[] args)
    {

        CryptoKeys keys = new CryptoKeys();
        keys.CreateKeys();
        ISigner signer = new Signer();
        signer.SetKeys(keys);
        IVerifier verifier = new Verifier();
        verifier.SetKeys(keys);

        Transaction transaction = new Transaction();
        transaction.CoinsToSend = 10;
        transaction.SenderWallet = keys.PublicKeyXml;
        transaction.ReceiverWallet = "asdsad";

        signer.Sign(transaction);
        bool succesful = verifier.Verify(transaction);
        Console.WriteLine("Verify return = {0}", succesful ? "True" : "False");


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
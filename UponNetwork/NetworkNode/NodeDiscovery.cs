using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using TcpNetwork;
using UponNetwork.NetworkSession;
using System;
using System.Timers;

namespace UponNetwork.NetworkNode
{
    [Serializable]
    public class DiscoveryMessage
    {
        public string Address { get; set; }
        public int Port { get; set; }
        string? AdditionalMessage { get; set; }
    }

    public class NodeDiscovery
    {
        Node node;
        DiscoveryMessage discoveryRequest;
        protected System.Timers.Timer beaconTimer;
        string requestMessage;

        public NodeDiscovery(Node node)
        {
            this.node = node;
            node.NodeServer.SessionReceivedTechnicalMessage += TechnicalMessageHandler;
            ComputeRequestString();
        }

        public async Task ComputeRequestString()
        {
            string ip = await GetExternalIPAddress();
            if (ip == null)
                throw new ApplicationException("Cannot access site for IP define");

            discoveryRequest = new DiscoveryMessage();
            discoveryRequest.Address = ip;
            discoveryRequest.Port = node.ListeningPort;

            DiscoveryMessage discoveryMessage = discoveryRequest;
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(DiscoveryMessage));
            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, discoveryMessage);
                requestMessage = textWriter.ToString();
            }
        }

        public async void SendDiscoveryRequest()
        {
            if (requestMessage == null)
                await ComputeRequestString();
            node.SendMessage(Encoding.UTF8.GetBytes(requestMessage), true);
        }

        public async Task SendDiscoveryRequest(NodeSession session)
        {
            if (requestMessage == null)
                await ComputeRequestString();
            session.SendMessage(Encoding.UTF8.GetBytes(requestMessage), null, true, 1);
        }

        public async void TechnicalMessageHandler(object sender, ReceivedPacket packet)
        {
            DiscoveryMessage discoveryMessage;
            Peer peer;

            try
            {
                var message = Encoding.UTF8.GetString(packet.Data);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(DiscoveryMessage));
                discoveryMessage = (DiscoveryMessage)xmlSerializer.Deserialize(new StringReader(message));
                peer = new Peer(discoveryMessage.Address, discoveryMessage.Port);
            }
            catch
            {
                // Some errors during deserialization
                return;
            }


            TcpConnection connection = new TcpConnection();
            connection.PacketInfoBuilder = new SessionPacketInfoBuilder();
            var success = await connection.ConnectToAsync(discoveryMessage.Address, discoveryMessage.Port);
            if (success)
                success &= await connection.VerifyConnection();

            if (success)
            {
                node.NodePeersStorage?.AddPeer(peer);
                NodeSession session = new NodeSession(connection, node.NodeServer);
                SendDiscoveryRequest(session);
            }
            connection.DropConnection();
        }


        public void StartBeacon()
        {
            beaconTimer = new System.Timers.Timer();
            beaconTimer.Interval = 5 * 60 * 1000;
            beaconTimer.Elapsed += (Object source, System.Timers.ElapsedEventArgs e) => SendDiscoveryRequest();
            beaconTimer.AutoReset = true;
            beaconTimer.Enabled = true;
            beaconTimer.Start();
        }

        public void StopBeacon()
        {
            beaconTimer.Stop();
        }

        protected static async Task<string> GetExternalIPAddress()
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync("https://api.ipify.org");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
            }

            return null;
        }
    }
}

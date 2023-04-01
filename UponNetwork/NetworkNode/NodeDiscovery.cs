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

        public NodeDiscovery(Node node)
        {
            this.node = node;
            node.NodeServer.SessionReceivedTechnicalMessage += TechnicalMessageHandler;
           
        }

        public async void SendDiscoveryRequest()
        {
            if(discoveryRequest == null )
            {
                string ip = await GetExternalIPAddress();
                if (ip == null)
                    throw new ApplicationException("Cannot access site for IP define");

                discoveryRequest = new DiscoveryMessage();
                discoveryRequest.Address = ip;
                discoveryRequest.Port = node.ListeningPort;

            }

            DiscoveryMessage discoveryMessage = discoveryRequest;
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(DiscoveryMessage));
            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, discoveryMessage);
                string message = textWriter.ToString();
                node.SendMessage(Encoding.UTF8.GetBytes(message), true);
            }
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
            } catch
            {
                // Some errors during deserialization
                return;
            }


            TcpConnection connection = new TcpConnection();
            connection.PacketInfoBuilder = new SessionPacketInfoBuilder();
            var success = await connection.ConnectToAsync(discoveryMessage.Address, discoveryMessage.Port);
            if (success)
                success |= await connection.VerifyConnection();
            connection.DropConnection();

            if (success)
                node.NodePeersStorage?.AddPeer(peer);
            
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

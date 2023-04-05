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
using Microcoin.UponNetwork.NetworkSession;
using System;
using System.Timers;
using Microcoin.UponNetwork.NetworkSession;

namespace Microcoin.UponNetwork.NetworkNode
{
    [Serializable]
    public class DiscoveryMessage
    {
        public DateTime requestTime;
        public string Address { get; set; }
        public int Port { get; set; }
        public string? AdditionalMessage { get; set; }

        public DiscoveryMessage()
        {
            requestTime = DateTime.UtcNow;
        }

        public DiscoveryMessage(DateTime requestTime, string address, int port, string? additionalMessage)
        {
            this.requestTime = requestTime;
            Address = address;
            Port = port;
            AdditionalMessage = additionalMessage;
        }

        public DiscoveryMessage(string address, int port, string? additionalMessage)
        {
            requestTime = DateTime.UtcNow;
            Address = address;
            Port = port;
            AdditionalMessage = additionalMessage;
        }
    }

    internal class NodeDiscoveryIntervalPID
    {
        public double Interval { get; protected set; }
        public int TargetTrafficCount { get; set; }
        protected int TrafficCount { get; set; }
        protected System.Timers.Timer PIDTimer { get; set; }

        public NodeDiscoveryIntervalPID(int targetTrafficPerMinute)
        {
            Interval = 1000;
            TargetTrafficCount = targetTrafficPerMinute;
        }

        public void Start()
        {
            PIDTimer = new System.Timers.Timer();
            PIDTimer.Interval = 10 * 1000;
            PIDTimer.Elapsed += RecalculateInterval;
            PIDTimer.AutoReset = true;
            PIDTimer.Enabled = true;
            PIDTimer.Start();
        }

        public void Stop()
        {
            PIDTimer.Stop();
        }

        public void TrafficIncreaseCount()
        {
            TrafficCount++;
        }

        protected void RecalculateInterval(object source, ElapsedEventArgs e)
        {
            double difference = TargetTrafficCount - TrafficCount * 6;
            Interval -= difference * 0.25;
            TrafficCount = 0;
            if (Interval < 400)
                Interval = 400;
            InvervalChanged?.Invoke(this, (int)Interval);
        }

        public event EventHandler<int> InvervalChanged;
    }

    public class NodeDiscovery
    {
        NodeDiscoveryIntervalPID nodeDiscoveryIntervalPID;
        Node node;
        DiscoveryMessage discoveryRequest;
        protected System.Timers.Timer beaconTimer;
        string requestMessage;
        string externalIP;

        public NodeDiscovery(Node node)
        {
            nodeDiscoveryIntervalPID = new NodeDiscoveryIntervalPID(60);
            this.node = node;
            node.NodeServer.SessionReceivedTechnicalMessage += TechnicalMessageHandler;
            ComputeRequestString();
        }

        public async Task ComputeRequestString()
        {
            if (externalIP == null)
                externalIP = await GetExternalIPAddress();
            if (externalIP == null)
                throw new ApplicationException("Cannot access site for IP define");


            DiscoveryMessage discoveryMessage = new DiscoveryMessage();
            discoveryMessage.Address = externalIP;
            discoveryMessage.Port = node.ListeningPort;
            discoveryMessage.AdditionalMessage = "Interval time = " + nodeDiscoveryIntervalPID.Interval.ToString();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(DiscoveryMessage));
            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, discoveryMessage);
                requestMessage = textWriter.ToString();
            }
        }

        public async Task SendDiscoveryRequest()
        {
            await ComputeRequestString();
            node.SendMessage(Encoding.UTF8.GetBytes(requestMessage), true);
        }

        public async Task SendDiscoveryRequest(NodeSession session)
        {
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

            nodeDiscoveryIntervalPID.TrafficIncreaseCount();
            if (node.NodePeersStorage.Peers.Contains(peer))
                return;

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
            beaconTimer.Interval = 1 * 1000;
            beaconTimer.Elapsed += (source, e) => SendDiscoveryRequest();
            beaconTimer.AutoReset = true;
            beaconTimer.Enabled = true;
            beaconTimer.Start();

            nodeDiscoveryIntervalPID.InvervalChanged += (sender, newInterval) => beaconTimer.Interval = newInterval;
            nodeDiscoveryIntervalPID.Start();
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

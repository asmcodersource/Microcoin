using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace UponNetwork.NetworkNode
{
    [Serializable]
    public class Peer
    {
        public string Address;
        public int Port;
        public DateTime LastOnlineTime { get; protected set; }

        public Peer(string address, int port)
        {
            this.Address = address;
            this.Port = port;
            this.LastOnlineTime = DateTime.UtcNow;
        }

        public Peer(string address, int port, DateTime lastOnlineTime )
        {
            this.Address = address;
            this.Port = port;
            this.LastOnlineTime = lastOnlineTime;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Address, Port);
        }
    }

    [Serializable]
    public class PeerComparer : IEqualityComparer<Peer>
    {
        bool IEqualityComparer<Peer>.Equals(Peer? x, Peer? y)
        {
            return (x.Address == y.Address && x.Port == y.Port);
        }

        int IEqualityComparer<Peer>.GetHashCode(Peer obj)
        {
            return obj.GetHashCode();
        }
    }

    public class NodePeersStorage
    {
        protected string peersStorageFileName;
        public DateTime LastUpdateTime { get; protected set; }
        public HashSet<Peer> Peers { get; protected set; }

        public NodePeersStorage()
        {
            LastUpdateTime = DateTime.UtcNow;
            Peers = new HashSet<Peer>(new PeerComparer());
        }

        public void SavePeers(string filePath)
        {
            LastUpdateTime = DateTime.UtcNow;
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                formatter.Serialize(fs, LastUpdateTime);
                formatter.Serialize(fs, Peers);
            }
        }

        public void LoadPeers(string filePath)
        {
            if (!File.Exists(filePath))
                throw new ApplicationException($"Cant load peers, file {filePath} not exists");

            this.peersStorageFileName = filePath;
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                LastUpdateTime = (DateTime)formatter.Deserialize(fs);
                Peers = new HashSet<Peer>((HashSet<Peer>)formatter.Deserialize(fs), new PeerComparer());
            }
        }

        public void AddPeer(Peer peer)
        {
            Peer? storedPeer = null;
            if (!Peers.TryGetValue(peer, out storedPeer))
            {
                Peers.Add(peer);
                SavePeers(peersStorageFileName);
                return;
            }

            Peer peerToStore = peer.LastOnlineTime > storedPeer.LastOnlineTime ? peer : null;
            if (peerToStore == null)
                return;
            Peers.Remove(storedPeer);
            Peers.Add(peerToStore);
            SavePeers(peersStorageFileName);
        }

        public void RemovePeer(Peer peer)
        {
            Peers.Remove(peer);
        }
    }
}

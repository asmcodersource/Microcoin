using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

using TcpNetwork;
using UponNetwork.NetworkSession;
using System.Net;

namespace UponNetwork.NetworkServer
{    
    public class Node
    {
        public NodeCrypto? NodeCrypto { get; protected set; }
        public NodeServer? NodeServer { get; protected set; }


        public void PrepareNodeCrypto()
        {
            var dir = System.AppDomain.CurrentDomain.BaseDirectory;
            System.IO.Directory.CreateDirectory(dir + "/keys");
            if( File.Exists(dir + "/keys/User.keys"))
                NodeCrypto = NodeCrypto.LoadKeysFromFile(dir + "/keys/User.keys");

            if( NodeCrypto == null )
            {
                NodeCrypto = new NodeCrypto();
                NodeCrypto.CreateKeys(2048);
                NodeCrypto.SaveKeysToFile(dir + "/keys/User.keys");
            }
        }

        public void PrepareNodeServer()
        {
            if (NodeCrypto == null)
                throw new ApplicationException("NodeServer cant be initialized with out NodeCrypto");

            NodeServer = new NodeServer(this);
            var task = NodeServer.StartSpecificListener(IPAddress.Any, 1300);
            Task.WaitAny(task);
            if (task.Result == false)
                throw new ApplicationException("Something went wrong with node server initialize");
        }
    }
}

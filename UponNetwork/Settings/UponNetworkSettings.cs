using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UponNetwork.Settings
{
    [Serializable]
    public class UponNetworkSettings
    {
        public string PeerKeysFileName = "User"; 
        public string ListeningAdress = "0.0.0.0";
        public int ListeningPort = 1300;
    }
}

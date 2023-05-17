using Microcoin.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

namespace Microcoin.Data
{
    [Serializable]
    public class Block
    {
        public List<Transaction> Transactions { get; set; }
        public int Id { get; set; }
        public string PrewBlockHash { get; set; }
        public DateTime CreationTime { get; set; }
        public decimal MiningReward { get; set; }
        public UInt64 MagikValue { get; set; }
        public string MinerWallet { get; set; }


        public byte[] BlockHash()
        {
            var SHA = SHA512.Create();
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream();
            binaryFormatter.Serialize(memoryStream, this);
            return SHA.ComputeHash(memoryStream.ToArray());
        }

        public Block Clone()
        {
            return (Block)this.MemberwiseClone();
        }
    }
}

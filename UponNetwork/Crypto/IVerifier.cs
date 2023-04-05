using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microcoin.Data.Block;
using Microcoin.Data.Transaction;

namespace Microcoin.Crypto
{
    public interface IVerifier
    {
        public void SetKeys(ICryptoKeys keys);
        public bool VerifyTransaction(Transaction transaction);
        public bool VerifyBlock(Block block);
    }
}

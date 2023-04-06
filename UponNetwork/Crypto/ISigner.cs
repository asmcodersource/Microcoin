using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microcoin.Data.Block;
using Microcoin.Data.Transaction;

namespace Microcoin.Crypto
{
    public interface ISigner
    {
        public void SetKeys(ICryptoKeys keys);
        public string Sign(ISignable dataToSign);
    }
}

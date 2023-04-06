using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microcoin.Data;

namespace Microcoin.Crypto
{
    public interface IVerifier
    {
        public void SetKeys(ICryptoKeys keys);
        public bool Verify(ISignable dataToVerify);
    }
}

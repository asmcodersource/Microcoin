using Microcoin.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Microcoin.Data
{
    [Serializable]
    public class Transaction : ISignable
    {
        public decimal CoinsToSend { get; set; }
        public string SenderWallet { get; set; }
        public string ReceiverWallet { get; set; }
        public string Signature { get; set; }
        public DateTime CreationTime { get; set; }


        public bool IsTransactionCorrect()
        {
            ICryptoKeys cryptoKeys = new CryptoKeys();
            cryptoKeys.InitializeByXml(this.SenderWallet);
            IVerifier verifier = new Verifier();
            verifier.SetKeys(cryptoKeys);

            // Verify transaction for correct
            if (verifier.Verify(this) == false )
                return false;
            return true;
        }
    }
}

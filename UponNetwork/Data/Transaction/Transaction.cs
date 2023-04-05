using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microcoin.Data.Transaction
{
    [Serializable]
    public class Transaction
    {
        public decimal CoinsToSend { get; set; }
        public string SenderWallet { get; set; }
        public string ReceiverWallet { get; set; }
        public string TransactionSign { get; set; }

    }
}

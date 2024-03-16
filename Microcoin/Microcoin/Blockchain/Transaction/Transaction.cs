using System.Text.Json;

namespace Transaction
{
    [Serializable]
    public class Transaction
    {
        public double TransferAmount { get; set; } = 0.0;
        public string SenderPublicKey { get; set; }
        public string ReceiverPublicKey { get; set; }
        public string Sign { get; set; }
        public DateTime DateTime { get; set; }

        public override string ToString()
        {
            return string.Join("\n", new object[] { SenderPublicKey, ReceiverPublicKey, TransferAmount, Sign });
        }

        public static Transaction? ParseTransactionFromJson(string transactionJson)
            => JsonSerializer.Deserialize<Transaction>(transactionJson);
    }
}

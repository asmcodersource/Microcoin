using Microcoin.RSAEncryptions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Transaction
{
    public class TransactionSigner : ITransactionSigner
    {
        public SenderSignOptions SignOptions { get; set; }


        public void SetSignOptions(ISenderSignOptions options)
        {
            SignOptions = options as SenderSignOptions;
            if (SignOptions == null)
                throw new ArgumentException(nameof(options));
        }

        public void Sign(Transaction transaction)
        {
            if (SignOptions == null)
                throw new NullReferenceException(nameof(SignOptions));
            using (MemoryStream memoryStream = new MemoryStream())
            {
                transaction.Sign = "";
                transaction.DateTime = DateTime.UtcNow;
                transaction.SenderPublicKey = SignOptions.PublicKey;
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, transaction);
                string sign = RSAEncryption.Sign(memoryStream.ToArray(), SignOptions);
                transaction.Sign = sign;
            }
        }
    }
}

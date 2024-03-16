using Microcoin.RSAEncryptions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Transaction
{
    public class TransactionValidator : ITransactionValidator
    {
        public ReceiverSignOptions ValidateOptions { get; protected set; }


        public void SetValidateOptions(IReceiverSignOptions options)
        {
            ValidateOptions = options as ReceiverSignOptions;
            if (ValidateOptions == null)
                throw new ArgumentException(nameof(options));
        }

        public bool Validate(Transaction transaction)
        {
            if (ValidateOptions == null)
                throw new NullReferenceException(nameof(ValidateOptions));
            using (MemoryStream memoryStream = new MemoryStream())
            {
                var tempSign = transaction.Sign;
                transaction.Sign = "";
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, transaction);
                transaction.Sign = tempSign;
                return RSAEncryption.VerifySign(memoryStream.ToArray(), transaction.Sign, ValidateOptions);
            }
        }

        public static IReceiverSignOptions GetReceiverValidateOptions(Transaction transaction)
        {
            return new ReceiverSignOptions(transaction.SenderPublicKey);
        }
    }
}

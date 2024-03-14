using Microcoin.RSAEncryptions;

namespace Microcoin.Blockchain.Transaction
{
    public interface ITransactionValidator
    {
        void SetValidateOptions(IReceiverSignOptions options);
        bool Validate(Transaction message);
    }
}

using Microcoin.RSAEncryptions;

namespace Transaction
{
    public interface ITransactionValidator
    {
        void SetValidateOptions(IReceiverSignOptions options);
        bool Validate(Transaction message);
    }
}

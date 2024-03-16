using Microcoin.RSAEncryptions;

namespace Transaction
{
    public interface ITransactionSigner
    {
        public void SetSignOptions(ISenderSignOptions options);
        public void Sign(Transaction message);
    }
}

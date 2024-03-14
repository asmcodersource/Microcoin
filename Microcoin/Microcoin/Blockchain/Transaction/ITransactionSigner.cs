using Microcoin.RSAEncryptions;

namespace Microcoin.Blockchain.Transaction
{
    public interface ITransactionSigner
    {
        public void SetSignOptions(ISenderSignOptions options);
        public void Sign(Transaction message);
    }
}

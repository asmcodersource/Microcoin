using Microcoin.Blockchain.Transaction;
using System.Xml.Serialization;

namespace Microcoin
{
    public class PeerWalletKeys
    {
        public TransactionSigner? TransactionSigner { get; set; }

        public void CreateKeys()
        {
            var signOptions = RSAEncryptions.RSAEncryption.CreateSignOptions();
            TransactionSigner = new TransactionSigner();
            TransactionSigner.SetSignOptions(signOptions);
        }

        public void SaveKeys(string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    XmlSerializer xSer = new XmlSerializer(typeof(TransactionSigner));
                    xSer.Serialize(fs, TransactionSigner);
                }
            }
            catch (Exception ex)
            {
                // well, at least, I should remove broken file
                File.Delete(filePath);
                throw ex;
            }
        }

        public void LoadKeys(string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                XmlSerializer xSer = new XmlSerializer(typeof(TransactionSigner));
                TransactionSigner = xSer.Deserialize(fs) as TransactionSigner;
            }
        }

        public void SignTransaction(Transaction transaction)
        {
            if (TransactionSigner == null)
                throw new NullReferenceException("Keys is not initialized");
            TransactionSigner.Sign(transaction);
        }
    }
}

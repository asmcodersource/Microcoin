using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microcoin.Data.Block;
using Microcoin.Data.Transaction;

namespace Microcoin.Crypto
{
    public class Verifier
    {
        CryptoKeys cryptoKeys;
        protected object SHA = SHA256.Create();

        public void SetKeys(ICryptoKeys keys)
        {
            cryptoKeys = keys as CryptoKeys;
            if (cryptoKeys == null || cryptoKeys.PublicKeyXml == null)
                throw new ApplicationException("Used non initialized keys, or wrong keys object");
        }

        public bool VerifyTransaction(Transaction transaction)
        {
            if (cryptoKeys == null)
                throw new ApplicationException("Used not initialized Verifier to verify transaction");

            byte[] signature = Convert.FromBase64String(transaction.TransactionSign);
            var tempSign = transaction.TransactionSign;
            transaction.TransactionSign = null;
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream();
            binaryFormatter.Serialize(memoryStream, transaction);
            bool verifyResult = cryptoKeys.RSA.VerifyData(memoryStream.ToArray(), SHA, signature);
            transaction.TransactionSign = tempSign;
            return verifyResult;
        }

        public bool VerifyBlock(Block block)
        {
            if (cryptoKeys == null)
                throw new ApplicationException("Used not initialized Verifier to verify block");

            byte[] signature = Convert.FromBase64String(block.BlockSign);
            var tempSign = block.BlockSign;
            block.BlockSign = null;
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream();
            binaryFormatter.Serialize(memoryStream, block);
            bool verifyResult = cryptoKeys.RSA.VerifyData(memoryStream.ToArray(), SHA, signature);
            block.BlockSign = tempSign;
            return verifyResult;
        }
    }
}

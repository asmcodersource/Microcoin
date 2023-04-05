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
    public class Signer
    {
        CryptoKeys cryptoKeys;
        protected object SHA = SHA256.Create();

        public void SetKeys(ICryptoKeys keys)
        {
            cryptoKeys = keys as CryptoKeys;
            if (cryptoKeys == null || cryptoKeys.PublicKeyXml == null)
                throw new ApplicationException("Used non initialized keys, or wrong keys object");
        }

        public string SignTransaction(Transaction transaction)
        {
            if (cryptoKeys == null)
                throw new ApplicationException("Used not initialized Crypto to sign transaction");

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream();
            binaryFormatter.Serialize(memoryStream, transaction);
            byte[] sign = cryptoKeys.RSA.SignData(memoryStream, SHA);
            return Convert.ToBase64String(sign);
        }

        public string SignBlock(Block block)
        {
            if (cryptoKeys == null)
                throw new ApplicationException("Used not initialized Crypto to sign block");

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream();
            binaryFormatter.Serialize(memoryStream, block);
            byte[] sign = cryptoKeys.RSA.SignData(memoryStream, SHA);
            return Convert.ToBase64String(sign);
        }
    }
}

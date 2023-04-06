using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microcoin.Data;

namespace Microcoin.Crypto
{
    public class Signer: ISigner
    {
        CryptoKeys cryptoKeys;
        protected object SHA = SHA512.Create();

        public void SetKeys(ICryptoKeys keys)
        {
            cryptoKeys = keys as CryptoKeys;
            if (cryptoKeys == null || cryptoKeys.PublicKeyXml == null)
                throw new ApplicationException("Used non initialized keys, or wrong keys object");
        }

        public string Sign(ISignable dataToSign)
        {
            if (cryptoKeys == null)
                throw new ApplicationException("Used not initialized Crypto to sign transaction");

            dataToSign.Signature = "";
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream();
            binaryFormatter.Serialize(memoryStream, dataToSign);
            byte[] sign = cryptoKeys.RSA.SignData(memoryStream.ToArray(), SHA);
            dataToSign.Signature = Convert.ToBase64String(sign);
            return dataToSign.Signature;
        }
    }
}

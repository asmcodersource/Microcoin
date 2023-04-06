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
    public class Verifier : IVerifier
    {
        CryptoKeys cryptoKeys;
        protected object SHA = SHA512.Create();

        public void SetKeys(ICryptoKeys keys)
        {
            cryptoKeys = keys as CryptoKeys;
            if (cryptoKeys == null || cryptoKeys.PublicKeyXml == null)
                throw new ApplicationException("Used non initialized keys, or wrong keys object");
        }

        public bool Verify(ISignable dataToVerify)
        {
            if (cryptoKeys == null)
                throw new ApplicationException("Used not initialized Verifier to verify transaction");

            byte[] signature = Convert.FromBase64String(dataToVerify.Signature);
            var tempSign = dataToVerify.Signature;
            dataToVerify.Signature = "";
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream();
            binaryFormatter.Serialize(memoryStream, dataToVerify);
            bool verifyResult = cryptoKeys.RSA.VerifyData(memoryStream.ToArray(), SHA, signature);
            dataToVerify.Signature = tempSign;
            return verifyResult;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using System;
using System.IO;
using System.Text;

namespace Microcoin.Crypto
{
    public class CryptoKeys : ICryptoKeys
    {
        public RSACryptoServiceProvider? RSA { get; protected set; }
        public string PublicKeyXml { get; protected set; }
        public string KeysXml { get; protected set; }

        public void CreateKeys()
        {
            RSA = new RSACryptoServiceProvider(2048);
            PublicKeyXml = RSA.ToXmlString(false);
            KeysXml = RSA.ToXmlString(true);
        }

        public void InitializeByXml(string keysXml)
        {
            RSA = new RSACryptoServiceProvider();
            RSA.FromXmlString(keysXml);
            PublicKeyXml = RSA.ToXmlString(false);
        }

        public void SaveKeys(string filePath)
        {
            if (RSA == null)
                throw new ApplicationException("Cant save not initialized RSA keys");

            using (FileStream fileStream = File.Open(filePath, FileMode.Create))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(KeysXml);
                fileStream.Write(bytes);
            }
        }

        public void LoadKeys(string filePath) 
        {
            RSA = new RSACryptoServiceProvider();
            using (FileStream fileStream = File.Open(filePath, FileMode.Open))
            {
                byte[] bytes = new byte[fileStream.Length];
                fileStream.Read(bytes, 0, bytes.Length);
                string keys = Encoding.UTF8.GetString(bytes);
                RSA.FromXmlString(keys);
            }
        }
    }
}

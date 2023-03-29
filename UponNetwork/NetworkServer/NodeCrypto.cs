using System.ComponentModel;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using TcpNetwork;
using UponNetwork.NetworkSession;

using System.IO;
using System.Text;

namespace UponNetwork.NetworkServer
{
    [Serializable]
    public class NodeCrypto
    {
        public RSACryptoServiceProvider? RSA { get; protected set; }
        public string? publicKeyXml { get; protected set; }
        public string? privateKeyXml { get; protected set; }
        object halg = SHA256.Create();

        public byte[] SignMessage(byte[] message)
        {
            if (RSA == null)
                throw new ApplicationException("Use non initialized NodeCrypto");
            var result = RSA.SignData(message, halg);
            return result;
        }

        public bool VerifyMessageSign(byte[] message, byte[] signature)
        {
            if (RSA == null)
                throw new ApplicationException("Use non initialized NodeCrypto");
            var success = RSA.VerifyData(message, halg, signature);
            return success;
        }

        public static bool VerifyMessageSign(byte[] message, byte[] signature, string publicKeyXml)
        {
            var RSA = new RSACryptoServiceProvider();
            RSA.FromXmlString(publicKeyXml);
            if (RSA == null)
                throw new ApplicationException("Use non initialized NodeCrypto");
            var success = RSA.VerifyData(message, SHA256.Create(), signature);
            return success;
        }

        public void CreateKeys(int keySize = 2048)
        {
            RSA = new RSACryptoServiceProvider(keySize);
            publicKeyXml = RSA.ToXmlString(false);
            privateKeyXml = RSA.ToXmlString(true);
        }

        public bool SaveKeysToFile(string filePath)
        {
            StreamWriter? keysFile = null;
            bool success = false;
            try
            {
                keysFile = File.CreateText(filePath);
                keysFile.WriteLine(privateKeyXml);
                keysFile.WriteLine(publicKeyXml);
                success = true;
            }
            catch
            {
                // Nope here
                success = false;
            }
            keysFile?.Close();
            keysFile?.Dispose();
            return success;
        }

        public static NodeCrypto LoadKeysFromFile(string filePath)
        {
            NodeCrypto nodeCrypto = null;
            StreamReader? keysFile = null;
            try
            {
                nodeCrypto = new NodeCrypto();
                keysFile = File.OpenText(filePath);
                nodeCrypto.privateKeyXml = keysFile.ReadLine();
                nodeCrypto.publicKeyXml = keysFile.ReadLine();

                nodeCrypto.RSA = new RSACryptoServiceProvider();
                nodeCrypto.RSA.FromXmlString(nodeCrypto.privateKeyXml);
            }
            catch
            {
                nodeCrypto = null;
                // Nope here
            }
            keysFile?.Close();
            keysFile?.Dispose();
            return nodeCrypto;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microcoin.Crypto
{
    public interface ICryptoKeys
    {
        public void InitializeByXml(string keysXml);
        public void SaveKeys(string filePath);
        public void LoadKeys(string filePath);
        public void CreateKeys();
    }
}

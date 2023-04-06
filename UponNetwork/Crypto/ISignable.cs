using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microcoin.Crypto
{
    public interface ISignable
    {
        public string Signature { get; set; }
    }
}

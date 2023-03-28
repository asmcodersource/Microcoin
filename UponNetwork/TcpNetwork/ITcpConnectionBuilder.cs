using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpNetwork
{
    public class ITcpConnectionBuilder
    {
        public virtual ITcpConnection CreateConnection()
        {
            throw new ApplicationException("Use of interface ITcpConnectionBuilder");
        }
    }
}

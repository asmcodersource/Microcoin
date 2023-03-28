using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpNetwork
{
    public class TcpConnectionBuilder: ITcpConnectionBuilder
    {
        public override ITcpConnection CreateConnection()
        {
            return new TcpConnection();
        }
    }
}

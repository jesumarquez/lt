using System;
using System.Collections.Generic;
using System.Text;

namespace avl2cmd
{
    public interface ILogger
    {
        void log(string s);
        void take(cmd_acceptor a);
        void ack(bool acked);
    }
}

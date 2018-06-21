using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace clientBSD2
{
    
    class Program
    {
        static ClientTcp player = new ClientTcp();
        static void Main(string[] args)
        {
            player.connect();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace serverBSD2
{
    class Position
    {
        public int x { get; set; }
        public int y { get; set; }

        public Position()
        {
            Random random = new Random();
            x = random.Next(0, 50);
            y = random.Next(0, 50);
        }
    }
    class Player
    {
        public TcpClient client { get; set; }
        public Position position { get; set; }
        public string name { get; set; }

        public Player()
        {
            position = new Position();
        }
    }
}

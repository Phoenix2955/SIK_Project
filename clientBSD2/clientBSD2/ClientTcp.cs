using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace clientBSD2
{
   
    class ClientTcp
    {
        static int[,] playgroudArray = new int[50, 50];
        static string[] separators = { "-", "," };
        string name;
        IPAddress ip = IPAddress.Parse("127.0.0.1");
        int port = 5000;
        TcpClient client;
        Thread thread;

        public string getName()
        {
            return name;
        }

        public TcpClient getClient()
        {
            return client;
        }



        public void connect()
        {

            client = new TcpClient();
            try
            {
                client.Connect(ip, port);
                thread = new Thread(o => ReceiveData((TcpClient)o));
                thread.Start(client);
                thread = new Thread(o => WriteData((TcpClient)o));
                thread.Start(client);
            }
            catch
            {
                Console.WriteLine("Serwer nie odpowiada");
            }
        }




        void WriteData(TcpClient client)
        {
            using (NetworkStream stream = new NetworkStream(client.Client))
            {
                while (true)
            {


                //wyslałe wszystkich wiadomosci
                string loginMessage = Console.ReadLine(); ;

                    byte[] loginMessage_buffer = Encoding.ASCII.GetBytes(loginMessage);
                    stream.Write(loginMessage_buffer, 0, loginMessage_buffer.Length);
                }
            }

        }



        void ReceiveData(TcpClient client)
        {
            
            byte[] receivedBytes = new byte[client.Client.ReceiveBufferSize];
            int byte_count;



            using (NetworkStream stream = new NetworkStream(client.Client))
            {


                //odebranie informacji że połączono
                while ((byte_count = stream.Read(receivedBytes, 0, receivedBytes.Length)) > 0)
                {
                    string messages = Encoding.ASCII.GetString(receivedBytes, 0, byte_count);

                        if (messages == "CONNECT")
                        {
                            Console.WriteLine(messages);
                            break;
                    }
                }
                //koniec odbierania info ze połączono




               



                //czytanie komunikatów z serwera i wysyłanie swoich
                while ((byte_count = stream.Read(receivedBytes, 0, receivedBytes.Length)) > 0)
                {
                    string[] messages = Encoding.ASCII.GetString(receivedBytes, 0, byte_count).Split(' ');
                    for (int i = 0; i < messages.Length; i++)
                    {
                        if (messages[i] == "START")
                        {
                            Console.WriteLine("START" + messages[i+1]);

                        }

                        //pobranie wspolzednych graczy
                        else if (messages[i] == "PLAYERS")
                        {
                            for (int j = i + 1; j < i + 6; j++)
                            {
                                string[] wspolrzedne = messages[j].Split(',');
                                playgroudArray[Int32.Parse(wspolrzedne[0]), Int32.Parse(wspolrzedne[1])] = j - i; 
                            }
                        }
                        else if (messages[i] == "GAME")
                        {
                            Console.WriteLine("GAME");
                        }
                        else if (messages[i] == "BOARD")
                        {
                            string[] board = new string[2500];
                            Array.Copy(messages, i + 1, board, 0,2500);
                            int p = 0;
                            for(int j = 0; j< board.Length; j++)
                            {
                                playgroudArray[j%50,p] = Int32.Parse(board[j]);
                                if(j != 0 && j%50 == 0)
                                {
                                    p = 0;
                                }
                            }

                            for ( int q = 0; q < 50; q++)
                            {
                                for (int w = 0; w < 50; w++)
                                {
                                    Console.Write(playgroudArray[q, w] + " ");
                                }
                                Console.Write('\n');
                            }
                        }

                    }
                }
            }
        }


    }
}

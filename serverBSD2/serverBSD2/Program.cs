using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace serverBSD2
{
    class Program
    {
        static int[,] playgroundArray = new int[50, 50];
        static int countOfPlayer = 0;
        static Player[] players = new Player[5];
        static void Main(string[] args)
        {
            //zapisywanie logów do pliku
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            Trace.Listeners.Add(new TextWriterTraceListener("log.txt"));
            Trace.AutoFlush = true;
            Trace.Indent();

            TcpListener ServerSocket = new TcpListener(IPAddress.Any, 5000);
            ServerSocket.Start();

            while (true)
            {
                TcpClient client = ServerSocket.AcceptTcpClient();
                using (NetworkStream stream = new NetworkStream(client.Client))
                {

                    if (stream.CanRead && stream.CanWrite)

                    {

                        //info
                        //ze
                        //polaczono
                        byte[] connectMessage = Encoding.ASCII.GetBytes("CONNECT");
                        stream.Write(connectMessage, 0, connectMessage.Length);
                        //koniec



                        //pobranie loginu
                        byte[] receivedBytes = new byte[1024];
                        int byte_count;

                        while ((byte_count = stream.Read(receivedBytes, 0, receivedBytes.Length)) > 0)
                        {
                            string[] messages = Encoding.ASCII.GetString(receivedBytes, 0, byte_count).Split(' ');
                            if (messages[0] == "LOGIN")
                            {
                                Trace.WriteLine("LOGIN " + messages[1]);
                                //dodanie gracza do tablicy
                                players[countOfPlayer] = new Player();
                                players[countOfPlayer].client = client;
                                players[countOfPlayer].name = messages[1];
                                countOfPlayer++;
                                break;
                            }
                            else
                            {
                                Trace.WriteLine("ERROR");
                            }
                        }


                        //utworzenie wątków dla każdego gracza. // i wątku do wysyłania komunikatu board
                        if (countOfPlayer==5)
                        {
                            for(int i = 0; i<5; i++)
                            {
                                Thread thread = new Thread(handle_clients);
                                thread.Start(i);
                            }
                            Thread boardThread = new Thread(board);
                            boardThread.Start("board");
                        }

                    }
                }
            }

            

        }
        public static void board(object o)
        {
            while (true)
            {
                
                string board = "BOARD ";
                for (int x = 0; x < 50; x++)
                {
                    for (int y = 0; y < 50; y++)
                    {
                        board += playgroundArray[y, x].ToString() + " ";
                    }
                }
                for (int i = 0; i < 5; i++)
                {
                    using (NetworkStream stream = new NetworkStream(players[i].client.Client))
                    {
                        byte[] boardByte = Encoding.ASCII.GetBytes(board);
                        stream.Write(boardByte, 0, boardByte.Length);
                    }
                }
                Thread.Sleep(30000);
            }
        }

        public static void handle_clients(object o)
        {
            int id = (int)o;


            //utworzenie połaczenia na osobnym wątku
            TcpClient client = players[id].client;
            using (NetworkStream stream = new NetworkStream(client.Client))
            {


                //wysłanie komunikatu start
                byte[] connectMessage = Encoding.ASCII.GetBytes("START " + id);
                stream.Write(connectMessage, 0, connectMessage.Length);

                //koniec wysyłania komuniaktu start



                //wysłanie współrzędnych graczy
                string listofplayers = "PLAYERS ";
                //for (int i =0; i<5; i++)
                //{
                //    listofplayers += players[i].position.x +","+ players[i].position.y +" ";
                //}
                
                foreach(var player in players)
                {
                    listofplayers += player.position.x + "," + player.position.y + " ";
                }






                //pobieranie i wysyłanie danych od i do gracza
                byte[] playerlist = Encoding.ASCII.GetBytes(listofplayers);
                stream.Write(playerlist, 0, listofplayers.Length);


                byte[] receivedBytes = new byte[1024];
                int byte_count;

                while ((byte_count = stream.Read(receivedBytes, 0, receivedBytes.Length)) > 0)
                {
                    //rozpoczęcie gry, czekanie na begin
                    string[] messages = Encoding.ASCII.GetString(receivedBytes, 0, byte_count).Split(' ');
                    if (messages[0] == "BEGIN")
                    {
                        Trace.WriteLine("BEGIN " + messages[1]);
                        //dodanie gracza do tablicy
                        if (messages[1] == "N") {
                            if(playgroundArray[players[id].position.x, players[id].position.y -1] == 0)
                            {
                                playgroundArray[players[id].position.x, players[id].position.y - 1] = id;
                                players[id].position.y -= 1;
                                break;
                            }
                                
                        }
                        else if(messages[1] =="S"){
                            if (playgroundArray[players[id].position.x, players[id].position.y + 1] == 0)
                            {
                                playgroundArray[players[id].position.x, players[id].position.y + 1] = id;
                                players[id].position.y += 1;
                                break;
                            }
                        }
                        else if(messages[1] =="E") {
                            if (playgroundArray[players[id].position.x+1, players[id].position.y] == 0)
                            {
                                playgroundArray[players[id].position.x+1, players[id].position.y] = id;
                                players[id].position.x += 1;
                                break;
                            }
                        }
                        else if(messages[1] =="W")
                        {
                            if (playgroundArray[players[id].position.x-1, players[id].position.y] == 0)
                            {
                                playgroundArray[players[id].position.x-1, players[id].position.y] = id;
                                players[id].position.x -= 1;
                                break;
                            }
                        }
                        else
                        {
                            Trace.WriteLine("error");
                        }
                    }
                    else
                    {
                        Trace.WriteLine("ERROR");
                    }
                }
                //koniec czekania na begin


                //wysłanie komunikatu game
                playerlist = Encoding.ASCII.GetBytes("GAME");
                stream.Write(playerlist, 0, playerlist.Length);
                //koniec komunikatu game
            }
        }
    }
}

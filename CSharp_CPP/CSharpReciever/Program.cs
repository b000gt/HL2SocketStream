using System;

using System.Net;

using System.Net.Sockets;

using System.Text;

namespace UDPServer

{

    class Program

    {

        static void Main(string[] args)

        {

            RecData(28250);

        }

        static void RecData(int Port)

        {

            UdpClient client = null;

            try

            {

                client = new UdpClient(Port);

            }

            catch (Exception ex)

            {

                Console.WriteLine(ex.Message);

            }

            IPEndPoint RemoteServer = new IPEndPoint(IPAddress.Any, 0);

            for (; ; )

            {

                try

                {

                    byte[] RecPacket = client.Receive(ref RemoteServer);

                    Console.WriteLine("Connected to the client {0}, {1}", RemoteServer, Encoding.ASCII.GetString(RecPacket));

                }

                catch (Exception ex)

                {

                    Console.WriteLine(ex.Message);

                }

            }

        }

    }

}
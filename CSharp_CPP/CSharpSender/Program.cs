using System;

using System.Net;

using System.Net.Sockets;

using System.Text;

namespace UDPClient

{

    class Program

    {

        static void Main(string[] args)

        {

            SendUDPData("127.0.0.1", 28250, "This is UDP Test");

        }

        static void SendUDPData(string ServerIP, int Port, string Mess)

        {

            UdpClient client = new UdpClient();

            byte[] SendPacket = Encoding.ASCII.GetBytes(Mess);

            try

            {
                Console.ReadKey();
                client.Send(SendPacket, SendPacket.Length, ServerIP, Port);

                Console.WriteLine("Sent {0} bytes to the server", SendPacket.Length);

            }

            catch (Exception ex)

            {

                Console.WriteLine(ex.Message);

            }

            client.Close();

            Console.ReadKey();
        }

    }

}
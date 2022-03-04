using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Globalization;

namespace UDPClient

{
    public class UdpStreamingClient: IDisposable
    {
        public event EventHandler PackageReceived;
        private UdpClient server;
        private IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
        private CancellationTokenSource cancellationToken;
        public UdpStreamingClient(string serverIp, int serverPort)
        {
            cancellationToken = new CancellationTokenSource();
            server = new UdpClient(serverIp, serverPort);
            ThreadPool.QueueUserWorkItem(new WaitCallback(KeepAlive), cancellationToken.Token);
            ThreadPool.QueueUserWorkItem(new WaitCallback(Listen), cancellationToken.Token);
        }

        public void Dispose()
        {
            cancellationToken.Cancel();
            cancellationToken.Dispose();
            server.Close();
        }

        public void Listen(object cancellationToken)
        {
            var token = (CancellationToken) cancellationToken;
            while (!token.IsCancellationRequested)
            {
                try
                {
                    Byte[] receiveBytes = server.Receive(ref remoteIpEndPoint);
                    string returnData = Encoding.ASCII.GetString(receiveBytes);
                    PackageReceived?.Invoke(this, new UdpStreamMessageReceivedEventArgs() { Message = returnData });
                }
                catch (SocketException ex)
                {
                    continue;
                }
            }
        }

        private void KeepAlive(object cancellationToken)
        {
            var token = (CancellationToken)cancellationToken;
            while(!token.IsCancellationRequested)
            {
                byte[] SendPacket = Encoding.ASCII.GetBytes("KEEPALIVE");
                server.Send(SendPacket, SendPacket.Length);
                Thread.Sleep(1000);
            }
        }
    }

    public class UdpStreamMessageReceivedEventArgs: EventArgs
    {
        public string Message { get; set; }
    }

    class Program

    {

        static void Main(string[] args)

        {
            using(var client = new UdpStreamingClient("127.0.0.1", 28250))
            {
                client.PackageReceived += OnMessageReceived;
                Console.ReadLine();
            }
        }

        static void OnMessageReceived(object sender, EventArgs args)
        {
            if(args is UdpStreamMessageReceivedEventArgs)
            {
                var matrix = new float[4,4];
                var message = (args as UdpStreamMessageReceivedEventArgs).Message;
                var rows = message.Split("\n");
                for(var i = 0; i < 4; i++)
                {
                    var row = rows[i].Split("\t");
                    for(var j = 0; j < 4; j++)
                    {
                        matrix[i, j] = float.Parse(row[j], CultureInfo.InvariantCulture);
                    }
                }

                Console.WriteLine("Message received:\n");
                for(int i = 0; i < 4; i++)
                {
                    for(var j = 0; j < 4; j++)
                    {
                        Console.Write($"{matrix[i, j]}\t");
                    }
                    Console.Write("\n");
                }
            }
        }

    }

}
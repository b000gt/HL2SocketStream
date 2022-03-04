using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System;
using System.Globalization;

public class StreamReciever : MonoBehaviour
{
    public TextMesh TextObject;
    public TextMesh DebugText;
    public GameObject ObjectTransformed;
    public int Port;

    float[,] transformationMatrix = new float[4, 4];
    string debugMessage = String.Empty;
    string counter = String.Empty;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start sending...");
        var client = new UdpStreamingClient("10.5.1.34", 28250);
        client.PackageReceived += OnMessageReceived;
    }

    // Update is called once per frame
    void Update()
    {
        TextObject.text = counter;
        DebugText.text = debugMessage;
        transform.Translate(
            transformationMatrix[0, 3],
            transformationMatrix[1, 3],
            transformationMatrix[2, 3]);
        transformationMatrix = new float[4, 4];
    }

    void OnMessageReceived(object sender, EventArgs args)
    {
        if (args is UdpStreamMessageReceivedEventArgs)
        {
            counter = (args as UdpStreamMessageReceivedEventArgs).Message;

            var message = (args as UdpStreamMessageReceivedEventArgs).Message;
            var rows = message.Split('\n');
            for (var i = 0; i < 4; i++)
            {
                var row = rows[i].Split('\t');
                for (var j = 0; j < 4; j++)
                {
                    transformationMatrix[i, j] = float.Parse(row[j], CultureInfo.InvariantCulture);
                }
            }
        }
    }
}

public class UdpStreamingClient : IDisposable
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
        var token = (CancellationToken)cancellationToken;
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
        while (!token.IsCancellationRequested)
        {
            byte[] SendPacket = Encoding.ASCII.GetBytes("KEEPALIVE");
            server.Send(SendPacket, SendPacket.Length);
            Thread.Sleep(1000);
        }
    }
}
public class UdpStreamMessageReceivedEventArgs : EventArgs
{
    public string Message { get; set; }
}
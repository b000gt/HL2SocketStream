using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System;

#if !UNITY_EDITOR
    using Windows.Networking.Sockets;
#endif

public class StreamReciever : MonoBehaviour
{
    public TextMesh TextObject;
    public TextMesh DebugText;
    public int Port;

    string counter = String.Empty;
    // Start is called before the first frame update
    async void Start()
    {
#if !UNITY_EDITOR
        DebugText.text = "Starting...";
        DatagramSocket socket = new DatagramSocket();
        socket.MessageReceived += MessageRecieved;
        try
        {
            await socket.BindEndpointAsync(null, Port.ToString());
            DebugText.text = "Socket Bound";
        }
        catch (Exception ex)
        {
            DebugText.text = ex.ToString();
            Debug.Log(ex.ToString());
        }
#endif
        counter = TextObject.text;
        // Task.Run(() => { Listen(); });
    }

    // Update is called once per frame
    void Update()
    {
        TextObject.text = counter;
    }

#if !UNITY_EDITOR
    void MessageRecieved(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
    {
        using (var reader = args.GetDataReader())
        {
            var buf = new byte[reader.UnconsumedBufferLength];
            reader.ReadBytes(buf);
            string message = Encoding.UTF8.GetString(buf);
            TextObject.text = message;
        }
    }
#endif

    void Listen()
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
        for(; ; )
        {
            try
            {
                byte[] RecPacket = client.Receive(ref RemoteServer);
                var message = Encoding.ASCII.GetString(RecPacket).ToString();
                Debug.Log($"Message: {message}");
                counter = message;
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
    }
}
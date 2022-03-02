using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System;

public class StreamReciever : MonoBehaviour
{
    public TextMesh TextObject;
    public TextMesh DebugText;
    public int Port;

    string debugMessage = String.Empty;
    string counter = String.Empty;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start sending...");
        SendUDPData("10.5.1.50", 28250, "This is UDP Test");
    }

    // Update is called once per frame
    void Update()
    {
        TextObject.text = counter;
        DebugText.text = debugMessage;
    }

    void SendUDPData(string ServerIP, int Port, string Mess)

    {

        UdpClient client = new UdpClient(ServerIP, Port);

        byte[] SendPacket = Encoding.ASCII.GetBytes(Mess);

        try

        {
            client.Send(SendPacket, SendPacket.Length);

            debugMessage += $"Sent {SendPacket.Length} bytes to the server";

        }

        catch (Exception ex)

        {

            debugMessage += ex.Message;

        }

        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
        Byte[] receiveBytes = client.Receive(ref RemoteIpEndPoint);
        string returnData = Encoding.ASCII.GetString(receiveBytes);
        debugMessage += "This is the message you received: " +
                                  returnData;

        client.Close();
    }
}

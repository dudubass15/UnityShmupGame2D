using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Sockets;

using UnityEngine;

public class UDPClient : MonoBehaviour
{

    string IP = "165.227.7.56";
    static IPEndPoint server;
    static UdpClient client;
    Thread receiveThread;
    int port = 9500;

    void Main()
    {
        try
        {
            server = new IPEndPoint(IPAddress.Parse(IP), port);
            client = new UdpClient(UnityEngine.Random.Range(9500, 9999));

            receiveThread = new Thread(new ThreadStart(ReceiveData));
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }
        catch (Exception err)
        {
            print(err.ToString());
        }

    }

    public static void sendUDP(string text)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            client.Send(data, data.Length, server);
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }

    static string recv = "";
    void ReceiveData()
    {
        while (true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);
                recv = Encoding.UTF8.GetString(data);

            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }

    public static string GetLastRecv()
    {
        string _recv = recv;
        recv = "";
        return _recv;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.AI;
using System;
using System.Net;
using System.Net.Sockets;

public class DogInterface : MonoBehaviour
{  
    private const int lin_port = 7000;
    private const int ang_port = 7001;
    private const string network = "localhost";
    private TcpClient lin_client;
    private TcpClient ang_client;

    void Start()
    {
        lin_client = new TcpClient(network, lin_port);
        ang_client = new TcpClient(network, ang_port);
    }

    public void SendLinearVelocity(in float message)
    {
        try
        {            
            Byte[] byte_data = BitConverter.GetBytes(message);
            NetworkStream stream = lin_client.GetStream();
            stream.Write(byte_data, 0, byte_data.Length);
            Debug.Log("VectorStream sent: " + message);
        }
        catch (Exception e)
        {
            Debug.Log("Error sending message: " + e);
        }
    }
    
    public void SendAngularVelocity(in float message)
    {
        try
        {            
            Byte[] byte_data = BitConverter.GetBytes(message);
            NetworkStream stream = ang_client.GetStream();
            stream.Write(byte_data, 0, byte_data.Length);
            Debug.Log("VectorStream sent: " + message);
        }
        catch (Exception e)
        {
            Debug.Log("Error sending message: " + e);
        }
    }
}


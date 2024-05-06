using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.AI;
using System;
using System.Net;
using System.Net.Sockets;
using System.IO;

public class DogInterface : MonoBehaviour
{  
    private const string network = "localhost";
    private const int lin_port = 7000;
    private const int ang_port = 7001;
    private const int vis_port = 7002;
    private const int spk_port = 7003;
    private TcpClient lin_client;
    private TcpClient ang_client;
    private TcpClient vis_client;
    private TcpClient spk_client;

    void Start()
    {
        lin_client = new TcpClient(network, lin_port);
        ang_client = new TcpClient(network, ang_port);
        vis_client = new TcpClient(network, vis_port);
        spk_client = new TcpClient(network, spk_port);
    }

    public void SendLinearVelocity(in float message)
    {
        try
        {            
            Byte[] byte_data = BitConverter.GetBytes(message);
            NetworkStream stream = lin_client.GetStream();
            stream.Write(byte_data, 0, byte_data.Length);
            Debug.Log("Moving with speed: " + message);
        }
        catch (Exception e)
        {
            Debug.Log("Error sending message: " + e);
        }
    }
    
    public bool DetectPerson()
    {
        try
        {
            const int trigger = 0;
            NetworkStream stream = vis_client.GetStream();
            byte[] data = BitConverter.GetBytes(trigger);
            stream.Write(data, 0, data.Length);
            byte[] buffer = new byte[4];
            stream.Read(buffer, 0, buffer.Length);
            int response = BitConverter.ToInt32(buffer, 0);
            Debug.Log($"Response: {response}");     
            return response != 0;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error: {e.Message}");
            return false;
        }
    }

    public void SendAngularVelocity(in float message)
    {
        try
        {            
            Byte[] byte_data = BitConverter.GetBytes(message);
            NetworkStream stream = ang_client.GetStream();
            stream.Write(byte_data, 0, byte_data.Length);
            Debug.Log("Turning with speed: " + message);
        }
        catch (Exception e)
        {
            Debug.Log("Error sending message: " + e);
        }
    }

    public void Speak(in string message)
    {
        try
        {            
            byte[] byte_data = Encoding.UTF8.GetBytes(message);
            NetworkStream stream = spk_client.GetStream();
            stream.Write(byte_data, 0, byte_data.Length);
            stream.Flush();
            Debug.Log("Sent Message to Be Spoken: " + message);
        }
        catch (Exception e)
        {
            Debug.Log("Error sending message: " + e);
        }
    }
}
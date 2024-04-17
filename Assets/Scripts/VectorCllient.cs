using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.AI;
using System;
using System.Net;
using System.Net.Sockets;

public class VectorClient : MonoBehaviour
{  
    public NavMeshAgent nav_data;
    private int port = 7000;
    private TcpClient client;

    private struct VectorStream
    {
        public float x;
        public float z;
        public float yaw;
    }

    void Start()
    {
        Application.targetFrameRate = 15;
        client = new TcpClient("localhost", port);
        nav_data = GetComponent<NavMeshAgent>();
        Debug.Log(nav_data.velocity);
    }

    void Update()
    {
        VectorStream message = new VectorStream();
        Vector3 inverse_velocity = transform.InverseTransformDirection(nav_data.velocity);
        Vector3 forward_axis = Vector3.forward;
        Vector3 projected_axis = new Vector3(inverse_velocity.x, 0, inverse_velocity.z);
        float diff_angle = Vector3.Angle(forward_axis, projected_axis);
        if (Vector3.Cross(forward_axis, projected_axis).y < 0){
        	diff_angle = -1 * diff_angle;
        }
        float yaw_speed = 0f;
      //  float scale_angle = 5.8f;
 	       
        //if (Mathf.Abs(diff_angle) < scale_angle) {
        //	diff_angle *= scale_angle;
        //} 	
        //else {
        //	diff_angle *= ((360f - (2*scale_angle)) / 360f);
        //} 	
        //float yaw_speed = diff_angle * 2 * 20* 3.14f / 360;
        if (Math.Abs(diff_angle) < 2.5f){
        yaw_speed= 0f;
        }
        else {
        	yaw_speed = 3.1415f/6.6f;
        	if (diff_angle <0){
        		yaw_speed = -1*yaw_speed;
        		}
        }
         
        Debug.Log(diff_angle);
        message.x = inverse_velocity.x;
        message.z = inverse_velocity.z;
        message.yaw = yaw_speed;
        SendVectorStream(message);
    }

    Byte[] ConvertMessageToBytes(in VectorStream message)
    {
        byte[] dataX = BitConverter.GetBytes(message.x);
        byte[] dataZ = BitConverter.GetBytes(message.z);
        byte[] dataYaw = BitConverter.GetBytes(message.yaw);
        byte[] data = new byte[dataX.Length + dataZ.Length + dataYaw.Length];
        System.Buffer.BlockCopy(dataX, 0, data, 0, dataX.Length);
        System.Buffer.BlockCopy(dataZ, 0, data, dataX.Length, dataZ.Length);
        System.Buffer.BlockCopy(dataYaw, 0, data, dataX.Length + dataZ.Length, dataYaw.Length);
        return data;
    }

    void SendVectorStream(in VectorStream message)
    {
        try
        {            
            Byte[] byte_data = ConvertMessageToBytes(message);
            NetworkStream stream = client.GetStream();
            stream.Write(byte_data, 0, byte_data.Length);
            Debug.Log("VectorStream sent: " + message);
        }
        catch (Exception e)
        {
            Debug.Log("Error sending message: " + e);
        }
    }
}


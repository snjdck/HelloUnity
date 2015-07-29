using UnityEngine;
using System.Collections;

using System;
using System.Net;
using System.Net.Sockets;

public class SocketClient : MonoBehaviour {

	public String host;
	public int port;

	private Socket client;
	private IList sendQueue;
	
	void Start () {
		sendQueue = new ArrayList ();

		client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		Connect ();
	}

	void Connect()
	{
		client.Connect (host, port);
		Debug.Log ("connected");
		//client.BeginReceive(null, 0, 4096,SocketFlags.
	}

	void Send(byte[] data)
	{
		sendQueue.Add (data);
	}

	void Update()
	{
		if(false == client.Poll(0, SelectMode.SelectRead)){
			return;
		}
		byte[] buffer = new byte[4096];
		int nRecv = client.Receive(buffer, 0, client.Available, SocketFlags.None);
	}

	void LateUpdate()
	{
		if(false == client.Poll(0, SelectMode.SelectWrite)){
			return;
		}
		byte[] buffer = (byte[])sendQueue[0];
		int nSend = client.Send(buffer);
		if(nSend >= buffer.Length){
			sendQueue.RemoveAt(0);
		}else{
			int size = buffer.Length - nSend;
			byte[] left = new byte[size];
			Array.Copy(buffer, nSend, left, 0, size);
			sendQueue[0] = left;
		}
	}
}

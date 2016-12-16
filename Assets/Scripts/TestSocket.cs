using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net.Sockets;
using System;

public class TestSocket : MonoBehaviour {
    private Socket socket;
    private byte[] bufferRecv;
    private byte[] bufferSend;
    private int indexSend;
    private int indexRecv;

    public delegate void PacketHandler(Packet packet);
    public event PacketHandler onPacket;

    List<Packet> packetList;
    // Use this for initialization
    void Start () {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Connect("127.0.0.1", 7410);
	}
	
	void Update () {
        if (socket.Poll(0, SelectMode.SelectRead))
        {
            int bytesRecv = socket.Receive(bufferRecv, indexRecv, bufferRecv.Length - indexRecv, SocketFlags.None);
            indexRecv += bytesRecv;
            int offset = 0;
            for(;;){
                Packet packet = Packet.Parse(bufferRecv, indexRecv, ref offset);
                if (packet != null){
                    packetList.Add(packet);
                    continue;
                }
                if (offset <= 0) {
                    break;
                }
                indexRecv -= offset;
                if (indexRecv > 0){
                    Array.Copy(bufferRecv, offset, bufferRecv, 0, indexRecv);
                }
                break;
            }
        }
        if (indexSend > 0 && socket.Poll(0, SelectMode.SelectWrite))
        {
            int bytesSend = socket.Send(bufferSend, indexSend, SocketFlags.None);
            indexSend -= bytesSend;
            if (indexSend > 0) {
                Array.Copy(bufferSend, bytesSend, bufferSend, 0, indexSend);
            }
        }
        foreach (Packet packet in packetList)
        {
            onPacket(packet);
        }
        packetList.Clear();

    }

    void Send(int msgId, byte[] msgData)
    {

    }
}

public class Packet
{
    static public Packet Parse(byte[] buffer, int index, ref int offset)
    {
        int dataSize = index - offset;
        if (dataSize < 9)
            return null;
        ushort packetSize = BitConverter.ToUInt16(buffer, offset);
        if (dataSize < packetSize)
            return null;
        offset += packetSize;
        return new Packet();
    }

    int msgId;

    Packet()
    {

    }
}

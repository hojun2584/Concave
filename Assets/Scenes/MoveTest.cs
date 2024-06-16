using ServerCore;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DummyClient;
using System.Net;
using System.Text;
using Assets.Network.Packet;

public class MoveTest : MonoBehaviour
{

    public string number;

    const int VECTORSIZE = 12;


    ServerSession session = new ServerSession();
    

    private void Awake()
    {
        
        string host = Dns.GetHostName();
        IPHostEntry iPHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = iPHost.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

        Connector connector = new Connector();

        connector.Connect(endPoint, () => 
        { 
            return session; 
        });


    }
    
    Packet packet;
    Packet test;

    // Start is called before the first frame update
    void Start()
    {
        packet = new RPCPacket();
        test = new MakeWhiteRock();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.S))
        {
            ArraySegment<byte> send = packet.Write();
            session.Send(send);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            ArraySegment<byte> send = test.Write();
            session.Send(send);
        }



    }


    public ArraySegment<byte> SenderTest()
    {
        ushort size = 0;
        bool success = true;

        ArraySegment<byte> sendBuffer = SendBufferHelper.Open(4096);

        size += 2;
        success &= BitConverter.TryWriteBytes(new Span<byte>(sendBuffer.Array , sendBuffer.Offset + size , sendBuffer.Count - size), 3);
        
        // TODO ���� ���÷� 1001 �������� ������ ���ؼ� �޾ƾ� �ҵ�? ������ ���� ���ڸ���
        // �������� �� ģ���� ���� ���° ģ������ Ȯ���ϴ� �� ���� ��
        size += 2;
        success &= BitConverter.TryWriteBytes(new Span<byte>(sendBuffer.Array, sendBuffer.Offset + size, size - sendBuffer.Count), 1001);

        size += 4;
        success &= BitConverter.TryWriteBytes(new Span<byte>(sendBuffer.Array , sendBuffer.Offset + size ,size - sendBuffer.Count), 5);
        sendBuffer = BitConvertVecrtor3(sendBuffer , size, success, transform.position);

        ArraySegment<byte> send = SendBufferHelper.Close(size);


        return send;
    }
    
    public ArraySegment<byte> BitConvertVecrtor3(ArraySegment<byte> array, int size , bool suceess ,Vector3 oper)
    {
        size += sizeof(float);
        suceess &= BitConverter.TryWriteBytes(new Span<byte>(array.Array , array.Offset+ size , size - array.Count) , oper.x);
        size += sizeof(float);
        suceess &= BitConverter.TryWriteBytes(new Span<byte>(array.Array, array.Offset + size, size - array.Count), oper.y);
        size += sizeof(float);
        suceess &= BitConverter.TryWriteBytes(new Span<byte>(array.Array, array.Offset + size, size - array.Count), oper.z);


        return array;
    }

    private void OnDestroy()
    {
        
    }
}

using DummyClient;
using JetBrains.Annotations;
using ServerCore;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class NetWorkObj : MonoBehaviour
{

    public static NetWorkObj instance;
    public ServerSession session = new ServerSession();

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
        
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


}

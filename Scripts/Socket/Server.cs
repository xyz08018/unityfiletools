using DwGoing.Socket;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Server : MonoBehaviour
{
    SocketServer server;
    List<Guid> guidList = new List<Guid>();

    List<Guid> prepareUserList = new List<Guid>();
    [SerializeField]
    bool isSuccess = false;

    void Start()
    {
        try
        {
            server = new SocketServer("", 8000, 100);//"192.168.64.80", 9527, 30
        }
        catch
        {
            isSuccess = false;
            Debug.Log("启动服务器失败");

        }
        server.CB_Start = () => { Debug.Log("成功启动服务器"); isSuccess = true; restartTime = 0;/*当Server开启时*/ };
        server.CB_Accept = (guid) => { Debug.LogError("设备连接: " + guid); guidList.Add(guid);/*当Client连接时*/ };
        server.CB_Receive = (guid, msg) => { ReceiveMsg(guid, msg);/*当接收到消息时*/ };
        server.CB_SendMsg = (guid) => { /*当发送消息时*/ };
        server.CB_ClientClose = (guid) => { ClientClose(guid); /*当Client断开时*/ };
        server.CB_Stop = () => { /*当Server关闭时*/ };
        try
        {
            server.Start();
        }
        catch
        {
            isSuccess = false;
            Debug.Log("启动服务器失败");
        }
    }

    float restartTime = 0;
    void Update()
    {
        if (!isSuccess)
        {
            restartTime += Time.deltaTime;
            if (restartTime >= 10)
            {
                restartTime = 0;
                Debug.Log("重启服务器");
                Debug.Log("guidList" + guidList.Count);
                try
                {
                    server.Start();
                }
                catch
                {
                    isSuccess = false;
                    Debug.Log("启动服务器失败");
                }
            }
        }
        //心跳包
        //client.SendMsg("00");
        //isSuccess = false;

        //Thread.Sleep(3000);
    }

    void ReceiveMsg(Guid id, string reMsg)
    {
        Debug.Log("接收到客户端id： " + id + "  消息： " + reMsg);
        if (reMsg == "")
        {
            SendMsg("");
            return;
        }
        if (reMsg == "")
        {
            //if (!prepareUserList.Contains(id))
            //    prepareUserList.Add(id);
            SendMsg("userId_" + prepareUserList.Count.ToString());
            return;
        }
        ServerRuner.QueueOnMainThread(reMsg, new UnityServerCallback((rt) => { Debug.Log("消息异步处理"); }, reMsg));
    }



    public void SendMsg(string msg)
    {

        foreach (var guid in guidList)
        {
            try
            {
                server.SendMsg(guid, msg);
            }
            catch
            {
                isSuccess = false;
                Debug.Log("启动服务器失败");
            }
        }
        //处理心跳包
        //if (msg == "0")
        //{
        //    foreach (var guid in guidList)
        //    {
        //        server.SendMsg(guid, msg);
        //    }
        //}
        //else
        //{
        //    foreach (var guid in prepareUserList)
        //    {
        //        server.SendMsg(guid, msg);
        //    }
        //}

    }

    void ClientClose(Guid guid)
    {
        guidList.Remove(guid);
        Debug.Log("客户端关闭: " + guid);
    }

    private void OnDestroy()
    {
        if (null != server)
        {
            server.Stop();
            server.Dispose();
        }
    }

}

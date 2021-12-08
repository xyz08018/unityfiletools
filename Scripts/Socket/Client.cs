using DwGoing.Socket;
using System.Threading;
using UnityEngine;

public class Client : MonoBehaviour
{
    //用于控制相机角度client
    public static Client Instance;
    SocketClient client;

    float timr = 2f;
    Thread thread_Conect_Reset;
    Thread thread_Conect_Heart;

    private bool isSuccess;

    public void Awake()
    {
        if (null == Instance)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }
    private void Start()
    {
        isDes = false;
        Init();
        thread_Conect_Reset = new Thread(new ThreadStart(() =>
        {
            ResetConnectThread();
        }));
        thread_Conect_Reset.Start();

        thread_Conect_Heart = new Thread(new ThreadStart(() =>
        {
            SendHeart();
        }));
        thread_Conect_Heart.Start();
    }

    private void OnDestroy()
    {
        isDes = true;
        if (null != thread_Conect_Heart)
        {
            thread_Conect_Heart.Abort();
            thread_Conect_Heart = null;
        }
        if (null != thread_Conect_Reset)
        {
            thread_Conect_Reset.Abort();
            thread_Conect_Reset = null;
        }
    }

    private void Init()
    {
        client = null;
        client = new SocketClient("", 8000);//"192.168.64.223", 9527
        client.CB_Connect = () => { Debug.Log("连接图片坐标服务器成功");  /*当连接Server时*/ };
        client.CB_Receive = (msg) => { ReceiveMsg(msg);/*当接收到消息时*/ };
        client.CB_SendMsg = () => { /*当发送消息时*/ };
        client.CB_Disconnect = () => { /*当断开连接时*/ };
        client.CB_ServerClose = () => { /*当Server断开时*/ };

        client.Connect();
    }

    private void SendHeart()
    {
        while (true)
        {
            Thread.Sleep(3000);
            //心跳包
            client.SendMsg("00");
            isSuccess = false;
        }
    }

    public void ResetConnectThread()
    {
        while (true)
        {
            Thread.Sleep(5000);
            if (!client.IsConnected)// || !isSuccess)
            {
                Debug.LogError("client断线重连");
                Init();
            }
        }
    }
    bool isDes = false;

    void ReceiveMsg(string reMsg)
    {
        if (isDes || !client.IsConnected)
            return;
        Debug.Log("相机client客户端接收消息： " + reMsg);
        if (reMsg == "0")
        {
            isSuccess = true;
            Debug.Log("心跳包");
            return;
        }
        ServerRuner.QueueOnMainThread(reMsg, new UnityServerCallback((rt) => { }, reMsg));
    }

    public void SendMsg(string msg)
    {
        Debug.LogError("相机client发送消息：" + msg);
        // char[] data = new char[] { '1', '2' };
        client.SendMsg(msg);
        //client.SendMsg(data.ToString());
    }
}

using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

/*
Room_3501
Room_3502
Room_3503
Room_3504
*/
public class SocketMgr : MonoSingleton<SocketMgr>
{
    [DllImport("wininet")]
    private extern static bool InternetGetConnectedState(out int connectionDescription, int reservedValue);

    Socket clientSocket;
    bool isLogin;
    private static byte[] result = new byte[4096];
    bool isDes;
    private string recvStr;
    const int buffer_Size = 4096;
    private byte[] readBuff = new byte[buffer_Size];
    IPAddress ip;
    Thread thread_Conect;
    Thread thread_HeartbeatRuest;

    Thread thread_Conect_Reset;

    string ipStr;
    int port;
    string userId;
    private void Start()
    {
        ConfigInfo config = Config.Instance.Data;
        ipStr = config.ip; //"192.168.64.48";// ;
        port = config.port; //8081;// ;
        userId = config.userId;// "Room_3501";// ;
        isLogin = true;
        ip = IPAddress.Parse(ipStr);//192.168.64.19

        thread_Conect = new Thread(new ThreadStart(ConnectSocket));
        thread_Conect.Start();
        thread_HeartbeatRuest = new Thread(new ThreadStart(() =>
        {
            SendHeartbeatThread();
        }));
        thread_HeartbeatRuest.Start();

        thread_Conect_Reset = new Thread(new ThreadStart(() =>
        {
            ResetConnectThread();
        }));
        thread_Conect_Reset.Start();
        //ConnectSocket();
    }

    public void SendHeartbeatThread()
    {
        while (true)
        {
            Thread.Sleep(3000);
            if (clientSocket.Connected)
            {
                HeartbeatRuest();
            }
        }
    }

    public void ResetConnectThread()
    {
        while (true)
        {
            Thread.Sleep(5000);
            if (!clientSocket.Connected)
            {
                Debug.LogError("断线重连");
                ConnectSocket();
            }
        }
    }

    /// <summary>
    /// C#判断是否联网
    /// </summary>
    /// <returns></returns>
    public bool IsConnectedInternet()
    {
        int i = 0;
        if (InternetGetConnectedState(out i, 0))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void ConnectSocket()
    {
        Debug.Log("连接请求");
        if (IsConnectedInternet())//; IsConnect.NetAvailable)
        {
            try
            {
                Debug.Log("ip: " + ip + "     port: " + port + "    userId: " + userId);
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientSocket.Connect(new IPEndPoint(ip, port));
                if (clientSocket.Connected)
                {
                    Debug.Log("连接服务器成功");
                    clientSocket.BeginReceive(readBuff, 0, buffer_Size, SocketFlags.None, ReceiveCb, clientSocket);
                    //主动登录检测
                    if (isLogin)
                    {
                        isLogin = false;
                        ClientLogin();
                    }
                }
                else
                {
                    Debug.Log("连接服务器失败！");
                }
            }
            catch (Exception e)
            {
                Debug.Log("ggggggggg: " + e.Message);
            }

        }
    }

    public void ClientLogin()
    {
        Debug.Log("登录");
        toc_login login = new toc_login();
        login.msgType = "0";
        login.userId = userId;
        login.userType = "2";
        string loginJson = LitJson.JsonMapper.ToJson(login);
        SendMsg(loginJson);
    }

    public void HeartbeatRuest()
    {
        if (thread_Conect != null)
        {
            thread_Conect.Abort();
            thread_Conect = null;
        }
        Debug.Log("心跳包");
        toc_login login = new toc_login();
        login.msgType = "1";
        login.userId = userId;
        login.userType = "2";
        string heartbeatJson = LitJson.JsonMapper.ToJson(login);
        SendMsg(heartbeatJson);
    }

    float timr = 0;
    float timrCont = 0;
    private void Update()
    {
        //timr += Time.deltaTime;
        //if (timr >= 3 && clientSocket.Connected)
        //{
        //    HeartbeatRuest();
        //    timr = 0;
        //}
        //if (!clientSocket.Connected)
        //{
        //    timrCont += Time.deltaTime;
        //    if (timrCont >= 5)
        //    {
        //        Debug.Log("正在重连socket");
        //        timrCont = 0;
        //        //重连
        //        ConnectSocket();
        //    }
        //}
    }

    /// <summary>
    /// Socket 发送消息
    /// </summary>
    /// <param name="strmsg">消息</param>
    public void SendMsg(string strmsg)
    {
        string tos_msg = strmsg + "$_";
        Debug.Log("发送消息: " + tos_msg);
        try
        {
            byte[] msg = System.Text.Encoding.UTF8.GetBytes(tos_msg);
            clientSocket.Send(msg);
        }
        catch (Exception ex)
        {
            clientSocket.Close();
            Debug.Log(ex.Message);
        }
    }

    public void ReceiveCb(IAsyncResult ar)
    {
        if (isDes || !clientSocket.Connected)
            return;
        try
        {
            int count = clientSocket.EndReceive(ar);
            //"$_"
            string strMsg = System.Text.Encoding.UTF8.GetString(readBuff, 0, count).Split('$')[0];
            if (!string.IsNullOrEmpty(strMsg))
            {
                Debug.Log(strMsg);
                Debug.Log("接收消息处理");
            }
            clientSocket.BeginReceive(readBuff, 0, buffer_Size, SocketFlags.None, ReceiveCb, clientSocket);
            if (!string.IsNullOrEmpty(strMsg))
            {
                ServerRuner.QueueOnMainThread(strMsg, new UnityServerCallback((rt) => { }, strMsg));
            }
        }
        catch (SocketException ex)
        {
            Debug.Log(ex);
        }
    }
    private void OnDestroy()
    {
        isDes = true;
        if (clientSocket != null && clientSocket.Connected)
            clientSocket.Disconnect(false);
        if (thread_Conect != null)
        {
            thread_Conect.Abort();
            thread_Conect = null;
        }
        if (thread_Conect_Reset != null)
        {
            thread_Conect_Reset.Abort();
            thread_Conect_Reset = null;
        }
        if (thread_HeartbeatRuest != null)
        {
            thread_HeartbeatRuest.Abort();
            thread_HeartbeatRuest = null;
        }
    }
}

public class toc_login
{
    public string msgType;
    public string userId;
    public string userType;
}

public class Message
{
    public byte Type;
    public int Id;
    public object Content;

    public Message() { }

    public Message(byte type, int id, object content)
    {
        Type = type;
        Id = id;
        Content = content;
    }
}

public class MessageType
{
    public static byte Type_Stream = 1;
    public static byte Type_UI = 2;
    public static byte Type_Scene = 3;
    public static byte Type_WebSocket = 4;
}

public class MessageCmd
{
    public static int Strteam_Refresh = 100;
    public static int Strteam_Close = 101;

    public static int UI_Refresh = 200;
    public static int UI_Login = 201;
    public static int UI_CloseLogin = 202;

    public static int UI_Cancle_Queue = 301;

    public static int UI_Change_Room = 302;

    public static int UI_Change_BUCKET = 303;

    public static int UI_Cancel_BUCKET = 304;

    public static int UI_Time_HTREETOONE = 305;//3-1倒计时

    //   public static int GameOver = 306;

    public static int WebSocket_Foam_CallBack = 307;   //泡沫整蛊按键返回

    public static int UI_MX_ExitGame = 308;
    public static int UI_QT_ExitGame = 309;


    public static int WebSocket_QUEUE_CHANGE = 10001;   //排队信息变化
    public static int WebSocket_ROOM_GAME_START = 10002;//压水桶游戏开始
    public static int WebSocket_ROOM_GAME_END = 10003;  //压水桶游戏结束
    public static int WebSocket_WATER_CHANGE = 10004;   //压水桶水量变化通知
    public static int WebSocket_PROJECT_CHANGE = 10005; //项目状态变更
    public static int WebSocket_QUEUE_CALL = 10006;     //叫号通知
    public static int WebSocket_QUEUE_INTO = 10007;     //压水桶第二个进入第一个人接到通知

    //迷旋
    public static int WebSocket_MX_ROOM_CHANGE = 10008;//房间状态变更
    public static int WebSocket_MX_IN_USER = 10009;    //房间线下用户来人了
    public static int WebSocket_MX_PRO_CHANGE = 10010; //项目状态变更
    public static int WebSocket_MX_ROOM_BARRAGE = 10011;//弹幕信息

}

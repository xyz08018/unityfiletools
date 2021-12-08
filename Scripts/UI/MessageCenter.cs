using System.Collections.Generic;
using UnityEngine;

public class MessageCenter
{
    public static List<PanelBase> Managers = new List<PanelBase>();

    public static void SendMessage(Message msg)
    {
        Debug.Log("PanelBaseList:" + Managers.Count.ToString());
        for (int i = 0; i < Managers.Count; i++)
        {
            PanelBase mb = Managers[i];
            if (mb != null)
            {
                mb.ReceiveMessage(msg);
            }
            else
            {
                //               Debug.LogError("没有找到这个PanelBase    ");
            }
        }
    }

    public static void SendMessage(byte type, int cmd, object content)
    {
        Message msg = new Message(type, cmd, content);
        SendMessage(msg);
    }
}

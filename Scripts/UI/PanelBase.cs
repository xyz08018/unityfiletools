using System.Collections.Generic;
using UnityEngine;

public class PanelBase : MonoBehaviour
{
    /// <summary>
    /// 面板对象
    /// </summary>
    [HideInInspector]
    public GameObject panelObj;
    //层级
    public PanelLayer layer;
    //面板参数
    public object[] args;
    [HideInInspector]

    public List<PanelBase> ReceiveList = new List<PanelBase>();

    protected byte messageType;
    #region 生命周期
    //初始化
    public virtual void Init(params object[] args)
    {
        this.args = args;
        string name = this.GetType().ToString();
        //       Debug.Log("Panel name: " + name + "   args Length: " + args.Length);
    }
    //开始面板前
    public virtual void OnShowing() { }
    //显示面板后
    public virtual void OnShowed() { }
    //帧更新
    //public virtual void Update() { }
    //关闭前

    public virtual void OnClosing() { }
    //关闭后
    public virtual void OnClosed() { }

    #endregion

    protected virtual void Awake()
    {
        messageType = SetMessageType();
        MessageCenter.Managers.Add(this);
    }
    // uI 接收消息
    public virtual void ReceiveMessage(Message message)
    {
        if (message.Type != messageType)
            return;
        foreach (PanelBase mb in ReceiveList)
        {
            if (mb != null)
                mb.ReceiveMessage(message);
        }
    }
    protected virtual byte SetMessageType()
    {
        return MessageType.Type_UI;
    }

    // 把ui 加到list 里面
    public void RegisterReceiver(PanelBase mb)
    {
        if (!ReceiveList.Contains(mb))
        {
            ReceiveList.Add(mb);
        }
    }

    public void RemoveReceiver(PanelBase mb)
    {
        if (ReceiveList.Contains(mb))
            ReceiveList.Remove(mb);
    }

    public void RemoveAllReceiver()
    {
        while (ReceiveList.Count > 0)
        {
            int index = -1;
            index++;
            ReceiveList.Remove(ReceiveList[index]);
        }
    }

    // 派发消息
    protected void DispatchMessage(Message message)
    {
        MessageCenter.SendMessage(message);
    }

    protected void DispatchMessage(byte type, int id, object content)
    {
        MessageCenter.SendMessage(type, id, content);
    }

    #region 操作
    /// <summary>
    /// 激活面板
    /// </summary>
    protected virtual void Active()
    {
        string name = this.GetType().ToString();
        PanelMgr.GetInstance().ActivePanel(name);
    }
    /// <summary>
    /// 隐藏面板
    /// </summary>
    protected virtual void Disable()
    {
        string name = this.GetType().ToString();
        PanelMgr.GetInstance().DisablePanel(name);
    }
    /// <summary>
    /// 关闭面板
    /// </summary>
    protected virtual void Close()
    {
        string name = this.GetType().ToString();
        RemoveReceiver(this);

        PanelMgr.GetInstance().ClosePanel(name);
    }

    void Close(string name)
    {
        PanelMgr.GetInstance().ClosePanel(name);
    }

    public virtual void Close(float t)
    {
        Invoke("Close", t);
    }

    #endregion
}

/// <summary>
/// 面板层
/// </summary>
public enum PanelLayer
{
    /// <summary>
    /// 面板
    /// </summary>
    Panel,
    /// <summary>
    /// 直播
    /// </summary>
    Stream,
    /// <summary>
    /// 提示
    /// </summary>
    Tips,
}

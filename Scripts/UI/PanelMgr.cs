using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PanelMgr : MonoBehaviour
{
    private GameObject canvas;
    public Dictionary<string, PanelBase> dict;
    private Dictionary<PanelLayer, Transform> layerDict;
    private static PanelMgr _Instance;
    public static PanelMgr GetInstance()
    {
        if (_Instance == null)
        {
            _Instance = new GameObject("_PanelMgr").AddComponent<PanelMgr>();
        }
        return _Instance;
    }
    public void Awake()
    {
        _Instance = this;
        InitLayer();
        dict = new Dictionary<string, PanelBase>();
    }

    private void InitLayer()
    {
        //Canvas
        canvas = GameObject.Find("Canvas");
        if (canvas == null)
        {
            Debug.LogError("panelMgr.InitLayer fail, canvas is null");
        }
        //Layer
        layerDict = new Dictionary<PanelLayer, Transform>();
        foreach (PanelLayer pl in Enum.GetValues(typeof(PanelLayer)))
        {
            string name = pl.ToString();
            Transform transform = canvas.transform.Find(name);
            layerDict.Add(pl, transform);
        }
    }

    /// <summary>
    /// Open Panel
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <param name="args"></param>
    public PanelBase OpenPanel<T>(string path, params object[] args) where T : PanelBase
    {
        string panelName = typeof(T).ToString();
        var pa = FindObjectOfType<T>();
        if (pa == null)
            dict.Remove(panelName);
        Debug.LogWarning("panelName:" + panelName);
        if (dict.ContainsKey(panelName))
        {
            PanelBase p;
            if (dict.TryGetValue(panelName, out p))
            {
                //if (null == p.panelObj)
                //{
                //    return null
                //}
                if (!p.panelObj.activeSelf)
                {
                    p.panelObj.SetActive(true);
                    p = p.panelObj.GetComponent<T>();
                    p.Init(args);
                }
                return p;
            }
        }
        var obj = ResMgr.GetInstance().LoadPrefad(Path.Combine(path, panelName), true);
        PanelBase panel = obj.GetComponent<T>();
        panel.Init(args);
        dict.Add(panelName, panel);
        panel.panelObj = obj;
        panel.panelObj.name = panelName;
        Transform skinTrans = panel.panelObj.transform;
        PanelLayer layer = panel.layer;
        Transform parent = layerDict[layer];
        skinTrans.SetParent(parent, false);
        //panel
        panel.OnShowing();
        //anm
        panel.OnShowed();

        return panel;
    }

    public void ClosePanel(string panelName)
    {
        if (string.IsNullOrEmpty(panelName))
            if (!dict.ContainsKey(panelName))
            {
                return;
            }
        PanelBase panel;
        if (dict.TryGetValue(panelName, out panel))
        {
            panel.OnClosing();
            dict.Remove(panelName);
            panel.OnClosed();
            GameObject.Destroy(panel.panelObj);
            Component.Destroy(panel);
            Resources.UnloadUnusedAssets();
        }
        else
        {
            Debug.Log("ClosePanel 没有找到当前Panel: " + panelName);
        }

    }

    //Close Panel
    public void ClosePanel<T>() where T : PanelBase
    {
        string panelName = typeof(T).ToString();
        if (string.IsNullOrEmpty(panelName))
            if (!dict.ContainsKey(panelName))
            {
                return;
            }
        PanelBase panel = (PanelBase)dict[panelName];
        if (panel == null)
        {
            return;
        }
        panel.OnClosing();
        dict.Remove(panelName);
        panel.OnClosed();
        GameObject.Destroy(panel.panelObj);
        Component.Destroy(panel);
        Resources.UnloadUnusedAssets();
    }

    /// <summary>
    /// Active Panel
    /// </summary>
    /// <param name="name"></param>
    public void ActivePanel(string name)
    {
        PanelBase panel = (PanelBase)dict[name];
        if (panel == null)
        {
            return;
        }
        panel.panelObj.SetActive(true);
    }

    public void ActivePanel<T>() where T : PanelBase
    {
        string panelName = typeof(T).ToString();
        PanelBase panel = (PanelBase)dict[panelName];
        if (panel == null)
        {
            return;
        }
        panel.panelObj.SetActive(true);
    }

    public void DisablePanel(string name)
    {
        PanelBase panel = (PanelBase)dict[name];
        if (panel == null)
        {
            return;
        }
        panel.panelObj.SetActive(false);
    }

    /// <summary>
    /// Hide Panel
    /// </summary>
    /// <param name="name"></param>
    public void DisablePanel<T>() where T : PanelBase
    {
        string panelName = typeof(T).ToString();
        PanelBase panel = (PanelBase)dict[panelName];
        if (panel == null)
        {
            return;
        }
        panel.panelObj.SetActive(false);
    }

}

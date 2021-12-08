using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PerformanceListMgr : Singleton<PerformanceListMgr>
{
    public List<Performance> m_UserData = new List<Performance>();
    private string jsonDirPath;
    private string jsonFilePath;
    public void SaveUserScore(Performance performance)
    {
        //System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        jsonDirPath = string.Format("{0}/Score", System.Environment.CurrentDirectory);
        jsonFilePath = string.Format("{0}/{1}.json", jsonDirPath, "ScoreLevel" + System.DateTime.Now.ToString("yyyy-MM-dd"));
        if (!Directory.Exists(jsonDirPath))
            Directory.CreateDirectory(jsonDirPath);
        if (!File.Exists(jsonFilePath))
        {
            FileStream fs = File.Create(jsonFilePath);
            fs.Close();
            fs.Dispose();
        }
        List<Performance> performances = new List<Performance>();
        var olddata = LitJson.JsonMapper.ToObject<List<Performance>>(File.ReadAllText(jsonFilePath));
        if (null != olddata)
            performances = olddata;
        performances.Add(performance);
        string jsonCont = LitJson.JsonMapper.ToJson(performances);
        File.WriteAllText(jsonFilePath, jsonCont);
        Debug.Log("保存分数成功");
    }

    public List<Performance> LoadUserScore()
    {
        if (m_UserData.Count > 0)
            m_UserData.Clear();
        jsonDirPath = string.Format("{0}/Score", System.Environment.CurrentDirectory);
        jsonFilePath = string.Format("{0}/{1}.json", jsonDirPath, "排行榜" + System.DateTime.Now.ToString("yyyy-MM-dd"));
        if (!Directory.Exists(jsonDirPath) || !File.Exists(jsonFilePath))
        {
            Debug.Log("文件或路径不存在：GGGG");
            return null;
        }
        m_UserData = LitJson.JsonMapper.ToObject<List<Performance>>(File.ReadAllText(jsonFilePath));
        return m_UserData;
    }
}

public class Performance
{
    public string name;
    public string level;
    public string score;
    public string time;
}

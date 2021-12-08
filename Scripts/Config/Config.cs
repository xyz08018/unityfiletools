using System.IO;
using UnityEngine;
public class Config
{
    static ConfigInfo info;

    static Config instance;

    public static Config Instance
    {
        get
        {
            if (null == instance)
            {
                instance = new Config();
                LoadJson();
            }
            return instance;
        }
    }

    public ConfigInfo Data
    {
        get { return info; }
    }


    static void LoadJson()
    {
        string path = Application.streamingAssetsPath + "/Config.json";

        //Debug.LogError("JsonPath : " + path);

        if (File.Exists(path))
        {
            info = LitJson.JsonMapper.ToObject<ConfigInfo>(File.ReadAllText(path)); // JsonConvert.DeserializeObject<ConfigInfo>(File.ReadAllText(path));
        }
    }
}

public class ConfigInfo
{
    public int width;

    public int height;

    public int fullscreen;

    public string ip;

    public int port;

    public string userId;
}

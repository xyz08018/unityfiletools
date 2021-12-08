using System.Collections;
using UnityEngine;

public class ResMgr : MonoBehaviour
{

    private static ResMgr _Instance;
    private Hashtable ht = null;
    public static ResMgr GetInstance()
    {
        if (_Instance == null)
        {
            _Instance = new GameObject("_ResourceMgr").AddComponent<ResMgr>();
        }
        return _Instance;
    }

    void Awake()
    {
        ht = new Hashtable();
    }

    /// <summary>
    /// Load T Resources
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <param name="isCatch"></param>
    /// <returns></returns>
    public T LoadResource<T>(string path, bool isCatch) where T : UnityEngine.Object
    {
        if (ht.Contains(path))
        {
            return ht[path] as T;
        }

        T TResource = Resources.Load<T>(path);
        if (TResource == null)
        {
            Debug.LogError(GetType() + "/GetInstance()/TResource 提取的资源找不到，请检查。 path=" + path);
        }
        else if (isCatch)
        {
            ht.Add(path, TResource);
        }

        return TResource;
    }

    /// <summary>
    /// Load GameObject
    /// </summary>
    /// <param name="path"></param>
    /// <param name="isCatch"></param>
    /// <returns></returns>
    public GameObject LoadPrefad(string path, bool isCatch)
    {
        var goObj = LoadResource<GameObject>(path, isCatch);
        var goObjClone = GameObject.Instantiate<GameObject>(goObj);
        if (goObjClone == null)
        {
            Debug.LogError(GetType() + "/LoadAsset()/克隆资源不成功，请检查。 path=" + path);
        }
        return goObjClone;
    }

    public Sprite GetSprite(string path, bool isCatch, string spName = null)
    {
        string loadPath = path + spName;
        var sprite = LoadResource<Sprite>(loadPath, isCatch);
        if (sprite == null)
        {
            Debug.LogError(GetType() + "/LoadAsset()/克隆资源不成功，请检查。 path=" + path);
        }
        return sprite;
    }

    public Texture2D GetTexture(string path, bool isCatch)
    {
        var texture = LoadResource<Texture2D>(path, isCatch);
        if (texture == null)
        {
            Debug.LogError(GetType() + "/LoadAsset()/克隆资源不成功，请检查。 path=" + path);
        }
        return texture;
    }

}

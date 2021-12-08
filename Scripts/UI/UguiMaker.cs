using UnityEngine;
using UnityEngine.UI;

public class UguiMaker// : MonoBehaviour
{

    public static GameObject InitGameObj(GameObject tar, Transform _parent, string _name, Vector3 _pos, Vector3 _scale)
    {
        RectTransform rtran = tar.GetComponent<RectTransform>();
        if (null == rtran)
            rtran = tar.AddComponent<RectTransform>();

        tar.name = _name;
        tar.transform.SetParent(_parent);
        rtran.anchoredPosition3D = _pos;
        tar.transform.localScale = _scale;

        return tar;
    }

    public static GameObject newGameObject(string n, Transform p)
    {
        RectTransform rtran = new GameObject().AddComponent<RectTransform>(); ;
        rtran.gameObject.name = n;
        rtran.SetParent(p);
        rtran.localScale = Vector3.one;
        rtran.anchoredPosition3D = Vector3.zero;
        return rtran.gameObject;
    }

    public static GameObject newGameObject(string n, Transform p, Vector3 _pos, Vector3 _scale)
    {
        GameObject obj = newGameObject(n, p);
        InitGameObj(obj, p, n, _pos, _scale);
        return obj;
    }

    public static T newGameObject<T>(string _name, Transform _parent) where T : Component
    {
        RectTransform rtran = new GameObject().AddComponent<RectTransform>(); ;
        rtran.gameObject.name = _name;
        rtran.SetParent(_parent);
        rtran.localScale = Vector3.one;
        rtran.anchoredPosition3D = Vector3.zero;
        rtran.gameObject.layer = LayerMask.NameToLayer("UI");
        return rtran.gameObject.AddComponent<T>();
    }


    public static Image newImage(string _name, string _path, Transform _parent, bool _isCatch = true, bool _raycast = true)
    {
        GameObject obj = newGameObject(_name, _parent);
        Image result = obj.AddComponent<Image>();
        result.sprite = ResMgr.GetInstance().GetSprite(_path, _isCatch);

        result.SetNativeSize();
        result.raycastTarget = _raycast;
        return result;
    }

    public static RawImage newRawImage(string _name, string _path, Transform _parent, bool _isCatch = true, bool _raycast = true)
    {
        GameObject obj = newGameObject(_name, _parent);
        RawImage result = obj.AddComponent<RawImage>();
        result.texture = ResMgr.GetInstance().GetTexture(_path, _isCatch);

        result.SetNativeSize();
        result.raycastTarget = _raycast;
        return result;
    }

    public static Button newButton(string _name, string _path, Transform _parent, bool _isCatch = true, bool _raycast = true)
    {
        Image img = newImage(_name, _path, _parent, _isCatch, _raycast);
        Button btn = img.gameObject.AddComponent<Button>();

        return btn;
    }

    public static Text newText(string _name, Transform _parent)
    {
        GameObject obj = newGameObject(_name, _parent);
        Text text = obj.gameObject.AddComponent<Text>();
        //text.font = Game.instance.m_PingFang_Bold;
        return text;

    }

    public static RectTransform setOffect(RectTransform rtran, float minx, float miny, float maxx, float maxy)
    {
        rtran.offsetMin = new Vector2(minx, miny);
        rtran.offsetMax = new Vector2(maxx, maxy);
        return rtran;
    }

    public static void newGameObjects(Transform p, params object[] param)
    {
        for (int i = 0; i < param.Length; i++)
        {
            newGameObject(param[i].ToString(), p).gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(
                1080, 1920);
        }
    }

}

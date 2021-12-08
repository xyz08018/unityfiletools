using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TipServer : MonoBehaviour
{
    public static TipServer Create()
    {
        var parent = GameObject.Find("Canvas").transform;
        var plane = UguiMaker.newGameObject<TipServer>("TipServer", parent);
        plane.mBg = UguiMaker.newImage("mBg", "Sprite/tip_bg", plane.transform);
        plane.mBg.type = Image.Type.Sliced;
        plane.mBg.color = new Color(0, 0, 0, 0.5f);

        plane.mText = UguiMaker.newText("mText", plane.transform);
        plane.mText.fontSize = 40;
        plane.mText.color = Color.white;
        plane.mText.alignment = TextAnchor.MiddleCenter;
        plane.mText.horizontalOverflow = HorizontalWrapMode.Overflow;
        return plane;
    }
    public static TipServer ShowTipServer_Code(ServerCode code)
    {
        string msg = string.Empty;
        switch (code)
        {
            case ServerCode.Failed:
                msg = "请求失败";
                break;

            case ServerCode.Success:
                msg = "请求成功";
                break;

            case ServerCode.AccountOrPwdError:
                msg = "个人中心帐号或密码错误";
                break;

            case ServerCode.MissingParam:
                msg = "参数缺失";
                break;

            case ServerCode.Exist:
                msg = "数据已存在";
                break;

            case ServerCode.NotExist:
                msg = "数据不存在";
                break;

            case ServerCode.VerifyError:
                msg = "验证码错误";
                break;

            case ServerCode.Unauthorized:
                msg = "未授权";
                break;

            case ServerCode.ServerError:
                msg = "服务器异常";
                break;

        }
        if (string.IsNullOrEmpty(msg))
            return null;
        var plane = Create();
        plane.SetMsg(msg);
        return plane;
    }
    public static bool NetAvailable()
    {
        if (true)//IsConnect.NetAvailable)
        {
            return true;
        }
        else
        {
            Create().SetMsg("没有网络！~");
            return false;
        }

    }


    public Image mBg;
    public Text mText;

    IEnumerator Start()
    {
        //yield break;
        for (float i = 0; i < 1f; i += Time.deltaTime)
        {
            float p = Mathf.Sin(Mathf.PI * 0.5f * i);
            mBg.color = Color.Lerp(new Color(0, 0, 0, 0), new Color(0, 0, 0, 0.5f), p);
            mText.color = Color.Lerp(new Color(1, 1, 1, 0), Color.white, p);
            yield return new WaitForEndOfFrame();
        }
        mBg.color = new Color(0, 0, 0, 0.5f);
        mText.color = Color.white;
        yield return new WaitForSeconds(1f);
        Vector3 pos0 = transform.localPosition;
        Vector3 pos1 = pos0 + new Vector3(0, 200, 0);
        for (float i = 0; i < 1f; i += Time.deltaTime)
        {
            float p = Mathf.Sin(Mathf.PI * 0.5f * i);
            mBg.color = Color.Lerp(new Color(0, 0, 0, 0.5f), new Color(1f, 1f, 1f, 0), p);
            mText.color = Color.Lerp(Color.white, new Color(1, 1, 1, 0), p);
            transform.localPosition = Vector3.Lerp(pos0, pos1, p);
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);

    }
    public void SetMsg(string msg, bool isVScreen = false)
    {

        if (string.IsNullOrEmpty(msg))
            return;
        mBg.rectTransform.sizeDelta = new Vector2(20 + 40 * msg.Length, 70);
        mText.text = msg;
        mText.fontSize = 30;
        mText.color = Color.black;
        //mText.font = Global.Instance.m_PingFang_Bold;
        if (isVScreen)
            transform.localEulerAngles = new Vector3(0, 0, 90);
    }

}

public enum ServerCode
{
    Failed,
    Success,
    AccountOrPwdError,
    MissingParam,
    Exist,
    NotExist,
    VerifyError,
    Unauthorized,
    ServerError
}

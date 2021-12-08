using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TipDataServer : MonoBehaviour
{
    public Image mBg;
    public static TipDataServer Create()
    {
        var parent = GameObject.Find("Canvas").transform;
        var plane = UguiMaker.newGameObject<TipDataServer>("TipServer", parent);
        plane.mBg = UguiMaker.newImage("mBg", "Sprite/quiz_begin_tip", plane.transform);
        plane.transform.localScale = Vector3.zero;
        plane.transform.localPosition = new Vector3(0, 150, 0);
        //plane.transform.Otc_ScaleZero2One(1f);
        return plane;
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}

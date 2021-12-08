using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteFrame : MonoBehaviour
{
    int m_tempIndex = 0;
    int m_delayUpdate = 1;
    Image m_image;
    List<Sprite> m_spriteList = new List<Sprite>();
    private List<Sprite> animationSprites = new List<Sprite>();
    private int AnimationAmount { get { return animationSprites.Count; } }
    public void SetData(string path, string sprite_name_head, int begin_index, int end_index, int delay_update)
    {
        m_delayUpdate = delay_update;
        m_image = gameObject.GetComponent<Image>();
        for (int i = begin_index; i <= end_index; i++)
        {
            string sprite_name_end = "";
            if (i < 10)
                sprite_name_end = "0" + i.ToString();
            else
                sprite_name_end = i.ToString();
            var sprite = ResMgr.GetInstance().GetSprite(path, false, sprite_name_head + sprite_name_end);
            if (null != sprite)
                m_spriteList.Add(sprite);
        }
    }
    void LateUpdate()
    {
        if (null == m_image || m_spriteList.Count == 0)
            return;
        if (Time.frameCount % m_delayUpdate != 0)
            return;
        m_image.sprite = m_spriteList[m_tempIndex];
        m_tempIndex = (m_tempIndex + 1) % m_spriteList.Count;
    }

    public IEnumerator PlayAnimationForwardIEnum(Image image, bool isLoop)
    {
        int index = 0;
        image.gameObject.SetActive(true);
        image.sprite = null;
        while (true)
        {
            if (index > AnimationAmount - 1)
            {
                if (isLoop)
                    index = 0;
                else
                    break;
            }
            image.sprite = animationSprites[index];
            index++;
            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator LoadFrame(string path, string sprite_name_head, int begin_index, int end_index)
    {
        yield return new WaitForEndOfFrame();
        for (int i = begin_index; i <= end_index; i++)
        {
            string sprite_name_end = "";
            if (i < 10)
                sprite_name_end = "0" + i.ToString();
            else
                sprite_name_end = i.ToString();
            var sprite = ResMgr.GetInstance().GetSprite(path, false, sprite_name_head + sprite_name_end);
            if (null != sprite)
                animationSprites.Add(sprite);
        }
    }

}

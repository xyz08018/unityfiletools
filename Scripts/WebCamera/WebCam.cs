using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WebCam : MonoSingleton<WebCam>
{
    [HideInInspector]
    public WebCamTexture camTexture;
    CanvasScaler CanScaler;
    Camera ca;
    public RawImage img;
    Coroutine co;
    public void InitCamera()
    {
        if (co != null)
            StopCoroutine(co);
        CanScaler = GetComponentInParent<CanvasScaler>();
        CanScaler.referenceResolution = new Vector2(Screen.width, Screen.height);
        ca = Camera.main;
        ca.orthographicSize = Screen.width / 100.0f / 2.0f;
        img.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        img.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        img.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        float width = Screen.height / 9 * 16;
        img.rectTransform.sizeDelta = new Vector2(width, Screen.height);
        //img.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 3072);
        //img.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1728);
        Coroutine c = StartCoroutine(CallCamera());
    }

    IEnumerator CallCamera()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            if (camTexture != null)
                camTexture.Stop();

            WebCamDevice[] cameraDevices = WebCamTexture.devices;
            string deviceName = cameraDevices[0].name;

            camTexture = new WebCamTexture(deviceName, 1920, 1080, 30);
            img.canvasRenderer.SetTexture(camTexture);

            camTexture.Play();
        }
    }
}

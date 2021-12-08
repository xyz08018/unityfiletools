//===========================================================================
//===================== 与服务器通信 =========================================
//===========================================================================
using LitJson;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using UnityEngine;
public class ServerManager : MonoSingleton<ServerManager>
{
    public string urlHead;
    public Action ResetAutoLogin;

    void Awake()
    {
        JsonSerializerSettings setting = new JsonSerializerSettings();
        JsonConvert.DefaultSettings = new System.Func<JsonSerializerSettings>(() =>
        {
            //空值处理
            setting.NullValueHandling = NullValueHandling.Ignore;
            return setting;
        });
    }

    #region 访问HTTPS 设置
    private void Start()
    {
        //访问https 需要添加以下安全证书设置 http不用
        //if (Global.Instance.IsBuildTestPage)
        //    ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;

    }
    public bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        bool isOk = true;
        if (sslPolicyErrors != SslPolicyErrors.None)
        {
            for (int i = 0; i < chain.ChainStatus.Length; i++)
            {
                if (chain.ChainStatus[i].Status == X509ChainStatusFlags.RevocationStatusUnknown)
                {
                    continue;
                }
                chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                bool chainIsValid = chain.Build((X509Certificate2)certificate);
                if (!chainIsValid)
                {
                    isOk = false;
                    break;
                }
            }
        }
        return isOk;
    }
    #endregion

    #region GET 

    public toc_default<T> Get<T>(string serverUrl, string token)
    {
        using (WebClient client = new WebClient())
        {
            try
            {
                client.Headers["Content-Type"] = "application/json";
                client.Encoding = System.Text.Encoding.UTF8;
                if (!string.IsNullOrEmpty(token)) client.Headers["token"] = token;
                var url = urlHead + serverUrl;
                if (Global.Instance.IsDebugPostUrl) Debug.Log(url);
                var result = client.DownloadString(url);
                if (Global.Instance.IsDebugPostUrl) Debug.Log(result);
                var resultjson = JsonMapper.ToObject<toc_default<T>>(result);
                //重新登录
                if (resultjson.code == 401)
                {
                    ResetAutoLogin?.Invoke();
                }
                return resultjson;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            return default(toc_default<T>);
        }
    }

    public toc_default<T> Get<T>(string serverUrl, tos_default tos, string token = null)
    {
        using (WebClient client = new WebClient())
        {
            try
            {
                client.Headers["Content-Type"] = "application/json";
                client.Encoding = System.Text.Encoding.UTF8;
                if (!string.IsNullOrEmpty(token)) client.Headers["token"] = token;
                Regex reg = new Regex("\u200b/g");
                var url = reg.Replace(urlHead + serverUrl + tos.ToString(), "");
                if (Global.Instance.IsDebugPostUrl) Debug.Log(url);
                var result = client.DownloadString(url);
                if (Global.Instance.IsDebugPostUrl) Debug.Log("json: jjjjj" + result);
                var resultjson = JsonMapper.ToObject<toc_default<T>>(result);
                //重新登录
                if (resultjson.code == 401)
                {
                    ResetAutoLogin?.Invoke();
                }
                return resultjson;
            }

            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            return default(toc_default<T>);
        }
    }

    public toc_default<T> Get<T>(string serverUrl, string token, tos_default tos)
    {
        using (WebClient client = new WebClient())
        {
            try
            {
                client.Headers["Content-Type"] = "application/json";
                client.Encoding = System.Text.Encoding.UTF8;
                client.Headers["token"] = token;
                Regex reg = new Regex("\u200b/g");
                var url = reg.Replace(urlHead + serverUrl + tos.ToString(), "");
                if (Global.Instance.IsDebugPostUrl) Debug.Log(url);
                var result = client.DownloadString(url);
                if (Global.Instance.IsDebugPostUrl) Debug.Log("json: jjjjj" + result);
                var resultjson = JsonMapper.ToObject<toc_default<T>>(result);
                //重新登录
                if (resultjson.code == 401)
                {
                    ResetAutoLogin?.Invoke();
                }
                return resultjson;
            }

            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            return default(toc_default<T>);
        }
    }

    public void GetAsync(string serverUrl, tos_default tos, System.Action<string> callback)
    {
        using (WebClient client = new WebClient())
        {
            try
            {
                var url = urlHead + serverUrl + tos.ToString();
                if (Global.Instance.IsDebugPostUrl) Debug.Log(url);
                client.Headers["Content-Type"] = "application/json";
                client.Encoding = System.Text.Encoding.UTF8;
                client.DownloadStringCompleted += new DownloadStringCompletedEventHandler((sender, e) =>
                {
                    if (null == e || null == e.Result)
                    {
                        Debug.LogError("反回了空数据");
                    }
                    else
                    {
                        if (Global.Instance.IsDebugPostUrl) Debug.Log(e.Result);
                        if (null != callback)
                            ServerRuner.QueueOnMainThread(serverUrl, new UnityServerCallback(callback, e.Result));
                        else
                            Debug.LogError("callback == null\n" + e.Result);
                    }
                });
                client.DownloadStringAsync(new Uri(url));
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }
    public void GetAsync(string serverUrl, System.Action<string> callback)
    {
        using (WebClient client = new WebClient())
        {
            try
            {
                var url = urlHead + serverUrl;
                if (Global.Instance.IsDebugPostUrl) Debug.Log(url);
                client.Headers["Content-Type"] = "application/json";
                client.Encoding = System.Text.Encoding.UTF8;
                client.DownloadStringCompleted += new DownloadStringCompletedEventHandler((sender, e) =>
                {
                    if (null == e || null == e.Result)
                    {
                        Debug.LogError("反回了空数据");
                    }
                    else
                    {
                        if (Global.Instance.IsDebugPostUrl) Debug.Log(e.Result);
                        if (null != callback)
                            ServerRuner.QueueOnMainThread(serverUrl, new UnityServerCallback(callback, e.Result));
                        else
                            Debug.LogError("callback == null\n" + e.Result);
                    }
                });
                client.DownloadStringAsync(new Uri(url));
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }
    public void GetAsync_FullUrl(string full_serverUrl, string app_key, tos_default tos, System.Action<string> callback)
    {
        using (WebClient client = new WebClient())
        {
            try
            {
                var url = full_serverUrl + tos.ToString();
                if (Global.Instance.IsDebugPostUrl) Debug.Log(url);
                client.Encoding = System.Text.Encoding.UTF8;
                client.Headers["Content-Type"] = "application/json";
                if (!string.IsNullOrEmpty(app_key))
                    client.Headers["AppKey"] = app_key;

                client.DownloadStringCompleted += new DownloadStringCompletedEventHandler((sender, e) =>
                {
                    if (null == e || null == e.Result)
                    {
                        Debug.LogError("反回了空数据");
                    }
                    else
                    {
                        if (Global.Instance.IsDebugPostUrl) Debug.Log(e.Result);
                        if (null != callback)
                            ServerRuner.QueueOnMainThread(full_serverUrl, new UnityServerCallback(callback, e.Result));
                        else
                            Debug.LogError("callback == null\n" + e.Result);
                    }
                });
                client.DownloadStringAsync(new Uri(url));
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }
    #endregion

    #region POST

    public toc_default<T> Post<T>(string serverUrl, string token = null)
    {
        using (WebClient client = new WebClient())
        {
            try
            {
                client.Headers["Content-Type"] = "application/json";
                if (!string.IsNullOrEmpty(token)) client.Headers["token"] = token;
                client.Encoding = System.Text.Encoding.UTF8;
                var result = client.UploadString(urlHead + serverUrl, "");
                if (Global.Instance.IsDebugPostUrl) Debug.Log("result: jjjjj" + result);
                var resultjson = JsonMapper.ToObject<toc_default<T>>(result);
                //重新登录
                if (resultjson.code == 401)
                {
                    ResetAutoLogin?.Invoke();
                }
                return resultjson;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            return default(toc_default<T>);
        }

    }
    public toc_default<T> Post<T>(string serverUrl, tos_default tos, string token = null)
    {
        using (WebClient client = new WebClient())
        {
            try
            {
                client.Headers["Content-Type"] = "application/json";
                client.Encoding = System.Text.Encoding.UTF8;
                if (!string.IsNullOrEmpty(token)) client.Headers["token"] = token;
                var result = client.UploadString(urlHead + serverUrl + tos.ToString(), "");
                if (Global.Instance.IsDebugPostUrl) Debug.Log("result: jjjjj" + result);
                var resultjson = JsonMapper.ToObject<toc_default<T>>(result);
                //重新登录
                if (resultjson.code == 401)
                {
                    ResetAutoLogin?.Invoke();
                }
                return JsonMapper.ToObject<toc_default<T>>(result);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            return default(toc_default<T>);
        }

    }

    public toc_default<T> PostObject<T>(string serverUrl, object jsonObject, string token = null)
    {
        using (WebClient client = new WebClient())
        {
            try
            {
                client.Headers["Content-Type"] = "application/json";
                client.Encoding = System.Text.Encoding.UTF8;
                if (!string.IsNullOrEmpty(token)) client.Headers["token"] = token;
                var json = JsonMapper.ToJson(jsonObject);
                if (Global.Instance.IsDebugPostUrl) Debug.Log("url: " + urlHead + serverUrl + " json: " + json);
                var result = client.UploadString(urlHead + serverUrl, json);
                if (Global.Instance.IsDebugPostUrl) Debug.Log("result: jjjjj" + result);
                var resultjson = JsonMapper.ToObject<toc_default<T>>(result);
                //重新登录
                if (resultjson.code == 401)
                {
                    ResetAutoLogin?.Invoke();
                }
                return JsonMapper.ToObject<toc_default<T>>(result);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            return new toc_default<T>();
        }
    }
    public toc_default<T> PostObject<T>(string serverUrl, tos_default tos, object jsonObject, string token = null)
    {
        using (WebClient client = new WebClient())
        {
            try
            {
                client.Headers["Content-Type"] = "application/json";
                client.Encoding = System.Text.Encoding.UTF8;
                if (!string.IsNullOrEmpty(token)) client.Headers["token"] = token;
                var json = JsonMapper.ToJson(jsonObject);
                if (Global.Instance.IsDebugPostUrl) Debug.Log("url: " + urlHead + serverUrl + tos.ToString() + " json: " + json);
                var result = client.UploadString(urlHead + serverUrl + tos.ToString(), json);
                if (Global.Instance.IsDebugPostUrl) Debug.Log("result: jjjjj" + result);
                //var resultjson = JsonMapper.ToObject<toc_default<T>>(result);

                var resultjson = JsonConvert.DeserializeObject<toc_default<T>>(result);


                //重新登录
                if (resultjson.code == 401)
                {
                    ResetAutoLogin?.Invoke();
                }
                //return JsonMapper.ToObject<toc_default<T>>(result);

                return JsonConvert.DeserializeObject<toc_default<T>>(result);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            return new toc_default<T>();
        }
    }

    public void PostAsync(string serverUrl, tos_default tos, System.Action<string> callback)
    {
        using (WebClient client = new WebClient())
        {
            try
            {
                client.Headers["Content-Type"] = "application/json";
                client.Encoding = System.Text.Encoding.UTF8;
                string url = urlHead + serverUrl + tos.ToString();
                if (Global.Instance.IsDebugPostUrl) Debug.Log(url);
                client.UploadStringCompleted += new UploadStringCompletedEventHandler((sender, e) =>
                {
                    if (null == e)
                    {
                        Debug.Log("反回了空数据");
                        if (null != callback)
                            ServerRuner.QueueOnMainThread(serverUrl, new UnityServerCallback(callback, string.Empty));
                        else
                            Debug.LogError("server:e == null && callback == null");
                    }
                    else
                    {
                        if (Global.Instance.IsDebugPostUrl) Debug.Log(e.Result);
                        if (null != callback)
                            ServerRuner.QueueOnMainThread(serverUrl, new UnityServerCallback(callback, e.Result));
                        else
                            Debug.LogError("callback == null\n" + e.Result);
                    }
                });
                client.UploadStringAsync(new Uri(url), "");
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

    }
    public void PostObjectAsync(string serverUrl, object jsonObject, System.Action<string> callback)
    {
        using (WebClient client = new WebClient())
        {
            try
            {
                client.Headers["Content-Type"] = "application/json";
                client.Encoding = System.Text.Encoding.UTF8;
                var json = JsonMapper.ToJson(jsonObject);
                if (Global.Instance.IsDebugPostUrl) Debug.Log(urlHead + serverUrl + "\n" + json);
                client.UploadStringCompleted += new UploadStringCompletedEventHandler((sender, e) =>
                {
                    if (null == e)
                    {
                        Debug.Log("反回了空数据");
                        Debug.LogError("server:e == null && callback == null");
                    }
                    else
                    {
                        if (Global.Instance.IsDebugPostUrl) Debug.Log(e.Result);
                        if (null != callback)
                            ServerRuner.QueueOnMainThread(serverUrl, new UnityServerCallback(callback, e.Result));
                    }
                });
                client.UploadStringAsync(new Uri(urlHead + serverUrl), json);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }
    public void PostObjectAsync_FullUrl(string full_serverUrl, string app_key, object jsonObject, System.Action<string> callback)
    {
        using (WebClient client = new WebClient())
        {
            try
            {
                client.Encoding = System.Text.Encoding.UTF8;
                client.Headers["Content-Type"] = "application/json";
                if (!string.IsNullOrEmpty(app_key))
                    client.Headers["AppKey"] = app_key;// "abcd-f4d6876423e92dc8a6b9fb8f22b0893f";

                var json = JsonMapper.ToJson(jsonObject);

                if (Global.Instance.IsDebugPostUrl) Debug.LogError("++++++:" + full_serverUrl + "\n" + json);
                client.UploadStringCompleted += new UploadStringCompletedEventHandler((sender, e) =>
                {
                    if (null == e)
                    {
                        Debug.Log("反回了空数据");
                    }
                    else
                    {
                        if (Global.Instance.IsDebugPostUrl) Debug.Log(e.Result);
                        if (null != callback)
                            ServerRuner.QueueOnMainThread(full_serverUrl, new UnityServerCallback(callback, e.Result));
                    }
                });
                client.UploadStringAsync(new Uri(full_serverUrl), json);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }
    #endregion

}

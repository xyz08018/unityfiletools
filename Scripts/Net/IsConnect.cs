using UnityEngine;

public class IsConnect
{
    /// <summary>  
    /// 网络可用否
    /// </summary>  
    public static bool NetAvailable
    {
        get
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }
    }

    /// <summary>  
    /// WIFI否
    /// </summary>  
    public static bool IsWifi
    {
        get
        {
            return Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
        }
    }
}

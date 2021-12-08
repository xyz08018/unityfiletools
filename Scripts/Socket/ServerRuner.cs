using System;
using System.Collections.Generic;
using UnityEngine;

public class ServerRuner : MonoBehaviour
{
    private static ServerRuner _current;
    public static ServerRuner Current { get { return _current; } }

    Dictionary<string, UnityServerCallback> _actions = new Dictionary<string, UnityServerCallback>();

    List<UnityServerCallback> _currentActions = new List<UnityServerCallback>();

    private void Awake()
    {
        _current = this;
    }

    public static void QueueOnMainThread(string key, UnityServerCallback _action)
    {
        lock (Current._actions)
        {
            if (Current._actions.ContainsKey(key))
                Current._actions[key] = _action;
            else
                Current._actions.Add(key, _action);
        }
    }

    private void Update()
    {
        lock (_actions)
        {
            _currentActions.Clear();
            _currentActions.AddRange(_actions.Values);
            _actions.Clear();
        }
        for (int i = 0; i < _currentActions.Count; i++)
        {
            _currentActions[i].Invoke();
        }
    }

}

public class UnityServerCallback
{
    public Action<string> _func;
    public string _param;

    public UnityServerCallback(Action<string> func, string param)
    {
        this._func = func;
        this._param = param;
    }

    public void Invoke()
    {
        if (null == _func) return;
        if (string.IsNullOrEmpty(_param)) return;
        _func(_param);
    }
}

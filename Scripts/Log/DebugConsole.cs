//#define USE_TESTCONSOLE
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Consolation
{
    /// <summary>
    /// A console to display Unity's debug logs in-game.
    /// </summary>
    class DebugConsole : MonoBehaviour
    {
        //#if USE_TESTCONSOLE
        struct Log
        {
            public string message;
            public string stackTrace;
            public LogType type;
        }

        #region Inspector Settings

        /// <summary>
        /// The hotkey to show and hide the console window.
        /// </summary>
        public KeyCode toggleKey = KeyCode.Space;

        /// <summary>
        /// Whether to open the window by shaking the device (mobile-only).
        /// </summary>
        public bool shakeToOpen = true;

        /// <summary>
        /// The (squared) acceleration above which the window should open.
        /// </summary>
        public float shakeAcceleration = 3f;

        /// <summary>
        /// Whether to only keep a certain number of logs.
        ///
        /// Setting this can be helpful if memory usage is a concern.
        /// </summary>
        public bool restrictLogCount = false;

        /// <summary>
        /// Number of logs to keep before removing old ones.
        /// </summary>
        public int maxLogs = 1000;

        #endregion

        readonly List<Log> logs = new List<Log>();
        Vector2 scrollPosition;
        [Header("是否启动log日志屏幕输出")]
        public bool visible = false;
        bool collapse;
        bool isClose = false;

        // Visual elements:

        static readonly Dictionary<LogType, Color> logTypeColors = new Dictionary<LogType, Color>
        {
            { LogType.Assert, Color.white },
            { LogType.Error, Color.red },
            { LogType.Exception, Color.red },
            { LogType.Log, Color.white },
            { LogType.Warning, Color.yellow },
        };

        const string windowTitle = "Console";
        const int margin = 20;
        static readonly GUIContent clearLabel = new GUIContent("Clear", "Clear the contents of the console.");
        static readonly GUIContent collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");
        static readonly GUIContent close = new GUIContent("Close", "close repeated messages.");

        readonly Rect titleBarRect = new Rect(0, 0, 10000, 20);
        Rect windowRect = new Rect(margin, margin, Screen.width - (margin * 2), Screen.height - (margin * 2));

        void OnEnable()
        {
            Application.logMessageReceived += HandleLog;

            //#if UNITY_5
            //            Application.logMessageReceived += HandleLog;
            //#else
            //            Application.RegisterLogCallback(HandleLog);
            //#endif
        }

        void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;

            //#if UNITY_5
            //            Application.logMessageReceived -= HandleLog;
            //#else
            //            Application.RegisterLogCallback(null);
            //#endif
        }
        private bool isShowDeveloper;//控制是显示还是隐藏
        private int click;
        DateTime lastClickTime;
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                visible = !visible;
                Debug.Log(windowRect.position);

            }

            if (Input.GetMouseButtonDown(0))
            {

                DateTime now = DateTime.Now;
                if (click == 0)
                    lastClickTime = DateTime.Now;
                // 两次点击间隔小于300毫秒时，算连续点击
                if ((now - lastClickTime).TotalMilliseconds <= 300)
                {
                    click++;
                    if (click >= 10)
                    {
                        click = 0;// 连续点击完毕时，清0
                                  // 10次点击触发的事件
                        isShowDeveloper = !isShowDeveloper;
                        windowRect.width = Screen.width - (margin * 2);
                        windowRect.height = Screen.height - (margin * 2);
                        windowRect.position = new Vector2(20, 20);
                        visible = true;
                    }
                }
                else
                {
                    click = 0;// 不是连续点击时，清0
                }
                lastClickTime = now;

            }

            if (shakeToOpen && Input.acceleration.sqrMagnitude > shakeAcceleration)
            {
                visible = true;
            }
        }

        void OnGUI()
        {
            if (!visible)
            {
                return;
            }
            windowRect = GUILayout.Window(123456, windowRect, DrawConsoleWindow, windowTitle);
        }

        /// <summary>
        /// Displays a window that lists the recorded logs.
        /// </summary>
        /// <param name="windowID">Window ID.</param>
        void DrawConsoleWindow(int windowID)
        {
            DrawLogsList();
            DrawToolbar();

            // Allow the window to be dragged by its title bar.
            GUI.DragWindow(titleBarRect);
        }

        /// <summary>
        /// Displays a scrollable list of logs.
        /// </summary>
        void DrawLogsList()
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            // Iterate through the recorded logs.
            for (var i = 0; i < logs.Count; i++)
            {
                var log = logs[i];

                // Combine identical messages if collapse option is chosen.
                if (collapse && i > 0)
                {
                    var previousMessage = logs[i - 1].message;

                    if (log.message == previousMessage)
                    {
                        continue;
                    }
                }

                GUI.contentColor = logTypeColors[log.type];
                GUILayout.Label(log.message);
            }

            GUILayout.EndScrollView();

            // Ensure GUI colour is reset before drawing other components.
            GUI.contentColor = Color.white;
        }

        /// <summary>
        /// Displays options for filtering and changing the logs list.
        /// </summary>
        void DrawToolbar()
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button(clearLabel))
            {
                logs.Clear();
            }
            if (GUILayout.Button(close))
            {
                windowRect.height = 0;
                windowRect.width = 0;
                windowRect.position = new Vector2(0, 10000);
                visible = !visible;
            }

            collapse = GUILayout.Toggle(collapse, collapseLabel, GUILayout.ExpandWidth(false));

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Records a log from the log callback.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="stackTrace">Trace of where the message came from.</param>
        /// <param name="type">Type of message (error, exception, warning, assert).</param>
        void HandleLog(string message, string stackTrace, LogType type)
        {
            logs.Add(new Log
            {
                message = message,
                stackTrace = stackTrace,
                type = type,
            });

            TrimExcessLogs();
        }

        /// <summary>
        /// Removes old logs that exceed the maximum number allowed.
        /// </summary>
        void TrimExcessLogs()
        {
            if (!restrictLogCount)
            {
                return;
            }

            var amountToRemove = Mathf.Max(logs.Count - maxLogs, 0);

            if (amountToRemove == 0)
            {
                return;
            }

            logs.RemoveRange(0, amountToRemove);
        }
        //#endif
    }
}

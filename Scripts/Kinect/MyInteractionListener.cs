////using com.rfilkov.components;
//using UnityEngine;

//public class MyInteractionListener : MonoBehaviour//, InteractionListenerInterface
//{
//    [Tooltip("Camera used for screen ray-casting. This is usually the main camera.  用于屏幕光线投射的照相机。这通常是主相机。")]
//    public Camera screenCamera;
//    [Tooltip("交互管理器实例，用于检测手交互。如果为空，它将是场景中发现的第一个交互管理器")]
//    //public InteractionManager interactionManager;
//    // hand interaction variables   手的交互变量
//    //public InteractionManager.HandEventType lastHandEvent = InteractionManager.HandEventType.None;

//    // normalized and pixel position of the cursor  鼠标光标的统一化和位置
//    private Vector3 screenNormalPos = Vector3.zero;
//    private Vector3 screenPixelPos = Vector3.zero;
//    private Vector3 newObjectPos = Vector3.zero;

//    [Tooltip("Whether the left hand interaction is allowed by the respective InteractionManager.")]
//    public bool leftHandInteraction = false;

//    [Tooltip("Whether the right hand interaction is allowed by the respective InteractionManager.")]
//    public bool rightHandInteraction = true;

//    float gripTime = 0;
//    float releaseTime = 0;

//    //private CubeGestureListener gestureListener;


//    void Start()
//    {
//        if (screenCamera == null)
//            screenCamera = Camera.main;

//        //if (interactionManager == null)
//            //interactionManager = InteractionManager.GetInstance(0, leftHandInteraction, rightHandInteraction);

//        // get the gestures listener
//        //gestureListener = CubeGestureListener.Instance;
//    }

//    void Update()
//    {
//        //如果交互管理器不为空，并且已经初始化
//        if (null != interactionManager && interactionManager.IsInteractionInited())
//        {
//            //check if there is an underlying object to be selected    检查是否存在要选择的基础对象
//            bool bHandIntAllowed = rightHandInteraction && interactionManager.IsRightHandPrimary();

//            if (gestureListener)
//            {
//                if (gestureListener.IsSwipeDown())
//                {
//                    Debug.LogError("手势上下滑log输出");
//                    GameCtrl.instance.handleType = HandleType.Close;
//                }
//                else
//                {
//                    GameCtrl.instance.handleType = HandleType.None;
//                }
//            }
//            else
//            {
//                GameCtrl.instance.handleType = HandleType.None;
//            }
//            return;
//            if (lastHandEvent == InteractionManager.HandEventType.Grip && bHandIntAllowed)
//            {
//                //Debug.LogError("握拳");
//                gripTime += Time.deltaTime;
//                if (gripTime >= 0.3f)
//                {
//                    //GameCtrl.instance.handleType = HandleType.Close;
//                    gripTime = 0;
//                }

//                if (gestureListener)
//                {
//                    if (gestureListener.IsSwipeDown())
//                    {
//                        Debug.LogError("手势上下滑log输出");
//                        GameCtrl.instance.handleType = HandleType.Close;
//                    }
//                }

//            }
//            else if (lastHandEvent == InteractionManager.HandEventType.Release && bHandIntAllowed)
//            {
//                //Debug.LogError("张开");
//                gripTime = 0;
//                GameCtrl.instance.isOpen = true;
//                GameCtrl.instance.handleType = HandleType.Open;
//            }
//            else
//            {
//                //Debug.LogError("半握拳");
//                gripTime = 0;
//                GameCtrl.instance.handleType = HandleType.None;
//            }

//        }



//    }

//    public void HandGripDetected(ulong userId, int userIndex, bool isRightHand, bool isHandInteracting, Vector3 handScreenPos)
//    {
//        if (!isHandInteracting || !interactionManager)
//            return;
//        if (userId != interactionManager.GetUserID())
//            return;
//        lastHandEvent = InteractionManager.HandEventType.Grip;
//        //isLeftHandDrag = !isRightHand;
//        screenNormalPos = handScreenPos;
//    }

//    public void HandReleaseDetected(ulong userId, int userIndex, bool isRightHand, bool isHandInteracting, Vector3 handScreenPos)
//    {

//        if (!isHandInteracting || !interactionManager)
//            return;
//        if (userId != interactionManager.GetUserID())
//            return;

//        lastHandEvent = InteractionManager.HandEventType.Release;
//        screenNormalPos = handScreenPos;
//    }

//    public bool HandClickDetected(ulong userId, int userIndex, bool isRightHand, Vector3 handScreenPos)
//    {
//        return true;
//    }
//}

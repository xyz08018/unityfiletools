//using com.rfilkov.components;
//using com.rfilkov.kinect;
//using UnityEngine;

//public class PlayerControl : MonoBehaviour//, InteractionListenerInterface
//{
//    [Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
//    public int playerIndex = 0;

//    public Transform playerTransform;

//    KinectManager kinectManager;
//    private Rigidbody rig;

//    private InteractionManager interactionManager;
//    //public InteractionManager.HandEventType lastHandEvent = InteractionManager.HandEventType.None;

//    public bool justMoveX = false;

//    public bool leftHandInteraction = false;
//    public bool rightHandInteraction = true;

//    private Vector3 preHandPos;

//    public float ballPrePos;

//    // hand interaction variables   手的交互变量
//    public InteractionManager.HandEventType lastHandEvent = InteractionManager.HandEventType.None;
//    public Camera screenCamera;


//    ConfigInfo config;
//    void Start()
//    {
//        config = ConfigData.Instance.Data;
//        //数据初始化
//        moveDinosaurSpeed = float.Parse(config.handMoveSpeed);
//        moveAmplificationX = float.Parse(config.moveAmplificationX);
//        moveAmplificationY = float.Parse(config.moveAmplificationY);
//        float leftPos = float.Parse(config.leftPos);
//        float rightPos = float.Parse(config.rightPos);
//        float upPos = float.Parse(config.upPos);
//        float downPos = float.Parse(config.downPos);
//        lr = new Vector2(leftPos, rightPos);
//        ud = new Vector2(upPos, downPos);
//        ballPrePos = float.Parse(config.ballPrePos);
//        GameCtrl.instance.gameTime = int.Parse(config.gameTime);
//        GameCtrl.instance.gameOverWaitTime = float.Parse(config.gameOverWaitTime);
//        GameCtrl.instance.isClearGameData = int.Parse(config.isClearGameData);
//        Screen.SetResolution(config.width, config.height, true);


//        preHandPos = playerTransform.transform.localPosition;
//        justMoveX = false;
//        //if (playerTransform == null)
//        //    playerTransform = this.transform;

//        rig = playerTransform.GetComponent<Rigidbody>();

//        //if (interactionManager == null)
//        //{
//        //    interactionManager = InteractionManager.GetInstance(0, leftHandInteraction, rightHandInteraction);
//        //}
//        //playerTransform.gameObject.SetActive(false);

//        kinectManager = KinectManager.Instance;

//        if (screenCamera == null)
//            screenCamera = Camera.main;

//        if (interactionManager == null)
//            interactionManager = InteractionManager.GetInstance(0, leftHandInteraction, rightHandInteraction);

//    }

//    void Update()
//    {

//        if (playerTransform == null) return;

//        if (kinectManager && kinectManager.IsInitialized())
//        {
//            // overlay all joints in the skeleton           
//            if (kinectManager.IsUserDetected(0) && kinectManager.IsJointTracked(kinectManager.GetUserIdByIndex(0), (int)KinectInterop.JointType.HandRight))
//            {
//                bool bHandIntAllowed = rightHandInteraction && interactionManager.IsRightHandPrimary();// rightHandInteraction && interactionManager.IsRightHandPrimary();
//                //////Debug.Log("测试数据分析： " + bHandIntAllowed);
//                //////////显示Player
//                if (!bHandIntAllowed)
//                {
//                    ResetPos();
//                    return;
//                }

//                //Debug.LogError("1");
//                //获取ID
//                ulong userId = kinectManager.GetUserIdByIndex(0);
//                //脊柱胸位置
//                Vector3 spineChestPos = kinectManager.GetJointKinectPosition(userId, 2, true);
//                //颈位置
//                Vector3 neckPos = kinectManager.GetJointKinectPosition(userId, 3, true);
//                //额头位置
//                Vector3 headPos = kinectManager.GetJointKinectPosition(userId, 4, true);

//                //右肩位置
//                Vector3 shoulderRightPos = kinectManager.GetJointKinectPosition(userId, 11, true);

//                //右手肘位置
//                //Vector3 elbowRightPos = kinectManager.GetJointKinectPosition(userId, 12, true);
//                //elbowRightPos = new Vector3(elbowRightPos.x + elbowOffset.x, elbowRightPos.y + elbowOffset.y, elbowRightPos.z + elbowOffset.z);
//                //右手腕位置
//                Vector3 wristRightPos = kinectManager.GetJointKinectPosition(userId, 13, true);

//                //右手指尖位置
//                Vector3 handtipRightPos = kinectManager.GetJointKinectPosition(userId, 30, true);
//                //wristRightPos = new Vector3(wristRightPos.x * scaleFactors.x, wristRightPos.y * scaleFactors.y, wristRightPos.z * scaleFactors.z);
//                //右手拇指位置
//                //Vector3 thumbRightPos = kinectManager.GetJointKinectPosition(userId, 31, true);
//                //手掌移动
//                MoveByHand(handtipRightPos, spineChestPos, shoulderRightPos);
//                //if (GameCtrl.instance.isPlay)
//                //    return;

//                if (!GameCtrl.instance.isGameBegin)
//                    return;
//                if (handtipRightPos.y > wristRightPos.y && !GameCtrl.instance.isOpen)
//                {
//                    GameCtrl.instance.isOpen = true;  //手势打开过
//                }
//                if (GameCtrl.instance.isPlay)
//                    return;
//                if (!GameCtrl.instance.isPlay && handtipRightPos.y < wristRightPos.y)
//                {
//                    Debug.LogError("指尖位置低于手腕位置");
//                    GameCtrl.instance.isPlay = true;  //是否在下落
//                    GameCtrl.instance.handleType = HandleType.Close;
//                }

//            }
//            else
//                ResetPos();

//        }
//        else
//            ResetPos();
//    }

//    public float moveAmplificationX = 1;
//    public float moveAmplificationY = 1;
//    public float moveOffsetY = 0;
//    public float moveDinosaurSpeed = 0.005f;
//    private void MoveByHand(Vector3 oriPos, Vector3 offsetX, Vector3 offsetY)
//    {
//        if (!justMoveX)
//            playerTransform.localPosition = Vector3.Lerp(playerTransform.position
//                , new Vector3(Mathf.Clamp(oriPos.x * moveAmplificationX - offsetX.x, lr.x, lr.y)
//                , Mathf.Clamp((oriPos.y - moveOffsetY) * moveAmplificationY - offsetY.y, ud.x, ud.y)
//                //, playerTransform.position.y
//                , playerTransform.position.z)
//                , moveDinosaurSpeed);
//        else
//        {
//            if (!GameCtrl.instance.isGameBegin)
//                return;
//            playerTransform.localPosition = Vector3.Lerp(playerTransform.position
//            , new Vector3(Mathf.Clamp(oriPos.x * moveAmplificationX - offsetX.x, lr.x, lr.y)
//            //, Mathf.Clamp((oriPos.y - moveOffsetY) * moveAmplificationY - offsetY.y, ud.x, ud.y)
//            , ballPrePos//playerTransform.position.y
//            , playerTransform.position.z)
//            , moveDinosaurSpeed);
//        }



//        MoveClamp();
//    }

//    public Vector2 lr = new Vector2(-8.5f, 8.5f);
//    public Vector2 ud = new Vector2(-4f, 4f);
//    private void MoveClamp()
//    {
//        if (!justMoveX)
//            playerTransform.localPosition = new Vector3(Mathf.Clamp(playerTransform.position.x, lr.x, lr.y)
//                                            , Mathf.Clamp(playerTransform.position.y, ud.x, ud.y)
//                                            , playerTransform.position.z);
//        else
//            playerTransform.localPosition = new Vector3(Mathf.Clamp(playerTransform.position.x, lr.x, lr.y)
//                                            , ballPrePos//Mathf.Clamp(playerTransform.position.y, ud.x, ud.y)
//                                            , playerTransform.position.z);
//    }

//    public void ResetPos()
//    {
//        if (!justMoveX)
//            playerTransform.localPosition = preHandPos;
//        //else
//        //    //Debug.LogError("位置重置");
//    }




//    public float t = 0;

//    void OnTriggerEnter2D(Collider2D other)
//    {
//        t = 0;
//    }
//    void OnTriggerStay2D(Collider2D other)
//    {
//        t += Time.deltaTime;
//        if (other.transform.tag != "Air")
//        {
//            if (t >= 2.0f)
//            {
//                //GameObject.Find("CodeControl").GetComponent<ScoreControl>().SaveScore();//保存分数
//                //SceneManager.LoadScene("Over");//切换场景
//                GameCtrl.instance.SaveUserData();
//                GameCtrl.instance.isGameBegin = false;
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
//        //screenNormalPos = handScreenPos;
//    }

//    public void HandReleaseDetected(ulong userId, int userIndex, bool isRightHand, bool isHandInteracting, Vector3 handScreenPos)
//    {

//        if (!isHandInteracting || !interactionManager)
//            return;
//        if (userId != interactionManager.GetUserID())
//            return;

//        lastHandEvent = InteractionManager.HandEventType.Release;
//        //screenNormalPos = handScreenPos;
//    }

//    public bool HandClickDetected(ulong userId, int userIndex, bool isRightHand, Vector3 handScreenPos)
//    {
//        return true;
//    }
//}

using UnityEngine;
//using com.rfilkov.kinect;

public class KinectPoseManager : MonoBehaviour
{



    //KinectManager kinectManager;
    // Start is called before the first frame update
    void Start()
    {
        //kinectManager = KinectManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        //if (kinectManager && kinectManager.IsInitialized())
        //{
        //    for (int i = 0; i < kinectManager.GetUsersCount(); i++)
        //    {
        //        // overlay all joints in the skeleton
        //        if (kinectManager.IsUserDetected(i))
        //        {
        //            userIndex = i;
        //            PoseCheck();
        //        }
        //    }
        //}
    }

    public float tolerance = 0.1f;
    ulong userId;
    int userIndex;
    private void PoseCheck()
    {
        //获取ID
        userId = 0;// kinectManager.GetUserIdByIndex(userIndex);
        //骨盆=0，//脊椎海军=1，//棘胸=2，//颈部=3，//额头=4，//锁骨左侧=5，//左肩=6，//左肘=7，//腕左=8，//左手=9，//锁骨右=10，//右肩=11，//右肘=12
        //右手腕=13 //右手=14//左髋=15，//左膝盖=16，//左脚踝=17，//左脚=18，//右髋=19，//右膝=20，//右脚踝=21，//右脚=22，//鼻=23，//左眼=24，//左耳=25，//右眼=26，
        //耳光=27，//左手尖=28，//拇指左=29，//右手柄=30，//拇指右=31，//计数=32
        if (PosCheck(6, 11, "y") && PosCheck(11, 13, "y") && PosCheck(6, 9, "y")
            && PosCheck(6, 11, "z") && PosCheck(11, 13, "z") && PosCheck(6, 9, "z")
            && PosCheck(6, 9, ">x") && PosCheck(13, 11, ">z"))
        {
            PoseResultImage(0);
        }
        else if (PosCheck(9, 13, "y") && PosCheck(9, 4, ">y")) { PoseResultImage(1); }
        else if (PosCheck(11, 12, "x") && PosCheck(12, 13, "x") && PosCheck(11, 13, "z") && PosCheck(13, 11, ">y")
            && PosCheck(6, 7, "x") && PosCheck(7, 8, "x") && PosCheck(6, 8, "z") && PosCheck(6, 8, ">y")) { PoseResultImage(2); }
        else if (PosCheck(11, 12, "z") && PosCheck(12, 13, "z")
            && PosCheck(9, 11, "vec")) { PoseResultImage(3); }
        else if (PosCheck(14, 9, "vec") && PosCheck(9, 11, ">x")) { PoseResultImage(4); }
        else if (PosCheck(9, 15, "vec") && PosCheck(14, 19, "vec")) { PoseResultImage(5); }
        else if (PosCheck(9, 16, "vec") && PosCheck(14, 20, "vec")) { PoseResultImage(6); }
        else if (PosCheck(12, 13, "z") && PosCheck(7, 8, "y") && PosCheck(12, 8, "vec")) { PoseResultImage(7); }
        else { PoseResultImage(-1); }
    }

    private bool PosCheck(int ori, int tar, string vec = "x")
    {
        Vector3 oriPos = Vector3.zero;// kinectManager.GetJointKinectPosition(userId, ori, true);
        Vector3 tarPos = Vector3.zero;// kinectManager.GetJointKinectPosition(userId, tar, true);
        float dis = 0;
        switch (vec)
        {
            case "x":
                dis = Mathf.Abs(oriPos.x - tarPos.x);
                if (dis < tolerance) return true; else return false;
            case "y":
                dis = Mathf.Abs(oriPos.y - tarPos.y);
                if (dis < tolerance) return true; else return false;
            case "z":
                dis = Mathf.Abs(oriPos.z - tarPos.z);
                if (dis < tolerance) return true; else return false;
            case ">x":
                dis = oriPos.y - tarPos.y;
                if (dis > 0) return true; else return false;
            case ">y":
                dis = oriPos.y - tarPos.y;
                if (dis > 0) return true; else return false;
            case ">z":
                dis = oriPos.y - tarPos.y;
                if (dis > 0) return true; else return false;
            case "vec":
                dis = Mathf.Abs(Vector3.Distance(oriPos, tarPos));
                if (dis < tolerance) return true; else return false;
        }
        return false;
    }
    private void PoseResultImage(int index)
    {
        //if (index == -1) { _images[userIndex].color = Color.clear; return; }

        //_images[userIndex].sprite = _sprites[index];
        //_images[userIndex].SetNativeSize();
        //_images[userIndex].color = Color.white;
    }

}


//public enum JointType : int
//{
//Pelvis = 0,
//SpineNaval = 1,
//SpineChest = 2,
//Neck = 3,
//Head = 4,

//ClavicleLeft = 5,
//ShoulderLeft = 6,
//ElbowLeft = 7,
//WristLeft = 8,
//HandLeft = 9,

//ClavicleRight = 10,
//ShoulderRight = 11,
//ElbowRight = 12,//右手肘
//WristRight = 13,//右手腕
//HandRight = 14,

//HipLeft = 15,
//KneeLeft = 16,
//AnkleLeft = 17,
//FootLeft = 18,

//HipRight = 19,
//KneeRight = 20,
//AnkleRight = 21,
//FootRight = 22,

//Nose = 23,
//EyeLeft = 24,
//EarLeft = 25,
//EyeRight = 26,
//EarRight = 27,

//HandtipLeft = 28,
//ThumbLeft = 29,
//HandtipRight = 30,
//ThumbRight = 31,

//Count = 32



//}

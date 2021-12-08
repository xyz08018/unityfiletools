//using OpenCVForUnity.CoreModule;
//using OpenCVForUnity.UnityUtils;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;


public class UnityCallPython : MonoBehaviour
{
    [DllImport("TensorRTProc")]
    //private extern static int ProcModule([MarshalAs(UnmanagedType.LPArray)] int[] aiClsTop5); //
    private extern static int ONNX2TRT(string onnxFileName, string trtFileName, int batchSize, bool bInt8, bool bFp16, int iDlaCore);
    [DllImport("TensorRTProc")]
    private extern static System.IntPtr LoadTRT(string trtFileName, int iDlaCore);
    [DllImport("TensorRTProc")]
    private extern static void ReleaseTRT(System.IntPtr trt);

    [DllImport("TensorRTProc")]
    private extern static bool DoInference(System.IntPtr trt, string input_name, string output_name, float[] input, float[] output, int input_size, int output_size);

    //[DllImport("TensorRTProc", EntryPoint = "?TRTProcYolov5@@YAXPEAXAEBVMat@cv@@AEAV12@PEAH@Z")]
    //private extern static void TRTProcYolov5(string trtFileName, IntPtr input_img, int[] output);
    [DllImport("TensorRTProc", EntryPoint = "?TRTProcYolov5@@YAXPEAXAEBVMat@cv@@AEAV12@PEAH@Z")]
    private extern static void TRTProcYolov5(System.IntPtr hdlTRT, IntPtr input_img, IntPtr output_img, int[] output);


    public RawImage photo;

    Texture2D mTexture;

    //Mat output_mat;
    System.IntPtr trt;

    Thread loadThread;

    #region 同步协程加载，会卡很久
    //IEnumerator Start()
    //{
    //    //每个首次需要主动生成相应环境得trt文件，由onnx---->trt得转换
    //    string pythonPath = string.Format("{0}/PythonTrt", System.Environment.CurrentDirectory);
    //    string trtFileName = string.Format("{0}/{1}", pythonPath, "crowdhuman_yolov5m_unity.trt"); //Path.Combine(Application.dataPath, "crowdhuman_yolov5m_unity.trt").Replace(@"Assets\", "");// "E:\\HQCPro\\UnityPro\\PedestrianDetection\\crowdhuman_yolov5m_unity.trt";
    //    string onnxFileName = Path.Combine(Application.streamingAssetsPath, "crowdhuman_yolov5m.onnx");//.Replace(@"Assets\", ""); //"E:\\HQCPro\\UnityPro\\PedestrianDetection\\crowdhuman_yolov5m.onnx";
    //    Debug.Log("trtFileName: " + trtFileName + "    onnxFileName:" + onnxFileName);
    //    if (!Directory.Exists(pythonPath))
    //    {
    //        Directory.CreateDirectory(pythonPath);
    //    }
    //    if (File.Exists(trtFileName))
    //    {
    //        Debug.Log("加载引用库文件trt");
    //        trt = LoadTRT(trtFileName, -1);
    //    }
    //    else if (File.Exists(onnxFileName))
    //    {
    //        Debug.Log("正在执行 onnx->trt 序列化，请耐心等待...");
    //        int iResult = ONNX2TRT(onnxFileName, trtFileName, 1, false, false, -1);
    //        Debug.Log("打印结果： " + iResult + "   加载");
    //        if (iResult == 0)
    //            trt = LoadTRT(trtFileName, -1);
    //        else
    //            Debug.Log("初始化加载失败");
    //    }
    //    else
    //    {
    //        Debug.Log("文件路径不存在GGGGGGGGGGGGGGGGGG");
    //    }
    //    yield return new WaitForEndOfFrame();
    //}

    #endregion

    private void Start()
    {
        //异步加载
        loadThread = new Thread(() =>
        {
            LoadPythonTrt();
        });

        loadThread.Start();
    }

    bool isLoadFinish = false;

    void LoadPythonTrt()
    {
        string pythonPath = string.Format("{0}/PythonTrt", System.Environment.CurrentDirectory);
        string trtFileName = string.Format("{0}/{1}", pythonPath, "crowdhuman_yolov5m_unity.trt");
        string onnxFileName = Path.Combine(Application.streamingAssetsPath, "crowdhuman_yolov5m.onnx");
        if (!Directory.Exists(pythonPath))
        {
            Directory.CreateDirectory(pythonPath);
        }
        if (File.Exists(trtFileName))
        {
            Debug.Log("加载引用库文件trt");
            trt = LoadTRT(trtFileName, -1);
            isLoadFinish = true;
        }
        else if (File.Exists(onnxFileName))
        {
            Debug.Log("正在执行 onnx->trt 序列化，请耐心等待...");
            int iResult = ONNX2TRT(onnxFileName, trtFileName, 1, false, false, -1);
            Debug.Log("打印结果： " + iResult + "   加载");
            if (iResult == 0)
            {
                trt = LoadTRT(trtFileName, -1);
                isLoadFinish = true;
            }
            else
                Debug.Log("初始化加载失败");
        }
        else
        {
            Debug.Log("文件路径不存在GGGGGGGGGGGGGGGGGG");
        }
    }


    void Update()
    {
        //if (testWebCam.isInit && isLoadFinish)
        //    CheckPeople();
    }

    void CheckPeople()
    {
        ////string inImgFileName = "D:\\123.jpg";
        //var src = testWebCam.GetPythonMat();
        ////var test = Imgcodecs.imread(inImgFileName, 1);
        //int[] output = new int[1];
        ////Imgcodecs.imwrite("C:\\SRC.jpg", src);
        ////row 高  col 宽
        //output_mat = new Mat(src.rows(), src.cols(), src.type());
        ////c++推理接口
        //TRTProcYolov5(trt, src.nativeObj, output_mat.nativeObj, output);
        ////Imgcodecs.imwrite("C:\\DST.jpg", output_mat);
        ////Imgcodecs.imwrite("D:\\result_test.jpg", output_mat);
        //mTexture = new Texture2D(testWebCam.camTexture.width, testWebCam.camTexture.height, TextureFormat.RGB24, false);
        //Utils.matToTexture2D(output_mat, mTexture);

        //photo.texture = mTexture;

        //Resources.UnloadUnusedAssets();
        //GC.Collect();
    }

    private void OnDestroy()
    {
        if (null != mTexture)
        {
            Texture2D.Destroy(mTexture);
            mTexture = null;
        }
        //if (output_mat != null)
        //{
        //    output_mat.Dispose();
        //    output_mat = null;
        //}
        if (null != loadThread)
        {
            loadThread.Abort();
            loadThread = null;
        }
        ReleaseTRT(trt);
    }
}

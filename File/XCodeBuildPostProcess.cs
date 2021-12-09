using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Linq;

#if UNITY_IOS && UNITY_EDITOR
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
#endif

public static class XCodeBuildPostProcess
{
    //https://blog.csdn.net/radiuscll/article/details/81219862
    /**,"CoreMotion.framework"**///"AlipaySDK.framework",
#if UNITY_IOS && UNITY_EDITOR
    private static readonly string[] csAddFrameworks = new string[]{
        "WebKit.framework","CoreTelephony.framework","AlipaySDK.framework","Security.framework","MediaToolbox.framework",
        "CoreText.framework","AudioToolbox.framework","AVFoundation.framework","CFNetwork.framework","CoreGraphics.framework","CoreLocation.framework","CoreMedia.framework","CoreMotion.framework","CoreVideo.framework","Foundation.framework","MediaPlayer.framework","OpenAL.framework","OpenGLES.framework","QuartzCore.framework","SystemConfiguration.framework","UIKit.framework","StoreKit.framework","Metal.framework"
    };

    private static readonly string[] csAddSystemLibrarys = new string[]{
        "libc.tbd", "libz.tbd" ,"libsqlite3.0.tbd","libz.dylib"
    };

    private static readonly string[] csRemoveSystemLibrarys = new string[]{

    };

    private static readonly string[] csRemoveFrameworks = new string[]{

    };

    private static readonly string[] csIgnoreExtensions = new string[]{
        ".meta"
    };

    [PostProcessBuild(1000)]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string pathToBuiltProject)
    {
        PlayerSettings.iOS.appleDeveloperTeamID = "748F3M7T86";
        PlayerSettings.iOS.appleEnableAutomaticSigning = true;
        //if (buildTarget != BuildTarget.iOS)
        //    return;
        Debug.Log("buildTarget:++++++++++++" + buildTarget);
        Debug.Log("IOSPostBuildProcessor::OnPostprocessBuild\t______________" + pathToBuiltProject);
        string projectPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
        resetFrameworks(projectPath, pathToBuiltProject);
        resetAppController(projectPath, pathToBuiltProject);
        resetInfoList(pathToBuiltProject);   
                                                                                          
    }

    private static void resetFrameworks(string path, string pathToBuiltProject)
    {
        PBXProject proj = new PBXProject();
        string linkerFlag = "OTHER_LDFLAGS";
        proj.ReadFromString(File.ReadAllText(path));
        string target = proj.TargetGuidByName("Unity-iPhone");
        proj.SetBuildProperty(target, "ENABLE_BITCODE", "NO");
        //
        proj.AddBuildProperty(target, linkerFlag, "-ObjC");
        proj.AddBuildProperty(target, linkerFlag, "-all_load");
        //
        proj.AddBuildProperty(target, "GCC_PREPROCESSOR_DEFINITIONS", "DISABLE_PUSH_NOTIFICATIONS=1");
        //
        proj.SetBuildProperty(target, "GCC_ENABLE_CPP_RTTI", "YES");
        //
        proj.SetBuildProperty(target, "GCC_ENABLE_OBJC_EXCEPTIONS", "YES");
        ////   
        //remove system librarys
        for (int i = 0; i < csRemoveSystemLibrarys.Length; ++i)
        {
            string removeGuid = proj.FindFileGuidByProjectPath(csRemoveSystemLibrarys[i]);
            if (!string.IsNullOrEmpty(removeGuid))
            {
                proj.RemoveFileFromBuild(target, removeGuid);
                proj.RemoveFile(removeGuid);
            }
        }

        //add frameworks
        for (int i = 0; i < csAddFrameworks.Length; ++i)
        {
            proj.AddFrameworkToProject(target, csAddFrameworks[i], false);
        }

        //add system library
        for (int i = 0; i < csAddSystemLibrarys.Length; ++i)
        {
            proj.AddFrameworkToProject(target, csAddSystemLibrarys[i], false);
        }

        //remove frameworks
        for (int i = 0; i < csRemoveFrameworks.Length; ++i)
        {
            proj.RemoveFrameworkFromProject(target, csRemoveFrameworks[i]);

        }
        File.WriteAllText(path, proj.WriteToString());
    }


    public static void resetInfoList(string buildPath)
    {
        string listPath = buildPath + "/Info.plist";
        PlistDocument plist = new PlistDocument();
        plist.ReadFromString(File.ReadAllText(listPath));
        PlistElementDict rootDic = plist.root;

        //不设置这项，提交时会显示缺少合规证明
        rootDic.SetBoolean("ITSAppUsesNonExemptEncryption", false);
        rootDic.SetBoolean("UIRequiresPersistentWiFi", true);
        rootDic.SetBoolean("Localized resources can be mixed", true);

        rootDic.SetString("NSCameraUsageDescription", "想用下你的相机啦"); //相机
        //rootDic.SetString("NSMicrophoneUsageDescription", "想要使用麦克风");//mic
        rootDic.SetString("NSPhotoLibraryUsageDescription", "想要访问相册");
        rootDic.SetString("NSLocationWhenInUseUsageDescription", "需要获取您的当前位置");
        rootDic.SetString("NSContactsUsageDescription", "contactsDesciption");
        rootDic.SetString("NSPhotoLibraryAddUsageDescription", "photoLibraryAdditionsDesciption");
        rootDic.SetString("CFBundleDevelopmentRegion", "China");

        PlistElementArray urlArray = rootDic.CreateArray("CFBundleURLTypes");
        string[] urlSchemes = { "com.kyd.kimikidsmath_wxda445c2b2b452f25", "ali_apipay" };
        //string[] urlSchemes = { "com.kyd.kimikidsmath_wxda445c2b2b452f25"};
        foreach (var item in urlSchemes)
        {
            string kay = item.Split('_')[0];
            string val = item.Split('_')[1];
            PlistElementDict typeRole = urlArray.AddDict();
            typeRole.SetString("CFBundleTypeRole", "Editor");
            typeRole.SetString("CFBundleURLName", kay);
            PlistElementArray urlScheme = typeRole.CreateArray("CFBundleURLSchemes");
            urlScheme.AddString(val);
        }

        PlistElementArray wxArray = plist.root.CreateArray("LSApplicationQueriesSchemes");
        wxArray.AddString("weixin");
        wxArray.AddString("weixinULAPI");


        File.WriteAllText(listPath, plist.WriteToString());
    }

    private static void resetAppController(string path, string pathToBuiltProject)
    {
        string projectRootPath = Path.GetFullPath(pathToBuiltProject);
        //UnityAppController.mm
        string fileName = Path.Combine(projectRootPath, "Classes/UnityAppController.mm");
        XCodeClass xc = new XCodeClass(fileName);
        xc.WriteBelow("#import \"UnityAppController.h\"", "#import \"WXApiManager.h\"\r#import <AlipaySDK/AlipaySDK.h>");
        xc.WriteBelow("UnityLowMemory();\n}", "- (BOOL)application:(UIApplication *)application handleOpenURL:(NSURL *)url\r{\r    return  [WXApi handleOpenURL:url delegate:[WXApiManager sharedManager]];\r}\r-(BOOL) application:(UIApplication *)application continueUserActivity:(NSUserActivity *)userActivity restorationHandler:(void (^)(NSArray<id<UIUserActivityRestoring>> * _Nullable))restorationHandler\r{\r  return [WXApi handleOpenUniversalLink:userActivity delegate:[WXApiManager sharedManager]];\r}");
        xc.WriteBelow("_unityView];\n", "BOOL b =[WXApi registerApp: @\"wxda445c2b2b452f25\" universalLink: @\"https://kimimath.kyd2002.cn/\"];\rNSLog(@ \"注册微信结果1-成功:%i\",b);");

        //xc.Replace("notifData);\n return YES;", "notifData);\n return [WXApi handleOpenURL:url delegate:[WXApiManager sharedManager]];");

    }

    public static void CopyAndReplaceDirectory(string srcPath, string dstPath)
    {
        if (Directory.Exists(dstPath))
            Directory.Delete(dstPath);
        if (File.Exists(dstPath))
            File.Delete(dstPath);

        Directory.CreateDirectory(dstPath);

        foreach (var file in Directory.GetFiles(srcPath))
        {
            string ext = Path.GetExtension(file);
            bool ignore = false;
            for (int i = 0; i < csIgnoreExtensions.Length; ++i)
            {
                ignore = (string.Compare(ext, csIgnoreExtensions[i], true) == 0);
                if (ignore)
                    break;
            }
            if (!ignore)
                File.Copy(file, Path.Combine(dstPath, Path.GetFileName(file)));
        }

        foreach (var dir in Directory.GetDirectories(srcPath))
            CopyAndReplaceDirectory(dir, Path.Combine(dstPath, Path.GetFileName(dir)));
    }
#endif
}
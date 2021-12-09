//
//  ViewController.m
//  WXCeshi
//
//

#import "WXApiManager.h"

#import <AlipaySDK/AlipaySDK.h>
#import <AVFoundation/AVFoundation.h>

#import <WebKit/WebKit.h>
/*
@interface UIViewController ()<WKScriptMessageHandler>
@property(nonatomic, strong) WKWebView *webView1; // 强引用一下
@property(nonatomic, strong) WKWebView *webView2; // 强引用一下
@end
 */
WKWebView *_webview;

void getUserAgentWK()
{
    _webview= [[WKWebView alloc] init];
        
        NSURLRequest *request = [[NSURLRequest alloc] initWithURL:[NSURL URLWithString:@"www.allhistory.com"] cachePolicy:NSURLRequestUseProtocolCachePolicy timeoutInterval:10];
        [_webview loadRequest:request];
        [_webview evaluateJavaScript:@"navigator.userAgent" completionHandler:^(id result, NSError *error) {
            NSString *userAgent = result;
            NSArray *stringArray = [userAgent componentsSeparatedByString:@" "];
            if ([stringArray containsObject:@" marble"]) {
                return;
            }
            
            NSString *newUserAgent = [userAgent stringByAppendingString:@" marble"];
            
            NSDictionary *dictionary = [NSDictionary dictionaryWithObjectsAndKeys:newUserAgent, @"UserAgent", nil];
            [[NSUserDefaults standardUserDefaults] registerDefaults:dictionary];
            [[NSUserDefaults standardUserDefaults] synchronize];
            NSLog(@"wk useragent %@",userAgent);
            UnitySendMessage("Global","GetUGIOSCallBack",userAgent.UTF8String);
        }];
}


void getUserAgent() {
       
    getUserAgentWK();
    /*    UIWebView 方式
    UIWebView *webView = [[UIWebView alloc] initWithFrame:CGRectZero];
        NSString *userAgent = [webView stringByEvaluatingJavaScriptFromString:@"navigator.userAgent"];
        NSLog(@"wk UIWebView useragent%@",userAgent);
    UnitySendMessage("Global","GetUGIOSCallBack",userAgent.UTF8String);
     */
    
}

void getDeviceVolume()
{
    AVAudioSession *audioSession = [AVAudioSession sharedInstance];
    CGFloat currentVol = audioSession.outputVolume;
    
    NSString *volume=[NSString stringWithFormat:@"%g",currentVol];
    NSLog(@"volume%@",volume);
    UnitySendMessage("Global","GetCurDeviceVolume",volume.UTF8String);
}

void clkSkipProgram()
{
    NSLog (@ "jun:%@" ,@"90");
    BOOL b=[WXApi isWXAppInstalled];
    NSLog (@ "微信安装与否:%i" ,b);
    if(b==1)
    {
        
        WXLaunchMiniProgramReq *launchMini = [WXLaunchMiniProgramReq object];
            launchMini.userName = @"gh_030703d45b4f";
            launchMini.path = @"pages/index/index";
            launchMini.miniProgramType = WXMiniProgramTypeRelease;
        __block BOOL isSuccess  = YES;
        [WXApi sendReq:launchMini completion:^(BOOL success) {
            isSuccess=success;
        }];
        NSLog (@ "跳转小程序结果:%i" ,isSuccess);
        
    }
    else
    {
        NSLog (@ "未安装:%@" ,@"安装微信先");
        UnitySendMessage("Global" ,"CallBackProgram","0");
    }
 
 
}

void weChatShare(char *imgPath)
{
    NSString *imgph = [NSString stringWithUTF8String:imgPath];
    UIImage *image = [UIImage imageNamed:imgph];
    NSData *imageData = UIImageJPEGRepresentation(image, 0.7);   //0.7压缩系数，1为不压缩
       
    WXImageObject *imageObject = [WXImageObject object];
    imageObject.imageData = imageData;

    WXMediaMessage *message = [WXMediaMessage message];
    NSString *filePath = [[NSBundle mainBundle] pathForResource:@"res5"
                                                         ofType:@"jpg"];
    message.thumbData = [NSData dataWithContentsOfFile:filePath];
    message.mediaObject = imageObject;

    SendMessageToWXReq *req = [[SendMessageToWXReq alloc] init];
    req.bText = NO;
    req.message = message;
    req.scene = 0;//
    __block BOOL isSuccess  = YES;
    [WXApi sendReq:req completion:^(BOOL success) {
        isSuccess=success;
    }];
    NSLog (@ "分享结果:%i" ,isSuccess);
}

void iOSApiPay(char *orderString)
{
     //应用注册scheme,在Info.plist定义URL types
    NSString *appScheme = @"apipay";
    NSString *orderStr = [NSString stringWithUTF8String:orderString];
    if (orderString != nil) {
        [[AlipaySDK defaultService] payOrder:orderStr fromScheme:appScheme callback:^(NSDictionary *resultDic) {
            NSLog(@"reslut = %@",resultDic);
            NSError *error;
            NSData *jsonData = [NSJSONSerialization dataWithJSONObject:resultDic options:NSJSONWritingPrettyPrinted error:&error];
            NSString *jsonString;
            if (!jsonData) {
                NSLog(@"%@",error);
            }else{
            jsonString = [[NSString alloc]initWithData:jsonData encoding:NSUTF8StringEncoding];
            }
            char *jsonResult= (char*) [jsonString UTF8String];
            UnitySendMessage("Global" ,"iOSApiPayCallback",jsonResult);
        }];
    }
}

void iOSWeChatPay(char *appId, char *partnerId,char *prepayId,char *nonceStr,char *timeStamp,char *sign)
{
    //@"Sign=WXPay"
    NSString *strappId = [NSString stringWithUTF8String:appId];
    NSString *strpartnerId = [NSString stringWithUTF8String:partnerId];
    NSString *strprepayId = [NSString stringWithUTF8String:prepayId];
    NSString *strnonceStr = [NSString stringWithUTF8String:nonceStr];
    NSString *strtimeStamp = [NSString stringWithUTF8String:timeStamp];
    NSString *strsign = [NSString stringWithUTF8String:sign];
    NSLog (@ "jun:%@" ,@"90");
    BOOL b=[WXApi isWXAppInstalled];
    NSLog (@ "微信安装与否:%i" ,b);
    if(b==1)
    {
        //调起微信支付
        PayReq* req = [[PayReq alloc] init];
        req.openID = strappId;
        req.partnerId = strpartnerId;
        req.prepayId = strprepayId;
        req.nonceStr = strnonceStr;
        req.timeStamp = strtimeStamp.longLongValue;
        req.package = @"Sign=WXPay";
        req.sign = strsign;
        //[WXApi sendReq:req];
         __block BOOL isSuccess  = YES;
        [WXApi sendReq:req completion:^(BOOL success) {
            isSuccess=success;
        }];
        NSLog (@ "微信支付结果:%i" ,isSuccess);
    }
    else
    {
        NSLog (@ "未安装:%@" ,@"安装微信先");
        UnitySendMessage("Global" ,"CallBackProgram","0");
    }
}

void CopyContent(char *readAddr)
{
    NSString *strReadAddr = [NSString stringWithUTF8String:readAddr];
    UIPasteboard *pasteboard = [UIPasteboard generalPasteboard];
    pasteboard.string =strReadAddr;
    UnitySendMessage("Global", "CopyMessage", "0");
}

void CancelAccountTakePhone()
{
//第一种不回原来应用
    //NSMutableString* str=[[NSMutableString alloc] initWithFormat:@"tel:%@",@"4006780315"];
    //[[UIApplication sharedApplication] openURL:[NSURL URLWithString:str]];

//第二种提示回到原来应用(采用)
    //第三种提示加回应用（telprompt）
    NSMutableString *str=[[NSMutableString alloc] initWithFormat:@"telprompt://%@",@"4006780315"];
    [[UIApplication sharedApplication] openURL:[NSURL URLWithString:str]];

}

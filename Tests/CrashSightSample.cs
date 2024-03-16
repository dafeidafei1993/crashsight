using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;

public class CrashSightSample : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // TODO NOT Required. Enable debug log print, please set false for release version
// Debug开关，Debug模式下会打印更多便于问题定位的Log.
#if DEBUG
        CrashSightAgent.ConfigDebugMode (true);
#endif
        CrashSightAgent.ConfigDebugMode (false);

// 设置上报的目标域名，请根据项目需求进行填写。（必填）
CrashSightAgent.ConfigCrashServerUrl("https://android.crashsight.qq.com/pb/async");
// 设置上报所指向的APP ID, 并进行初始化。APPID可以在管理端更多->产品设置->产品信息中找到。
CrashSightAgent.InitWithAppId("app id");
    }

    // Update is called once per frame
    public void CrashTestJavaCrash()
    {
        CrashSightAgent.TestJavaCrash();//测试Java崩溃（Android）
    }
     public void CrashtestNativeCrash()
    {
        CrashSightAgent.TestNativeCrash();//测试Native崩溃（Android&iOS）
    }

     public void CrashtestOcCrash()
    {
        CrashSightAgent.TestOcCrash();//测试测试Object-C崩溃（iOS）
    }
    public void CrashtestAccessViolationCrash()
    {
        UnityEngine.Diagnostics.Utils.ForceCrash(ForcedCrashCategory.AccessViolation);//强制Unity崩溃（Windows可用）
    }
}


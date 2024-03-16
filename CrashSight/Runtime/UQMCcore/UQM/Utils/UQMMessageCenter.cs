using AOT;
using System.Runtime.InteropServices;
using UnityEngine;


namespace GCloud.UQM
{
    public class RetArgsWrapper
    {
        private readonly int methodId;
        private readonly string retJson;
        private readonly int crashType;
        private readonly int logUploadResult;

        public int MethodId
        {
            get { return methodId; }
        }

        public string RetJson
        {
            get { return retJson; }
        }

        public int CrashType
        {
            get { return crashType; }
        }

        public int LogUploadResult
        {
            get { return logUploadResult; }
        }

        public RetArgsWrapper(int _methodId, string _retJson, int _crashType, int _logUploadResult)
        {
            methodId = _methodId;
            retJson = _retJson;
            crashType = _crashType;
            logUploadResult = _logUploadResult;
        }
    }

    #region UQMMessageCenter

    public class UQMMessageCenter : MonoBehaviour
    {
        #region json ret and callback

        private static bool initialzed = false;

        private delegate string UQMRetJsonEventHandler(int methodId, int callType, int logUploadResult);

        [MonoPInvokeCallback(typeof(UQMRetJsonEventHandler))]
        public static string OnUQMRet(int methodId, int crashType, int logUploadResult)
        {
            string ret = ""; // CrashSight C++层没有JSON序列换的库，不传递对象的JSON字符串
            var argsWrapper = new RetArgsWrapper(methodId, ret, crashType, logUploadResult);
            UQMLog.Log("OnUQMRet, the methodId is ( " + methodId + " )  crashType=" + crashType);
#if UNITY_WSA

#else
            if (methodId == (int)UQMMethodNameID.UQM_CRASH_CALLBACK_EXTRA_MESSAGE
                || methodId == (int)UQMMethodNameID.UQM_CRASH_CALLBACK_EXTRA_DATA
                || methodId == (int)UQMMethodNameID.UQM_CRASH_CALLBACK_SET_LOG_PATH
                || methodId == (int)UQMMethodNameID.UQM_CRASH_CALLBACK_LOG_UPLOAD_RESULT
                )
            {
                string result = SynchronousDelegate(argsWrapper);
                return result;
            }
#endif
            return "";
        }

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        [DllImport(UQM.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void cs_setUnityCallback(UQMRetJsonEventHandler eventHandler);
#endif
#endregion

        static UQMMessageCenter instance;

        public static UQMMessageCenter Instance
        {
            get
            {
                if (instance != null) return instance;
                var bridgeGameObject = new GameObject { name = "UQMMessageCenter" };
                DontDestroyOnLoad(bridgeGameObject);
                instance = bridgeGameObject.AddComponent(typeof(UQMMessageCenter)) as UQMMessageCenter;
                UQMLog.Log("UQMMessageCenter  instance=" + instance);
                return instance;
            }
        }

        public void Init()
        {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
           if (initialzed) {
                return;
            }
            cs_setUnityCallback(OnUQMRet);
            initialzed = true;
#endif
            UQMLog.Log("UQM Init, set unity callback");
        }

        public void Uninit()
        {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            cs_setUnityCallback(null);
#endif
            UQMLog.Log("UQM Uninit, set unity callback to null");
        }

        static string SynchronousDelegate(object arg)
        {
            var argsWrapper = (RetArgsWrapper)arg;
            var methodId = argsWrapper.MethodId;
            var json = argsWrapper.RetJson;
            var crashType = argsWrapper.CrashType;

            UQMLog.Log("the methodId is ( " + methodId + " ) and json= " + json + " crashType=" + crashType);
            switch (methodId)
            {
#if UNITY_WSA
#else
                case (int)UQMMethodNameID.UQM_CRASH_CALLBACK_EXTRA_MESSAGE:
                    return UQMCrash.OnCrashCallbackMessage(methodId, crashType);

                case (int)UQMMethodNameID.UQM_CRASH_CALLBACK_EXTRA_DATA:
                    return UQMCrash.OnCrashCallbackData(methodId, crashType);

                case (int)UQMMethodNameID.UQM_CRASH_CALLBACK_SET_LOG_PATH:
                    return UQMCrash.OnCrashSetLogPathMessage(methodId, crashType);

                case (int)UQMMethodNameID.UQM_CRASH_CALLBACK_LOG_UPLOAD_RESULT:
                    return UQMCrash.OnCrashLogUploadMessage(methodId, crashType, argsWrapper.LogUploadResult);
#endif
            }
            return "";
        }
#endregion
    }
}
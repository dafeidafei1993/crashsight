using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace GCloud.UQM
{
    public enum UQMCrashLevel
    {
        CSLogLevelSilent = 0, //关闭日志记录功能
        CSLogLevelError = 1,
        CSLogLevelWarn = 2,
        CSLogLevelInfo = 3,
        CSLogLevelDebug = 4,
        CSLogLevelVerbose = 5,
    }

    public static class UQMCrash
    {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        [DllImport(UQM.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void cs_configAutoReportLogLevelAdapter(int level);

        [DllImport(UQM.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void cs_configGameTypeAdapter(int gameType);

        [DllImport(UQM.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void cs_configCallbackTypeAdapter(Int32 callbackType);


        [DllImport(UQM.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void cs_configDefaultAdapter([MarshalAs(UnmanagedType.LPStr)] string channel,
            [MarshalAs(UnmanagedType.LPStr)] string version,
            [MarshalAs(UnmanagedType.LPStr)] string user,
            long delay);


        [DllImport(UQM.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void cs_configCrashServerUrlAdapter([MarshalAs(UnmanagedType.LPStr)] string serverUrl);


        [DllImport(UQM.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void cs_configDebugModeAdapter(bool enable);


        [DllImport(UQM.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void cs_initWithAppIdAdapter([MarshalAs(UnmanagedType.LPStr)] string appId);


        [DllImport(UQM.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void cs_logRecordAdapter(int level, [MarshalAs(UnmanagedType.LPStr)] string message);


        [DllImport(UQM.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void cs_addSceneDataAdapter([MarshalAs(UnmanagedType.LPStr)] string k, [MarshalAs(UnmanagedType.LPStr)] string v);


        [DllImport(UQM.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void cs_reportExceptionV1Adapter(int type, [MarshalAs(UnmanagedType.LPStr)] string name,
            [MarshalAs(UnmanagedType.LPStr)] string message, [MarshalAs(UnmanagedType.LPStr)] string stackTrace,
            [MarshalAs(UnmanagedType.LPStr)] string extras, bool quitProgram);


        [DllImport(UQM.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void cs_reportExceptionV2Adapter(int type, [MarshalAs(UnmanagedType.LPStr)] string exceptionName,
            [MarshalAs(UnmanagedType.LPStr)] string exceptionMsg, [MarshalAs(UnmanagedType.LPStr)] string exceptionStack,
            [MarshalAs(UnmanagedType.LPStr)] string paramsJson, int dumpNativeType);


        [DllImport(UQM.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void cs_setUserIdAdapter([MarshalAs(UnmanagedType.LPStr)] string userId);


        [DllImport(UQM.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void cs_setSceneAdapter(int sceneId);


        [DllImport(UQM.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void cs_unityCrashCallback();

        [DllImport(UQM.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void cs_unregisterUnityCrashCallback();

        [DllImport(UQM.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void cs_unityCrashLogCallback();

        [DllImport(UQM.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void cs_reRegistAllMonitorsAdapter();

        [DllImport(UQM.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void cs_reportLogInfo([MarshalAs(UnmanagedType.LPStr)] string msgType,[MarshalAs(UnmanagedType.LPStr)] string msg);

        [DllImport(UQM.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void cs_setAppVersionAdapter([MarshalAs(UnmanagedType.LPStr)] string appVersion);

        [DllImport(UQM.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void cs_setDeviceIdAdapter([MarshalAs(UnmanagedType.LPStr)] string deviceId);

        [DllImport(UQM.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void cs_setCustomizedDeviceIDAdapter([MarshalAs(UnmanagedType.LPStr)] string deviceId);

        [DllImport(UQM.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr cs_getSDKDefinedDeviceIDAdapter();

        [DllImport(UQM.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void cs_setCustomizedMatchIDAdapter([MarshalAs(UnmanagedType.LPStr)] string matchId);

        [DllImport(UQM.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr cs_getSDKSessionIDAdapter();

        [DllImport(UQM.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr cs_getCrashUuidAdapter();

        [DllImport(UQM.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void cs_setDeviceModelAdapter([MarshalAs(UnmanagedType.LPStr)] string deviceModel);

        [DllImport(UQM.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void cs_setLogPathAdapter([MarshalAs(UnmanagedType.LPStr)] string logPath);

        [DllImport(UQM.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void cs_testOomCrashAdapter();

        [DllImport(UQM.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void cs_testJavaCrashAdapter();

        [DllImport(UQM.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void cs_testOcCrashAdapter();

        [DllImport(UQM.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void cs_testNativeCrashAdapter();

        [DllImport(UQM.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern long cs_getCrashThreadId();
#endif

#if (UNITY_STANDALONE_WIN && !UNITY_EDITOR)
        [DllImport("CrashSight64", CallingConvention = CallingConvention.Cdecl)]
        private static extern void CS_InitContext([MarshalAs(UnmanagedType.LPStr)] string id, [MarshalAs(UnmanagedType.LPStr)] string version, [MarshalAs(UnmanagedType.LPStr)] string key);
        [DllImport("CrashSight64", CallingConvention = CallingConvention.Cdecl)]
        private static extern void CS_ReportException(int type,[MarshalAs(UnmanagedType.LPStr)] string name, [MarshalAs(UnmanagedType.LPStr)] string message,[MarshalAs(UnmanagedType.LPStr)] string stack_trace,
                                   [MarshalAs(UnmanagedType.LPStr)] string extras, bool is_async);
        //[DllImport("CrashSight64", CallingConvention = CallingConvention.Cdecl)]
        //private static extern void CS_SetCrashCallback(CrashCallbackFuncPtr callback);
        [DllImport("CrashSight64", CallingConvention = CallingConvention.Cdecl)]
        private static extern void CS_SetUserValue([MarshalAs(UnmanagedType.LPStr)] string key, [MarshalAs(UnmanagedType.LPStr)] string value);
        [DllImport("CrashSight64", CallingConvention = CallingConvention.Cdecl)]
        private static extern void CS_SetVehEnable(bool enable);
        [DllImport("CrashSight64", CallingConvention = CallingConvention.Cdecl)]
        private static extern void CS_SetExtraHandler(bool extra_handle_enable);
        [DllImport("CrashSight64", CallingConvention = CallingConvention.Cdecl)]
        private static extern void CS_SetCustomLogDir([MarshalAs(UnmanagedType.LPStr)] string log_path);
        [DllImport("CrashSight64", CallingConvention = CallingConvention.Cdecl)]
        private static extern void CS_SetUserId([MarshalAs(UnmanagedType.LPStr)] string user_id);
        [DllImport("CrashSight64", CallingConvention = CallingConvention.Cdecl)]
        private static extern void CS_MonitorEnable(bool enable);
        [DllImport("CrashSight64", CallingConvention = CallingConvention.Cdecl)]
        private static extern void CS_PrintLog(int level, [MarshalAs(UnmanagedType.LPStr)] string tag, [MarshalAs(UnmanagedType.LPStr)] string format);
        [DllImport("CrashSight64", CallingConvention = CallingConvention.Cdecl)]
        private static extern void CS_UploadGivenPathDump([MarshalAs(UnmanagedType.LPStr)] string dump_dir, bool is_extra_check);
#endif

        /// <summary>
        /// Crash回调方法，提供上报用户数据能力
        /// </summary>
        public static event OnUQMStringRetEventHandler<int> CrashBaseRetEvent;

        public static event OnUQMStringRetSetLogPathEventHandler<int> CrashSetLogPathRetEvent;
        public static event OnUQMRetLogUploadEventHandler<int> CrashLogUploadRetEvent;

        private static AndroidJavaClass _gameAgentClass = null;
        private static bool _isLoadedSo = false;
        private static int _gameType = 0; // COCOS=1, UNITY=2, UNREAL=3
        private static readonly string GAME_AGENT_CLASS = "com.uqm.crashsight.core.api.CrashSightPlatform";

        public static AndroidJavaClass CrashSightPlatform
        {
            get
            {
                if (_gameAgentClass == null)
                {
                    _gameAgentClass = new AndroidJavaClass(GAME_AGENT_CLASS);
                }
                return _gameAgentClass;
            }
        }

        private static void LoadCrashSightCoreSo()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (_isLoadedSo)
            {
                return;
            }
            try
            {
                CrashSightPlatform.CallStatic<bool>("loadCrashSightCoreSo");
                _isLoadedSo = true;
            }
            catch (Exception ex)
            {
                UQMLog.LogError("loadSo with unknown error = \n" + ex.Message + "\n" + ex.StackTrace);
            }
#endif
        }

        public static void ConfigCallbackType(Int32 callbackType)
        {
            try
            {
                UQMLog.Log("ConfigCallbackType  callbackType=" + callbackType);
                LoadCrashSightCoreSo();
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            cs_configCallbackTypeAdapter(callbackType);
#endif
            }
            catch (Exception ex)
            {
                UQMLog.LogError("ConfigCallbackType with unknown error = \n" + ex.Message + "\n" + ex.StackTrace);
            }
        }

        public static void ConfigGameType(int gameType)
        {
            try
            {
                UQMLog.Log("SetGameType gameType=" + gameType);
                _gameType = gameType;
                LoadCrashSightCoreSo();
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                cs_configGameTypeAdapter(gameType);
#endif
            }
            catch (Exception ex)
            {
                UQMLog.LogError("SetGameType with unknown error = \n" + ex.Message + "\n" + ex.StackTrace);
            }
        }

        public static void ConfigAutoReportLogLevel(int level)
        {
            try
            {
                UQMLog.Log("ConfigAutoReportLogLevel  level=" + level);
                LoadCrashSightCoreSo();
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                cs_configAutoReportLogLevelAdapter(level);
#endif
            }
            catch (Exception ex)
            {
                UQMLog.LogError("ConfigAutoReportLogLevel with unknown error = \n" + ex.Message + "\n" + ex.StackTrace);
            }
        }

        public static void ConfigCrashServerUrl(string serverUrl)
        {
            try
            {
                UQMLog.Log("ConfigCrashServerUrl  serverUrl=" + serverUrl);
                LoadCrashSightCoreSo();
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                cs_configCrashServerUrlAdapter(serverUrl);
#endif
            }
            catch (Exception ex)
            {
                UQMLog.LogError("ConfigCrashServerUrl with unknown error = \n" + ex.Message + "\n" + ex.StackTrace);
            }
        }

        public static void ConfigDebugMode(bool enable)
        {
            try
            {
                if (enable)
                {
                    UQMLog.SetLevel(UQMLog.Level.Log);
                }
                LoadCrashSightCoreSo();
                UQMLog.Log("ConfigDebugMode  enable=" + enable);
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                cs_configDebugModeAdapter(enable);
#endif
            }
            catch (Exception ex)
            {
                UQMLog.LogError("ConfigDebugMode with unknown error = \n" + ex.Message + "\n" + ex.StackTrace);
            }
        }

        public static void ConfigDefault(string channel, string version, string user, long delay)
        {
            try
            {
                UQMLog.Log("ConfigDefault  channel=" + channel + " version=" + version + " user=" + user + " delay=" + delay);
                LoadCrashSightCoreSo();
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                cs_configDefaultAdapter(channel, version, user, delay);
#endif
            }
            catch (Exception ex)
            {
                UQMLog.LogError("ConfigDefault with unknown error = \n" + ex.Message + "\n" + ex.StackTrace);
            }
        }

        public static void InitWithAppId(string appId)
        {
            try
            {
                UQMLog.Log("InitWithAppId appId = " + appId);
                LoadCrashSightCoreSo();
                if (_gameType == 0) {
                    ConfigGameType(2);  // 默认Unity
                }
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                    cs_initWithAppIdAdapter(appId);
#endif
            }
            catch (Exception ex)
            {
                UQMLog.LogError("InitWithAppId with unknown error = \n" + ex.Message + "\n" + ex.StackTrace);
            }
        }

        public static void InitContext(string userId, string version, string key)
        {
            try
            {
                UQMLog.Log("InitContext user_id = " + userId);
#if (UNITY_STANDALONE_WIN) && !UNITY_EDITOR
                CS_MonitorEnable(false);
                CS_InitContext(userId,version, key );
#endif
            }
            catch (Exception ex)
            {
                UQMLog.LogError("InitWithAppId with unknown error = \n" + ex.Message + "\n" + ex.StackTrace);
            }
        }


        public static void LogRecord(int level, string message)
        {
            try
            {
                UQMLog.Log("LogRecord  level=" + level + " message=" + message);
                LoadCrashSightCoreSo();
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                cs_logRecordAdapter (level, message);
#endif
#if (UNITY_STANDALONE_WIN) && !UNITY_EDITOR
                    CS_PrintLog(level, "", message );
#endif

            }
            catch (Exception ex)
            {
                UQMLog.LogError("LogRecord with unknown error = \n" + ex.Message + "\n" + ex.StackTrace);
            }
        }

        public static void AddSceneData(string k, string v)
        {
            try
            {
                UQMLog.Log("AddSceneData  key=" + k + " value=" + v);
                LoadCrashSightCoreSo();
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                cs_addSceneDataAdapter(k, v);
#endif
#if (UNITY_STANDALONE_WIN) && !UNITY_EDITOR
                    CS_SetUserValue(k, v);
#endif
            }
            catch (Exception ex)
            {
                UQMLog.LogError("AddSceneData with unknown error = \n" + ex.Message + "\n" + ex.StackTrace);
            }
        }

        public static void ReportException(int type, string name, string message, string stackTrace, string extras, bool quitProgram)
        {
            try
            {
                if (name == null)
                {
                    name = "";
                }
                if (message == null)
                {
                    message = "";
                }
                if (stackTrace == null)
                {
                    stackTrace = "";
                }
                if (extras == null)
                {
                    extras = "";
                }
                UQMLog.Log("ReportException  name=" + name + " quitProgram=" + quitProgram);
                LoadCrashSightCoreSo();
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                cs_reportExceptionV1Adapter (type, name, message, stackTrace, extras, quitProgram);
#endif
#if (UNITY_STANDALONE_WIN) && !UNITY_EDITOR
                    CS_ReportException(type, name, message, stackTrace, extras, true);
#endif
            }
            catch (Exception ex)
            {
                UQMLog.LogError("ReportException with unknown error = \n" + ex.Message + "\n" + ex.StackTrace);
            }
        }

        // CS 上报lua等堆栈信息
        public static void ReportException(int type, string exceptionName, string exceptionMsg, string exceptionStack, Dictionary<string, string> extInfo)
        {
            try
            {
                if (exceptionName == null)
                {
                    exceptionName = "";
                }
                if (exceptionMsg == null)
                {
                    exceptionMsg = "";
                }
                if (exceptionStack == null)
                {
                    exceptionStack = "";
                }
                UQMLog.Log("ReportException  exceptionName=" + exceptionName + " exceptionMsg=" + exceptionMsg);
                LoadCrashSightCoreSo();
                string paramsJson = MiniJSON.Json.Serialize(extInfo);
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                cs_reportExceptionV2Adapter (type, exceptionName, exceptionMsg, exceptionStack, paramsJson, 0);
#endif
#if (UNITY_STANDALONE_WIN) && !UNITY_EDITOR
                    CS_ReportException(type, exceptionName, exceptionMsg, exceptionStack, paramsJson, true);
                    Debug.Log("ReportException!");

#endif
            }
            catch (Exception ex)
            {
                UQMLog.LogError("ReportException with unknown error = \n" + ex.Message + "\n" + ex.StackTrace);
            }
        }

        public static void ReportException(int type, string exceptionName, string exceptionMsg, string exceptionStack, Dictionary<string, string> extInfo, int dumpNativeType)
        {
            try
            {
                if (exceptionName == null)
                {
                    exceptionName = "";
                }
                if (exceptionMsg == null)
                {
                    exceptionMsg = "";
                }
                if (exceptionStack == null)
                {
                    exceptionStack = "";
                }
                UQMLog.Log(string.Format("ReportException exceptionName={0} exceptionMsg={1} dumpNativeType={2}", exceptionName, exceptionMsg, dumpNativeType));
                LoadCrashSightCoreSo();
                string paramsJson = MiniJSON.Json.Serialize(extInfo);
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                cs_reportExceptionV2Adapter (type, exceptionName, exceptionMsg, exceptionStack, paramsJson, dumpNativeType);
#endif
            }
            catch (Exception ex)
            {
                UQMLog.LogError("ReportException with unknown error = \n" + ex.Message + "\n" + ex.StackTrace);
            }
        }

        public static void SetUserId(string userId)
        {
            try
            {
                UQMLog.Log("SetUserId userId = " + userId);
                LoadCrashSightCoreSo();
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                    cs_setUserIdAdapter(userId);
#endif
#if (UNITY_STANDALONE_WIN) && !UNITY_EDITOR
                    CS_SetUserId(userId);
#endif
            }
            catch (Exception ex)
            {
                UQMLog.LogError("SetUserId with unknown error = \n" + ex.Message + "\n" + ex.StackTrace);
            }
        }

        public static void SetScene(int sceneId)
        {
            try
            {
                UQMLog.Log("SetScene sceneId = " + sceneId);
                LoadCrashSightCoreSo();
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                    cs_setSceneAdapter(sceneId);
#endif
            }
            catch (Exception ex)
            {
                UQMLog.LogError("SetScene with unknown error = \n" + ex.Message + "\n" + ex.StackTrace);
            }
        }

        public static void ReRegistAllMonitors()
        {
            try
            {
                UQMLog.Log("ReRegistAllMonitors");
                LoadCrashSightCoreSo();
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                cs_reRegistAllMonitorsAdapter();
#endif
            }
            catch (Exception ex)
            {
                UQMLog.LogError("ReRegistAllMonitors with unknown error = \n" + ex.Message + "\n" + ex.StackTrace);
            }
        }

        public static void ReportLogInfo(string msgType, string msg) {
            try
            {
                UQMLog.Log("ReportLogInfo");
                LoadCrashSightCoreSo();
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                cs_reportLogInfo(msgType, msg);
#endif
            }
            catch (Exception ex)
            {
                UQMLog.LogError("ReportLogInfo with unknown error = \n" + ex.Message + "\n" + ex.StackTrace);
            }
        }

        public static void SetAppVersion(string appVersion)
        {
            try
            {
                UQMLog.Log("SetAppVersion appVersion = " + appVersion);
                LoadCrashSightCoreSo();
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                    cs_setAppVersionAdapter(appVersion);
#endif
            }
            catch (Exception ex)
            {
                UQMLog.LogError("SetAppVersion with unknown error = \n" + ex.Message + "\n" + ex.StackTrace);
            }
        }

        public static void SetDeviceId(string deviceId)
        {
            try
            {
                UQMLog.Log("SetDeviceId deviceId = " + deviceId);
                LoadCrashSightCoreSo();
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                    cs_setDeviceIdAdapter(deviceId);
#endif
            }
            catch (Exception ex)
            {
                UQMLog.LogError("SetDeviceId with unknown error = \n" + ex.Message + "\n" + ex.StackTrace);
            }
        }

        public static void SetCustomizedDeviceID(string deviceId)
        {
            try
            {
                UQMLog.Log("SetCustomizedDeviceID deviceId = " + deviceId);
                LoadCrashSightCoreSo();
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                    cs_setCustomizedDeviceIDAdapter(deviceId);
#endif
            }
            catch (Exception ex)
            {
                UQMLog.LogError("SetCustomizedDeviceID with unknown error = \n" + ex.Message + "\n" + ex.StackTrace);
            }
        }

        public static string GetSDKDefinedDeviceID()
        {
            try
            {
                UQMLog.Log("GetSDKDefinedDeviceID");
                LoadCrashSightCoreSo();
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                IntPtr tranResult = cs_getSDKDefinedDeviceIDAdapter();
                return Marshal.PtrToStringAnsi(tranResult);
#endif
            }
            catch (Exception ex)
            {
                UQMLog.LogError("GetSDKDefinedDeviceID with unknown error = \n" + ex.Message + "\n" + ex.StackTrace);
            }
            return "";
        }


        public static void SetCustomizedMatchID(string matchId)
        {
            try
            {
                UQMLog.Log("SetCustomizedMatchID matchId = " + matchId);
                LoadCrashSightCoreSo();
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                    cs_setCustomizedMatchIDAdapter(matchId);
#endif
            }
            catch (Exception ex)
            {
                UQMLog.LogError("SetCustomizedMatchID with unknown error = \n" + ex.Message + "\n" + ex.StackTrace);
            }
        }

        public static string GetSDKSessionID()
        {
            try
            {
                UQMLog.Log("GetSDKSessionID");
                LoadCrashSightCoreSo();
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                IntPtr tranResult = cs_getSDKSessionIDAdapter();
                return Marshal.PtrToStringAnsi(tranResult);
#endif
            }
            catch (Exception ex)
            {
                UQMLog.LogError("GetSDKSessionID with unknown error = \n" + ex.Message + "\n" + ex.StackTrace);
            }
            return "";
        }

        public static string GetCrashUuid()
        {
            try
            {
                UQMLog.Log("GetCrashUuid");
                LoadCrashSightCoreSo();
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                IntPtr tranResult = cs_getCrashUuidAdapter();
                return Marshal.PtrToStringAnsi(tranResult);
#endif
            }
            catch (Exception ex)
            {
                UQMLog.LogError("GetCrashUuid with unknown error = \n" + ex.Message + "\n" + ex.StackTrace);
            }
            return "";
        }

        public static void SetDeviceModel(string deviceModel)
        {
            try
            {
                UQMLog.Log("SetDeviceModel deviceModel = " + deviceModel);
                LoadCrashSightCoreSo();
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                    cs_setDeviceModelAdapter(deviceModel);
#endif
            }
            catch (Exception ex)
            {
                UQMLog.LogError("SetDeviceModel with unknown error = \n" + ex.Message + "\n" + ex.StackTrace);
            }
        }

        public static void SetLogPath(string logPath)
        {
            try
            {
                UQMLog.Log("SetLogPath logPath = " + logPath);
                LoadCrashSightCoreSo();
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                    cs_setLogPathAdapter(logPath);
#endif
            }
            catch (Exception ex)
            {
                UQMLog.LogError("SetLogPath with unknown error = \n" + ex.Message + "\n" + ex.StackTrace);
            }
        }

        public static void SetCrashCallback()
        {
            try
            {
                UQMLog.Log("SetCrashCallback");
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                cs_unityCrashCallback();
#endif
            }
            catch (Exception ex)
            {
                UQMLog.LogError("SetCrashCallback with unknown error = \n" + ex.Message + "\n" + ex.StackTrace);
            }

        }

        public static void UnsetCrashCallback()
        {
            try
            {
                UQMLog.Log("UnsetCrashCallback");
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                cs_unregisterUnityCrashCallback();
#endif
            }
            catch (Exception ex)
            {
                UQMLog.LogError("UnsetCrashCallback with unknown error = \n" + ex.Message + "\n" + ex.StackTrace);
            }

        }

        public static void SetCrashLogCallback()
        {
            try
            {
                UQMLog.Log("SetCrashLogCallback");
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                cs_unityCrashLogCallback();
#endif
            }
            catch (Exception ex)
            {
                UQMLog.LogError("SetCrashLogCallback with unknown error = \n" + ex.Message + "\n" + ex.StackTrace);
            }

        }

        //callback
        internal static string OnCrashCallbackMessage(int methodId, int crashType)
        {
            UQMLog.Log("OnCrashCallbackMessage  methodId= " + methodId + " crashType=" + crashType);
            if (CrashBaseRetEvent != null)
            {
                try
                {
                    return CrashBaseRetEvent(methodId, crashType);
                }
                catch (Exception e)
                {
                    UQMLog.LogError(e.StackTrace);
                }
            }
            else
            {
                UQMLog.LogError("No callback for OnCrashCallbackMessage !");
            }
            return "";
        }

        internal static string OnCrashCallbackData(int methodId, int crashType)
        {
            UQMLog.Log("OnCrashCallbackData  methodId= " + methodId + " crashType=" + crashType);
            if (CrashBaseRetEvent != null)
            {
                try
                {
                    return CrashBaseRetEvent(methodId, crashType);
                }
                catch (Exception e)
                {
                    UQMLog.LogError(e.StackTrace);
                }
            }
            else
            {
                UQMLog.LogError("No callback for OnCrashCallbackData !");
            }
            return "";
        }

        internal static string OnCrashSetLogPathMessage(int methodId, int crashType)
        {
            UQMLog.Log("OnCrashSetLogPathMessage  methodId= " + methodId + " crashType=" + crashType);
            if (CrashSetLogPathRetEvent != null)
            {
                try
                {
                    return CrashSetLogPathRetEvent(methodId, crashType);
                }
                catch (Exception e)
                {
                    UQMLog.LogError(e.StackTrace);
                }
            }
            else
            {
                UQMLog.LogError("No callback for OnCrashSetLogPathMessage !");
            }
            return "";
        }

        internal static string OnCrashLogUploadMessage(int methodId, int crashType, int result)
        {
            UQMLog.Log("OnCrashLogUploadMessage  methodId= " + methodId + " crashType=" + crashType);
            if (CrashLogUploadRetEvent != null)
            {
                try
                {
                    CrashLogUploadRetEvent(methodId, crashType, result);
                }
                catch (Exception e)
                {
                    UQMLog.LogError(e.StackTrace);
                }
            }
            else
            {
                UQMLog.LogError("No callback for OnCrashLogUploadMessage !");
            }
            return "";
        }

        public static void ConfigCallBack()
        {
            SetCrashCallback();
            UQMMessageCenter.Instance.Init();
        }

        public static void UnregisterCallBack()
        {
            UnsetCrashCallback();
            UQMMessageCenter.Instance.Uninit();
        }

        public static void ConfigLogCallBack()
        {
            SetCrashLogCallback();
            UQMMessageCenter.Instance.Init();
        }

        public static void SetCustomLogDir(string path)
        {
#if (UNITY_STANDALONE_WIN) && !UNITY_EDITOR
            CS_SetCustomLogDir(path);
#endif
        }

        // Test cases
        public static void TestOomCrash()
        {
            try
            {
                UQMLog.Log("TestOomCrash");
                LoadCrashSightCoreSo();
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                cs_testOomCrashAdapter();
#endif
            }
            catch (Exception ex)
            {
                UQMLog.LogError("TestOomCrash with unknown error = \n" + ex.Message + "\n" + ex.StackTrace);
            }
        }

        public static void TestJavaCrash()
        {
            try
            {
                UQMLog.Log("TestJavaCrash");
                LoadCrashSightCoreSo();
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                cs_testJavaCrashAdapter();
#endif
            }
            catch (Exception ex)
            {
                UQMLog.LogError("TestJavaCrash with unknown error = \n" + ex.Message + "\n" + ex.StackTrace);
            }
        }

        public static void TestOcCrash()
        {
            try
            {
                UQMLog.Log("TestOcCrash");
                LoadCrashSightCoreSo();
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                cs_testOcCrashAdapter();
#endif
            }
            catch (Exception ex)
            {
                UQMLog.LogError("TestOcCrash with unknown error = \n" + ex.Message + "\n" + ex.StackTrace);
            }
        }

        public static void TestNativeCrash()
        {
            try
            {
                UQMLog.Log("TestNativeCrash");
                LoadCrashSightCoreSo();
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                cs_testNativeCrashAdapter();
#endif
            }
            catch (Exception ex)
            {
                UQMLog.LogError("TestNativeCrash with unknown error = \n" + ex.Message + "\n" + ex.StackTrace);
            }
        }


        public static long GetCrashThreadId()
        {
            long thread_id = -1;
            try
            {
                UQMLog.Log("GetCrashThreadId");
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                thread_id = cs_getCrashThreadId();
#endif
            }
            catch (Exception ex)
            {
                UQMLog.LogError("GetCrashThreadId with unknown error = \n" + ex.Message + "\n" + ex.StackTrace);
            }
            return thread_id;
        }
    }
}
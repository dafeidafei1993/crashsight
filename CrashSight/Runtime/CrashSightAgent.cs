// ----------------------------------------
//
//  CrashSightAgent.cs
//
//  Author:
//       Yeelik,
//
//  Copyright (c) 2015 CrashSight. All rights reserved.
//
// ----------------------------------------
//
using UnityEngine;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

using GCloud.UQM;

// We dont use the LogType enum in Unity as the numerical order doesnt suit our purposes
/// <summary>
/// Log severity. 
/// { Log, LogDebug, LogInfo, LogWarning, LogAssert, LogError, LogException }
/// </summary>
public enum CSLogSeverity
{
    Log,
    LogDebug,
    LogInfo,
    LogWarning,
    LogAssert,
    LogError,
    LogException
}

/// <summary>
/// CrashSight agent.
/// </summary>
public sealed class CrashSightAgent
{
    private static string crashUploadUrl = "";

    // Define delegate support multicasting to replace the 'Application.LogCallback'
    public delegate void LogCallbackDelegate(string condition, string stackTrace, LogType type);


    /// <summary>
    /// Configs callback 
    /// 目前是5种类型，用5位表示。第一位表示crash，第二位表示anr，第三位表示u3d c# error，第四位表示js，第五位表示lua
    /// </summary>
    /// <param name="callbackType">default 0XFFFFFFFF</param>
    public static void ConfigCallbackType(Int32 callbackType) 
    {
        UQMCrash.ConfigCallbackType(callbackType);
    }

    /// <summary>
    /// Configs the type of the crash reporter and customized log level to upload for ios
    /// </summary>
    /// <param name="type">Type. Default=0, 1=CrashSight v2.x MSDK=2 (ignore)</param>
    /// <param name="logLevel">Log level. Off=0,Error=1,Warn=2,Info=3,Debug=4</param>
    public static void ConfigCrashReporter(int type, int logLevel)
    {
        UQMCrash.ConfigAutoReportLogLevel(logLevel);
    }

    public static void RegisterCrashCallback(CrashSightCallback callback) {
        if (callback != null)
        {
            UQMCrash.CrashBaseRetEvent += callback.OnCrashBaseRetEvent;
            UQMCrash.ConfigCallBack();
        }
        else {
            DebugLog(null, "RegisterCallback failed: callback is null.");
        }
    }

    public static void UnregisterCrashCallback() {
        UQMCrash.UnregisterCallBack();
    }

    public static void RegisterCrashLogCallback(CrashSightLogCallback callback)
    {
        if (callback != null)
        {
            UQMCrash.CrashSetLogPathRetEvent += callback.OnSetLogPathEvent;
            UQMCrash.CrashLogUploadRetEvent += callback.OnLogUploadResultEvent;
            UQMCrash.ConfigLogCallBack();
        }
        else
        {
            DebugLog(null, "RegisterCallback failed: callback is null.");
        }
    }

    /// <summary>
    /// Init sdk with the specified appId. 
    /// <para>This will initialize sdk to report native exception such as obj-c, c/c++, java exceptions, and also enable c# exception handler to report c# exception logs</para>
    /// </summary>
    /// <param name="appId">App identifier.</param>
    public static void InitWithAppId(string appId)
    {
        if (IsInitialized)
        {
            DebugLog(null, "CrashSightAgent has already been initialized.");

            return;
        }

        if (string.IsNullOrEmpty(appId))
        {
            return;
        }

        // init the sdk with app id
        UQMCrash.InitWithAppId(appId);
        DebugLog(null, "Initialized with app id: {0} crashUploadUrl: {1}", appId, crashUploadUrl);

        // Register the LogCallbackHandler by Application.RegisterLogCallback(Application.LogCallback)
        _RegisterExceptionHandler();
    }

        /// <summary>
    /// Init sdk with the specified appId. 
    /// <para>This will initialize sdk to report native exception such as obj-c, c/c++, java exceptions, and also enable c# exception handler to report c# exception logs</para>
    /// </summary>
    /// <param name="appId">App identifier.</param>
    public static void InitContext(string userId, string version, string key)
    {
        if (IsInitialized)
        {
            DebugLog(null, "CrashSightAgent has already been initialized.");

            return;
        }

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(version) || string.IsNullOrEmpty(key))
        {
            return;
        }

        _isInitialized = true;
        // init the sdk with app id
        UQMCrash.InitContext(userId,  version,  key);
        DebugLog(null, "Initialized with userId: {0} version: {1} key: {2}", userId, version, key);

        // Register the LogCallbackHandler by Application.RegisterLogCallback(Application.LogCallback)
        _RegisterExceptionHandler();
    }

    /// <summary>
    /// Only Enable the C# exception handler. 
    /// 
    /// <para>
    /// You can call it when you do not call the 'InitWithAppId(string)', but you must make sure initialized the sdk in elsewhere, 
    /// such as the native code in associated Android or iOS project.
    /// </para>
    /// 
    /// <para>
    /// Default Level is <c>LogError</c>, so the LogError, LogException will auto report.
    /// </para>
    /// 
    /// <para>
    /// You can call the method <code>CrashSightAgent.ConfigAutoReportLogLevel(CSLogSeverity)</code>
    /// to change the level to auto report if you known what are you doing.
    /// </para>
    /// 
    /// </summary>
    public static void EnableExceptionHandler()
    {
        if (IsInitialized)
        {
            DebugLog(null, "CrashSightAgent has already been initialized.");
            return;
        }

        DebugLog(null, "Only enable the exception handler, please make sure you has initialized the sdk in the native code in associated Android or iOS project.");

        // Register the LogCallbackHandler by Application.RegisterLogCallback(Application.LogCallback)
        _RegisterExceptionHandler();
    }

    /// <summary>
    /// Registers the log callback handler. 
    /// 
    /// If you need register logcallback using Application.RegisterLogCallback(LogCallback),
    /// you can call this method to replace it.
    /// 
    /// <para></para>
    /// </summary>
    /// <param name="handler">Handler.</param>
    public static void RegisterLogCallback(LogCallbackDelegate handler)
    {
        if (handler != null)
        {
            DebugLog(null, "Add log callback handler: {0}", handler);

            _LogCallbackEventHandler += handler;
        }
    }

    /// <summary>
    /// Sets the log callback extras handler.
    /// </summary>
    /// <param name="handler">Handler.</param>
    public static void SetLogCallbackExtrasHandler(Func<Dictionary<string, string>> handler)
    {
        if (handler != null)
        {
            _LogCallbackExtrasHandler = handler;

            DebugLog(null, "Add log callback extra data handler : {0}", handler);
        }
    }

    /// <summary>
    /// Reports the exception.
    /// </summary>
    /// <param name="e">E.</param>
    /// <param name="message">Message.</param>
    public static void ReportException(System.Exception e, string message)
    {
        if (!IsInitialized)
        {
            return;
        }

        DebugLog(null, "Report exception: {0}\n------------\n{1}\n------------", message, e);

        _HandleException(e, message, false);
    }

    /// <summary>
    /// Reports the exception.
    /// </summary>
    /// <param name="name">Name.</param>
    /// <param name="message">Message.</param>
    /// <param name="stackTrace">Stack trace.</param>
    public static void ReportException(string name, string message, string stackTrace)
    {
        if (!IsInitialized)
        {
            return;
        }

        DebugLog(null, "Report exception: {0} {1} \n{2}", name, message, stackTrace);

        _HandleException(CSLogSeverity.LogException, name, message, stackTrace, false);
    }

    /// <summary>
    /// Reports the exception.
    /// </summary>
    /// <param name="exceptionName">exceptionName.</param>
    /// <param name="exceptionMsg">exceptionMsg.</param>
    /// <param name="exceptionStack">exceptionStack.</param>
    /// <param name="extInfo">extInfo.</param>
    public static void ReportException(int type, string exceptionName, string exceptionMsg, string exceptionStack, Dictionary<string, string> extInfo)
    {
        if (!IsInitialized)
        {
            return;
        }
        UQMCrash.ReportException(type, exceptionName, exceptionMsg, exceptionStack, extInfo);
    }

    /// <summary>
    /// Reports the exception.
    /// </summary>
    /// <param name="exceptionName">exceptionName.</param>
    /// <param name="exceptionMsg">exceptionMsg.</param>
    /// <param name="exceptionStack">exceptionStack.</param>
    /// <param name="extInfo">extInfo.</param>
    /// <param name="dumpNativeType">dumpNativeType.</param>
    public static void ReportException(int type, string exceptionName, string exceptionMsg, string exceptionStack, Dictionary<string, string> extInfo, int dumpNativeType)
    {
        if (!IsInitialized)
        {
            return;
        }
        UQMCrash.ReportException(type, exceptionName, exceptionMsg, exceptionStack, extInfo, dumpNativeType);
    }

    /// <summary>
    /// Unregisters the log callback.
    /// </summary>
    /// <param name="handler">Handler.</param>
    public static void UnregisterLogCallback(LogCallbackDelegate handler)
    {
        if (handler != null)
        {
            DebugLog(null, "Remove log callback handler");

            _LogCallbackEventHandler -= handler;
        }
    }

    /// <summary>
    /// Sets the user identifier.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    public static void SetUserId(string userId)
    {
        if (!IsInitialized)
        {
            return;
        }
        DebugLog(null, "Set user id: {0}", userId);

        UQMCrash.SetUserId(userId);
    }

    /// <summary>
    /// Sets the scene.
    /// </summary>
    /// <param name="sceneId">Scene identifier.</param>
    public static void SetScene(int sceneId)
    {
        if (!IsInitialized)
        {
            return;
        }
        DebugLog(null, "Set scene: {0}", sceneId);

        UQMCrash.SetScene(sceneId);
    }

    /// <summary>
    /// Adds the scene data.
    /// </summary>
    /// <param name="key">Key.</param>
    /// <param name="value">Value.</param>
    public static void AddSceneData(string key, string value)
    {
        if (!IsInitialized)
        {
            return;
        }

        DebugLog(null, "Add scene data: [{0}, {1}]", key, value);

        UQMCrash.AddSceneData(key, value);
    }

    /// <summary>
    /// Configs the debug mode.
    /// </summary>
    /// <param name="enable">If set to <c>true</c> debug mode.</param>
    public static void ConfigDebugMode(bool enable)
    {
        _debugMode = enable;
        UQMCrash.ConfigDebugMode(enable);
        DebugLog(null, "{0} the log message print to console", enable ? "Enable" : "Disable");
    }

    /// <summary>
    /// Configs the auto quit application.
    /// </summary>
    /// <param name="autoQuit">If set to <c>true</c> auto quit.</param>
    public static void ConfigAutoQuitApplication(bool autoQuit)
    {
        _autoQuitApplicationAfterReport = autoQuit;
    }

    /// <summary>
    /// Configs the auto report log level. Default is CSLogSeverity.LogError.
    /// <example>
    /// CSLogSeverity { Log, LogDebug, LogInfo, LogWarning, LogAssert, LogError, LogException }
    /// </example>
    /// </summary>
    /// 
    /// <param name="level">Level.</param> 
    public static void ConfigAutoReportLogLevel(CSLogSeverity level)
    {
        UQMCrash.ConfigAutoReportLogLevel((int)level);
    }

    /// <summary>
    /// Configs the default.
    /// </summary>
    /// <param name="channel">Channel.</param>
    /// <param name="version">Version.</param>
    /// <param name="user">User.</param>
    /// <param name="delay">Delay.</param>
    public static void ConfigDefault(string channel, string version, string user, long delay)
    {
        DebugLog(null, "Config default channel:{0}, version:{1}, user:{2}, delay:{3}", channel, version, user, delay);
        UQMCrash.ConfigDefault(channel, version, user, delay);
    }

    /// <summary>
    /// Configs the crashServerUrl.
    /// </summary>
    /// <param name="crashServerUrl">crashServerUrl.</param>
    public static void ConfigCrashServerUrl(string crashServerUrl)
    {
        DebugLog(null, "Config crashServerUrl:{0}", crashServerUrl);
        UQMCrash.ConfigCrashServerUrl(crashServerUrl);
    }

    /// <summary>
    /// Sets app version.
    /// </summary>
    /// <param name="appVersion">App Version.</param>
    public static void SetAppVersion(string appVersion)
    {
        UQMCrash.SetAppVersion(appVersion);
    }

    /// <summary>
    /// Sets deviceId.
    /// </summary>
    /// <param name="deviceId">deviceId.</param>
    public static void SetDeviceId(string deviceId)
    {
        UQMCrash.SetDeviceId(deviceId);
    }

    /// <summary>
    /// Sets appDeviceId.
    /// </summary>
    /// <param name="deviceId">deviceId.</param>
    public static void SetCustomizedDeviceID(string deviceId)
    {
        UQMCrash.SetCustomizedDeviceID(deviceId);
    }

    /// <summary>
    /// Get backendDeviceId.
    /// </summary>
    public static string GetSDKDefinedDeviceID()
    {
        return UQMCrash.GetSDKDefinedDeviceID();
    }

    /// <summary>
    /// Get sdk session id.
    /// </summary>
    public static string GetSDKSessionID()
    {
        return UQMCrash.GetSDKSessionID();
    }

    /// <summary>
    /// set customized match id.
    /// </summary>
    public static void SetCustomizedMatchID(string matchId)
    {
        UQMCrash.SetCustomizedMatchID(matchId);
    }
    
    /// Get CrashUuid.
    /// </summary>
    public static string GetCrashUuid()
    {
        return UQMCrash.GetCrashUuid();
    }

    /// <summary>
    /// Sets deviceModel.
    /// </summary>
    /// <param name="deviceModel">deviceModel.</param>
    public static void SetDeviceModel(string deviceModel)
    {
        UQMCrash.SetDeviceModel(deviceModel);
    }

    /// <summary>
    /// Sets logPath.
    /// </summary>
    /// <param name="logPath">logPath.</param>
    public static void SetLogPath(string logPath)
    {
        UQMCrash.SetLogPath(logPath);
    }

    /// <summary>
    /// Logs the debug.
    /// </summary>
    /// <param name="tag">Tag.</param>
    /// <param name="format">Format.</param>
    /// <param name="args">Arguments.</param>
    public static void DebugLog(string tag, string format, params object[] args)
    {
        if (!_debugMode)
        {
            return;
        }

        if (string.IsNullOrEmpty(format))
        {
            return;
        }

        UQMLog.Log(string.Format("{0}:{1}", tag, string.Format(format, args)));
    }

    /// <summary>
    /// Prints the log.
    /// </summary>
    /// <param name="level">Level.</param>
    /// <param name="format">Format.</param>
    /// <param name="args">Arguments.</param>
    public static void PrintLog(CSLogSeverity level, string format, params object[] args)
    {
        if (string.IsNullOrEmpty(format))
        {
            return;
        }
        UQMCrash.LogRecord((int)level, string.Format(format, args));
    }

    public static void ReRegistAllMonitors()
    {
                _isInitialized = true;
        UQMCrash.ReRegistAllMonitors();
        DebugLog(null, "ReRegistAllMonitors");
    }

    public static void ReportLogInfo(string msgType, string msg) {
        UQMCrash.ReportLogInfo(msgType, msg);
    }

    public static void SetCustomLogDir(string path)
    {
        UQMCrash.SetCustomLogDir(path);
    }

    #region Privated Fields and Methods 
    private static event LogCallbackDelegate _LogCallbackEventHandler;

    private static bool _isInitialized = false;
    private static CSLogSeverity _autoReportLogLevel = CSLogSeverity.LogError;

#pragma warning disable 414
    private static bool _debugMode = false;
    private static bool _autoQuitApplicationAfterReport = false;

    private static readonly int EXCEPTION_TYPE_UNCAUGHT = 1;
    private static readonly int EXCEPTION_TYPE_CAUGHT = 2;
    private static readonly string _pluginVersion = "1.5.1";

    private static Func<Dictionary<string, string>> _LogCallbackExtrasHandler;

    public static string PluginVersion
    {
        get { return _pluginVersion; }
    }

    public static bool IsInitialized
    {
        get { return _isInitialized; }
    }

    public static bool AutoQuitApplicationAfterReport
    {
        get { return _autoQuitApplicationAfterReport; }
    }

    private static void _RegisterExceptionHandler()
    {
        try
        {
            // hold only one instance 

#if UNITY_5
            Application.logMessageReceived += _OnLogCallbackHandler;
#else
            Application.RegisterLogCallback(_OnLogCallbackHandler);
#endif
            AppDomain.CurrentDomain.UnhandledException += _OnUncaughtExceptionHandler;

            _isInitialized = true;

            DebugLog(null, "Register the log callback in Unity {0}", Application.unityVersion);
        }
        catch
        {

        }
    }

    private static void _UnregisterExceptionHandler()
    {
        try
        {
#if UNITY_5
            Application.logMessageReceived -= _OnLogCallbackHandler;
#else
            Application.RegisterLogCallback(null);
#endif
            System.AppDomain.CurrentDomain.UnhandledException -= _OnUncaughtExceptionHandler;
            DebugLog(null, "Unregister the log callback in unity {0}", Application.unityVersion);
        }
        catch
        {

        }
    }

    private static void _OnLogCallbackHandler(string condition, string stackTrace, LogType type)
    {
        if (_LogCallbackEventHandler != null)
        {
            _LogCallbackEventHandler(condition, stackTrace, type);
        }

        if (!IsInitialized)
        {
            return;
        }

        if (!string.IsNullOrEmpty(condition) && condition.Contains("[CrashSightAgent] <Log>"))
        {
            return;
        }

        if (_uncaughtAutoReportOnce)
        {
            return;
        }

        // convert the log level
        CSLogSeverity logLevel = CSLogSeverity.Log;
        switch (type)
        {
            case LogType.Exception:
                logLevel = CSLogSeverity.LogException;
                break;
            case LogType.Error:
                logLevel = CSLogSeverity.LogError;
                break;
            case LogType.Assert:
                logLevel = CSLogSeverity.LogAssert;
                break;
            case LogType.Warning:
                logLevel = CSLogSeverity.LogWarning;
                break;
            case LogType.Log:
                logLevel = CSLogSeverity.LogDebug;
                break;
            default:
                break;
        }

        if (CSLogSeverity.Log == logLevel)
        {
            return;
        }

        _HandleException(logLevel, null, condition, stackTrace, true);
    }

    private static void _OnUncaughtExceptionHandler(object sender, System.UnhandledExceptionEventArgs args)
    {
        if (args == null || args.ExceptionObject == null)
        {
            return;
        }

        try
        {
            if (args.ExceptionObject.GetType() != typeof(System.Exception))
            {
                return;
            }
        }
        catch
        {
            if (UnityEngine.Debug.isDebugBuild == true)
            {
                UnityEngine.Debug.Log("CrashSightAgent: Failed to report uncaught exception");
            }

            return;
        }

        if (!IsInitialized)
        {
            return;
        }

        if (_uncaughtAutoReportOnce)
        {
            return;
        }

        _HandleException((System.Exception)args.ExceptionObject, null, true);
    }

    private static void _HandleException(System.Exception e, string message, bool uncaught)
    {
        if (e == null)
        {
            return;
        }

        if (!IsInitialized)
        {
            return;
        }

        string name = e.GetType().Name;
        string reason = e.Message;

        if (!string.IsNullOrEmpty(message))
        {
            reason = string.Format("{0}{1}***{2}", reason, Environment.NewLine, message);
        }

        StringBuilder stackTraceBuilder = new StringBuilder("");

        StackTrace stackTrace = new StackTrace(e, true);
        int count = stackTrace.FrameCount;
        for (int i = 0; i < count; i++)
        {
            StackFrame frame = stackTrace.GetFrame(i);

            stackTraceBuilder.AppendFormat("{0}.{1}", frame.GetMethod().DeclaringType.Name, frame.GetMethod().Name);

            ParameterInfo[] parameters = frame.GetMethod().GetParameters();
            if (parameters == null || parameters.Length == 0)
            {
                stackTraceBuilder.Append(" () ");
            }
            else
            {
                stackTraceBuilder.Append(" (");

                int pcount = parameters.Length;

                ParameterInfo param = null;
                for (int p = 0; p < pcount; p++)
                {
                    param = parameters[p];
                    stackTraceBuilder.AppendFormat("{0} {1}", param.ParameterType.Name, param.Name);

                    if (p != pcount - 1)
                    {
                        stackTraceBuilder.Append(", ");
                    }
                }
                param = null;

                stackTraceBuilder.Append(") ");
            }

            string fileName = frame.GetFileName();
            if (!string.IsNullOrEmpty(fileName) && !fileName.ToLower().Equals("unknown"))
            {
                fileName = fileName.Replace("\\", "/");

                int loc = fileName.ToLower().IndexOf("/assets/");
                if (loc < 0)
                {
                    loc = fileName.ToLower().IndexOf("assets/");
                }

                if (loc > 0)
                {
                    fileName = fileName.Substring(loc);
                }

                stackTraceBuilder.AppendFormat("(at {0}:{1})", fileName, frame.GetFileLineNumber());
            }
            stackTraceBuilder.AppendLine();
        }

        // report
        _reportException(uncaught, name, reason, stackTraceBuilder.ToString());
    }

    private static void _reportException(bool uncaught, string name, string reason, string stackTrace)
    {
        if (string.IsNullOrEmpty(name))
        {
            return;
        }

        if (string.IsNullOrEmpty(stackTrace))
        {
            stackTrace = StackTraceUtility.ExtractStackTrace();
        }

        if (string.IsNullOrEmpty(stackTrace))
        {
            stackTrace = "Empty";
        }
        else
        {

            try
            {
                string[] frames = stackTrace.Split('\n');

                if (frames != null && frames.Length > 0)
                {

                    StringBuilder trimFrameBuilder = new StringBuilder();

                    string frame = null;
                    int count = frames.Length;
                    for (int i = 0; i < count; i++)
                    {
                        frame = frames[i];

                        if (string.IsNullOrEmpty(frame) || string.IsNullOrEmpty(frame.Trim()))
                        {
                            continue;
                        }

                        frame = frame.Trim();

                        // System.Collections.Generic
                        if (frame.StartsWith("System.Collections.Generic.") || frame.StartsWith("ShimEnumerator"))
                        {
                            continue;
                        }
                        if (frame.StartsWith("CrashSight"))
                        {
                            continue;
                        }
                        if (frame.Contains("..ctor"))
                        {
                            continue;
                        }

                        int start = frame.ToLower().IndexOf("(at");
                        int end = frame.ToLower().IndexOf("/assets/");

                        if (start > 0 && end > 0)
                        {
                            trimFrameBuilder.AppendFormat("{0}(at {1}", frame.Substring(0, start).Replace(":", "."), frame.Substring(end));
                        }
                        else
                        {
                            trimFrameBuilder.Append(frame.Replace(":", "."));
                        }

                        trimFrameBuilder.AppendLine();
                    }

                    stackTrace = trimFrameBuilder.ToString();
                }
            }
            catch
            {
                PrintLog(CSLogSeverity.LogWarning, "{0}", "Error to parse the stack trace");
            }

        }

        PrintLog(CSLogSeverity.LogError, "ReportException: {0} {1}\n*********\n{2}\n*********", name, reason, stackTrace);

        _uncaughtAutoReportOnce = uncaught && _autoQuitApplicationAfterReport;

        string extraInfo = "";
        Dictionary<string, string> extras = null;
        if (_LogCallbackExtrasHandler != null)
        {
            extras = _LogCallbackExtrasHandler();
        }
        if (extras == null || extras.Count == 0)
        {
            extras = new Dictionary<string, string>();
            extras.Add("UnityVersion", Application.unityVersion);
        }

        if (extras != null && extras.Count > 0)
        {
            if (!extras.ContainsKey("UnityVersion"))
            {
                extras.Add("UnityVersion", Application.unityVersion);
            }

            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<string, string> kvp in extras)
            {
                builder.Append(string.Format("\"{0}\" : \"{1}\"", kvp.Key, kvp.Value)).Append(" , ");
            }
            extraInfo = string.Format("{{ {0} }}", builder.ToString().TrimEnd(" , ".ToCharArray()));
        }
        UQMCrash.ReportException(4, name, reason, stackTrace, extraInfo, uncaught && _autoQuitApplicationAfterReport);
    }

    private static void _HandleException(CSLogSeverity logLevel, string name, string message, string stackTrace, bool uncaught)
    {
        if (!IsInitialized)
        {
            DebugLog(null, "It has not been initialized.");
            return;
        }

        if (logLevel == CSLogSeverity.Log)
        {
            return;
        }

        if ((uncaught && logLevel < _autoReportLogLevel))
        {
            DebugLog(null, "Not report exception for level {0}", logLevel.ToString());
            return;
        }

        string type = null;
        string reason = "";

        if (!string.IsNullOrEmpty(message))
        {
            try
            {
                if ((CSLogSeverity.LogException == logLevel) && message.Contains("Exception"))
                {

                    Match match = new Regex(@"^(?<errorType>\S+):\s*(?<errorMessage>.*)", RegexOptions.Singleline).Match(message);

                    if (match.Success)
                    {
                        type = match.Groups["errorType"].Value.Trim();
                        reason = match.Groups["errorMessage"].Value.Trim();
                    }
                }
                else if ((CSLogSeverity.LogError == logLevel) && message.StartsWith("Unhandled Exception:"))
                {

                    Match match = new Regex(@"^Unhandled\s+Exception:\s*(?<exceptionName>\S+):\s*(?<exceptionDetail>.*)", RegexOptions.Singleline).Match(message);

                    if (match.Success)
                    {
                        string exceptionName = match.Groups["exceptionName"].Value.Trim();
                        string exceptionDetail = match.Groups["exceptionDetail"].Value.Trim();

                        // 
                        int dotLocation = exceptionName.LastIndexOf(".");
                        if (dotLocation > 0 && dotLocation != exceptionName.Length)
                        {
                            type = exceptionName.Substring(dotLocation + 1);
                        }
                        else
                        {
                            type = exceptionName;
                        }

                        int stackLocation = exceptionDetail.IndexOf(" at ");
                        if (stackLocation > 0)
                        {
                            // 
                            reason = exceptionDetail.Substring(0, stackLocation);
                            // substring after " at "
                            string callStacks = exceptionDetail.Substring(stackLocation + 3).Replace(" at ", "\n").Replace("in <filename unknown>:0", "").Replace("[0x00000]", "");
                            //
                            stackTrace = string.Format("{0}\n{1}", stackTrace, callStacks.Trim());

                        }
                        else
                        {
                            reason = exceptionDetail;
                        }

                        // for LuaScriptException
                        if (type.Equals("LuaScriptException") && exceptionDetail.Contains(".lua") && exceptionDetail.Contains("stack traceback:"))
                        {
                            stackLocation = exceptionDetail.IndexOf("stack traceback:");
                            if (stackLocation > 0)
                            {
                                reason = exceptionDetail.Substring(0, stackLocation);
                                // substring after "stack traceback:"
                                string callStacks = exceptionDetail.Substring(stackLocation + 16).Replace(" [", " \n[");

                                //
                                stackTrace = string.Format("{0}\n{1}", stackTrace, callStacks.Trim());
                            }
                        }
                    }

                }
            }
            catch
            {

            }

            if (string.IsNullOrEmpty(reason))
            {
                reason = message;
            }
        }

        if (string.IsNullOrEmpty(name))
        {
            if (string.IsNullOrEmpty(type))
            {
                type = string.Format("Unity{0}", logLevel.ToString());
            }
        }
        else
        {
            type = name;
        }

        _reportException(uncaught, type, reason, stackTrace);
    }

    private static bool _uncaughtAutoReportOnce = false;


    // test cases
    public static void TestOomCrash()
    {
        if (!IsInitialized)
        {
            return;
        }
        DebugLog(null, "test oom crash");

        UQMCrash.TestOomCrash();
    }

    public static void TestJavaCrash()
    {
        if (!IsInitialized)
        {
            return;
        }
        DebugLog(null, "test java crash");

        UQMCrash.TestJavaCrash();
    }

    public static void TestOcCrash()
    {
        if (!IsInitialized)
        {
            return;
        }
        DebugLog(null, "test oc crash");

        UQMCrash.TestOcCrash();
    }

    public static void TestNativeCrash()
    {
        if (!IsInitialized)
        {
            return;
        }
        DebugLog(null, "test native crash");

        UQMCrash.TestNativeCrash();
    }

    public static long GetCrashThreadId()
    {
        if (!IsInitialized)
        {
            return -1;
        }
        DebugLog(null, "GetCrashThreadId");

        return UQMCrash.GetCrashThreadId();
    }
    #endregion

}

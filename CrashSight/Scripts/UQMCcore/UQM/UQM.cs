using UnityEngine;

namespace GCloud.UQM
{
    #region UQM
	/// <summary>
	/// 回调的范型
	/// </summary>
	public delegate void OnUQMRetEventHandler<T> (T ret);
	public delegate string OnUQMStringRetEventHandler<T> (T ret, T crashType);

    public delegate string OnUQMStringRetSetLogPathEventHandler<T>(T ret, T crashType);
    public delegate void OnUQMRetLogUploadEventHandler<T>(T ret, T crashType, T result);
    

    public class UQM
    {
#if GCLOUD_UQM_WINDOWS
        public const string LibName = "CrashSight";
#elif UNITY_ANDROID
		public const string LibName = "CrashSight";
#else
        public const string LibName = "__Internal";
#endif
        private static bool initialized;

        public static bool isDebug = true;

		/// <summary>
		/// UQM init，游戏开始的时候设置
		/// </summary>
        public static void Init()
		{
            if (initialized) return;
            initialized = true;
            if (isDebug)
                UQMLog.SetLevel(UQMLog.Level.Log);
            else
                UQMLog.SetLevel(UQMLog.Level.Error);
			UQMLog.Log ("UQM initialed !");
        }
    }

    #endregion
}
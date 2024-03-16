// ----------------------------------------
//
//  CrashSightCallbackDelegate.cs
//
//  Author:
//       Yeelik,
//
//  Copyright (c) 2015 CrashSight.  All rights reserved.
//
// ----------------------------------------
//

public abstract class CrashSightCallback
{
	public abstract string OnCrashBaseRetEvent(int methodId, int crashType);

}

public abstract class CrashSightLogCallback
{
	public abstract string OnSetLogPathEvent(int methodId, int crashType);

	public abstract void OnLogUploadResultEvent(int methodId, int crashType, int result);

}


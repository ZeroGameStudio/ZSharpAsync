﻿// Copyright Zero Games. All Rights Reserved.

using System.Runtime.InteropServices;

namespace ZeroGames.ZSharp.Async;

internal static class EventLoop_Interop
{

	[UnmanagedCallersOnly]
	public static void NotifyEvent(EEventLoopTickingGroup group, float worldDeltaSeconds, float realDeltaSeconds, double worldElapsedSeconds, double realElapsedSeconds) => Uncaught.FatalIfUncaught(() =>
	{
		EventLoop.Get().NotifyEvent(group, worldDeltaSeconds, realDeltaSeconds, worldElapsedSeconds, realElapsedSeconds);
	});

}



// Copyright Zero Games. All Rights Reserved.

using System.Runtime.InteropServices;

namespace ZeroGames.ZSharp.Async;

internal static class EventLoop_Interop
{

	[UnmanagedCallersOnly]
	public static void NotifyEvent(EEventLoopTickingGroup group, float worldDeltaTime, float realDeltaTime, double worldElapsedTime, double realElapsedTime) => Uncaught.FatalIfUncaught(() =>
	{
		EventLoop.Get().NotifyEvent(group, worldDeltaTime, realDeltaTime, worldElapsedTime, realElapsedTime);
	});

}



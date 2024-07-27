// Copyright Zero Games. All Rights Reserved.

namespace ZeroGames.ZSharp.Async;

public static class ThreadHelper
{

	public static void ValidateGameThread(string? message = null)
	{
		if (!IsInGameThread)
		{
			throw new InvalidOperationException(message);
		}
	}

	public static bool IsInGameThread => UnrealEngineStatics.IsInGameThread;

}



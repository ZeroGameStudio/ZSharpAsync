// Copyright Zero Games. All Rights Reserved.

using System.Runtime.CompilerServices;

namespace ZeroGames.ZSharp.Async.Task;

// These exist only for ensuring that the implement class matches specific pattern.
internal interface IAwaiter : ICriticalNotifyCompletion
{
	bool IsCompleted { get; }
}

internal interface IAwaiterVoid : IAwaiter
{
	void GetResult();
}

internal interface IAwaiter<out TResult> : IAwaiter
{
	TResult GetResult();
}



﻿// Copyright Zero Games. All Rights Reserved.

namespace ZeroGames.ZSharp.Async.ZeroTask;

// These exist only for ensuring that the implement class matches specific pattern.
internal interface IAwaiter : IZeroTaskAwaiter
{
	bool IsCompleted { get; }
	void GetResult();
}

internal interface IAwaiter<out TResult> : IAwaiter
{
	new TResult GetResult();
}



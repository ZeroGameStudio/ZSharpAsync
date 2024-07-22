// Copyright Zero Games. All Rights Reserved.

namespace ZeroGames.ZSharp.Async.Task;

/// <summary>
/// Background heap-allocated object for task which completes asynchronously.
/// Similar to IValueTaskSource.
/// </summary>
public interface IUnderlyingTask
{
	EUnderlyingTaskStatus GetStatus(uint64 token);
	void SetContinuation(Action continuation, uint64 token);
	uint64 Token { get; }
}

public interface IUnderlyingTaskVoid : IUnderlyingTask
{
	void GetResult(uint64 token);
}

public interface IUnderlyingTask<out TResult> : IUnderlyingTask
{
	TResult GetResult(uint64 token);
}



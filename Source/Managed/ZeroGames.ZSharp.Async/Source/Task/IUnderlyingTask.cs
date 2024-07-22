// Copyright Zero Games. All Rights Reserved.

namespace ZeroGames.ZSharp.Async.Task;

public interface IUnderlyingTask
{
	EUnderlyingTaskStatus GetStatus(uint64 token);
	void OnCompleted(Action continuation, uint64 token);
}

public interface IUnderlyingTaskVoid : IUnderlyingTask
{
	void GetResult(uint64 token);
}

public interface IUnderlyingTask<out TResult> : IUnderlyingTask
{
	TResult GetResult(uint64 token);
}



// Copyright Zero Games. All Rights Reserved.

using System.Runtime.CompilerServices;

namespace ZeroGames.ZSharp.Async.ZeroTask;

/// <summary>
/// Background heap-allocated object for task which completes asynchronously.
/// Similar to IValueTaskSource.
/// </summary>
public interface IUnderlyingZeroTask
{
	EUnderlyingZeroTaskStatus GetStatus(uint64 token);
	void SetContinuation(Action continuation, uint64 token);
	void SetStateMachine(IAsyncStateMachine stateMachine, uint64 token);
	uint64 Token { get; }
}

public interface IUnderlyingZeroTaskVoid : IUnderlyingZeroTask
{
	void GetResult(uint64 token);
}

public interface IUnderlyingZeroTask<out TResult> : IUnderlyingZeroTask
{
	TResult GetResult(uint64 token);
}



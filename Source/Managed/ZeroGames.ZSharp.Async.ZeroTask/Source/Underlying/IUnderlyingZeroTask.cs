// Copyright Zero Games. All Rights Reserved.

using System.Runtime.CompilerServices;

namespace ZeroGames.ZSharp.Async.ZeroTask;

/// <summary>
/// Background heap-allocated object for task which completes asynchronously.
/// Similar to IValueTaskSource.
/// </summary>
public interface IUnderlyingZeroTask
{
	EUnderlyingZeroTaskStatus GetStatus(UnderlyingZeroTaskToken token);
	void SetContinuation(Action continuation, UnderlyingZeroTaskToken token);
	void SetStateMachine(IAsyncStateMachine stateMachine, UnderlyingZeroTaskToken token);
	UnderlyingZeroTaskToken Token { get; }
}

public interface IUnderlyingZeroTaskVoid : IUnderlyingZeroTask
{
	void GetResult(UnderlyingZeroTaskToken token);
}

public interface IUnderlyingZeroTask<out TResult> : IUnderlyingZeroTask
{
	TResult GetResult(UnderlyingZeroTaskToken token);
}



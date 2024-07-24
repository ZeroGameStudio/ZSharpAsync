// Copyright Zero Games. All Rights Reserved.

using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace ZeroGames.ZSharp.Async.ZeroTask;

[AsyncMethodBuilder(typeof(AsyncZeroTaskMethodBuilderVoid))]
public readonly partial struct ZeroTask : IZeroTask, IAwaitableVoid<ZeroTask.Awaiter>, IEquatable<ZeroTask>
{
	
	public readonly struct Awaiter(ZeroTask _task) : IAwaiterVoid
	{
		public void OnCompleted(IAsyncStateMachine stateMachine) => _task.SetStateMachine(stateMachine);
		public void OnCompleted(Action continuation) => _task.SetContinuation(continuation);
		public void GetResult() => _task.GetResult();
		public bool IsCompleted => _task.IsCompleted;
	}

	public Awaiter GetAwaiter() => new(this);

	public bool Equals(ZeroTask other) => _underlyingTask == other._underlyingTask && _token == other._token && _error == other._error;
	public override bool Equals(object? obj) => obj is ZeroTask other && Equals(other);
	public override int32 GetHashCode() => _underlyingTask?.GetHashCode() ?? _error?.GetHashCode() ?? 0;
	public static bool operator ==(ZeroTask lhs, ZeroTask rhs) => lhs.Equals(rhs);
	public static bool operator !=(ZeroTask lhs, ZeroTask rhs) => !lhs.Equals(rhs);

	public bool IsCompleted => _underlyingTask is null || _underlyingTask.GetStatus(_token) != EUnderlyingZeroTaskStatus.Pending;

	private ZeroTask(Exception exception) => _error = ExceptionDispatchInfo.Capture(exception);

	private ZeroTask(IUnderlyingZeroTaskVoid? task, uint64 token)
	{
		_underlyingTask = task;
		_token = token;
	}

	private void GetResult()
	{
		if (_underlyingTask is not null)
		{
			_underlyingTask.GetResult(_token);
		}
	}
	
	private void SetStateMachine(IAsyncStateMachine stateMachine)
	{
		if (_underlyingTask is null)
		{
			stateMachine.MoveNext();
		}
		else
		{
			_underlyingTask.SetStateMachine(stateMachine, _token);
		}
	}

	private void SetContinuation(Action continuation)
	{
		if (_underlyingTask is null)
		{
			continuation();
		}
		else
		{
			_underlyingTask.SetContinuation(continuation, _token);
		}
	}
	
	private readonly IUnderlyingZeroTaskVoid? _underlyingTask;
	private readonly uint64 _token;
	private readonly ExceptionDispatchInfo? _error;

}



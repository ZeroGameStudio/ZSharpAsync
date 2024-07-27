// Copyright Zero Games. All Rights Reserved.

using System.Runtime.CompilerServices;

namespace ZeroGames.ZSharp.Async.ZeroTask;

[AsyncMethodBuilder(typeof(AsyncZeroTaskMethodBuilderVoid))]
public readonly partial struct ZeroTask(IUnderlyingZeroTaskVoid underlyingTask) : IZeroTask, IAwaitableVoid<ZeroTask.Awaiter>, IEquatable<ZeroTask>
{
	
	public readonly struct Awaiter(ZeroTask _task) : IAwaiterVoid
	{
		public void OnCompleted(IAsyncStateMachine stateMachine)
		{
			ThreadHelper.ValidateGameThread();
			_task.SetStateMachine(stateMachine);
		}

		public void OnCompleted(Action continuation)
		{
			ThreadHelper.ValidateGameThread();
			_task.SetContinuation(continuation);
		}

		public void GetResult()
		{
			ThreadHelper.ValidateGameThread();
			_task.GetResult();
		}

		public bool IsCompleted
		{
			get
			{
				ThreadHelper.ValidateGameThread();
				return _task.IsCompleted;
			}
		}
	}

	public Awaiter GetAwaiter() => new(this);

	public bool Equals(ZeroTask other) => _underlyingTask == other._underlyingTask && _capturedToken == other._capturedToken;
	public override bool Equals(object? obj) => obj is ZeroTask other && Equals(other);
	public override int32 GetHashCode() => _underlyingTask?.GetHashCode() ?? 0;
	public static bool operator ==(ZeroTask lhs, ZeroTask rhs) => lhs.Equals(rhs);
	public static bool operator !=(ZeroTask lhs, ZeroTask rhs) => !lhs.Equals(rhs);

	private bool IsCompleted => _underlyingTask is null || _underlyingTask.GetStatus(_capturedToken) != EUnderlyingZeroTaskStatus.Pending;

	private void GetResult()
	{
		if (_underlyingTask is not null)
		{
			_underlyingTask.GetResult(_capturedToken);
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
			_underlyingTask.SetStateMachine(stateMachine, _capturedToken);
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
			_underlyingTask.SetContinuation(continuation, _capturedToken);
		}
	}
	
	private readonly IUnderlyingZeroTaskVoid? _underlyingTask = underlyingTask;
	private readonly UnderlyingZeroTaskToken _capturedToken = underlyingTask.Token;

}



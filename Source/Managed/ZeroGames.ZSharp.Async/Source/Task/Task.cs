// Copyright Zero Games. All Rights Reserved.

using System.Runtime.CompilerServices;

namespace ZeroGames.ZSharp.Async.Task;

[AsyncMethodBuilder(typeof(AsyncTaskMethodBuilder))]
public readonly struct Task : ITask, IAwaitableVoid<Task.Awaiter>, IEquatable<Task>
{

	public static Task FromUnderlyingTask(IUnderlyingTaskVoid? task, uint64 token) => new(task, token);
	public static Task FromResult() => default;
	public static Task FromException(Exception exception) => new(exception);

	public Awaiter GetAwaiter() => new(this);

	public bool Equals(Task other) => _underlyingTask == other._underlyingTask && _token == other._token && _exception == other._exception;
	public override bool Equals(object? obj) => obj is Task other && Equals(other);
	public override int32 GetHashCode() => _underlyingTask?.GetHashCode() ?? _exception?.GetHashCode() ?? 0;
	public static bool operator ==(Task lhs, Task rhs) => lhs.Equals(rhs);
	public static bool operator !=(Task lhs, Task rhs) => !lhs.Equals(rhs);

	public bool IsCompleted => _underlyingTask is null || _underlyingTask.GetStatus(_token) != EUnderlyingTaskStatus.Pending;

	private Task(Exception exception) => _exception = exception;

	private Task(IUnderlyingTaskVoid? task, uint64 token)
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

	private void OnCompleted(Action continuation)
	{
		if (_underlyingTask is null)
		{
			continuation();
		}
		else
		{
			_underlyingTask.OnCompleted(continuation, _token);
		}
	}
	
	private readonly IUnderlyingTaskVoid? _underlyingTask;
	private readonly uint64 _token;
	private readonly Exception? _exception;
	
	public readonly struct Awaiter(Task _task) : IAwaiterVoid
	{
		public void OnCompleted(Action continuation) => _task.OnCompleted(continuation);
		public void UnsafeOnCompleted(Action continuation) => _task.OnCompleted(continuation);
		public void GetResult() => _task.GetResult();
		public bool IsCompleted => _task.IsCompleted;
	}

}



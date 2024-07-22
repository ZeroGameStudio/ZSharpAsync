// Copyright Zero Games. All Rights Reserved.

using System.Runtime.CompilerServices;

namespace ZeroGames.ZSharp.Async.ZeroTask;

public struct AsyncZeroTaskMethodBuilderVoid : IAsyncMethodBuilderVoid<AsyncZeroTaskMethodBuilderVoid, ZeroTask, ZeroTask.Awaiter>
{

	public static AsyncZeroTaskMethodBuilderVoid Create() => default;

	public void SetStateMachine(IAsyncStateMachine stateMachine) => throw new NotSupportedException();
	
	public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine => stateMachine.MoveNext();

	public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
	{
		if (_task is null)
		{
			_task = UnderlyingZeroTask_AsyncStateMachine.GetFromPool();
		}

		TStateMachine copy = stateMachine;
		awaiter.OnCompleted(() => { copy.MoveNext(); });
	}

	public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
	{
		if (_task is null)
		{
			_task = UnderlyingZeroTask_AsyncStateMachine.GetFromPool();
		}
		
		TStateMachine copy = stateMachine;
		awaiter.UnsafeOnCompleted(() => { copy.MoveNext(); });
	}

	public void SetResult()
	{
		if (_task is not null)
		{
			_task.SetResult();
		}
	}

	public void SetException(Exception exception)
	{
		if (_task is not null)
		{
			_task.SetException(exception);
		}
		else
		{
			_exception = exception;
		}
	}

	public ZeroTask Task
	{
		get
		{
			if (_task is not null)
			{
				return _task.Task;
			}
			else if (_exception is not null)
			{
				return ZeroTask.FromException(_exception);
			}
			else
			{
				return ZeroTask.FromResult();
			}
		}
	}

	private UnderlyingZeroTask_AsyncStateMachine? _task;
	private Exception? _exception;

}



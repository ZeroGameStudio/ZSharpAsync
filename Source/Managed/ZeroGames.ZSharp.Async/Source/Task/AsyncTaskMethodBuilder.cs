// Copyright Zero Games. All Rights Reserved.

using System.Runtime.CompilerServices;

namespace ZeroGames.ZSharp.Async.Task;

public struct AsyncTaskMethodBuilder : IAsyncMethodBuilderVoid<AsyncTaskMethodBuilder, Task, Task.Awaiter>
{

	public static AsyncTaskMethodBuilder Create() => default;

	public void SetStateMachine(IAsyncStateMachine stateMachine) => throw new NotSupportedException();
	
	public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine => stateMachine.MoveNext();

	public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
	{
		if (_task is null)
		{
			_task = StateMachineTask.GetFromPool();
		}

		TStateMachine copy = stateMachine;
		awaiter.OnCompleted(() => { copy.MoveNext(); });
	}

	public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
	{
		if (_task is null)
		{
			_task = StateMachineTask.GetFromPool();
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

	public Task Task
	{
		get
		{
			if (_task is not null)
			{
				return _task.Task;
			}
			else if (_exception is not null)
			{
				return Task.FromException(_exception);
			}
			else
			{
				return Task.FromResult();
			}
		}
	}

	private StateMachineTask? _task;
	private Exception? _exception;

}



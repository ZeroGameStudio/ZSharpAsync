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
		_task ??= ZeroTask_AsyncStateMachine.GetFromPool();
		
		// IZeroTaskAwaiter is internal and only implemented by struct ZeroTask.Awaiter.
		// The null tests here ensure that the jit can optimize away the interface tests when TAwaiter is a ref type.
		if (default(TAwaiter) is not null && awaiter is IZeroTaskAwaiter zeroAwaiter)
		{
			zeroAwaiter.OnCompleted(stateMachine);
		}
		else
		{
			// The awaiter isn't specially known. Fall back to doing a normal await.
			TStateMachine copy = stateMachine;
			awaiter.OnCompleted(() => { copy.MoveNext(); });
		}
	}

	public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
	{
		_task ??= ZeroTask_AsyncStateMachine.GetFromPool();
		
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

	private ZeroTask_AsyncStateMachine? _task;
	private Exception? _exception;

}



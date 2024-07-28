// Copyright Zero Games. All Rights Reserved.

using System.Runtime.CompilerServices;

namespace ZeroGames.ZSharp.Async.ZeroTask;

internal class ZeroTask_AsyncStateMachine<TResult> : IPoolableUnderlyingZeroTask<TResult, ZeroTask_AsyncStateMachine<TResult>>
{

	public static ZeroTask_AsyncStateMachine<TResult> GetFromPool() => _pool.Pop();

	public static ZeroTask_AsyncStateMachine<TResult> Create()
	{
		ZeroTask_AsyncStateMachine<TResult> task = new();
		task.Deinitialize();
		task.Initialize();
		return task;
	}
	
	public void Initialize() => _comp.Initialize();

	public void Deinitialize()
	{
		_comp.Deinitialize();
	}

	public EUnderlyingZeroTaskStatus GetStatus(UnderlyingZeroTaskToken token) => _comp.GetStatus(token);

	public void SetStateMachine(IAsyncStateMachine stateMachine, UnderlyingZeroTaskToken token) => _comp.SetStateMachine(stateMachine, token);

	public void SetContinuation(Action continuation, UnderlyingZeroTaskToken token) => _comp.SetContinuation(continuation, token);

	void IUnderlyingZeroTask.GetResult(UnderlyingZeroTaskToken token) => GetResult(token);
	public TResult GetResult(UnderlyingZeroTaskToken token)
	{
		TResult result = _comp.GetResult(token);
		_pool.Push(this);
		return result;
	}

	public void SetResult(TResult result) => _comp.SetResult(result);

	public void SetException(Exception exception) => _comp.SetException(exception);
	
	public UnderlyingZeroTaskToken Token => _comp.Token;
	
	public ZeroTask_AsyncStateMachine<TResult>? PoolNext { get; set; }

	public ZeroTask<TResult> Task => ZeroTask<TResult>.FromUnderlyingTask(this);

	private static UnderlyingZeroTaskPool<TResult, ZeroTask_AsyncStateMachine<TResult>> _pool;

	private UnderlyingZeroTaskComponent<TResult> _comp;

}



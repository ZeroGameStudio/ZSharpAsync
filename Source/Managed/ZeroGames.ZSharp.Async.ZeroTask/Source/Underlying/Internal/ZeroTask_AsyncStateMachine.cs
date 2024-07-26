// Copyright Zero Games. All Rights Reserved.

using System.Runtime.CompilerServices;

namespace ZeroGames.ZSharp.Async.ZeroTask;

internal class ZeroTask_AsyncStateMachine : IPoolableUnderlyingZeroTaskVoid<ZeroTask_AsyncStateMachine>
{

	public static ZeroTask_AsyncStateMachine GetFromPool() => _pool.Pop();

	public static ZeroTask_AsyncStateMachine Create()
	{
		ZeroTask_AsyncStateMachine task = new();
		task.Deinitialize();
		task.Initialize();
		return task;
	}
	
	public void Initialize() => _comp.Initialize();

	public void Deinitialize()
	{
		_comp.Deinitialize();
		Task = ZeroTask.FromUnderlyingTask(this);
	}

	public EUnderlyingZeroTaskStatus GetStatus(UnderlyingZeroTaskToken token) => _comp.GetStatus(token);
	
	public void SetStateMachine(IAsyncStateMachine stateMachine, UnderlyingZeroTaskToken token) => _comp.SetStateMachine(stateMachine, token);

	public void SetContinuation(Action continuation, UnderlyingZeroTaskToken token) => _comp.SetContinuation(continuation, token);

	public void GetResult(UnderlyingZeroTaskToken token)
	{
		_comp.GetResult(token);
		_pool.Push(this);
	}

	public void SetResult() => _comp.SetResult();

	public void SetException(Exception exception) => _comp.SetException(exception);
	
	public UnderlyingZeroTaskToken Token => _comp.Token;
	
	public ZeroTask_AsyncStateMachine? PoolNext { get; set; }
	
	public ZeroTask Task { get; private set; }

	private static readonly UnderlyingZeroTaskPool<ZeroTask_AsyncStateMachine> _pool = new();

	private PoolableUnderlyingZeroTaskComponentVoid _comp;

}



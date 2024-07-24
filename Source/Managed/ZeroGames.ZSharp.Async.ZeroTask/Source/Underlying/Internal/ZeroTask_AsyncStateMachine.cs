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
		Task = ZeroTask.FromUnderlyingTask(this, Token);
	}

	public EUnderlyingZeroTaskStatus GetStatus(uint64 token) => _comp.GetStatus(token);
	
	public void SetStateMachine(IAsyncStateMachine stateMachine, uint64 token) => _comp.SetStateMachine(stateMachine, token);

	public void SetContinuation(Action continuation, uint64 token) => _comp.SetContinuation(continuation, token);
	
	public void GetResult(uint64 token) => _comp.GetResult(token);

	public void SetResult() => _comp.SetResult();

	public void SetException(Exception exception) => _comp.SetException(exception);
	
	public uint64 Token => _comp.Token;
	
	public ZeroTask_AsyncStateMachine? PoolNext { get; set; }
	
	public ZeroTask Task { get; private set; }

	private static readonly UnderlyingZeroTaskPool<ZeroTask_AsyncStateMachine> _pool = new();

	private PoolableUnderlyingZeroTaskComponentVoid _comp;

}



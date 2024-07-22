// Copyright Zero Games. All Rights Reserved.

namespace ZeroGames.ZSharp.Async.ZeroTask;

internal class UnderlyingZeroTask_AsyncStateMachine : IPoolableUnderlyingZeroTaskVoid<UnderlyingZeroTask_AsyncStateMachine>
{

	public static UnderlyingZeroTask_AsyncStateMachine GetFromPool() => _pool.Pop();

	public static UnderlyingZeroTask_AsyncStateMachine Create()
	{
		UnderlyingZeroTask_AsyncStateMachine task = new();
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

	public void SetContinuation(Action continuation, uint64 token) => _comp.SetContinuation(continuation, token);

	public void GetResult(uint64 token) => _comp.GetResult(token);

	public void SetResult() => _comp.SetResult();

	public void SetException(Exception exception) => _comp.SetException(exception);
	
	public uint64 Token => _comp.Token;
	
	public UnderlyingZeroTask_AsyncStateMachine? PoolNext { get; set; }
	
	public ZeroTask Task { get; private set; }

	private static readonly UnderlyingZeroTaskPool<UnderlyingZeroTask_AsyncStateMachine> _pool = new();

	private PoolableUnderlyingZeroTaskComponentVoid _comp;

}



// Copyright Zero Games. All Rights Reserved.

namespace ZeroGames.ZSharp.Async.Task;

internal class StateMachineTask : IPoolableUnderlyingTaskVoid<StateMachineTask>
{

	public static StateMachineTask GetFromPool() => _pool.Pop();

	public static StateMachineTask Create()
	{
		StateMachineTask task = new();
		task.Initialize();
		return task;
	}
	
	public void Initialize() => _comp.Initialize();

	public void Deinitialize()
	{
		_comp.Deinitialize();
		Task = Task.FromUnderlyingTask(this, Token);
	}

	public EUnderlyingTaskStatus GetStatus(uint64 token) => _comp.GetStatus(token);

	public void SetContinuation(Action continuation, uint64 token) => _comp.SetContinuation(continuation, token);

	public void GetResult(uint64 token) => _comp.GetResult(token);

	public void SetResult() => _comp.SetResult();

	public void SetException(Exception exception) => _comp.SetException(exception);
	
	public uint64 Token => _comp.Token;
	
	public StateMachineTask? PoolNext { get; set; }
	
	public Task Task { get; private set; }

	private static readonly UnderlyingTaskPool<StateMachineTask> _pool = new();

	private PoolableUnderlyingTaskComponentVoid _comp;

}



// Copyright Zero Games. All Rights Reserved.

namespace ZeroGames.ZSharp.Async.Task;

internal class StateMachineTask : IPoolableUnderlyingTaskVoid<StateMachineTask>
{

	public static StateMachineTask GetFromPool() => _pool.Pop();

	public StateMachineTask()
	{
		Task = Task.FromUnderlyingTask(this, 0);
	}
	
	public void Initialize()
	{
		throw new NotImplementedException();
	}

	public void Deinitialize()
	{
		throw new NotImplementedException();
	}

	public EUnderlyingTaskStatus GetStatus(uint64 token)
	{
		throw new NotImplementedException();
	}

	public void OnCompleted(Action continuation, uint64 token)
	{
		throw new NotImplementedException();
	}

	public void GetResult(uint64 token)
	{
		throw new NotImplementedException();
	}
	
	public void SetResult()
	{
		
	}

	public void SetException(Exception exception)
	{
		
	}
	
	public StateMachineTask? PoolNext { get; set; }
	
	public Task Task { get; }

	private static UnderlyingTaskPool<StateMachineTask> _pool;

}



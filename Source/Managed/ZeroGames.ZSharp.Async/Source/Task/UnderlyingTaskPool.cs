// Copyright Zero Games. All Rights Reserved.

namespace ZeroGames.ZSharp.Async.Task;

public struct UnderlyingTaskPool<T> where T : class, IPoolableUnderlyingTask<T>, new()
{

	public T Pop()
	{
		T task;
		if (_head is null)
		{
			task = new();
		}
		else
		{
			task = _head;
			_head = task.PoolNext;
		}

		task.Initialize();
		return task;
	}

	public void Push(T task)
	{
		task.Deinitialize();
		
		if (_head is null)
		{
			_head = task;
		}
		else
		{
			task.PoolNext = _head;
			_head = task;
		}
	}

	private T? _head;

}



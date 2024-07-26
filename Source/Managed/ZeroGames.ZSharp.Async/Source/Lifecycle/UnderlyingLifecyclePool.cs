// Copyright Zero Games. All Rights Reserved.

namespace ZeroGames.ZSharp.Async;

public class UnderlyingLifecyclePool<T> where T : class, IPoolableUnderlyingLifecycle<T>
{
	
	public T Pop()
	{
		lock (_lock)
		{
			if (_head is null)
			{
				return T.Create();
			}
			else
			{
				T task = _head;
				task.Initialize();
				_head = task.PoolNext;
				return task;
			}
		}
	}

	public void Push(T task)
	{
		lock (_lock)
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
	}

	private T? _head;
	private readonly object _lock = new();

}
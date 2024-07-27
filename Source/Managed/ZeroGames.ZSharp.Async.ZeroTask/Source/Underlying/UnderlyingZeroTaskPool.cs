// Copyright Zero Games. All Rights Reserved.

namespace ZeroGames.ZSharp.Async.ZeroTask;

public struct UnderlyingZeroTaskPool<T> where T : class, IPoolableUnderlyingZeroTask<T>
{
	
	public T Pop()
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



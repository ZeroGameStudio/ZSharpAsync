// Copyright Zero Games. All Rights Reserved.

namespace ZeroGames.ZSharp.Async.ZeroTask;

public struct UnderlyingZeroTaskPool<TResult, TImpl> where TImpl : class, IPoolableUnderlyingZeroTask<TResult, TImpl>
{
	
	public TImpl Pop()
	{
		if (_head is null)
		{
			return TImpl.Create();
		}
		else
		{
			TImpl task = _head;
			task.Initialize();
			_head = task.PoolNext;
			return task;
		}
	}

	public void Push(TImpl task)
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

	private TImpl? _head;

}



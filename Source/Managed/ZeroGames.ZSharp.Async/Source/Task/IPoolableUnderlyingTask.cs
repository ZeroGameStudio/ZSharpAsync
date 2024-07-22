// Copyright Zero Games. All Rights Reserved.

namespace ZeroGames.ZSharp.Async.Task;

public interface IPoolableUnderlyingTask<TImpl> : IUnderlyingTask where TImpl : class, IPoolableUnderlyingTask<TImpl>, new()
{
	static abstract TImpl GetFromPool();
	void Initialize();
	void Deinitialize();
	protected internal TImpl? PoolNext { get; set; }
}

public interface IPoolableUnderlyingTaskVoid<TImpl> : IUnderlyingTaskVoid, IPoolableUnderlyingTask<TImpl> where TImpl : class, IPoolableUnderlyingTaskVoid<TImpl>, new();

public interface IPoolableUnderlyingTask<out TResult, TImpl> : IUnderlyingTask<TResult>, IPoolableUnderlyingTask<TImpl> where TImpl : class, IPoolableUnderlyingTask<TResult, TImpl>, new();



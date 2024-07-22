// Copyright Zero Games. All Rights Reserved.

namespace ZeroGames.ZSharp.Async.ZeroTask;

public interface IPoolableUnderlyingZeroTask<TImpl> : IUnderlyingZeroTask where TImpl : class, IPoolableUnderlyingZeroTask<TImpl>
{
	static abstract TImpl GetFromPool();
	static abstract TImpl Create();
	void Initialize();
	void Deinitialize();
	protected internal TImpl? PoolNext { get; set; }
}

public interface IPoolableUnderlyingZeroTaskVoid<TImpl> : IUnderlyingZeroTaskVoid, IPoolableUnderlyingZeroTask<TImpl> where TImpl : class, IPoolableUnderlyingZeroTaskVoid<TImpl>;

public interface IPoolableUnderlyingZeroTask<out TResult, TImpl> : IUnderlyingZeroTask<TResult>, IPoolableUnderlyingZeroTask<TImpl> where TImpl : class, IPoolableUnderlyingZeroTask<TResult, TImpl>;



// Copyright Zero Games. All Rights Reserved.

using System.Runtime.CompilerServices;

namespace ZeroGames.ZSharp.Async;

[Obsolete]
internal class Lifecycle_ExplicitLifecycle : IPoolableReactiveUnderlyingLifecycle<Lifecycle_ExplicitLifecycle>
{

	public static Lifecycle_ExplicitLifecycle GetFromPool(IExplicitLifecycle explicitLifecycle)
	{
		static Lifecycle_ExplicitLifecycle PopAndRegister(IExplicitLifecycle explicitLifecycle, bool isGameThreadOnly)
		{
			Lifecycle_ExplicitLifecycle lifecycle = _pool.Pop();
			if (explicitLifecycle.IsExpired)
			{
				lifecycle.SetExpired();
			}
			else
			{
				explicitLifecycle.RegisterOnExpired(isGameThreadOnly ? HandleExpired_GameThread : HandleExpired_AnyThread, lifecycle);
			}
			
			return lifecycle;
		}

		if (explicitLifecycle.IsGameThreadOnly)
		{
			return PopAndRegister(explicitLifecycle, true);
		}
		else
		{
			lock (explicitLifecycle.SyncRoot)
			{
				return PopAndRegister(explicitLifecycle, false);
			}
		}
	}
	
	public static Lifecycle_ExplicitLifecycle Create()
	{
		Lifecycle_ExplicitLifecycle lifecycle = new();
		lifecycle.Deinitialize();
		lifecycle.Initialize();
		return lifecycle;
	}

	public Lifecycle_ExplicitLifecycle()
	{
		_comp = new(this);
	}

	public void Initialize() => _comp.Initialize();
	
	public void Deinitialize() => _comp.Deinitialize();

	public LifecycleExpiredRegistration RegisterOnExpired(Action<IReactiveUnderlyingLifecycle, object?> callback, object? state, UnderlyingLifecycleToken token) => _comp.RegisterOnExpired(callback, state, token);

	public void UnregisterOnExpired(LifecycleExpiredRegistration registration, UnderlyingLifecycleToken token) => _comp.UnregisterOnExpired(registration, token);

	bool IUnderlyingLifecycle.IsExpired(UnderlyingLifecycleToken token) => _comp.IsExpired(token);

	public UnderlyingLifecycleToken Token => _comp.Token;

	public Lifecycle_ExplicitLifecycle? PoolNext { get; set; }

	private static void HandleExpired_GameThread(IExplicitLifecycle _, object? @this)
	{
		// Although the target says it IsGameThreadOnly but it depends on whether it implements properly, so we do a check here.
		ThreadHelper.ValidateGameThread();
		UnsafeHandleExpired(_, @this);
	}

	private static void HandleExpired_AnyThread(IExplicitLifecycle _, object? @this)
	{
		if (ThreadHelper.IsInGameThread)
		{
			UnsafeHandleExpired(_, @this);
		}
		else
		{
			ThreadHelper.GameThreadSynchronizationContext.Send(UnsafeHandleExpired, @this);
		}
	}
	
	private static void UnsafeHandleExpired(IExplicitLifecycle _, object? @this)
	{
		Unsafe.As<Lifecycle_ExplicitLifecycle>(@this!).SetExpired();
	}
	
	private static void UnsafeHandleExpired(object? @this)
	{
		Unsafe.As<Lifecycle_ExplicitLifecycle>(@this!).SetExpired();
	}

	private void SetExpired()
	{
		_comp.SetExpired();
		_pool.Push(this);
	}
	
	private static UnderlyingLifecyclePool<Lifecycle_ExplicitLifecycle> _pool;
	
	private UnderlyingLifecycleComponent _comp;

}



// Copyright Zero Games. All Rights Reserved.

using System.Runtime.CompilerServices;

namespace ZeroGames.ZSharp.Async;

public class Lifecycle_ExplicitLifecycle : IPoolableUnderlyingLifecycle<Lifecycle_ExplicitLifecycle>
{

	public static Lifecycle_ExplicitLifecycle GetFromPool(IExplicitLifecycle explicitLifecycle)
	{
		Lifecycle_ExplicitLifecycle lifecycle = _pool.Pop();
		if (explicitLifecycle.IsExpired)
		{
			lifecycle.SetExpired();
		}
		else
		{
			explicitLifecycle.RegisterOnExpired(static (_, @this) => Unsafe.As<Lifecycle_ExplicitLifecycle>(@this!).SetExpired(), lifecycle);
		}

		return lifecycle;
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

	public LifecycleExpiredRegistration RegisterOnExpired(Action<IUnderlyingLifecycle, object?> callback, object? state, UnderlyingLifecycleToken token) => _comp.RegisterOnExpired(callback, state, token);

	public void UnregisterOnExpired(LifecycleExpiredRegistration registration, UnderlyingLifecycleToken token) => _comp.UnregisterOnExpired(registration, token);

	bool IUnderlyingLifecycle.IsExpired(UnderlyingLifecycleToken token) => _comp.IsExpired(token);

	public UnderlyingLifecycleToken Token => _comp.Token;

	public Lifecycle_ExplicitLifecycle? PoolNext { get; set; }

	private void SetExpired()
	{
		_comp.SetExpired();
		_pool.Push(this);
	}
	
	private static readonly UnderlyingLifecyclePool<Lifecycle_ExplicitLifecycle> _pool = new();
	
	private PoolableUnderlyingLifecycleComponent _comp;

}
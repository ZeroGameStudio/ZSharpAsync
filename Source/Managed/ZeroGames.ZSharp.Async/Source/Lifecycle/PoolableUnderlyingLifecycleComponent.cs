// Copyright Zero Games. All Rights Reserved.

namespace ZeroGames.ZSharp.Async;

public class PoolableUnderlyingLifecycleComponent(IUnderlyingLifecycle lifecycle)
{

	public void Initialize()
	{
		_isExpired = false;
		_handle = 0;
		_registry?.Clear();
	}

	public void Deinitialize()
	{
		Token = Token.Next;
	}
	
	public LifecycleExpiredRegistration RegisterOnExpired(Action<IUnderlyingLifecycle, object?> callback, object? state, UnderlyingLifecycleToken token)
	{
		ValidateToken(token);
		_registry ??= new();
		LifecycleExpiredRegistration reg = new(new(_lifecycle), ++_handle);
		_registry[reg] = new(callback, state);

		return reg;
	}

	public void UnregisterOnExpired(LifecycleExpiredRegistration registration, UnderlyingLifecycleToken token)
	{
		ValidateToken(token);
		_registry?.Remove(registration);
	}

	public bool IsExpired(UnderlyingLifecycleToken token)
	{
		ValidateToken(token);
		return _isExpired;
	}

	public void SetExpired()
	{
		if (_isExpired)
		{
			throw new InvalidOperationException();
		}
		
		_isExpired = true;
		if (_registry is not null)
		{
			foreach (var pair in _registry)
			{
				Rec rec = pair.Value;
				rec.Callback(_lifecycle, rec.State);
			}
		}
	}

	public UnderlyingLifecycleToken Token { get; private set; }
	
	private void ValidateToken(UnderlyingLifecycleToken token)
	{
		if (token != Token)
		{
			throw new InvalidOperationException();
		}
	}
	
	private readonly record struct Rec(Action<IUnderlyingLifecycle, object?> Callback, object? State);

	private IUnderlyingLifecycle _lifecycle = lifecycle;
	private uint64 _handle;
	
	private bool _isExpired;
	private Dictionary<LifecycleExpiredRegistration, Rec>? _registry;

}



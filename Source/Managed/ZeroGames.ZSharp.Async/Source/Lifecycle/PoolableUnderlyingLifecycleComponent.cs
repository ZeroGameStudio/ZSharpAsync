// Copyright Zero Games. All Rights Reserved.

using System.Threading;

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
		lock (this)
		{
			ValidateToken(token);
			if (_isExpired)
			{
				if (UnrealEngineStatics.IsInGameThread())
				{
					callback(_lifecycle, state);
				}
				else
				{
					SynchronizationContext.Current?.Send(s => callback(_lifecycle, s), state);
				}
                
				return default;
			}
			else
			{
				_registry ??= new();
				LifecycleExpiredRegistration reg = new(new(_lifecycle), ++_handle);
				_registry[reg] = new(callback, state);

				return reg;
			}
		}
	}

	public void UnregisterOnExpired(LifecycleExpiredRegistration registration, UnderlyingLifecycleToken token)
	{
		lock (this)
		{
			ValidateToken(token);
			if (!_isExpired)
			{
				_registry?.Remove(registration);
			}
		}
	}

	public bool IsExpired(UnderlyingLifecycleToken token)
	{
		lock (this)
		{
			ValidateToken(token);
			return _isExpired;
		}
	}

	public void SetExpired()
	{
		lock (this)
		{
			if (_isExpired)
			{
				throw new InvalidOperationException();
			}
			
			_isExpired = true;
			bool isInGameThread = UnrealEngineStatics.IsInGameThread();
			if (_registry is not null)
			{
				foreach (var pair in _registry)
				{
					Rec rec = pair.Value;
					if (isInGameThread)
					{
						rec.Callback(lifecycle, rec.State);
					}
					else
					{
						SynchronizationContext.Current?.Send(_ => rec.Callback(lifecycle, rec.State), null);
					}
				}
			}
		}
	}

	public UnderlyingLifecycleToken Token
	{
		get
		{
			lock (this)
			{
				return _token;
			}
		}
		private set
		{
			lock (this)
			{
				_token = _token.Next;
			}
		}
	}
	
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
	
	private UnderlyingLifecycleToken _token;
	private bool _isExpired;
	private Dictionary<LifecycleExpiredRegistration, Rec>? _registry;

}



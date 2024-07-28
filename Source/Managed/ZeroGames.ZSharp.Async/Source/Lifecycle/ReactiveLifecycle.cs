﻿// Copyright Zero Games. All Rights Reserved.

using System.Runtime.CompilerServices;

namespace ZeroGames.ZSharp.Async;

public readonly partial struct ReactiveLifecycle : IEquatable<ReactiveLifecycle>
{

	public bool Equals(ReactiveLifecycle other) => Equals(_underlyingLifecycle, other._underlyingLifecycle) && _capturedToken == other._capturedToken;
	public override bool Equals(object? obj) => obj is ReactiveLifecycle other && Equals(other);
	public override int32 GetHashCode() => _underlyingLifecycle?.GetHashCode() ?? 0;
	public static bool operator==(ReactiveLifecycle lhs, ReactiveLifecycle rhs) => lhs.Equals(rhs);
	public static bool operator!=(ReactiveLifecycle lhs, ReactiveLifecycle rhs) => !lhs.Equals(rhs);

	public static implicit operator Lifecycle(ReactiveLifecycle @this)
	{
		if (@this.IsExpired)
		{
			return Lifecycle.ExpiredLifecycle;
		}

		if (@this._underlyingLifecycle is null)
		{
			return default;
		}

		return new(@this._underlyingLifecycle);
	}
	
	public LifecycleExpiredRegistration RegisterOnExpired(Action<object, object?> callback, object? state)
	{
		ThreadHelper.ValidateGameThread();

		if (IsExpired)
		{
			callback(_underlyingLifecycle ?? this, state);
			return default;
		}

		if (_underlyingLifecycle is IExplicitLifecycle explicitLifecycle)
		{
			if (explicitLifecycle.IsGameThreadOnly)
			{
				return new(this, explicitLifecycle.RegisterOnExpired(callback, state));
			}
			else
			{
				lock (explicitLifecycle.SyncRoot)
				{
					return new(this, explicitLifecycle.RegisterOnExpired(callback, state));
				}
			}
		}
		else if (_underlyingLifecycle is not null)
		{
			var reactiveUnderlyingLifecycle = Unsafe.As<IReactiveUnderlyingLifecycle>(_underlyingLifecycle);
			return reactiveUnderlyingLifecycle.RegisterOnExpired(callback, state, _capturedToken);
		}

		return default;
	}

	public void UnregisterOnExpired(in LifecycleExpiredRegistration registration)
	{
		ThreadHelper.ValidateGameThread();

		if (IsExpired)
		{
			return;
		}
		
		if (_underlyingLifecycle is IExplicitLifecycle explicitLifecycle)
		{
			if (explicitLifecycle.IsGameThreadOnly)
			{
				explicitLifecycle.UnregisterOnExpired(registration.Explicit);
			}
			else
			{
				lock (explicitLifecycle.SyncRoot)
				{
					explicitLifecycle.UnregisterOnExpired(registration.Explicit);
				}
			}
		}
		else if (_underlyingLifecycle is not null)
		{
			var reactiveUnderlyingLifecycle = Unsafe.As<IReactiveUnderlyingLifecycle>(_underlyingLifecycle);
			reactiveUnderlyingLifecycle.UnregisterOnExpired(registration, _capturedToken);
		}
	}

	public bool IsExpired
	{
		get
		{
			ThreadHelper.ValidateGameThread();

			if (_underlyingLifecycle is null)
			{
				return _capturedToken == _inlineExpiredToken;
			}

			if (_capturedToken.IsValid)
			{
				var interfaceUnderlyingLifecycle = Unsafe.As<IUnderlyingLifecycle>(_underlyingLifecycle);
				return _capturedToken != interfaceUnderlyingLifecycle.Token || interfaceUnderlyingLifecycle.IsExpired(_capturedToken);
			}
			else
			{
				return Unsafe.As<IExplicitLifecycle>(_underlyingLifecycle).IsExpired;
			}
		}
	}
	
	internal ReactiveLifecycle(IReactiveUnderlyingLifecycle underlyingLifecycle)
	{
		_underlyingLifecycle = underlyingLifecycle;
		_capturedToken = underlyingLifecycle.Token;
	}

	internal ReactiveLifecycle(IExplicitLifecycle underlyingLifecycle)
	{
		lock (underlyingLifecycle.SyncRoot)
		{
			if (underlyingLifecycle.IsExpired)
			{
				_capturedToken = _inlineExpiredToken;
			}
			else
			{
				_underlyingLifecycle = underlyingLifecycle;
			}
		}
	}
	
	private ReactiveLifecycle(in UnderlyingLifecycleToken inlineExpiredToken)
	{
		_capturedToken = _inlineExpiredToken;
	}

	private static UnderlyingLifecycleToken _inlineExpiredToken = new(0xDEAD);
	
	private readonly object? _underlyingLifecycle;
	private readonly UnderlyingLifecycleToken _capturedToken;
	
}



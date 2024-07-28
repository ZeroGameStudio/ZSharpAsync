// Copyright Zero Games. All Rights Reserved.

using System.Runtime.CompilerServices;

namespace ZeroGames.ZSharp.Async;

public readonly partial struct Lifecycle : IEquatable<Lifecycle>
{

	public bool Equals(Lifecycle other) => _underlyingLifecycle == other._underlyingLifecycle && _capturedToken == other._capturedToken;
	public override bool Equals(object? obj) => obj is Lifecycle other && Equals(other);
	public override int32 GetHashCode() => _underlyingLifecycle?.GetHashCode() ?? 0;
	public static bool operator==(Lifecycle lhs, Lifecycle rhs) => lhs.Equals(rhs);
	public static bool operator!=(Lifecycle lhs, Lifecycle rhs) => !lhs.Equals(rhs);

	public bool IsExpired
	{
		get
		{
			ThreadHelper.ValidateGameThread();

			if (_underlyingLifecycle is null)
			{
				return _capturedToken == _inlineExpiredToken;
			}

			if (_underlyingLifecycle is WeakReference wr)
			{
				return !wr.IsAlive;
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

	internal Lifecycle(IUnderlyingLifecycle underlyingLifecycle)
	{
		_underlyingLifecycle = underlyingLifecycle;
		_capturedToken = underlyingLifecycle.Token;
	}

	internal Lifecycle(IExplicitLifecycle underlyingLifecycle)
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
	
	internal Lifecycle(object underlyingLifecycle)
	{
		if (underlyingLifecycle is IUnderlyingLifecycle interfaceUnderlyingLifecycle)
		{
			_underlyingLifecycle = interfaceUnderlyingLifecycle;
			_capturedToken = interfaceUnderlyingLifecycle.Token;
		}
		else if (underlyingLifecycle is IExplicitLifecycle explicitLifecycle)
		{
			lock (explicitLifecycle.SyncRoot)
			{
				if (explicitLifecycle.IsExpired)
				{
					_capturedToken = _inlineExpiredToken;
				}
				else
				{
					_underlyingLifecycle = explicitLifecycle;
				}
			}
		}
		else
		{
			_underlyingLifecycle = new WeakReference(underlyingLifecycle);
		}
	}

	private Lifecycle(UnderlyingLifecycleToken inlineExpiredToken)
	{
		_capturedToken = _inlineExpiredToken;
	}

	private static UnderlyingLifecycleToken _inlineExpiredToken = new(0xDEAD);
	
	private readonly object? _underlyingLifecycle;
	private readonly UnderlyingLifecycleToken _capturedToken;
	
}



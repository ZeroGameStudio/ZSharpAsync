// Copyright Zero Games. All Rights Reserved.

namespace ZeroGames.ZSharp.Async;

public readonly struct Lifecycle(IUnderlyingLifecycle underlyingLifecycle) : IEquatable<Lifecycle>
{

	public bool Equals(Lifecycle other) => Equals(_underlyingLifecycle, other._underlyingLifecycle) && _capturedToken == other._capturedToken;
	public override bool Equals(object? obj) => obj is Lifecycle other && Equals(other);
	public override int32 GetHashCode() => _underlyingLifecycle?.GetHashCode() ?? 0;
	public static bool operator==(Lifecycle lhs, Lifecycle rhs) => lhs.Equals(rhs);
	public static bool operator!=(Lifecycle lhs, Lifecycle rhs) => !lhs.Equals(rhs);

	public LifecycleExpiredRegistration RegisterOnExpired(Action<IUnderlyingLifecycle, object?> callback, object? state) => _underlyingLifecycle?.RegisterOnExpired(callback, state) ?? default;

	public bool IsExpired => _underlyingLifecycle?.IsExpired ?? false;
	
	private readonly IUnderlyingLifecycle? _underlyingLifecycle = underlyingLifecycle;
	private readonly UnderlyingLifecycleToken _capturedToken = underlyingLifecycle.Token;
	
}



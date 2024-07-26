// Copyright Zero Games. All Rights Reserved.

namespace ZeroGames.ZSharp.Async;

public readonly struct LifecycleExpiredRegistration(Lifecycle lifecycle, uint64 handle) : IEquatable<LifecycleExpiredRegistration>
{

	public bool Equals(LifecycleExpiredRegistration other) => _lifecycle == other._lifecycle && _handle == other._handle;
	public override bool Equals(object? obj) => obj is LifecycleExpiredRegistration other && Equals(other);
	public override int32 GetHashCode() => _handle.GetHashCode();
	public static bool operator==(LifecycleExpiredRegistration lhs, LifecycleExpiredRegistration rhs) => lhs.Equals(rhs);
	public static bool operator!=(LifecycleExpiredRegistration lhs, LifecycleExpiredRegistration rhs) => !lhs.Equals(rhs);

	public void Unregister() => _lifecycle.UnregisterOnExpired(this);

	private readonly Lifecycle _lifecycle = lifecycle;
	private readonly uint64 _handle = handle;

}



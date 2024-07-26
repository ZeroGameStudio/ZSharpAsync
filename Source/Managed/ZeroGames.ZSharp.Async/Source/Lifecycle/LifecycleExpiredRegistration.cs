// Copyright Zero Games. All Rights Reserved.

namespace ZeroGames.ZSharp.Async;

public readonly struct LifecycleExpiredRegistration(IUnderlyingLifecycle owner, uint64 handle) : IEquatable<LifecycleExpiredRegistration>
{

	public bool Equals(LifecycleExpiredRegistration other) => _owner == other._owner && _handle == other._handle;
	public override bool Equals(object? obj) => obj is LifecycleExpiredRegistration other && Equals(other);
	public override int32 GetHashCode() => _handle.GetHashCode();
	public static bool operator==(LifecycleExpiredRegistration lhs, LifecycleExpiredRegistration rhs) => lhs.Equals(rhs);
	public static bool operator!=(LifecycleExpiredRegistration lhs, LifecycleExpiredRegistration rhs) => !lhs.Equals(rhs);

	public void Unregister() => _owner?.UnregisterOnExpired(this);

	private readonly IUnderlyingLifecycle? _owner = owner;
	private readonly uint64 _handle = handle;

}



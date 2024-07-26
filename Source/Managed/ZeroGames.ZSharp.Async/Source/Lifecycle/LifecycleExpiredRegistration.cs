// Copyright Zero Games. All Rights Reserved.

namespace ZeroGames.ZSharp.Async;

public readonly struct LifecycleExpiredRegistration(uint64 handle, Action<uint64> unregister) : IEquatable<LifecycleExpiredRegistration>
{

	public bool Equals(LifecycleExpiredRegistration other) => _handle == other._handle && _unregister == other._unregister;
	public override bool Equals(object? obj) => obj is LifecycleExpiredRegistration other && Equals(other);
	public override int32 GetHashCode() => _handle.GetHashCode();
	public static bool operator==(LifecycleExpiredRegistration lhs, LifecycleExpiredRegistration rhs) => lhs.Equals(rhs);
	public static bool operator!=(LifecycleExpiredRegistration lhs, LifecycleExpiredRegistration rhs) => !lhs.Equals(rhs);

	public void Unregister() => _unregister(_handle);

	private readonly uint64 _handle = handle;
	private readonly Action<uint64> _unregister = unregister;

}



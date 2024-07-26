// Copyright Zero Games. All Rights Reserved.

namespace ZeroGames.ZSharp.Async;

public readonly struct EventLoopRegistration : IEquatable<EventLoopRegistration>
{

	public bool Equals(EventLoopRegistration other) => _handle == other._handle;
	public override bool Equals(object? obj) => obj is EventLoopRegistration other && Equals(other);
	public override int32 GetHashCode() => _handle.GetHashCode();
	public static bool operator==(EventLoopRegistration lhs, EventLoopRegistration rhs) => lhs.Equals(rhs);
	public static bool operator!=(EventLoopRegistration lhs, EventLoopRegistration rhs) => !lhs.Equals(rhs);

	public void Unregister()
	{
		if (_owner is null)
		{
			throw new InvalidOperationException();
		}
		
		_owner.InternalUnregister(this);
	}
	
	public bool IsValid => _handle > 0;

	internal EventLoopRegistration(EventLoop owner, uint64 handle)
	{
		_owner = owner;
		_handle = handle;
	}

	private readonly EventLoop? _owner;
	private readonly uint64 _handle;

}



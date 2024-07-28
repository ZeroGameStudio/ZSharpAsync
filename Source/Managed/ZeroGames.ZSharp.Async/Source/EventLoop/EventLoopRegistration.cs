﻿// Copyright Zero Games. All Rights Reserved.

namespace ZeroGames.ZSharp.Async;

public readonly struct EventLoopRegistration : IEquatable<EventLoopRegistration>
{

	public bool Equals(EventLoopRegistration other) => _handle == other._handle;
	public override bool Equals(object? obj) => obj is EventLoopRegistration other && Equals(other);
	public override int32 GetHashCode() => _handle.GetHashCode();
	public static bool operator==(EventLoopRegistration lhs, EventLoopRegistration rhs) => lhs.Equals(rhs);
	public static bool operator!=(EventLoopRegistration lhs, EventLoopRegistration rhs) => !lhs.Equals(rhs);

	public void Unregister() => _owner?.Unregister(this);
	
	public bool IsValid => _owner?.IsValidRegistration(this) ?? false;

	internal EventLoopRegistration(IEventLoop owner, uint64 handle)
	{
		_owner = owner;
		_handle = handle;
	}

	private readonly IEventLoop? _owner;
	private readonly uint64 _handle;

}



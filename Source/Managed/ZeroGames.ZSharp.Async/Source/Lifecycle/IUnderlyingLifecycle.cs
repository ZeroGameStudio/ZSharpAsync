// Copyright Zero Games. All Rights Reserved.

namespace ZeroGames.ZSharp.Async;

public interface IUnderlyingLifecycle
{
	LifecycleExpiredRegistration RegisterOnExpired(Action<IUnderlyingLifecycle, object?> callback, object? state);
	UnderlyingLifecycleToken Token { get; }
	bool IsExpired { get; }
}



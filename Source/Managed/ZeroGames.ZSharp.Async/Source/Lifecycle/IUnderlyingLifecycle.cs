// Copyright Zero Games. All Rights Reserved.

namespace ZeroGames.ZSharp.Async;

public interface IUnderlyingLifecycle
{
	LifecycleExpiredRegistration RegisterOnExpired(Action<IUnderlyingLifecycle, object?> callback, object? state, UnderlyingLifecycleToken token);
	void UnregisterOnExpired(LifecycleExpiredRegistration registration, UnderlyingLifecycleToken token);
	bool IsExpired(UnderlyingLifecycleToken token);
	UnderlyingLifecycleToken Token { get; }
}



﻿// Copyright Zero Games. All Rights Reserved.

namespace ZeroGames.ZSharp.Async;

public delegate void EventLoopCallback(in EventLoopArgs args, object? state);

public interface IEventLoop
{
	public static IEventLoop Get() => EventLoop.Get();

	EventLoopRegistration Register(EEventLoopTickingGroup group, EventLoopCallback callback, object? state, Lifecycle lifecycle = default, Action<LifecycleExpiredException>? onExpired = null);
	void Unregister(EventLoopRegistration registration);
	void UnregisterAll(Lifecycle lifecycle);
	bool IsValidRegistration(EventLoopRegistration registration);
}



// Copyright Zero Games. All Rights Reserved.

using ZeroGames.ZSharp.Async.Timer;

namespace ZeroGames.ZSharp.Async.EventLoop;

public delegate void EventLoopHandler(in EventLoopArgs args);

public interface IEventLoop
{
	public static IEventLoop Get() => EventLoop.Get();
	public static ITimerManager GetTimerManager() => EventLoop.GetTimerManager();
	public static ITimerManager GetTimerManagerSlim() => EventLoop.GetTimerManagerSlim();
	EventLoopObserverHandle RegisterObserver(IEventLoopObserver observer, object? lifecycle);
	EventLoopObserverHandle RegisterObserver(EEventLoopTickingGroup group, EventLoopHandler observer, object? lifecycle);
	EventLoopObserverHandle RegisterObserver(EEventLoopTickingGroup group, Action observer, object? lifecycle);
	EventLoopObserverHandle RegisterObserver(EEventLoopTickingGroup group, Action<float> observer, object? lifecycle);
	void UnregisterObserver(IEventLoopObserver observer);
	void UnregisterObserver(EventLoopObserverHandle observer);
}



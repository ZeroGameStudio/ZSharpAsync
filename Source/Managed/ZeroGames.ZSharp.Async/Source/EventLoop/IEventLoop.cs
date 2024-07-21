// Copyright Zero Games. All Rights Reserved.

using ZeroGames.ZSharp.Async.Timer;

namespace ZeroGames.ZSharp.Async.EventLoop;

public delegate void EventLoopHandler(in EventLoopArgs args);
public delegate void EventLoopHandler<in T>(in EventLoopArgs args, T state);

public interface IEventLoop
{
	public static IEventLoop Get() => EventLoop.Get();
	
	public static ITimerManager GetTimerManager() => EventLoop.GetTimerManager();
	public static ITimerManager GetTimerManagerSlim() => EventLoop.GetTimerManagerSlim();
	
	EventLoopObserverHandle RegisterObserver(IEventLoopObserver observer, object? lifecycle);
	
	EventLoopObserverHandle RegisterObserver(EEventLoopTickingGroup group, EventLoopHandler observer, object? lifecycle);
	EventLoopObserverHandle RegisterObserver<T>(EEventLoopTickingGroup group, EventLoopHandler<T> observer, T state, object? lifecycle);
	
	EventLoopObserverHandle RegisterObserver(EEventLoopTickingGroup group, Action observer, object? lifecycle);
	EventLoopObserverHandle RegisterObserver<T>(EEventLoopTickingGroup group, Action<T> observer, T state, object? lifecycle);
	
	EventLoopObserverHandle RegisterObserver(EEventLoopTickingGroup group, Action<float> observer, object? lifecycle);
	EventLoopObserverHandle RegisterObserver<T>(EEventLoopTickingGroup group, Action<float, T> observer, T state, object? lifecycle);
	
	void UnregisterObserver(IEventLoopObserver observer);
	void UnregisterObserver(EventLoopObserverHandle observer);
}



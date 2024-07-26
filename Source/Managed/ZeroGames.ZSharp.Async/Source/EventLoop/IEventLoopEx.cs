// Copyright Zero Games. All Rights Reserved.

namespace ZeroGames.ZSharp.Async;

public static class IEventLoopEx
{
	
	public static EventLoopObserverHandle RegisterObserver(this IEventLoop @this, IEventLoopObserver observer) => @this.RegisterObserver(observer, observer);

	// These are likely to misuse so add an explicit <Strong> to method name to help debugging.
	public static EventLoopObserverHandle RegisterStrongObserver(this IEventLoop @this, IEventLoopObserver observer) => @this.RegisterObserver(observer, null);
	public static EventLoopObserverHandle RegisterStrongObserver(this IEventLoop @this, EEventLoopTickingGroup group, EventLoopHandler observer) => @this.RegisterObserver(group, observer, null);
	public static EventLoopObserverHandle RegisterStrongObserver(this IEventLoop @this, EEventLoopTickingGroup group, Action observer) => @this.RegisterObserver(group, observer, null);
	public static EventLoopObserverHandle RegisterStrongObserver(this IEventLoop @this, EEventLoopTickingGroup group, Action<float> observer) => @this.RegisterObserver(group, observer, null);
	
}



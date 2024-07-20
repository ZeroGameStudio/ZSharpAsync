// Copyright Zero Games. All Rights Reserved.

namespace ZeroGames.ZSharp.Async.EventLoop;

public interface IEventLoopObserver
{
	void NotifyEvent(in EventLoopArgs args);
	EEventLoopTickingGroup EventType { get; }
	EventLoopObserverHandle Handle { get; set; } // WARNING: NEVER SET this in user code.
}



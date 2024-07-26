// Copyright Zero Games. All Rights Reserved.

namespace ZeroGames.ZSharp.Async;

public interface IEventLoopObserver
{
	void NotifyEvent(in EventLoopArgs args);
	EEventLoopTickingGroup TickingGroup { get; }
	EventLoopObserverHandle Handle { get; set; } // WARNING: NEVER SET this in user code.
}



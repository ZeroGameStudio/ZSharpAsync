﻿// Copyright Zero Games. All Rights Reserved.

namespace ZeroGames.ZSharp.Async.EventLoop;

public enum EEventLoopTickingGroup : uint8
{
	PreWorldTick,
	PreActorTick,
	PrePhysicsTick,
	DuringPhysicsTick,
	PostPhysicsTick,
	DuringWorldTimerTick,
	PostUpdateTick,
	PostActorTick,
	PostWorldTick,
}



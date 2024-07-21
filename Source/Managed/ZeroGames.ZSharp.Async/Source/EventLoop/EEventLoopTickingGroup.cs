﻿// Copyright Zero Games. All Rights Reserved.

namespace ZeroGames.ZSharp.Async;

public enum EEventLoopTickingGroup : uint8
{
	PreWorldTick,
	PreActorTick,
	PrePhysicsTick,
	DuringPhysicsTick,
	PostPhysicsTick,
	PostWorldTimerTick,
	PostUpdateTick,
	PostActorTick,
	PostWorldTick,
}



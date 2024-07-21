// Copyright Zero Games. All Rights Reserved.

#pragma once

namespace ZSharp
{
	enum class EZSharpEventLoopTickingGroup : uint8
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
	};
}



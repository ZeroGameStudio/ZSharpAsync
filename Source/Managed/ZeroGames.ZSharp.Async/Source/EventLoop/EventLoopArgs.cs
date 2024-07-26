// Copyright Zero Games. All Rights Reserved.

namespace ZeroGames.ZSharp.Async;

public readonly struct EventLoopArgs
{
	public required EEventLoopTickingGroup TickingGroup { get; init; }
	public required float WorldDeltaTime { get; init; }
	public required float RealDeltaTime { get; init; }
	public required double WorldElapsedTime { get; init; }
	public required double RealElapsedTime { get; init; }
	public required double WorldAccumulatedTime { get; init; }
	public required double RealAccumulatedTime { get; init; }
}



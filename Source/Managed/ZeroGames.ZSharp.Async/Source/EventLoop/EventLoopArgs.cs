// Copyright Zero Games. All Rights Reserved.

namespace ZeroGames.ZSharp.Async;

public readonly struct EventLoopArgs
{
	public required EEventLoopTickingGroup TickingGroup { get; init; }
	public required TimeSpan WorldDeltaTime { get; init; }
	public required TimeSpan RealDeltaTime { get; init; }
	public required TimeSpan WorldElapsedTime { get; init; }
	public required TimeSpan RealElapsedTime { get; init; }
	public required TimeSpan WorldAccumulatedTime { get; init; }
	public required TimeSpan RealAccumulatedTime { get; init; }
}



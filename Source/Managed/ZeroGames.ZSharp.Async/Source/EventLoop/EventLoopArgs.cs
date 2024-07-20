// Copyright Zero Games. All Rights Reserved.

namespace ZeroGames.ZSharp.Async.EventLoop;

public readonly ref struct EventLoopArgs
{
	public EEventLoopTickingGroup EventType { get; init; }
	public float WorldDeltaTime { get; init; }
	public float RealDeltaTime { get; init; }
	public double WorldElapsedTime { get; init; }
	public double RealElapsedTime { get; init; }
	public double WorldAccumulatedTime { get; init; }
	public double RealAccumulatedTime { get; init; }
}



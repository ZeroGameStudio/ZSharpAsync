// Copyright Zero Games. All Rights Reserved.

namespace ZeroGames.ZSharp.Async;

public readonly struct AccumulatedSeconds
{

	public static AccumulatedSeconds operator +(AccumulatedSeconds lhs, float deltaSeconds) => new(lhs._rounds, lhs._seconds + deltaSeconds);
	public static AccumulatedSeconds operator +(AccumulatedSeconds lhs, double deltaSeconds) => new(lhs._rounds, lhs._seconds + deltaSeconds);

	public TimeSpan TimeSpan => TimeSpan.FromSeconds(Seconds);
	public double Seconds => _secondsPerRound * _rounds + _seconds;

	private AccumulatedSeconds(uint64 rounds, double seconds)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(seconds);
		
		uint64 extraRounds = (uint64)(seconds / _secondsPerRound);
		rounds += extraRounds;
		seconds -= _secondsPerRound * extraRounds;
		
		_rounds = rounds;
		_seconds = seconds;
	}

	private const double _secondsPerRound = 1000.0;
	
	private readonly uint64 _rounds;
	private readonly double _seconds;

}



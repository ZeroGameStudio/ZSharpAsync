// Copyright Zero Games. All Rights Reserved.

namespace ZeroGames.ZSharp.Async.Timer;

public interface ITimerManager
{
	void Tick(float deltaTime);
	double BudgetMsPerTick { get; set; }
}



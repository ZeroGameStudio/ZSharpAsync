// Copyright Zero Games. All Rights Reserved.

namespace ZeroGames.ZSharp.Async.Timer;

public interface ITimerManager
{
	TimerHandle Add(Action action, object? lifecycle);
	TimerHandle Add<T>(Action<T> action, T state, object? lifecycle);
	TimerHandle Add(Action<float> action, object? lifecycle);
	TimerHandle Add<T>(Action<float, T> action, T state, object? lifecycle);
	void Remove(TimerHandle timer);
	void RemoveAllForLifecycle(object lifecycle);
	void Suspend(TimerHandle timer);
	void SuspendAllForLifecycle(object lifecycle);
	void Resume(TimerHandle timer);
	void ResumeAllForLifecycle(object lifecycle);
	void Tick(float deltaTime);
	double BudgetMsPerTick { get; set; }
}



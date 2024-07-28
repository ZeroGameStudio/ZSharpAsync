// Copyright Zero Games. All Rights Reserved.

namespace ZeroGames.ZSharp.Async;

public interface ITimerScheduler
{
	Timer Register(Action<float, object?> action, object? state, Lifecycle lifecycle = default);
	
	void Unregister(Timer timer);
	void UnregisterAll(Lifecycle lifecycle);
	
	void Suspend(Timer timer);
	void SuspendAll(Lifecycle lifecycle);
	
	void Resume(Timer timer);
	void ResumeAll(Lifecycle lifecycle);

	bool IsValidTimer(Timer timer);
}



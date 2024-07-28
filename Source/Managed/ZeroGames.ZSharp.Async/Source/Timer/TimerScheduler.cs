// Copyright Zero Games. All Rights Reserved.

namespace ZeroGames.ZSharp.Async;

public class TimerScheduler : ITimerScheduler
{
	
	public Timer Register(Action<float, object?> action, object? state, Lifecycle lifecycle = default)
	{
		throw new NotImplementedException();
	}

	public void Unregister(Timer timer)
	{
		throw new NotImplementedException();
	}

	public void UnregisterAll(Lifecycle lifecycle)
	{
		throw new NotImplementedException();
	}

	public void Suspend(Timer timer)
	{
		throw new NotImplementedException();
	}

	public void SuspendAll(Lifecycle lifecycle)
	{
		throw new NotImplementedException();
	}

	public void Resume(Timer timer)
	{
		throw new NotImplementedException();
	}

	public void ResumeAll(Lifecycle lifecycle)
	{
		throw new NotImplementedException();
	}

	public bool IsValidTimer(Timer timer)
	{
		throw new NotImplementedException();
	}
	
}



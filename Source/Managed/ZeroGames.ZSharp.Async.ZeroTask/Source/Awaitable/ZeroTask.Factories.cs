// Copyright Zero Games. All Rights Reserved.

namespace ZeroGames.ZSharp.Async.ZeroTask;

public partial struct ZeroTask
{

	public static ZeroTask FromUnderlyingTask(IUnderlyingZeroTask<AsyncVoid> task)
	{
		ThreadHelper.ValidateGameThread();

		return new(task);
	}
	
	public static ZeroTask FromException(Exception exception) => throw new NotImplementedException();

	public static ZeroTask Delay(double delayTimeMs, Lifecycle lifecycle = default) => Delay(TimeSpan.FromMilliseconds(delayTimeMs), lifecycle);
	public static ZeroTask Delay(TimeSpan delayTime, Lifecycle lifecycle = default)
	{
		ThreadHelper.ValidateGameThread();
		
		if (delayTime <= TimeSpan.Zero)
		{
			return CompletedTask;
		}
		
		ZeroTask_Delay delay = ZeroTask_Delay.GetFromPool(delayTime, lifecycle);
		ZeroTask task = FromUnderlyingTask(delay);
		delay.Run();
		return task;
	}

	public static ZeroTask CompletedTask => default;

}

public partial struct ZeroTask<TResult>
{
	
	public static ZeroTask<TResult> FromResult(TResult result) => new(result);

	public static ZeroTask<TResult> FromUnderlyingTask(IUnderlyingZeroTask<TResult> task)
	{
		ThreadHelper.ValidateGameThread();

		return new(task);
	}
	
	public static ZeroTask<TResult> FromException(Exception exception) => throw new NotImplementedException();

}



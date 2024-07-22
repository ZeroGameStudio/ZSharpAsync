// Copyright Zero Games. All Rights Reserved.

namespace ZeroGames.ZSharp.Async.ZeroTask;

public partial struct ZeroTask
{
	
	public static ZeroTask FromUnderlyingTask(IUnderlyingZeroTaskVoid? task, uint64 token) => new(task, token);
	public static ZeroTask FromResult() => default;
	public static ZeroTask FromException(Exception exception) => new(exception);

	public static ZeroTask Delay(double delayTimeMs)
	{
		if (delayTimeMs <= 0.0)
		{
			return FromResult();
		}
		
		ZeroTask_Delay delay = ZeroTask_Delay.GetFromPool();
		ZeroTask task = FromUnderlyingTask(delay, delay.Token);
		delay.Run(delayTimeMs);
		return task;
	}

}



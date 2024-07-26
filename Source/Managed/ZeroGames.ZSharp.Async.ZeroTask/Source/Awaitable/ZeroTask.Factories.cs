// Copyright Zero Games. All Rights Reserved.

namespace ZeroGames.ZSharp.Async.ZeroTask;

public partial struct ZeroTask
{
	
	public static ZeroTask FromUnderlyingTask(IUnderlyingZeroTaskVoid task) => new(task);
	public static ZeroTask FromResult() => default;
	public static ZeroTask FromException(Exception exception) => throw new NotImplementedException();

	public static ZeroTask Delay(double delayTimeMs)
	{
		if (delayTimeMs <= 0.0)
		{
			return FromResult();
		}
		
		ZeroTask_Delay delay = ZeroTask_Delay.GetFromPool(delayTimeMs);
		ZeroTask task = FromUnderlyingTask(delay);
		delay.Run();
		return task;
	}

}



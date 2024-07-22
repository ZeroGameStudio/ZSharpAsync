// Copyright Zero Games. All Rights Reserved.

namespace ZeroGames.ZSharp.Async.Task;

public partial struct Task
{
	
	public static Task FromUnderlyingTask(IUnderlyingTaskVoid? task, uint64 token) => new(task, token);
	public static Task FromResult() => default;
	public static Task FromException(Exception exception) => new(exception);

	public static Task Delay(double delayTimeMs)
	{
		if (delayTimeMs <= 0.0)
		{
			return FromResult();
		}
		
		DelayTask delay = DelayTask.GetFromPool();
		Task task = FromUnderlyingTask(delay, delay.Token);
		delay.Run(delayTimeMs);
		return task;
	}

}



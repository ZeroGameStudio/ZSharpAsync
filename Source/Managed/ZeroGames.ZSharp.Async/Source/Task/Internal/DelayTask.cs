// Copyright Zero Games. All Rights Reserved.

using ZeroGames.ZSharp.Async.EventLoop;

namespace ZeroGames.ZSharp.Async.Task;

internal class DelayTask : IPoolableUnderlyingTaskVoid<DelayTask>
{

	public static DelayTask GetFromPool() => _pool.Pop();
	
	public static DelayTask Create()
	{
		DelayTask task = new();
		task.Initialize();
		return task;
	}

	public void Initialize() => _comp.Initialize();

	public void Deinitialize() => _comp.Deinitialize();

	public EUnderlyingTaskStatus GetStatus(uint64 token) => _comp.GetStatus(token);

	public void SetContinuation(Action continuation, uint64 token) => _comp.SetContinuation(continuation, token);

	public void GetResult(uint64 token) => _comp.GetResult(token);

	public void Run(double delayTimeMs)
	{
		_delaySeconds = delayTimeMs * 0.001;
		_handle = IEventLoop.Get().RegisterObserver(EEventLoopTickingGroup.DuringWorldTimerTick, deltaSeconds =>
		{
			_elapsedSeconds += deltaSeconds;
			if (_elapsedSeconds >= _delaySeconds)
			{
				_comp.SetResult();
				IEventLoop.Get().UnregisterObserver(_handle);
			}
		}, this);
	}

	public uint64 Token => _comp.Token;

	public DelayTask? PoolNext { get; set; }

	private static readonly UnderlyingTaskPool<DelayTask> _pool = new();

	private PoolableUnderlyingTaskComponentVoid _comp;
	
	private double _elapsedSeconds;
	private double _delaySeconds;
	private EventLoopObserverHandle _handle;

}
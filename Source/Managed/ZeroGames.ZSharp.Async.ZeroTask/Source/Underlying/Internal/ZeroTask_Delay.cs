// Copyright Zero Games. All Rights Reserved.

using System.Runtime.CompilerServices;

namespace ZeroGames.ZSharp.Async.ZeroTask;

internal class ZeroTask_Delay : IPoolableUnderlyingZeroTaskVoid<ZeroTask_Delay>
{

	public static ZeroTask_Delay GetFromPool() => _pool.Pop();
	
	public static ZeroTask_Delay Create()
	{
		ZeroTask_Delay task = new();
		task.Deinitialize();
		task.Initialize();
		return task;
	}

	public void Initialize() => _comp.Initialize();

	public void Deinitialize() => _comp.Deinitialize();

	public EUnderlyingZeroTaskStatus GetStatus(UnderlyingZeroTaskToken token) => _comp.GetStatus(token);
	
	public void SetStateMachine(IAsyncStateMachine stateMachine, UnderlyingZeroTaskToken token) => _comp.SetStateMachine(stateMachine, token);

	public void SetContinuation(Action continuation, UnderlyingZeroTaskToken token) => _comp.SetContinuation(continuation, token);
	
	public void GetResult(UnderlyingZeroTaskToken token) => _comp.GetResult(token);

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

	public UnderlyingZeroTaskToken Token => _comp.Token;

	public ZeroTask_Delay? PoolNext { get; set; }

	private static readonly UnderlyingZeroTaskPool<ZeroTask_Delay> _pool = new();

	private PoolableUnderlyingZeroTaskComponentVoid _comp;
	
	private double _elapsedSeconds;
	private double _delaySeconds;
	private EventLoopObserverHandle _handle;

}
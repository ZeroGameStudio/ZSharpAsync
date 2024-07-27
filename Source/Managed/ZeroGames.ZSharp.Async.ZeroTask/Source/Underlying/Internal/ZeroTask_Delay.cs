// Copyright Zero Games. All Rights Reserved.

using System.Runtime.CompilerServices;

namespace ZeroGames.ZSharp.Async.ZeroTask;

internal class ZeroTask_Delay : IPoolableUnderlyingZeroTask<AsyncVoid, ZeroTask_Delay>
{

	public static ZeroTask_Delay GetFromPool(double delayTimeMs, Lifecycle lifecycle)
	{
		ZeroTask_Delay task = _pool.Pop();
		task._lifecycle = lifecycle;
		task._delaySeconds = delayTimeMs * 0.001;
		
		return task;
	}
	
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

	void IUnderlyingZeroTask.GetResult(UnderlyingZeroTaskToken token) => GetResult(token);
	public AsyncVoid GetResult(UnderlyingZeroTaskToken token)
	{
		AsyncVoid result = _comp.GetResult(token);
		_pool.Push(this);
		return result;
	}

	public void Run()
	{
		_reg = IEventLoop.Get().Register(EEventLoopTickingGroup.DuringWorldTimerTick, static (in EventLoopArgs args, object? state) =>
		{
			ZeroTask_Delay @this = Unsafe.As<ZeroTask_Delay>(state!);
			if (@this._lifecycle.IsExpired)
			{
				try
				{
					@this._comp.SetException(new LifecycleExpiredException(@this._lifecycle));
				}
				finally
				{
					@this._reg.Unregister();
				}
			}
			
			@this._elapsedSeconds += args.WorldDeltaTime;
			if (@this._elapsedSeconds >= @this._delaySeconds)
			{
				try
				{
					@this._comp.SetResult(default);
				}
				finally
				{
					@this._reg.Unregister();
				}
			}
		}, this, null);
	}

	public UnderlyingZeroTaskToken Token => _comp.Token;

	public ZeroTask_Delay? PoolNext { get; set; }

	private static UnderlyingZeroTaskPool<AsyncVoid, ZeroTask_Delay> _pool;

	private PoolableUnderlyingZeroTaskComponent<AsyncVoid> _comp;

	private Lifecycle _lifecycle;
	
	private double _elapsedSeconds;
	private double _delaySeconds;
	private EventLoopRegistration _reg;

}
// Copyright Zero Games. All Rights Reserved.

using System.Runtime.CompilerServices;
using System.Threading;
using ZeroGames.ZSharp.Async.Timer;

namespace ZeroGames.ZSharp.Async.EventLoop;

// WARNING: This class contains complex concurrent logic so do NOT modify if you don't know what you are doing!
internal class EventLoop : IEventLoop
{

	internal static EventLoop Get() => s_singleton;
	
	public ITimerManager GetTimerManager() => null!;
	public ITimerManager GetTimerManagerSlim() => null!;

	public EventLoopObserverHandle RegisterObserver(IEventLoopObserver observer, object? lifecycle) => InternalRegisterObserver(observer.TickingGroup, observer, ObserverType.Interface, false, null, lifecycle);
	public EventLoopObserverHandle RegisterObserver(EEventLoopTickingGroup group, EventLoopHandler observer, object? lifecycle) => InternalRegisterObserver(group, observer, ObserverType.Handler, false, null, lifecycle);
	public EventLoopObserverHandle RegisterObserver<T>(EEventLoopTickingGroup group, EventLoopHandler<T> observer, T state, object? lifecycle) => InternalRegisterObserver(group, observer, ObserverType.Handler, true, state, lifecycle);
	public EventLoopObserverHandle RegisterObserver(EEventLoopTickingGroup group, Action observer, object? lifecycle) => InternalRegisterObserver(group, observer, ObserverType.Action, false, null, lifecycle);
	public EventLoopObserverHandle RegisterObserver<T>(EEventLoopTickingGroup group, Action<T> observer, T state, object? lifecycle) => InternalRegisterObserver(group, observer, ObserverType.Action, true, state, lifecycle);
	public EventLoopObserverHandle RegisterObserver(EEventLoopTickingGroup group, Action<float> observer, object? lifecycle) => InternalRegisterObserver(group, observer, ObserverType.FloatAction, false, null, lifecycle);
	public EventLoopObserverHandle RegisterObserver<T>(EEventLoopTickingGroup group, Action<float, T> observer, T state, object? lifecycle) => InternalRegisterObserver(group, observer, ObserverType.FloatAction, true, state, lifecycle);
	
	public void UnregisterObserver(IEventLoopObserver observer)
	{
		EventLoopObserverHandle handle;
		try
		{
			_handleLock.EnterReadLock();
			
			handle = observer.Handle;
		}
		finally
		{
			_handleLock.ExitReadLock();
		}
		
		UnregisterObserver(handle);
	}

	public void UnregisterObserver(EventLoopObserverHandle observer)
	{
		if (!observer.IsValid)
		{
			return;
		}
		
		if (UnrealEngineStatics.IsInGameThread() && _isNotifing)
		{
			// Unregister a deferred observer before the same time notify end.
			if (_deferredRequestMap.Remove(observer))
			{
				return;
			}
			
			foreach (var pair in _observerMap)
			{
				if (pair.Value.ContainsKey(observer))
				{
					if (pair.Key == _lockedGroup)
					{
						// Just mark as garbage because NotifyEvent() is iterating the registry
						pair.Value[observer] = new();
					}
					else
					{
						pair.Value.Remove(observer);
					}
					
					return;
				}
			}
		}
		else
		{
			InternalUnregisterObserver(observer);
		}
	}

	internal void NotifyEvent(EEventLoopTickingGroup group, float worldDeltaTime, float realDeltaTime, double worldElapsedTime, double realElapsedTime)
	{
		lock (_registryLock)
		{
			try
			{
				_isNotifing = true;
				_lockedGroup = group;

				if (group == EEventLoopTickingGroup.PreWorldTick)
				{
					_worldAccumulatedTime += worldDeltaTime;
					_realAcccumulatedTime += realDeltaTime;
				}
		
				EventLoopArgs args = new()
				{
					TickingGroup = group,
					WorldDeltaTime = worldDeltaTime,
					RealDeltaTime = realDeltaTime,
					WorldElapsedTime = worldElapsedTime,
					RealElapsedTime = realElapsedTime,
					WorldAccumulatedTime = _worldAccumulatedTime,
					RealAccumulatedTime = _realAcccumulatedTime,
				};

				bool mayHaveSideEffects = false;
				if (_observerMap.TryGetValue(group, out var registry))
				{
					foreach (var pair in registry)
					{
						Rec rec = pair.Value;
						object? observer = null;
						if (rec.Lifecycle is null)
						{
							// Case 1: observer is not null: Need explicit unregister
							// Case 2: observer is null: Left garbage while unregister during notify.
							observer = rec.Observer;
						}
						else if (rec.Observer is null)
						{
							// Use self lifecycle scope
							observer = rec.Lifecycle.Target;
						}
						else
						{
							// Use explicit lifecycle scope
							object? lifecycle = rec.Lifecycle.Target;
							if (lifecycle is not null && (lifecycle is not IExplicitLifecycle explicitLifecycle || explicitLifecycle.IsAlive))
							{
								observer = rec.Observer;
							}
						}

						if (observer is not null)
						{
							mayHaveSideEffects = true;
							try
							{
								switch (rec.Type)
								{
									case ObserverType.Interface:
									{
										Unsafe.As<IEventLoopObserver>(observer).NotifyEvent(args);
										break;
									}
									case ObserverType.Action:
									{
										if (rec.HasState)
										{
											if (observer is Action<object?> action)
											{
												action.Invoke(rec.State);
											}
											else
											{
												_reusedStateActionParams[0] = rec.State;
												Unsafe.As<Delegate>(observer).DynamicInvoke(_reusedStateActionParams);
											}
										}
										else
										{
											Unsafe.As<Action>(observer).Invoke();
										}
										break;
									}
									case ObserverType.FloatAction:
									{
										if (rec.HasState)
										{
											if (observer is Action<float, object?> action)
											{
												action.Invoke(worldDeltaTime, rec.State);
											}
											else
											{
												_reusedFloatStateActionParams[0] = worldDeltaTime;
												_reusedFloatStateActionParams[1] = rec.State;
												Unsafe.As<Delegate>(observer).DynamicInvoke(_reusedFloatStateActionParams);
											}
										}
										else
										{
											Unsafe.As<Action<float>>(observer).Invoke(worldDeltaTime);
										}
										break;
									}
									case ObserverType.Handler:
									{
										if (rec.HasState)
										{
											if (observer is EventLoopHandler<object?> handler)
											{
												handler.Invoke(args, rec.State);
											}
											else
											{
												_reusedStateHandlerParams[0] = args;
												_reusedStateHandlerParams[1] = rec.State;
												Unsafe.As<Delegate>(observer).DynamicInvoke(_reusedStateHandlerParams);
											}
										}
										else
										{
											Unsafe.As<EventLoopHandler>(observer).Invoke(args);
										}
										break;
									}
								}
							}
							catch (Exception ex)
							{
								Logger.Error($"Unhandled Exception Detected in Event Loop.\n{ex}");
							}
						}
						else if (_numStales < _stales.Length)
						{
							_stales[_numStales++] = pair.Key;
						}
					}
				}

				if (group == EEventLoopTickingGroup.DuringWorldTimerTick)
				{
					// Tick built-in timers
					DateTime start = DateTime.Now;
					GetTimerManager().Tick(worldDeltaTime);
					double cost = (DateTime.Now - start).TotalMilliseconds;
					double remain = _timerBudgetMs - cost;
					if (remain > 0)
					{
						GetTimerManagerSlim().BudgetMsPerTick = remain;
						GetTimerManagerSlim().Tick(worldDeltaTime);
					}
				}
				
				// Compact
				for (int32 i = 0; i < _numStales; ++i)
				{
					registry!.Remove(_stales[i], out _);
				}
				_numStales = 0;
				
				// Apply deferred requests
				if (mayHaveSideEffects)
				{
					foreach (var pair in _deferredRequestMap)
					{
						pair.Value.Invoke();
					}
					_deferredRequestMap.Clear();
				}
			}
			finally
			{
				_isNotifing = false;
			}
		}
	}

	private EventLoopObserverHandle InternalRegisterObserver(EEventLoopTickingGroup group, object observer, ObserverType type, bool hasState, object? state, object? lifecycle)
	{
		EventLoopObserverHandle handle = AllocateHandle();

		if (UnrealEngineStatics.IsInGameThread() && _isNotifing)
		{
			_deferredRequestMap[handle] = () => { ActualRegisterObserver(group, handle, observer, type, hasState, state, lifecycle); };
		}
		else
		{
			ActualRegisterObserver(group, handle, observer, type, hasState, state, lifecycle);
		}
		
		return handle;
	}

	private void ActualRegisterObserver(EEventLoopTickingGroup group, EventLoopObserverHandle handle, object observer, ObserverType type, bool hasState, object? state, object? lifecycle)
	{
		lock (_registryLock)
		{
			if (!_observerMap.TryGetValue(group, out var registry))
			{
				registry = new();
				_observerMap[group] = registry;
			}

			object? observerRec = null;
			WeakReference? lifecycleRec = null;
			if (lifecycle is null)
			{
				// Need explicit unregister
				observerRec = observer;
			}
			else if (ReferenceEquals(observer, lifecycle))
			{
				if (observer is Delegate)
				{
					throw new InvalidOperationException("Self-bind lifecycle used for delegate.");
				}
				// Use self lifecycle scope
				lifecycleRec = new(observer);
			}
			else
			{
				// Use explicit lifecycle scope
				observerRec = observer;
				lifecycleRec = new(lifecycle);
			}

			registry[handle] = new(observerRec, type, hasState, state, lifecycleRec);
		}
	}
	
	private void InternalUnregisterObserver(EventLoopObserverHandle observer)
	{
		lock (_registryLock)
		{
			foreach (var pair in _observerMap)
			{
				if (pair.Value.Remove(observer, out var rec))
				{
					if (rec.Observer is IEventLoopObserver typedObserver)
					{
						typedObserver.Handle = default;
					}

					return;
				}
			}
		}
	}

	private EventLoopObserverHandle AllocateHandle()
	{
		try
		{
			_handleLock.EnterWriteLock();
			
			return new(++_currentHandle);
		}
		finally
		{
			_handleLock.ExitWriteLock();
		}
	}

	private EventLoopObserverHandle AllocateHandleForObserver(IEventLoopObserver observer)
	{
		try
		{
			_handleLock.EnterWriteLock();
			
			if (observer.Handle.IsValid)
			{
				throw new InvalidOperationException();
			}
		
			EventLoopObserverHandle handle = new(++_currentHandle);
			observer.Handle = handle;

			return handle;
		}
		finally
		{
			_handleLock.ExitWriteLock();
		}
	}

	private void ClearHandleForObserver(IEventLoopObserver observer)
	{
		try
		{
			_handleLock.EnterWriteLock();
			
			observer.Handle = default;
		}
		finally
		{
			_handleLock.ExitWriteLock();
		}
	}

	// This is pretty commonly used so create at startup.
	private static EventLoop s_singleton = new();

	// All codepaths that modify these containers have to acquire this lock.
	private object _registryLock = new();
	private Dictionary<EEventLoopTickingGroup, Dictionary<EventLoopObserverHandle, Rec>> _observerMap = new();

	// All codepaths that access _currentHandle or any IEventLoopObserver's Handle property have to acquire this lock.
	private ReaderWriterLockSlim _handleLock = new();
	private uint64 _currentHandle;
	
	// These two are only used by NotifyEvent() to clear stale weak observers.
	private EventLoopObserverHandle[] _stales = new EventLoopObserverHandle[4];
	private int32 _numStales;
	
	// These two are only used by GameThread to detect register/unregister during NotifyEvent().
	// By the way, other threads will be blocked until NotifyEvent() returns so they have no problem.
	private Dictionary<EventLoopObserverHandle, Action> _deferredRequestMap = new();
	private bool _isNotifing;
	private EEventLoopTickingGroup _lockedGroup;

	private object?[] _reusedStateActionParams = new object[1];
	private object?[] _reusedFloatStateActionParams = new object[2];
	private object?[] _reusedStateHandlerParams = new object[2];
	
	private double _worldAccumulatedTime;
	private double _realAcccumulatedTime;

	private double _timerBudgetMs = 3.0;

	private enum ObserverType
	{
		Interface,
		Action,
		FloatAction,
		Handler,
	}

	private record struct Rec(object? Observer, ObserverType Type, bool HasState, object? State, WeakReference? Lifecycle);

}



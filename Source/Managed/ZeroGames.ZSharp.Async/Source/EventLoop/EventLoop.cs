﻿// Copyright Zero Games. All Rights Reserved.

using System.Diagnostics.CodeAnalysis;

namespace ZeroGames.ZSharp.Async;

internal class EventLoop : IEventLoop
{

	public EventLoopRegistration Register(EEventLoopTickingGroup group, EventLoopCallback callback, object? state, object? lifecycle)
	{
		if (ReferenceEquals(callback, lifecycle))
		{
			throw new InvalidOperationException();
		}
		
		if (lifecycle is not null)
		{
			if (lifecycle is IExplicitLifecycle { IsExpired: true })
			{
				throw new InvalidOperationException();
			}
			if (lifecycle is Lifecycle { IsExpired: true })
			{
				throw new InvalidOperationException();
			}
		}
		
		return InternalRegister(group, callback, state, lifecycle);
	}
	
	public void UnregisterAll(object lifecycle) => InternalUnregisterAll(lifecycle);

	internal static EventLoop Get() => _singleton;
	
	internal void NotifyEvent(EEventLoopTickingGroup group, float worldDeltaTime, float realDeltaTime, double worldElapsedTime, double realElapsedTime)
	{
		lock (_registryLock)
		{
			_isNotifing = true;
			
			try
			{
				if (group == EEventLoopTickingGroup.PreWorldTick)
				{
					_worldAccumulatedTime += worldDeltaTime;
					_realAccumulatedTime += realDeltaTime;
				}
				
				EventLoopArgs args = new()
				{
					TickingGroup = group,
					WorldDeltaTime = worldDeltaTime,
					RealDeltaTime = realDeltaTime,
					WorldElapsedTime = worldElapsedTime,
					RealElapsedTime = realElapsedTime,
					WorldAccumulatedTime = _worldAccumulatedTime,
					RealAccumulatedTime = _realAccumulatedTime,
				};
				
				if (_registry.TryGetValue(group, out var registry))
				{
					bool mayHaveSideEffects = false;
					uint64 stale = 0;
					foreach (var pair in registry)
					{
						Rec rec = pair.Value;
						if (IsValidRec(rec))
						{
							try
							{
								rec.Callback!(args, rec.State);
							}
							catch (Exception ex)
							{
								Logger.Error($"Unhandled Exception Detected in Event Loop.\n{ex}");
							}
							finally
							{
								mayHaveSideEffects = true;
							}
						}
						else
						{
							stale = pair.Key;
						}
					}

					// Clear only one stale registration because we run very frequently.
					if (stale > 0)
					{
						registry.Remove(stale);
					}

					if (mayHaveSideEffects)
					{
						FlushDeferredRegistry();
					}
				}
			}
			finally
			{
				_isNotifing = false;
			}
		}
	}
	
	internal void InternalUnregister(uint64 handle)
	{
		static bool Traverse(Dictionary<EEventLoopTickingGroup, Dictionary<uint64, Rec>> registry, uint64 handle)
		{
			foreach (var pair in registry)
			{
				var innerRegistry = pair.Value;
				if (innerRegistry.ContainsKey(handle))
				{
					innerRegistry[handle] = default;
					return true;
				}
			}

			return false;
		}

		if (UnrealEngineStatics.IsInGameThread() && _isNotifing && Traverse(_deferredRegistry, handle))
		{
			return;
		}

		lock (_registryLock)
		{
			Traverse(_registry, handle);
		}
	}

	private EventLoopRegistration InternalRegister(EEventLoopTickingGroup group, EventLoopCallback callback, object? state, object? lifecycle)
	{
		if (UnrealEngineStatics.IsInGameThread() && _isNotifing)
		{
			return InternalRegisterTo(_deferredRegistry, group, callback, state, lifecycle);
		}

		lock (_registryLock)
		{
			return InternalRegisterTo(_registry, group, callback, state, lifecycle);
		}
	}

	private EventLoopRegistration InternalRegisterTo(Dictionary<EEventLoopTickingGroup, Dictionary<uint64, Rec>> registry, EEventLoopTickingGroup group, EventLoopCallback callback, object? state, object? lifecycle)
	{
		if (!registry.TryGetValue(group, out var innerRegistry))
		{
			innerRegistry = new();
			registry[group] = innerRegistry;
		}

		uint64 handle = _handle;
		EventLoopRegistration reg = new(this, _handle++);
		
		WeakReference? wr = null;
		Lifecycle lc = default;
		if (lifecycle is not null)
		{
			if (lifecycle is IExplicitLifecycle explicitLifecycle)
			{
				// @FIXME: Use lc.
				wr = new(lifecycle);
			}
			else if (lifecycle is Lifecycle valueLifecycle)
			{
				lc = valueLifecycle;
			}
			else
			{
				wr = new(lifecycle);
			}
		}
		
		innerRegistry[handle] = new(callback, state, wr, lc);

		return reg;
	}

	private void InternalUnregisterAll(object lifecycle)
	{
		static void Traverse(Dictionary<EEventLoopTickingGroup, Dictionary<uint64, Rec>> registry, object lifecycle)
		{
			foreach (var pair in registry)
			{
				var innerRegistry = pair.Value;
				foreach (var innerPair in innerRegistry)
				{
					if (innerPair.Value.WeakLifecycle?.Target == lifecycle)
					{
						innerRegistry[innerPair.Key] = default;
					}
				}
			}
		}

		if (UnrealEngineStatics.IsInGameThread() && _isNotifing)
		{
			Traverse(_deferredRegistry, lifecycle);
		}

		lock (_registryLock)
		{
			Traverse(_registry, lifecycle);
		}
	}

	private void FlushDeferredRegistry()
	{
		foreach (var pair in _deferredRegistry)
		{
			var innerRegistry = pair.Value;
			foreach (var innerPair in innerRegistry)
			{
				Rec rec = innerPair.Value;
				if (IsValidRec(rec))
				{
					// This method is only used by NotifyEvent which already acquires lock, so here we don't lock again.
					RegisterDeferred(pair.Key, innerPair.Key, rec);
				}
			}
			innerRegistry.Clear();
		}
	}
	
	private void RegisterDeferred(EEventLoopTickingGroup group, uint64 handle, in Rec rec)
	{
		if (!_registry.TryGetValue(group, out var innerRegistry))
		{
			innerRegistry = new();
			_registry[group] = innerRegistry;
		}

		innerRegistry[handle] = rec;
	}

	private bool IsValidRec(in Rec rec)
	{
		if (rec.Callback is null)
		{
			return false;
		}

		if (rec.WeakLifecycle is not null && rec.WeakLifecycle.Target is not null)
		{
			return true;
		}

		return rec.Lifecycle.IsExpired;
	}
	
	private readonly record struct Rec(EventLoopCallback? Callback, object? State, WeakReference? WeakLifecycle, Lifecycle Lifecycle);
	
	private static EventLoop _singleton = new();
	
	private double _worldAccumulatedTime;
	private double _realAccumulatedTime;
	
	private object _registryLock = new();
	private Dictionary<EEventLoopTickingGroup, Dictionary<uint64, Rec>> _registry = new();
	private Dictionary<EEventLoopTickingGroup, Dictionary<uint64, Rec>> _deferredRegistry = new();

	private bool _isNotifing;

	private uint64 _handle;
	
}



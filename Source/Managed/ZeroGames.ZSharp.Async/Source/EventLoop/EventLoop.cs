// Copyright Zero Games. All Rights Reserved.

namespace ZeroGames.ZSharp.Async;

internal class EventLoop : IEventLoop
{

	public EventLoopRegistration Register(EEventLoopTickingGroup group, EventLoopCallback callback, object? state, Lifecycle lifecycle = default)
	{
		ThreadHelper.ValidateGameThread();
		if (lifecycle.IsExpired)
		{
			return default;
		}
		
		return InternalRegister(group, callback, state, lifecycle);
	}

	public void Unregister(EventLoopRegistration registration)
	{
		ThreadHelper.ValidateGameThread();
		if (registration == default || registration.Owner != this)
		{
			return;
		}
		
		InternalUnregister(registration);
	}

	public void UnregisterAll(Lifecycle lifecycle)
	{
		ThreadHelper.ValidateGameThread();
		InternalUnregisterAll(lifecycle);
	}

	public bool IsValidRegistration(EventLoopRegistration registration)
	{
		ThreadHelper.ValidateGameThread();
		if (registration == default || registration.Owner != this)
		{
			return false;
		}
		
		return InternalIsValidRegistration(registration);
	}

	private bool InternalIsValidRegistration(EventLoopRegistration registration)
	{
		static bool Traverse(Dictionary<EEventLoopTickingGroup, Dictionary<EventLoopRegistration, Rec>> registry, EventLoopRegistration registration, ref bool value)
		{
			foreach (var pair in registry)
			{
				foreach (var p in pair.Value)
				{
					if (p.Key == registration)
					{
						value = IsValidRec(p.Value);
						return true;
					}
				}
			}

			return false;
		}

		bool valid = false;
		if (_notifing && Traverse(_deferredRegistry, registration, ref valid))
		{
			return valid;
		}

		Traverse(_registry, registration, ref valid);
		return valid;
	}

	internal static EventLoop Get() => _singleton;
	
	internal void NotifyEvent(EEventLoopTickingGroup group, TimeSpan worldDeltaTime, TimeSpan realDeltaTime, TimeSpan worldElapsedTime, TimeSpan realElapsedTime)
	{
		_notifing = true;
			
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
				EventLoopRegistration stale = default;
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
				if (stale.IsValid)
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
			_notifing = false;
		}
	}
	
	internal void InternalUnregister(EventLoopRegistration registration)
	{
		static bool Traverse(Dictionary<EEventLoopTickingGroup, Dictionary<EventLoopRegistration, Rec>> registry, EventLoopRegistration reg)
		{
			foreach (var pair in registry)
			{
				var innerRegistry = pair.Value;
				if (innerRegistry.ContainsKey(reg))
				{
					innerRegistry[reg] = default;
					return true;
				}
			}

			return false;
		}

		if (!_notifing || !Traverse(_deferredRegistry, registration))
		{
			Traverse(_registry, registration);
		}
	}

	private static bool IsValidRec(in Rec rec)
	{
		if (rec.Callback is null)
		{
			return false;
		}

		return !rec.Lifecycle.IsExpired;
	}
	
	private EventLoopRegistration InternalRegister(EEventLoopTickingGroup group, EventLoopCallback callback, object? state, Lifecycle lifecycle)
	{
		return InternalRegisterTo(_notifing ? _deferredRegistry : _registry, group, callback, state, lifecycle);
	}

	private EventLoopRegistration InternalRegisterTo(Dictionary<EEventLoopTickingGroup, Dictionary<EventLoopRegistration, Rec>> registry, EEventLoopTickingGroup group, EventLoopCallback callback, object? state, Lifecycle lifecycle)
	{
		if (!registry.TryGetValue(group, out var innerRegistry))
		{
			innerRegistry = new();
			registry[group] = innerRegistry;
		}

		EventLoopRegistration reg = new(this, ++_handle);
		innerRegistry[reg] = new(callback, state, lifecycle);

		return reg;
	}

	private void InternalUnregisterAll(Lifecycle lifecycle)
	{
		static void Traverse(Dictionary<EEventLoopTickingGroup, Dictionary<EventLoopRegistration, Rec>> registry, Lifecycle lifecycle)
		{
			foreach (var pair in registry)
			{
				var innerRegistry = pair.Value;
				foreach (var innerPair in innerRegistry)
				{
					if (innerPair.Value.Lifecycle == lifecycle)
					{
						innerRegistry[innerPair.Key] = default;
					}
				}
			}
		}

		
		if (_notifing)
		{
			Traverse(_deferredRegistry, lifecycle);
		}

		Traverse(_registry, lifecycle);
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
	
	private void RegisterDeferred(EEventLoopTickingGroup group, EventLoopRegistration registration, in Rec rec)
	{
		if (!_registry.TryGetValue(group, out var innerRegistry))
		{
			innerRegistry = new();
			_registry[group] = innerRegistry;
		}

		innerRegistry[registration] = rec;
	}
	
	private readonly record struct Rec(EventLoopCallback? Callback, object? State, Lifecycle Lifecycle);
	
	private static EventLoop _singleton = new();
	
	private TimeSpan _worldAccumulatedTime;
	private TimeSpan _realAccumulatedTime;
	
	private Dictionary<EEventLoopTickingGroup, Dictionary<EventLoopRegistration, Rec>> _registry = new();
	private Dictionary<EEventLoopTickingGroup, Dictionary<EventLoopRegistration, Rec>> _deferredRegistry = new();

	private bool _notifing;

	private uint64 _handle;
	
}



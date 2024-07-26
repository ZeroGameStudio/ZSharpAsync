// Copyright Zero Games. All Rights Reserved.

namespace ZeroGames.ZSharp.Async;

public partial struct Lifecycle
{

	public static Lifecycle Explicit(IExplicitLifecycle explicitLifecycle)
	{
		Lifecycle_ExplicitLifecycle lifecycle = Lifecycle_ExplicitLifecycle.GetFromPool(explicitLifecycle);
		return new(lifecycle);
	}
	
}



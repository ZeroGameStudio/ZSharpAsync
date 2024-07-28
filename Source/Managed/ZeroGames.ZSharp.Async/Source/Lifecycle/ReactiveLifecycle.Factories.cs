// Copyright Zero Games. All Rights Reserved.

namespace ZeroGames.ZSharp.Async;

public partial struct ReactiveLifecycle
{

	public static ReactiveLifecycle ExpiredLifecycle => new(_inlineExpiredToken);

	public static ReactiveLifecycle Explicit(IExplicitLifecycle explicitLifecycle)
	{
		Lifecycle_ExplicitLifecycle lifecycle = Lifecycle_ExplicitLifecycle.GetFromPool(explicitLifecycle);
		return new(lifecycle);
	}
	
}



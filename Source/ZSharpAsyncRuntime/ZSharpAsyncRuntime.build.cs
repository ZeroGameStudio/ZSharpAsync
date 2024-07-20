// Copyright Zero Games. All Rights Reserved.

using UnrealBuildTool;

public class ZSharpAsyncRuntime : ModuleRules
{
	public ZSharpAsyncRuntime(ReadOnlyTargetRules Target) : base(Target)
	{
		PCHUsage = ModuleRules.PCHUsageMode.UseExplicitOrSharedPCHs;
		
		PublicIncludePaths.AddRange
		(
			new string[]
			{
			}
		);
		
		PrivateIncludePaths.AddRange
		(
			new string[]
			{
			}
		);
		
		PublicDependencyModuleNames.AddRange
		(
			new string[]
			{
				"Core",
			}
		);
		
		PrivateDependencyModuleNames.AddRange
		(
			new string[]
			{
				"CoreUObject",
				"Engine",
				
				"ZSharpCore",
			}
		);
		
		DynamicallyLoadedModuleNames.AddRange
		(
			new string[]
			{
			}
		);
	}
}

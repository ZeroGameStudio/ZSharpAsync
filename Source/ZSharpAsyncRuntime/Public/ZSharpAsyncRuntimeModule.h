// Copyright Zero Games. All Rights Reserved.

#pragma once

#include "CoreMinimal.h"
#include "Modules/ModuleInterface.h"
#include "Modules/ModuleManager.h"

class IZSharpAsyncRuntimeModule : public IModuleInterface
{
public:
	static FORCEINLINE IZSharpAsyncRuntimeModule& Get()
	{
		static IZSharpAsyncRuntimeModule& Singleton = FModuleManager::LoadModuleChecked<IZSharpAsyncRuntimeModule>("ZSharpAsyncRuntime");
		return Singleton;
	}

	static FORCEINLINE bool IsAvailable()
	{
		return FModuleManager::Get().IsModuleLoaded("ZSharpAsyncRuntime");
	}
};

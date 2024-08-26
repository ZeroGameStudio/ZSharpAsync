// Copyright Zero Games. All Rights Reserved.

#include "ZSharpAsyncRuntimeModule.h"

#include "ALC/IZMasterAssemblyLoadContext.h"
#include "CLR/IZSharpClr.h"
#include "Interop/ZSharpEventLoop_Interop.h"

class FZSharpAsyncRuntimeModule : public IZSharpAsyncRuntimeModule
{
	// Begin IModuleInterface
	virtual void StartupModule() override;
	virtual void ShutdownModule() override;
	// End IModuleInterface
};

IMPLEMENT_MODULE(FZSharpAsyncRuntimeModule, ZSharpAsyncRuntime)

void FZSharpAsyncRuntimeModule::StartupModule()
{
	static void** managedFunctions[] =
	{
#define ADDRESS_OF(Pointer) reinterpret_cast<void**>(&Pointer)

		ADDRESS_OF(ZSharp::FZSharpEventLoop_Interop::GNotifyEvent),
				
#undef ADDRESS_OF
	};
		
	static struct
	{
		void*** ManagedFunctions = managedFunctions;
	} args{};
	
	ZSharp::IZSharpClr::Get().RegisterMasterAlcLoadFrameworks(ZSharp::FZOnMasterAlcLoadFrameworks::FDelegate::CreateLambda([](ZSharp::IZMasterAssemblyLoadContext* alc)
	{
		alc->LoadAssembly(ZSHARP_ASYNC_ASSEMBLY_NAME, &args);
	}));
}

void FZSharpAsyncRuntimeModule::ShutdownModule()
{
	ZSharp::IZSharpClr::Get().UnregisterMasterAlcLoadFrameworks(this);
}

// Copyright Zero Games. All Rights Reserved.

using System.Runtime.InteropServices;

[assembly: DllEntry(typeof(ZeroGames.ZSharp.Async.DllEntry))]

namespace ZeroGames.ZSharp.Async;

internal static class DllEntry
{

    [StructLayout(LayoutKind.Sequential)]
    private struct Args
    {
        public unsafe void*** ManagedFunctions;
    }
    
    [DllMain]
    private static unsafe void DllMain(Args* args) => Uncaught.FatalIfUncaught(() =>
    {
        int32 offset = 0;
        
        *args->ManagedFunctions[offset++] = (delegate* unmanaged<EEventLoopTickingGroup, float, float, double, double, void>)&EventLoop_Interop.NotifyEvent;

        return 0;
    }, -1);

}



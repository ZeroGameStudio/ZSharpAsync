// Copyright Zero Games. All Rights Reserved.

namespace ZeroGames.ZSharp.Async.ZeroTask;

// These exist only for ensuring that the implement class matches specific pattern.
internal interface IAwaitable<out TAwaiter> where TAwaiter : struct, IAwaiter
{
	TAwaiter GetAwaiter();
}

internal interface IAwaitableVoid<out TAwaiter> : IAwaitable<TAwaiter> where TAwaiter : struct, IAwaiterVoid;

internal interface IAwaitable<TResult, out TAwaiter> : IAwaitable<TAwaiter> where TAwaiter : struct, IAwaiter<TResult>;



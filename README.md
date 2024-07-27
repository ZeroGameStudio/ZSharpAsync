# ZSharpAsync
ZSharpAsync 是 ZSharp 的扩展插件，实现了一套适用于虚幻引擎的异步模型。插件包含三个程序集，分别提供了基于**事件循环**和**定时器**的传统异步模型，async/await 集成，以及 AsyncEnumerable+Linq 集成。

## ZeroGames.ZSharp.Async
提供最基本的**事件循环**和**定时器**，以及其他构建高级抽象必须的组件。该程序集是 ZSharpAsync 的基础组件，必须开启。

@TODO

## ZeroGames.ZSharp.Async.ZeroTask
提供一种新的任务类型 ZeroTask，可以对其像原生任务类型一样使用 async/await 语法。其行为类似于 ValueTask，但也有一些关键的差异：

**相同点**

1. 本体都是 0GC 的，只有异步操作真正发生时才会去对象池里申请堆上的对象。
2. 都可以通过实现后台任务的接口来定制异步操作，ValueTask 对应 IValueTaskSource，而 ZeroTask 对应 IUnderlyingZeroTask。
3. 都是消耗品，即只能等待一次；都不支持阻塞等待，即直接调用 Result 接口；都有 Preserve() 接口可以转换为可多次使用的对象。

**不同点**

1. ValueTask 绝大多数时候是基于线程池的多线程异步任务，而 ZeroTask 是基于事件循环的单线程异步任务。
2. ValueTask 会传递 SynchronizationContext 和 ExecutionContext，而 ZeroTask 则不考虑这些。
3. ValueTask 不支持组合操作，而 ZeroTask 有类似于 Task 的 ContinueWith、WhenAll、WhenAny 等操作。

**为什么要有 ZeroTask**

原生的 Task 使用线程池进行调度，而 UObject 不是线程安全的，与引擎交互的代码大部分都需要跑在虚幻引擎的 Game Thread（主线程）。这就要求我们有一套针对于主线程的异步模型。同时，使用 Task 会频繁分配堆内存，对于游戏这种高度并行的系统来说，这些频繁的内存分配带来的 GC 压力是肉眼可见的。
ZeroTask 并不阻止用户使用其他任务类型。对于不与引擎交互的代码，如纯托管侧的计算，仍然可以使用 Task 或 ValueTask 将任务指派给其他线程来提高效率。
需要注意的是，ZeroTask 是单线程的异步任务，只支持在主线程运行，在其他线程运行 ZeroTask 将会抛出异常。这是一个折中方案，一方面 ZeroTask 确实有在其他线程使用的潜在需求；另一方面，处理线程问题会影响性能。综合考量，我们认为 ZeroTask 绝大多数时候都只在主线程运行，为一些极端需求牺牲整体性能和开发成本是不值得的，因此我们在设计上直接将 ZeroTask 限制在了主线程。
## 内置的异步操作
```C#
//////////////////////////////// 事件循环与时间相关
// 可以将任务调度到事件循环的下一次特定事件。
await ZeroTask.Yield(EEventLoopTickingGroup.PostPhysicsTick); // 下一次 PostPhysicsTick 继续执行

// 可以将任务调度到下一帧事件循环的特定事件。
await ZeroTask.NextFrame(EEventLoopTickingGroup.PrePhysicsTick); // 下一帧的 PostPhysicsTick 继续执行

// 可以将任务延迟一定时间。
// 基于游戏时间延迟，游戏暂停不计时，主线程阻塞时可能失真，用于对可靠性要求不高的场景。
await ZeroTask.Delay(1000); // 世界时间流逝 1000 ms 后继续执行

// 基于真实时间延迟，游戏暂停也计时，主线程阻塞时可能失真，用于对可靠性要求不高的场景。
await ZeroTask.RealDelay(1000); // 现实时间流逝 1000 ms 后继续执行

// 对可靠性要求高的场景需要使用原生计时器。此时任务会被调度到线程池而不是主线程，同时这也是保证高精度的必要条件。
await Task.Delay(1000); // 现实时间流逝 1000 ms 后继续执行

// 可以将任务延迟一定帧数
await ZeroTask.DelayFrame(60); // 60帧之后继续执行

// 可以将任务延迟到指定时间
// 使用世界时间
await ZeroTask.UntilWorldTime(1000); // 世界时间 1000 ms 时继续执行

// 使用真实世界时间
await ZeroTask.UntilRealWorldTime(1000); // 世界的真实时间 1000 ms 时继续执行

// 使用真实时间
await ZeroTask.UntilTime(DateTime.Now + TimeSpan.FromMilliseconds(1000)); // 真实时间 1000 ms 后继续执行

//////////////////////////////// 逻辑相关
// 可以等待一个谓词第一次为真。谓词第一次为真时，如果在主线程，则直接完成，否则使用 ZSharpSynchronizationContext 调度到主线程。
await ZeroTask.Predicate(() => player.IsDead); // 玩家死亡时继续执行
await ZeroTask.Predicate(p => p.Health < 100, player); // 玩家血量小于100时继续执行

// 可以等待一个值第一次发生变化。第一次观测到值变化时，如果在主线程，则直接完成，否则使用 ZSharpSynchronizationContext 调度到主线程。
await ZeroTask.ValueChanged(() => gameState.NumPlayers); // 玩家数变化时继续执行
await ZeroTask.ValueChanged(p => p.Score, playerState); // 玩家分数变化时继续执行

// 可以等待一次委托触发。委托触发时，如果在主线程，则直接完成，否则使用 ZSharpSynchronizationContext 调度到主线程。
await ZeroTask.Delegate(gameState.OnVictory); // 游戏胜利时继续执行

// 可以等待一次事件触发。事件触发时，如果在主线程，则直接完成，否则使用 ZSharpSynchronizationContext 调度到主线程。
await ZeroTask.Event(h => player.OnDead += h, h => player.OnDead -= h); // 玩家死亡时继续执行
await ZeroTask.Event((m, h) => m.OnDamaged += h, (m, h) => m.OnDamaged -= h, monster); // 怪物被攻击时继续执行

// 如果希望以上任务调度到事件循环，可以使用 .ContinueWith(() => Task.Yield(...))。

//////////////////////////////// 引擎相关
// 可以等待一个虚幻动态多播委托触发。触发时如果在主线程，则完成，否则失败。（这是因为虚幻的动态委托是非线程安全的）
await ZeroTask.UnrealMulticastDelegate(btn.OnClicked); // 按钮被点击时继续执行

// 可以等待一个资源异步加载完成。内部使用 LatentAction，加载完成后，通过 LatentActionManager 调度到主线程。
SoftObjectPtr<UTexture2D> ptr = ...;
await ptr.LoadAsync(); // 贴图加载完成后继续执行

// 可以等待一个 CVar 相关的谓词第一次为真。谓词第一次为真时，如果在主线程，则直接完成，否则使用 ZSharpSynchronizationContext 调度到主线程。
await ZeroTask.CVar("t.maxfps", value => int.Parse(value) < 10); // 帧率小于10时继续执行

//////////////////////////////// 组合
// 可以对任务进行串联
await ZeroTask.Until(() => player.IsDead).Delay(3000); // 玩家死亡三秒后继续执行
// 等价于以下写法
await ZeroTask.Until(() => player.IsDead).ContinueWith(() => ZeroTask.Delay(3000));
// 等价于以下写法
await ZeroTask.Until(() => player.IsDead);
await ZeroTask.Delay(3000);

// 可以对任务进行并联
// 全部成功时成功，有一个失败时失败
SoftObjectPtr<UWorld> ptr = ...;
await ZeroTask.All(ptr.LoadAsync(), ZeroTask.Delay(3000)); // 地图加载完成且超过三秒后继续执行，常用于 Loading 图保底覆盖时间
// 等价于以下写法
await (ptr.LoadAsync(), ZeroTask.Delay(3000)); // 可以等待 ZeroTask 的元组

// 有一个成功时成功，全部失败时失败
SoftObjectPtr<UNiagaraSystem> fx = ...;
SoftObjectPtr<UNiagaraSystem> alterFx = ...;
await ZeroTask.Any(fx.LoadAsync(), alterFx.LoadAsync()); // 两个特效谁先加载完就用谁

// 有一个成功/失败时成功/失败
await ZeroTask.Race(ptr.LoadAsync(), ZeroTask.Delay(1000).FromCancelled()); // 地图在一秒内加载完成继续执行，否则超时失败

```

## ZeroGames.ZSharp.Async.Enumerable
提供将各种形式的事件转换成异步数据流的机制，并通过集成 Linq 实现对事件的流式操作。

@TODO



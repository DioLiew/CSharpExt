namespace CSharpExt;

/// <summary>
/// RunAsync would cause Object reference not set to an instance of an object if the A is unit, where K<Eff, Unit> failed to infer as Eff<Unit>.
/// </summary>
public partial class Prelude {
   /// <summary>
   /// task involvement only if flag is fulfilled.
   /// </summary>
   /// <param name="flag"></param>
   /// <param name="task"></param>
   /// <returns></returns>
   public static async Task @ifTrue(bool flag, Task task) =>
      await (
         not(flag) ? new Task(() => { }) : task
      );

   /// <summary>
   /// task involvement only if flag is fulfilled.
   /// </summary>
   /// <param name="flag"></param>
   /// <param name="task"></param>
   /// <returns></returns>
   public static async Task @ifTrue(bool flag, ValueTask task) =>
      await (
         not(flag) ? new ValueTask() : task
      );

   // /// <summary>
   // /// Portray fail if produced Fin is in failure state.
   // /// </summary>
   // /// <typeparam name="A"></typeparam>
   // /// <param name="ma"></param>
   // /// <param name="fail"></param>
   // /// <param name="timeout"></param>
   // /// <param name="envIO"></param>
   // /// <returns></returns>
   // public static async Task<A> runAsync<A>(K<Eff, A> ma, Func<Error, A> fail, TimeSpan? timeout = null, EnvIO? envIO = null) =>
   //    timeout is null
   //    ? envIO is null ? ifFail(await ma.RunAsync(), e => fail(e)) : ifFail(await ma.RunAsync(envIO), e => fail(e))
   //    : envIO is null ? ifFail(await ma.TimeoutIO(timeout.Value).RunAsync(), e => fail(e)) : ifFail(await ma.TimeoutIO(timeout.Value).RunAsync(envIO), e => fail(e));

   /// <summary>
   /// Portray fail if produced Fin is in failure state.
   /// </summary>
   /// <typeparam name="A"></typeparam>
   /// <param name="ma"></param>
   /// <param name="fail"></param>
   /// <param name="timeout"></param>
   /// <param name="envIO"></param>
   /// <returns></returns>
   public static Task<A> runAsync<A>(K<Eff, A> ma, Func<Error, A> fail, TimeSpan? timeout = null, EnvIO? envIO = null) where A : notnull =>
      timeout is null
      ? envIO is null ? ma.RunAsync().Map(x => x.IfFail(fail)) : ma.RunAsync(envIO).Map(x => x.IfFail(fail))
      : envIO is null ? ma.TimeoutIO(timeout.Value).RunAsync().Map(x => x.IfFail(fail)) : ma.TimeoutIO(timeout.Value).RunAsync(envIO).Map(x => x.IfFail(fail));





   // public static async Task<A> runAsync<A>(Eff<A> ma, Func<Error, A> fail, TimeSpan? timeout = null, EnvIO? envIO = null) =>
   //    timeout is null
   //    ? envIO is null
   //      ? ifFail(await ma.RunAsync(), e => fail(e))
   //      : ifFail(await ma.RunAsync(envIO), e => fail(e))
   //    : envIO is null
   //      ? ifFail(await ma.TimeoutIO(timeout.Value).RunAsync(), e => fail(e))
   //      : ifFail(await ma.TimeoutIO(timeout.Value).RunAsync(envIO), e => fail(e));

   /// <summary>
   /// Portray alternative if produced Fin is in failure state.
   /// </summary>
   /// <typeparam name="A"/>
   /// <param name="ma"/>
   /// <param name="alternative"/>
   /// <paramref name="timeout"/>
   /// <paramref name="envIO"/>
   /// <returns></returns>
   public static Task<A> runAsync<A>(K<Eff, A> ma, A alternative, TimeSpan? timeout = null, EnvIO? envIO = null) where A : notnull =>
      timeout is null
      ? envIO is null ? ma.RunAsync().Map(x => x.IfFail(alternative)) : ma.RunAsync(envIO).Map(x => x.IfFail(alternative))
      : envIO is null ? ma.TimeoutIO(timeout.Value).RunAsync().Map(x => x.IfFail(alternative)) : ma.TimeoutIO(timeout.Value).RunAsync(envIO).Map(x => x.IfFail(alternative));

   // /// <summary>
   // /// Portray fail if produced Fin<A> is in failure state. This usage is beyond deterministic.
   // /// </summary>
   // /// <typeparam name="A"/>
   // /// <param name="ma"/>
   // /// <param name="trigger"/>
   // /// <paramref name="timeout"/>
   // /// <paramref name="envIO"/>
   // /// <returns></returns>
   // public static async Task<Unit> runAsync<A>(K<Eff, A> ma, Action<Error> trigger, TimeSpan? timeout = null, EnvIO? envIO = null) where A : notnull =>
   //    timeout is null
   //    ? envIO is null
   //       ? ifFail(await ma.RunAsync(), trigger)
   //       : ifFail(await ma.RunAsync(envIO), trigger)
   //    : envIO is null
   //      ? ifFail(await ma.TimeoutIO(timeout.Value).RunAsync(), trigger)
   //      : ifFail(await ma.TimeoutIO(timeout.Value).RunAsync(envIO), trigger);

   /// <summary>
   /// Invoke the lift asynchronously.
   /// </summary>
   /// <typeparam name="A"></typeparam>
   /// <param name="ma"></param>
   /// <returns></returns>
   [Obsolete(message: "Use AsTask instead.")]
   public static Task<A> runAsync<A>(Lift<A> ma) where A : notnull =>
      Task.Run(ma.Function);

   /// <summary>
   /// Task is being lifted as Eff.
   /// Continuity of Task<B> if Option<A> is in Some state and ignore B.
   /// Portray fail if Fin<B> is in failure state.
   /// </summary>
   /// <typeparam name="A"></typeparam>
   /// <typeparam name="B"></typeparam>
   /// <param name="option"></param>
   /// <param name="callBack"></param>
   /// <param name="fail"></param>
   /// <returns></returns>
   // public static async Task runSome<A, B>(Option<A> option, Func<A, Task<B>> callBack, Action<Error> fail) =>
   //    await match(
   //       option: option,
   //       None: () => Task.CompletedTask,
   //       Some: async a => _ = ifFail(await liftEff(() => callBack(a)).Run().AsTask(), e => fail(e))
   //    );
   public static Task runSome<A, B>(Option<A> option, Func<A, Task<B>> callBack, Action<Error> fail) =>
      match(
         option,
         Some: a => _ = liftEff(() => callBack(a)).Run().IfFail(fail).AsTask(),
         None: () => Task.CompletedTask
      );

   /// <summary>
   /// Run only if Option<A> is in Some state and ignore B.
   /// Portray fail if Fin<B> is in failure state.
   /// </summary>
   /// <typeparam name="A"></typeparam>
   /// <typeparam name="B"></typeparam>
   /// <param name="option"></param>
   /// <param name="callBack"></param>
   /// <param name="fail"></param>
   /// <returns></returns>
   // public static Task runSome<A, B>(Option<A> option, Func<A, Eff<B>> callBack, Action<Error> fail) =>
   //    match(
   //       option: option,
   //       None: () => Task.CompletedTask,
   //       Some: async a => _ = ifFail(await callBack(a).RunAsync(), e => fail(e))
   //    );
   public static Task runSome<A, B>(Option<A> option, Func<A, Eff<B>> callBack, Action<Error> fail) =>
      match(
         option: option,
         Some: a => _ = callBack(a).RunAsync().Map(x => x.IfFail(fail)),
         None: () => Task.CompletedTask)
      ;

   /// <summary>
   /// Invoke all tasks amp; complete when all have completed.
   /// </summary>
   /// <param name="tasks"></param>
   /// <returns></returns>
   [Obsolete(message: "Use Task.WhenAll instead.")]
   public static async Task run(params Task[] tasks) => await Task.WhenAll(tasks);
}
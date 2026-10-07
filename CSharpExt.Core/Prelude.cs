namespace CSharpExt;

public partial class Prelude {
   /// <summary>
   /// Append a into list, or create new list with a only if list is null.
   /// </summary>
   /// <typeparam name="A"></typeparam>
   /// <param name="subjects"></param>
   /// <param name="a"></param>
   /// <returns></returns>
   [Pure]
   public static IList<A> append<A>(IList<A>? subjects, A a) where A : notnull =>
      [.. Optional(subjects).IfNone([]), a];

   /// <summary>
   /// Append rhs into lhs, or use rhs if lhs is null.
   /// </summary>
   /// <typeparam name="A"></typeparam>
   /// <param name="lhs"></param>
   /// <param name="rhs"></param>
   /// <returns></returns>
   public static IList<A> append<A>(IList<A>? lhs, IList<A> rhs) where A : notnull =>
      [.. Optional(lhs).IfNone([]), .. rhs];

   /// <summary>
   /// Cast to A by defining the cast function. It should be used like cast<A>(() => x as A) to prevent system throwing exception.
   /// </summary>
   /// <typeparam name="A"></typeparam>
   /// <param name="f"></param>
   /// <returns></returns>
   [Pure]
   public static Option<A> cast<A>(Func<A?> f) where A : class? =>
       Optional(f());

   /// <summary>
   /// An action does nothing.
   /// </summary>
   [Pure]
   public static Action dismiss =>
      () => { };

   /// <summary>
   /// Invoke all actions sequentially.
   /// </summary>
   /// <param name="actions"></param>
   /// <returns></returns>
   public static Unit invoke(params Action[] actions) =>
      iter(actions, x => x.Invoke());

   /// <summary>
   /// Invoke all actions parallelly.
   /// </summary>
   /// <param name="options"></param>
   /// <param name="actions"></param>
   /// <returns></returns>
   public static Unit invoke(ParallelOptions options, params Action[] actions) {
      Parallel.Invoke(options, actions);
      return unit;
   }

   /// <summary>
   /// Sequantially invoke the actions only if flag is true, else ignore actions.
   /// </summary>
   /// <param name="flag"></param>
   /// <param name="actions"></param>
   /// <returns></returns>
   public static Unit invokeIf(bool flag, params Action[] actions) =>
      not(flag) ? unit : invoke(actions);

   /// <summary>
   /// Parallely invoke the actions only if flag is true, else ignore actions.
   /// </summary>
   /// <param name="flag"></param>
   /// <param name="options"></param>
   /// <param name="actions"></param>
   /// <returns></returns>
   public static Unit invokeIf(bool flag, ParallelOptions options, params Action[] actions) =>
      not(flag) ? unit : invoke(options, actions);

   /// <summary>
   /// Convert a to a keyvaluepairs.
   /// </summary>
   /// <typeparam name="A"></typeparam>
   /// <param name="a"></param>
   /// <returns></returns>
   public static IEnumerable<KeyValuePair<string, object?>> keyValuePairs<A>(A a) where A : class? =>
      typeof(A).GetProperties().AsIterable().Map(x => KeyValuePair.Create(x.Name, x.GetValue(a)));

   /// <summary>
   /// MultiCast actions.
   /// </summary>
   /// <param name="actions"></param>
   /// <returns></returns>
   public static Action mCast(params Action[] actions) =>
      actions.Reduce((x, s) => x + s);

   /// <summary>
   /// A variant of io Run.
   /// </summary>
   /// <typeparam name="A"></typeparam>
   /// <param name="io"></param>
   /// <param name="timeout"></param>
   /// <param name="envIO"></param>
   /// <returns></returns>
   public static A run<A>(K<IO, A> io, TimeSpan? timeout = null, EnvIO? envIO = null) where A : notnull =>
      timeout is null
      ? envIO is null
         ? io.Run() : io.Run(envIO)
      : envIO is null
         ? io.TimeoutIO(timeout.Value).Run() : io.TimeoutIO(timeout.Value).Run(envIO);

   /// <summary>
   /// Invoking monad action if flag is true with failover fail.
   /// </summary>
   /// <param name="flag"></param>
   /// <param name="ma"></param>
   /// <param name="fail"></param>
   /// <returns></returns>
   public static Unit runWhen(bool flag, Eff<Unit> ma, Func<Error, Action> fail) =>
      when(flag, ma).Run().IfFail(e => fail(e));

   /// <summary>
   /// Same behaviour with C# built in SequenceEquals except it compare nullable list.
   /// </summary>
   /// <typeparam name="A"></typeparam>
   /// <param name="lhs"></param>
   /// <param name="rhs"></param>
   /// <returns></returns>
   [Pure]
   public static bool seqEquals<A>(IList<A>? lhs, IList<A>? rhs) =>
      (lhs is null && rhs is null)
      || (lhs?.Count == rhs?.Count && (lhs?.SequenceEqual(rhs ?? []) ?? false));

   /// <summary>
   /// Get the typeof(A).Name in title case.
   /// </summary>
   /// <typeparam name="A"></typeparam>
   /// <returns></returns>
   [Pure]
   public static string titleCase<A>() =>
      string.Concat(
         from x in typeof(A).Name
         select char.IsUpper(x) ? " " + x.ToString() : x.ToString()
      ).TrimStart(' ');
}
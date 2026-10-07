namespace CSharpExt;

public partial class Prelude {
   /// <summary>
   /// Invokes monad action only if flag is true with failover fail.
   /// </summary>
   /// <param name="flag"></param>
   /// <param name="ma"></param>
   /// <param name="fail"></param>
   /// <returns></returns>
   public static Task runWhen(bool flag, K<Eff, Unit> ma, Action<Error> fail) =>
      when(flag, ma).RunAsync().Map(x => x.IfFail(fail));

   /// <summary>
   /// Invokes monad action only if flag is true with failover fail.
   /// </summary>
   /// <param name="flag"></param>
   /// <param name="ma"></param>
   /// <param name="fail"></param>
   /// <returns></returns>
   public static Task runWhen(bool flag, ValueTask<Unit> ma, Action<Error> fail) =>
      when(flag, liftEff(async () => await ma)).RunAsync().Map(x => x.IfFail(fail));
}
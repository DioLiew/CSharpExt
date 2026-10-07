namespace CSharpExt;

public partial class Prelude {
   /// <summary>
   /// Initialize new instance of lifted A and inject proper actions to it.
   /// </summary>
   /// <typeparam name="A"></typeparam>
   /// <param name="create"></param>
   /// <param name="injects"></param>
   /// <returns></returns>
   [Obsolete(message: "Use lift instead.")]
   public static Lift<A> init<A>(Func<A> create, params Action<A>[] injects) where A : notnull =>
      lift(create).Map(a => {
         injects.Reduce((s, x) => s + x)(a);
         return a;
      });

   /// <summary>
   /// lift a function with actions applied.
   /// </summary>
   /// <typeparam name="A"></typeparam>
   /// <param name="function"></param>
   /// <param name="actions"></param>
   /// <returns></returns>
   public static Lift<A> lift<A>(Func<A> function, params Action<A>[] actions) where A : notnull =>
      // LanguageExt.Prelude.lift(function)
      //                    .Map(a => {
      //                       actions.Reduce((s, x) => s + x)(a);
      //                       return a;
      //                    });
      actions.Length == 0 ? LanguageExt.Prelude.lift(function)
      : LanguageExt.Prelude.lift(
         () => {
            var a = function();
            actions.Reduce((s, x) => s + x)(a);
            return a;
         }
      );

   /// <summary>
   /// Lift all actions.
   /// </summary>
   /// <param name="actions"></param>
   /// <returns></returns>
   public static Lift<Unit> lift(params Action[] actions) =>
      LanguageExt.Prelude.lift(() => iter(actions, x => x()));
}
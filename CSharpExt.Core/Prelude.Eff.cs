namespace CSharpExt;

public partial class Prelude {
   /// <summary>
   /// Construct a failed effect with failMessage.
   /// </summary>
   /// <typeparam name="A"></typeparam>
   /// <param name="failMessage"></param>
   /// <returns></returns>
   public static Eff<A> FailEff<A>(string failMessage) =>
      LanguageExt.Prelude.FailEff<A>(Error.New(failMessage));

   /// <summary>
   /// Propagates operation only if flag is true.
   /// </summary>
   /// <param name="flag"></param>
   /// <param name="operation"></param>
   /// <returns></returns>
   public static Eff<Unit> @ifTrue(bool flag, Eff<Unit> operation) =>
      not(flag) ? unitEff : operation;

}
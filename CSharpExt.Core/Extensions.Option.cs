namespace CSharpExt;

public static partial class Extensions {
   /// <summary>
   /// Convertion from Option to Eff, with a specific none Error.
   /// </summary>
   /// <typeparam name="A"></typeparam>
   /// <param name="self"></param>
   extension<A>(Option<A> self) {
      public Eff<A> ToEff(Error noneError) => liftEff(
         () => ifNone(
            option: self,
            None: () => raise<A>(noneError)
         )
      );
   }
}

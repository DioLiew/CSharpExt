namespace CSharpExt;

public static partial class Extensions {
   /// <summary>
   /// Monad Flatten.
   /// </summary>
   /// <typeparam name="A"></typeparam>
   /// <typeparam name="M"></typeparam>
   /// <param name="self"></param>
   extension<A, M>(K<M, K<M, A>> self) where M : Monad<M> {
      public K<M, A> Flatten() => self.Bind(x => x);
   }

   extension<A>(K<Option, A> self) {
      /// <summary>
      /// Conversion from Option to Eff, with a specific none Error.
      /// </summary>
      /// <param name="noneMessage"></param>
      /// <returns></returns>
      public K<Eff, A> ToEff(Error noneError) => liftEff(
         () => ifNone(
            option: self.As(),
            None: () => raise<A>(noneError)
         )
      ).Kind();
   }

   /// <summary>
   /// Lift an IO into Eff monad.
   /// </summary>
   /// <typeparam name="A"></typeparam>
   /// <param name="self"></param>
   extension<A>(K<IO, A> self) {
      public Eff<A> ToEff() => Eff<A>.LiftIO(self.As());
   }
}
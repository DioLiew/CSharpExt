namespace CSharpExt;

public static partial class Extensions {
   extension(Task self) {
      /// <summary>
      /// Lift self into an Eff.
      /// </summary>
      /// <returns></returns>
      public Eff<Unit> AsEff() => liftEff(self.ToUnit);

      /// <summary>
      /// Lift self into an Eff with a specific fail Error.
      /// </summary>
      /// <param name="fail"></param>
      /// <returns></returns>
      public K<Eff, Unit> AsEff(Error fail) => liftEff(self.ToUnit).Catch(_ => FailEff<Unit>(fail));

      /// <summary>
      /// Lift self into IO.
      /// </summary>
      /// <returns></returns>
      public IO<Unit> AsIO() => liftIO(() => self);
   }

   extension<A>(Task<A> self) {
      /// <summary>
      /// Lift self into Eff.
      /// </summary>
      /// <returns></returns>
      public Eff<A> ToEff() => liftEff(() => self);

      /// <summary>
      /// Lift self into IO.
      /// </summary>
      /// <returns></returns>
      public IO<A> ToIO() => liftIO(() => self);
   }

   extension<A>(Task<IEnumerable<A>> self) {
      /// <summary>
      /// Lift self into Eff.
      /// </summary>
      /// <returns></returns>
      public Eff<Iterable<A>> ToEff() => liftEff(
         () => self.Map(x => x.AsIterable())
      );

      /// <summary>
      /// Lift self into IO.
      /// </summary>
      /// <returns></returns>
      public IO<Iterable<A>> ToIO() => liftIO(
         () => self.Map(x => x.AsIterable())
      );
   }

   extension<A>(Task<A?> self) {
      /// <summary>
      /// Lift self into Eff with a specific null Error.
      /// </summary>
      /// <param name="nullError"></param>
      /// <returns></returns>
      public Eff<A> ToEff(Error nullError) => liftEff(
         () => self.Map(
            x => x is not null ? x : raise<A>(nullError)
         )
      );

      /// <summary>
      /// Lift self into IO with a specific nullError.
      /// </summary>
      /// <param name="nullError"></param>
      /// <returns></returns>
      public IO<A> ToIO(Error nullError) => liftIO(
         () => self.Map(
            x => x is not null ? x : raise<A>(nullError)
         )
      );
   }
}

namespace CSharpExt;

public partial class Prelude {
   readonly static Func<FieldInfo?, DescriptionAttribute[]?> attributes = fieldInfo =>
      fieldInfo is null ?
         null
         : fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

   [Pure]
   public static ImmutableSortedDictionary<A, string?> enumsSorted<A>() where A : Enum =>
      map(
         Enum.GetValues(typeof(A)).Cast<A>(),
         x => (Key: x, Description: enumDesc(x))
      )
      .ToImmutableSortedDictionary(
         x => x.Key, x => x.Description
      );

   /// <summary>
   /// The return is A instead of A. Unable to use in MudSelect where it required A?.
   /// </summary>
   /// <typeparam name="A"></typeparam>
   /// <returns></returns>
   [Pure]
   public static ImmutableList<(A?, string?)> enums<A>() where A : Enum =>
      [
         .. Enum.GetValues(typeof(A)).Cast<A?>().Select(x => (Key: x, Description: enumDesc(x)))
      ];

   [Pure]
   public static string enumDesc<A>(string enumString) where A : Enum =>
      not(Enum.TryParse(typeof(A), enumString, false, out object? t)) ? enumString
      : attributes(t!.GetType().GetField(((A)t).ToString()))?.First().Description ?? enumString;

   [Pure]
   static string? enumDesc<A>(A? a) where A : Enum =>
      fun(
         (FieldInfo? f) => f?.GetCustomAttribute<DescriptionAttribute>()?.Description
      )(a?.GetType().GetField(a.ToString()));

   [Pure]
   public static IEnumerable<A> enumList<A>() where A : Enum =>
      Enum.GetValues(typeof(A)).Cast<A>();
}
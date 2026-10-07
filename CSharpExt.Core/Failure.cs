namespace CSharpExt;

public record class Failure(int Code, string Message, [property: DataMember] string? Instance = default, [property: DataMember] string? TraceId = default): Expected(Message, Code) {
   public Failure() : this(Code: default, Message: string.Empty, Instance: default, TraceId: default) { }

   /// <summary>
   /// Create an `Expected` Error.
   /// </summary>
   /// <param name="code"></param>
   /// <param name="message"></param>
   /// <returns></returns>
   [Pure]
   public static Error Fail(int code, string message) =>
      New(code, message);

   /// <summary>
   /// Create a Failure, a variant of `Expected` Error carries with additional instance and traceid.
   /// </summary>
   /// <param name="code"></param>
   /// <param name="message"></param>
   /// <param name="instance"></param>
   /// <param name="traceId"></param>
   /// <returns></returns>
   [Pure]
   public static Failure Fail(int code, string message, string? instance = default, string? traceId = default) =>
      new(code, message, instance, traceId);
}
using CSharpExt.Net;
using System.Net;

namespace CSharpExt;

using static CSharpExt.Failure;
using static CSharpExt.Prelude;

public static partial class Extensions {
   const string charSet = "utf-8";
   const string jsonType = "application/json";

   static readonly JsonSerializerOptions serializerOptions = new() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };

   static bool matchError(Error e, IList<int>? errorCodes) =>
      e.Exception.Bind(
         ex => Optional(
            ex as HttpRequestException
         ).Map(x => (int?)x.StatusCode)
      ).Match(
         None: () => false,
         Some: code => errorCodes?.Any(x => x == code) ?? false
      );

   extension<A>(HttpClient http)
   {
      /// <summary>
      /// Standard GET method where request content as json.
      /// </summary>
      /// <param name="uri"></param>
      /// <param name="replied"></param>
      /// <param name="authnHeader"></param>
      /// <param name="recurrence"></param>
      /// <param name="ct"></param>
      /// <returns></returns>
      public IO<A> GetAsJson(Uri uri, Func<HttpResponseMessage, K<IO, A>> replied, AuthenticationHeaderValue? authnHeader = null, Recurrence? recurrence = null, CancellationToken ct = default) => retryWhile(
         recurrence is null ? Schedule.Once : Schedule.recurs(recurrence.Times) | Schedule.fibonacci(new Duration(recurrence.Delay)),
         bracketIO(
            from request in use(
               () => lift(
                  () => new HttpRequestMessage(HttpMethod.Get, uri),
                  x => x.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(jsonType)),
                  x => x.Headers.Authorization = authnHeader
               ).Function()
            )
            from response in use(IO.liftAsync(() => http.SendAsync(request, HttpCompletionOption.ResponseContentRead, ct)))
            from a in replied(response)
            select a
         ),
         e => matchError(e, recurrence?.ErrorCodes)
      );

      /// <summary>
      /// Standard PATCH method where request content as json.
      /// </summary>
      /// <param name="uri"></param>
      /// <param name="param"></param>
      /// <param name="replied"></param>
      /// <param name="authnHeader"></param>
      /// <param name="recurrence"></param>
      /// <param name="ct"></param>
      /// <returns></returns>
      public IO<A> PatchAsJson(Uri uri, object param, Func<HttpResponseMessage, K<IO, A>> replied, AuthenticationHeaderValue? authnHeader = null, Recurrence? recurrence = null, CancellationToken ct = default) => retryWhile(
         recurrence is null ? Schedule.Once : Schedule.recurs(recurrence.Times) | Schedule.fibonacci(new Duration(recurrence.Delay)),
         bracketIO(
            from content in use(() => JsonContent.Create(param, mediaType: new MediaTypeHeaderValue(jsonType) { CharSet = charSet }, options: serializerOptions))
            from request in use(
               () => lift(
                  () => new HttpRequestMessage(HttpMethod.Patch, uri),
                  x => x.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(jsonType)),
                  x => x.Headers.Authorization = authnHeader,
                  x => x.Content = content
               ).Function()
            )
            from response in use(IO.liftAsync(() => http.SendAsync(request, HttpCompletionOption.ResponseContentRead, ct)))
            from a in replied(response)
            select a
         ),
         e => matchError(e, recurrence?.ErrorCodes)
      );

      /// <summary>
      /// Standard POST method where request content as json.
      /// </summary>
      /// <param name="uri"></param>
      /// <param name="replied"></param>
      /// <param name="authnHeader"></param>
      /// <param name="param"></param>
      /// <param name="recurrence"></param>
      /// <param name="ct"></param>
      /// <returns></returns>
      public IO<A> PostAsJson(Uri uri, Func<HttpResponseMessage, K<IO, A>> replied, AuthenticationHeaderValue? authnHeader = null, object? param = null, Recurrence? recurrence = null, CancellationToken ct = default) => retryWhile(
         recurrence is null ? Schedule.Once : Schedule.recurs(recurrence.Times) | Schedule.fibonacci(new Duration(recurrence.Delay)),
         bracketIO(
            from content in use(() => JsonContent.Create(param, mediaType: new MediaTypeHeaderValue(jsonType) { CharSet = charSet }, options: serializerOptions))
            from request in use(
               () => lift(
                  () => new HttpRequestMessage(HttpMethod.Post, uri),
                  x => x.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(jsonType)),
                  x => x.Headers.Authorization = authnHeader,
                  x => x.Content = content
               ).Function()
            )
               // from response in use(IO.liftAsync(async () => await http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct)))
            from response in use(IO.liftAsync(() => http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct)))
            from a in replied(response)
            select a
         ),
         e => matchError(e, recurrence?.ErrorCodes)
      );

      /// <summary>
      /// Standard PUT method where request content as json.
      /// </summary>
      /// <param name="uri"></param>
      /// <param name="replied"></param>
      /// <param name="authnHeader"></param>
      /// <param name="param"></param>
      /// <param name="recurrence"></param>
      /// <param name="ct"></param>
      /// <returns></returns>
      public IO<A> PutAsJson(Uri uri, Func<HttpResponseMessage, K<IO, A>> replied, AuthenticationHeaderValue? authnHeader = null, object? param = null, Recurrence? recurrence = null, CancellationToken ct = default) => retryWhile(
         recurrence is null ? Schedule.Once : Schedule.recurs(recurrence.Times) | Schedule.fibonacci(new Duration(recurrence.Delay)),
         bracketIO(
            from content in use(() => JsonContent.Create(param, mediaType: new MediaTypeHeaderValue(jsonType) { CharSet = charSet }, options: serializerOptions))
            from request in use(
               () => lift(
                  () => new HttpRequestMessage(HttpMethod.Put, uri),
                  x => x.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(jsonType)),
                  x => x.Headers.Authorization = authnHeader,
                  x => x.Content = content
               ).Function()
            )
            from response in use(IO.liftAsync(() => http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct)))
            from a in replied(response)
            select a
         ),
         e => matchError(e, recurrence?.ErrorCodes)
      );

      /// <summary>
      /// Stand method to send HttpRequestMessage.
      /// </summary>
      /// <param name="requestMessage"></param>
      /// <param name="replied"></param>
      /// <param name="recurrence"></param>
      /// <param name="ct"></param>
      /// <returns></returns>
      public IO<A> Send(Lift<HttpRequestMessage> requestMessage, Func<HttpResponseMessage, K<IO, A>> replied, Recurrence? recurrence = null, CancellationToken ct = default) => retryWhile(
         recurrence is null ? Schedule.Once : Schedule.recurs(recurrence.Times) | Schedule.fibonacci(new Duration(recurrence.Delay)),
         bracketIO(
            from request in use(
               () => requestMessage.Function()
            )
            from response in use(
               IO.liftAsync(() => http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct))
            )
            from a in replied(response)
            select a
         ),
         e => matchError(e, recurrence?.ErrorCodes)
      );
   }

   extension<A>(HttpResponseMessage response)
   {
      /// <summary>
      /// Verify if IsSuccessStatusCode. Later Content.ReadFromJsonAsync.
      /// </summary>
      /// <param name="ct"></param>
      /// <returns></returns>
      public IO<A> ReadFromJson(CancellationToken ct = default) => response.IsSuccessStatusCode
      ? response.Content.ReadFromJsonAsync<A?>(ct).ToIO((Error)"Fail reading response")
      : response.Content.ReadFromJsonAsync<ProblemDetails?>(ct).ToIO((Error)"Fail reading ProblemDetails from response").Map(
         x => raise<A>(
            Fail(
               x.Status ?? 500,
               x.Detail ?? x.Extensions.Select(x => $"{x.Key} with error: {x.Value}").ToString() ?? "No extra detail."
            )
         )
      );
   }

   extension(HttpResponseMessage response)
   {
      /// <summary>
      /// Verify if IsSuccessStatusCode. Later Content.ReadAsStringAsync.
      /// </summary>
      /// <param name="ct"></param>
      /// <returns></returns>
      public K<IO, string> ReadAsString(CancellationToken ct = default) => response.IsSuccessStatusCode
      ? response.StatusCode == HttpStatusCode.NoContent
         ? IO.pure(string.Empty)
         : response.Content.ReadAsStringAsync(ct).ToIO().Catch(
            _ => raiseapp<string>("Fail reading response.")
         )
      : response.Content.ReadFromJsonAsync<ProblemDetails?>(ct).ToIO((Error)"Fail reading ProblemDetails from response.").Map(
         x => raise<string>(Fail(x.Status ?? 0, x.Detail ?? x.Title ?? string.Empty))
      );

      /// <summary>
      /// Validate if IsSuccessStatusCode is true, else throw ProblemDetails.
      /// </summary>
      /// <param name="ct"></param>
      /// <returns></returns>
      public IO<Unit> EnsureSuccess(CancellationToken ct = default) => response.IsSuccessStatusCode
      ? unitIO
      : response.Content.ReadFromJsonAsync<ProblemDetails?>(ct).ToIO((Error)"Fail reading ProblemDetails from resonse.").Map(
         x => raise<Unit>(
            Fail(
               x.Status ?? 500,
               x.Detail ?? x.Extensions.Select(x => $"{x.Key} with error: {x.Value}").ToString() ?? "No extra detail."
            )
         )
      );
   }
}

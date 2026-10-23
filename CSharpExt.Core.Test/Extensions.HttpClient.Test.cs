using LanguageExt;
using Xunit.Abstractions;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace CSharpExt.Core.Test;

using static LanguageExt.Prelude;
using static CSharpExt.Extensions;
using static CSharpExt.Prelude;


public class Extensions(ITestOutputHelper output) {
   [Fact]
   public Task DGS3_Authenticate() =>
      (
         from authn in bracketIO(
            from http in use(() => new HttpClient() { BaseAddress = new Uri("http://localhost:5124/") })
            from authn in http.PostAsJson(
               uri: new Uri("dgs3/api/authenticate", UriKind.Relative),
               request: x => x.Headers.Add("X-API-Version", "1.0"),
               param: new { UserName = "nobody", Password = "nopassword" },
               replied: x => x.ReadFromJson<Authn>()
            )
            select authn
         )
         select invoke(
            () => output.WriteLine(authn.ToString())
         )

      ).CatchIO(e => IO.lift(() => output.WriteLine(e.Message))).RunAsync().ToRef();

   [Fact]
   public Task SPV_Authenticate() =>
      (
         from authn in bracketIO(
            from http in use(() => new HttpClient() { BaseAddress = new Uri("http://localhost:5143/") })
            from authn in http.PostAsJson(
               uri: new Uri("spv/api/authenticate", UriKind.Relative),
               request: x => x.Headers.Add("X-API-Version", "1.0"),
               param: new { UserName = "nobody", Password = "nopassword" },
               replied: x => x.ReadFromJson<Authn>()
            )
            select authn
         )
         select invoke(
            () => output.WriteLine(authn.ToString())
         )

      ).CatchIO(e => IO.lift(() => output.WriteLine(e.Message))).RunAsync().ToRef();
}


public sealed record class Token {
   // #pragma warning disable RCS1139
   /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjRmNDZlNmZlLTkyNjAtNGEwMi04Nzc2LTQ3ZjY1OTAwMDczYSIsInVzZXJpZCI6IjE0NTA3MzI1IiwibmFtZSI6IkxJRVcgTElBTiBGQVRUIiwiZGVwYXJ0bWVudCI6IlRFU1QiLCJkZXNpZ25hdGlvbiI6IkVuZ2luZWVyIiwicm9sZSI6IlNZU1RFTS1BRE1JTklTVFJBVE9SIiwiZW1haWwiOiJMaWFuRmF0dEB2ZW50dXJlLmNvbS5zZyIsImp0aSI6ImNlNzk5NWQwLTkwY2YtNDRiOC1iMzg0LTIyMDk2YWI2MWE1NCIsInN1YiI6IjQ5YjBhYTBkLThmNDMtNDA3OS05ZDNmLTI4MWNlODFlYjdkYSIsInN3bmFtZSI6IkRHUzMiLCJuYmYiOjE3NDc4MTcyMjYsImV4cCI6MTc0NzgyMDgyNiwiaWF0IjoxNzQ3ODE3MjI2LCJpc3MiOiJodHRwOi8vbG9jYWxob3N0OjUxMjQiLCJhdWQiOiJodHRwOi8vbG9jYWxob3N0OjUxMjQifQ.rGw1hZRNblKqVIJgcvT-ebRN6Duv9zJTNpl5pLulw7C</example>
   [Required] public string Value { get; init; } = string.Empty;

   /// <example>2025-05-21T17:47:06+08:00 </example>
   [Required] public DateTimeOffset ExpiryDate { get; init; } = default;

   /// <example>2025-05-21T17:47:06+08:00 </example>
   [Required] public DateTimeOffset IssuedDate { get; init; } = default;
   // #pragma warning restore RCS1139
}

public sealed record class Authn(
   [property: JsonPropertyName("accessToken"), Required] Token Access,
   [property: JsonPropertyName("refreshToken"), Required] Token Refresh
);
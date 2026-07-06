using System.Net;
using System.Text;
using System.Web;

namespace Bff.IntegrationTests.Infrastructure;

/// <summary>Canned JSON responses shaped exactly like the real domain APIs (camelCase).</summary>
public static class DownstreamResponses
{
    public const string KnownBusinessId = "biz-1";

    public static HttpResponseMessage Json(string json, HttpStatusCode status = HttpStatusCode.OK)
        => new(status)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

    public static HttpResponseMessage NotFound()
        => new(HttpStatusCode.NotFound)
        {
            Content = new StringContent(
                """{"status":404,"message":"not found"}""", Encoding.UTF8, "application/json")
        };

    private static int PageSize(HttpRequestMessage req)
        => int.TryParse(HttpUtility.ParseQueryString(req.RequestUri!.Query)["pageSize"], out var n) ? n : 0;

    // ── Business Units domain ────────────────────────────────────────────────────
    public static HttpResponseMessage DefaultBusiness(HttpRequestMessage req)
    {
        var path = req.RequestUri!.AbsolutePath;

        // Counts (pageSize=1 envelope).
        if (path == "/api/v1/businesses" && PageSize(req) == 1)
            return Json(Paged(items: "[]", totalCount: 3));

        if (path == "/api/v1/business-units" && PageSize(req) == 1)
            return Json(Paged(items: "[]", totalCount: 11));

        // Business units nested under a business.
        if (path == $"/api/v1/businesses/{KnownBusinessId}/business-units")
            return Json(Paged(
                items: """
                [
                  {"id":"bu-1","businessId":"biz-1","razaoSocial":"Filial SP","nomeFantasia":"SP","cnpj":"22.222.222/0001-22","createdAt":"2024-01-01T00:00:00Z"},
                  {"id":"bu-2","businessId":"biz-1","razaoSocial":"Filial RJ","nomeFantasia":"RJ","cnpj":"33.333.333/0001-33","createdAt":"2024-01-02T00:00:00Z"}
                ]
                """,
                totalCount: 2));

        // Business detail.
        if (path == $"/api/v1/businesses/{KnownBusinessId}")
            return Json("""
                {"id":"biz-1","razaoSocial":"Acme SA","nomeFantasia":"Acme","cnpj":"11.111.111/0001-11","isActive":true,"createdAt":"2024-01-01T00:00:00Z","updatedAt":null}
                """);

        return NotFound();
    }

    // ── OAuth (Users) domain ─────────────────────────────────────────────────────
    public static HttpResponseMessage DefaultUsers(HttpRequestMessage req)
    {
        var path = req.RequestUri!.AbsolutePath;

        // User count.
        if (path == "/api/v1/users" && PageSize(req) == 1)
            return Json(Paged(items: "[]", totalCount: 2));

        // User enumeration (page of summaries).
        if (path == "/api/v1/users")
            return Json(Paged(
                items: """
                [
                  {"id":"u-1","email":"ana@acme.com","fullName":"Ana Lima","isActive":true,"isSuperAdmin":false,"createdAt":"2024-01-01T00:00:00Z"},
                  {"id":"u-2","email":"bob@acme.com","fullName":"Bob Reis","isActive":true,"isSuperAdmin":false,"createdAt":"2024-01-02T00:00:00Z"}
                ]
                """,
                totalCount: 2));

        // User detail — u-1 has a permission on biz-1; u-2 is scoped elsewhere.
        if (path == "/api/v1/users/u-1")
            return Json("""
                {"id":"u-1","email":"ana@acme.com","fullName":"Ana Lima","isActive":true,"isSuperAdmin":false,
                 "permissions":[
                   {"businessId":"biz-1","businessUnitId":null,"module":"Business","function":null,"role":"Writer"},
                   {"businessId":"biz-1","businessUnitId":null,"module":"Users","function":null,"role":"Reader"}
                 ]}
                """);

        if (path == "/api/v1/users/u-2")
            return Json("""
                {"id":"u-2","email":"bob@acme.com","fullName":"Bob Reis","isActive":true,"isSuperAdmin":false,
                 "permissions":[
                   {"businessId":"other-biz","businessUnitId":null,"module":"Business","function":null,"role":"Reader"}
                 ]}
                """);

        return NotFound();
    }

    private static string Paged(string items, long totalCount)
        => $$"""
            {"items":{{items}},"totalCount":{{totalCount}},"page":1,"pageSize":100,"totalPages":1,"hasNextPage":false,"hasPreviousPage":false}
            """;
}

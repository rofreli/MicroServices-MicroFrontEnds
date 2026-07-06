using Bff.API.Gateway;
using Bff.API.Middleware;
using Bff.API.Security;
using Bff.Application;
using Bff.Application.Abstractions;
using Bff.Infrastructure;
using Bff.Infrastructure.Options;
using Microsoft.OpenApi;
using OpenIddict.Validation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// ── Application + Infrastructure ────────────────────────────────────────────────
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

// ── Authentication: validate access tokens issued by the OAuth server ───────────
// Introspection is used (rather than local JWT validation) because OpenIddict
// encrypts access tokens by default — only the issuer can inspect them. The BFF is a
// confidential client and never trusts a request without a valid, live token.
builder.Services.AddOpenIddict()
    .AddValidation(options =>
    {
        options.SetIssuer(builder.Configuration["OpenIddict:Issuer"] ?? "http://localhost:5001/");
        options.AddAudiences(builder.Configuration["OpenIddict:Audience"] ?? "resource_server");

        options.UseIntrospection()
            .SetClientId(builder.Configuration["OpenIddict:ClientId"] ?? "resource_server")
            .SetClientSecret(builder.Configuration["OpenIddict:ClientSecret"] ?? "bff-secret");

        options.UseSystemNetHttp();
        options.UseAspNetCore();
    });

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
});
builder.Services.AddAuthorization(options =>
{
    // Every proxied domain route requires a valid token before the BFF forwards it.
    options.AddPolicy(GatewayConfig.ApiAccessPolicy, policy => policy.RequireAuthenticatedUser());
});

// ── Reverse proxy: the BFF is the single entry point to the inner services ──────
var downstream = builder.Configuration.GetSection(DownstreamOptions.SectionName).Get<DownstreamOptions>()
                 ?? new DownstreamOptions();
var (gatewayRoutes, gatewayClusters) = GatewayConfig.Build(downstream);
builder.Services.AddReverseProxy().LoadFromMemory(gatewayRoutes, gatewayClusters);

// ── MVC + Swagger ───────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "BFF API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
    });
    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", document, null),
            new List<string>()
        }
    });
});

builder.Services.AddCors(options =>
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins(
                builder.Configuration.GetSection("Cors:Origins").Get<string[]>()
                ?? new[] { "http://localhost:3000" })
            .AllowAnyHeader()
            .AllowAnyMethod()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BFF API v1"));
}

app.UseMiddleware<ExceptionMiddleware>();

// OpenIddict validation is the default auth scheme, so UseAuthentication runs on every
// request and — for form POSTs — reads the body looking for an `access_token` parameter.
// That consumes the stream and leaves nothing for the reverse proxy to forward (login and
// the OIDC token exchange would break). Buffer the body up front and rewind it before the
// proxy so both the auth handler and YARP can read it.
app.Use(async (context, next) =>
{
    context.Request.EnableBuffering();
    await next();
});

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    if (context.Request.Body.CanSeek)
        context.Request.Body.Position = 0;
    await next();
});

// Custom aggregation controllers (specific attribute routes win over the proxy catch-all).
app.MapControllers();
// Everything else under the gateway routes is forwarded to the inner services.
app.MapReverseProxy();

app.Run();

// Exposed so the integration-test WebApplicationFactory can bootstrap the real pipeline.
public partial class Program { }

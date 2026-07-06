using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.OpenApi;
using OAuth.API.Middleware;
using OAuth.Application;
using OAuth.Infrastructure;
using OAuth.Infrastructure.Persistence;
using OpenIddict.Abstractions;

var builder = WebApplication.CreateBuilder(args);

// ── Application + Infrastructure ──────────────────────────────────────────────
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// ── Authentication ─────────────────────────────────────────────────────────────
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/account/login";
        options.LogoutPath = "/account/logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);

        // Behind the BFF gateway the request Host is the internal service name
        // (oauth-api:8080). The cookie handler builds ABSOLUTE redirect URLs from that
        // host, which the browser cannot reach. Emit host-relative redirects instead so
        // the browser follows them on the public gateway origin (localhost:5002).
        static string ToRelative(string uri)
            => Uri.TryCreate(uri, UriKind.Absolute, out var u) ? u.PathAndQuery : uri;

        options.Events.OnRedirectToLogin = ctx => { ctx.Response.Redirect(ToRelative(ctx.RedirectUri)); return Task.CompletedTask; };
        options.Events.OnRedirectToLogout = ctx => { ctx.Response.Redirect(ToRelative(ctx.RedirectUri)); return Task.CompletedTask; };
        options.Events.OnRedirectToAccessDenied = ctx => { ctx.Response.Redirect(ToRelative(ctx.RedirectUri)); return Task.CompletedTask; };
    })
    .AddCookie("ExternalCookie", options =>
    {
        options.Cookie.Name = "external_auth";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
    })
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Google:ClientId"] ?? "CONFIGURE_GOOGLE_CLIENT_ID";
        options.ClientSecret = builder.Configuration["Google:ClientSecret"] ?? "CONFIGURE_GOOGLE_CLIENT_SECRET";
        options.SignInScheme = "ExternalCookie";
        options.CallbackPath = "/account/callback-google";
    });

// ── OpenIddict ─────────────────────────────────────────────────────────────────
var mongoContext = builder.Services.BuildServiceProvider().GetRequiredService<MongoDbContext>();

builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseMongoDb().UseDatabase(mongoContext.Database);
    })
    .AddServer(options =>
    {
        options
            .SetAuthorizationEndpointUris("/connect/authorize")
            .SetTokenEndpointUris("/connect/token")
            .SetUserInfoEndpointUris("/connect/userinfo")
            .SetEndSessionEndpointUris("/connect/logout")
            .SetIntrospectionEndpointUris("/connect/introspect");

        options
            .AllowAuthorizationCodeFlow()
            .RequireProofKeyForCodeExchange()
            .AllowRefreshTokenFlow();

        options.RegisterScopes(
            OpenIddictConstants.Scopes.OpenId,
            OpenIddictConstants.Scopes.Email,
            OpenIddictConstants.Scopes.Profile,
            "roles",
            "api");

        if (builder.Environment.IsDevelopment())
            options.AddDevelopmentEncryptionCertificate().AddDevelopmentSigningCertificate();
        else
        {
            // In production, load from certificate store or file
            options.AddEphemeralEncryptionKey().AddEphemeralSigningKey();
        }

        options.UseAspNetCore()
            .EnableAuthorizationEndpointPassthrough()
            .EnableTokenEndpointPassthrough()
            .EnableUserInfoEndpointPassthrough()
            .EnableEndSessionEndpointPassthrough()
            .DisableTransportSecurityRequirement(); // remove in production (HTTPS only)
    })
    .AddValidation(options =>
    {
        options.UseLocalServer();
        options.UseAspNetCore();
    });

// ── MVC + Swagger ──────────────────────────────────────────────────────────────
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "OAuth API", Version = "v1" });
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

builder.Services.AddCors(opt =>
    opt.AddPolicy("AllowFrontend", p =>
        p.WithOrigins(
            builder.Configuration.GetSection("Cors:Origins").Get<string[]>()
            ?? new[] { "http://localhost:3000", "http://localhost:3001", "http://localhost:3002" })
         .AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OAuth API v1"));
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseStaticFiles();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

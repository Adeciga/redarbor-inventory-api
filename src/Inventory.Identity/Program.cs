using Inventory.Identity.Persistence;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AuthDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("AuthDb"));
    options.UseOpenIddict();
});

builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
               .UseDbContext<AuthDbContext>();
    })
    .AddServer(options =>
    {
        options.SetTokenEndpointUris("/connect/token");

        // ✅ OAuth2 Client Credentials
        options.AllowClientCredentialsFlow();

        // ⭐ Opcional para “completo”: Authorization Code + PKCE (Swagger login)
        // options.SetAuthorizationEndpointUris("/connect/authorize");
        // options.AllowAuthorizationCodeFlow().RequireProofKeyForCodeExchange();

        options.RegisterScopes("inventory.read", "inventory.write");

        options.AddDevelopmentEncryptionCertificate()
               .AddDevelopmentSigningCertificate();

        options.UseAspNetCore()
               .EnableTokenEndpointPassthrough();

        // Si habilitas /connect/authorize:
        // .EnableAuthorizationEndpointPassthrough();
    })
    .AddValidation(options =>
    {
        options.UseLocalServer();
        options.UseAspNetCore();
    });

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

await SeedOAuthClientAsync(app);

app.Run();

static async Task SeedOAuthClientAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

    // Client para client_credentials
    if (await manager.FindByClientIdAsync("redarbor-client") is null)
    {
        await manager.CreateAsync(new OpenIddictApplicationDescriptor
        {
            ClientId = "redarbor-client",
            ClientSecret = "redarbor-secret",
            DisplayName = "Redarbor Inventory Client",
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                OpenIddictConstants.Permissions.Prefixes.Scope + "inventory.read",
                OpenIddictConstants.Permissions.Prefixes.Scope + "inventory.write"
            }
        });
    }
}

using FluentValidation;
using Inventory.Application;
using Inventory.Application.Behaviors;
using Inventory.Application.Categories;
using Inventory.Application.InventoryMovements;
using Inventory.Application.Products;
using Inventory.Infrastructure;
using Inventory.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// --------------------
// Persistence / Infra
// --------------------
builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddInfrastructure(builder.Configuration);

// --------------------
// App Services (por ahora)
// --------------------
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<InventoryMovementService>();

// --------------------
// MediatR + Validation Pipeline
// --------------------
builder.Services.AddMediatR(typeof(AssemblyReference).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(AssemblyReference).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// --------------------
// Middleware(s)
// --------------------
builder.Services.AddTransient<Inventory.Api.Middlewares.ExceptionHandlingMiddleware>();

// --------------------
// AuthN (OAuth2/OIDC via Authority - OpenIddict)
// --------------------
// En Docker: OAuth__Authority=http://inventory-identity:8080
// En local:  OAuth__Authority=http://localhost:5001  (si expones identity 5001:8080)
var authority = builder.Configuration["OAuth:Authority"] ?? "http://localhost:5001";

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = authority;
        options.RequireHttpsMetadata = false; // dev + docker (http)
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false, // por simplicidad en prueba técnica
        };
    });

builder.Services.AddAuthorization();

// --------------------
// Controllers + Swagger
// --------------------
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Inventory.Api", Version = "v1" });

    // Mantener bearer para pegar el access_token obtenido del /connect/token
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter: Bearer {access_token} (obtained from OAuth2 /connect/token)",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    options.AddSecurityDefinition("Bearer", securityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});

var app = builder.Build();

// --------------------
// HTTP pipeline
// --------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// OJO: tu middleware de excepciones debe ir ANTES de MapControllers,
// y típicamente lo ponemos antes/después de auth según lo que quieras capturar.
// Yo lo dejo después de auth para que 401/403 salgan normales, y capture domain/validation.
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<Inventory.Api.Middlewares.ExceptionHandlingMiddleware>();

app.MapControllers();
app.Run();

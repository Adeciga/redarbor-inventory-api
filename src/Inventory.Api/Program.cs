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
builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<InventoryMovementService>();
builder.Services.AddMediatR(typeof(Inventory.Application.AssemblyReference).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(Inventory.Application.AssemblyReference).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient<Inventory.Api.Middlewares.ExceptionHandlingMiddleware>();
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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<Inventory.Api.Middlewares.ExceptionHandlingMiddleware>();
app.MapControllers();
app.Run();

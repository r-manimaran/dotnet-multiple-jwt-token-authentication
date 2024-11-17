using Scalar.AspNetCore;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using dotnet_MultiJwtAuth;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(CustomAuthSchemes.Keyccloak, options =>
    {
        options.RequireHttpsMetadata = false;
        options.Audience = builder.Configuration["Authentication:Keycloak:Audience"];
        options.MetadataAddress = builder.Configuration["Authentication:Keycloak:MetadataAddress"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = builder.Configuration["Authentication:Keycloak:ValidIssuer"],
            // ValidateIssuer = true,
            // ValidateAudience = true,
            // ValidateLifetime = true,
            // ValidateIssuerSigningKey = true,
            // IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Authentication:Keycloak:Secret"]))
        };        
    })
    .AddJwtBearer(CustomAuthSchemes.Supabase, options => {
        byte[] bytes = Encoding.UTF8.GetBytes(builder.Configuration["Authentication:Supabase:JwtSecret"]!);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            // ValidateIssuer = false,
            // ValidateAudience = false,
            // ValidateLifetime = true,
            // ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Authentication:Supabase:ValidIssuer"],
            ValidAudience = builder.Configuration["Authentication:Supabase:ValidAudience"],
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(bytes)
        };
    });

builder.Services.AddAuthorization( options => {
    AuthorizationPolicy defaultPolicy = 
            new AuthorizationPolicyBuilder(CustomAuthSchemes.Keyccloak, CustomAuthSchemes.Supabase)
        .RequireAuthenticatedUser()
        .Build();
    options.DefaultPolicy = defaultPolicy;
});

builder.Services.AddTransient<IClaimsTransformation, MultiJwtClaimsTransformation>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseAuthorization();

app.MapControllers();

app.MapGet("users/me", (ClaimsPrincipal claimsPrincipal) =>{
    return claimsPrincipal.Claims.ToDictionary(c => c.Type, c => c.Value);
}).RequireAuthorization();


app.Run();

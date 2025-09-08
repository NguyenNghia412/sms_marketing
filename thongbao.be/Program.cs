using AutoMapper;
using Azure.Identity;
using Hangfire;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using thongbao.be.application.Auth.Implements;
using thongbao.be.application.Auth.Interfaces;
using thongbao.be.application.Base;
using thongbao.be.application.DiemDanh.Implements;
using thongbao.be.application.DiemDanh.Interfaces;
using thongbao.be.application.GuiTinNhan.Implements;
using thongbao.be.application.GuiTinNhan.Interfaces;
using thongbao.be.domain.Auth;
using thongbao.be.infrastructure.data;
using thongbao.be.infrastructure.data.Seeder;
using thongbao.be.infrastructure.external.BackgroundJob;
using thongbao.be.shared.Constants.Auth;
using thongbao.be.shared.Settings;
using thongbao.be.Workers;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine("EnvironmentName => " + builder.Environment.EnvironmentName);

#region db
string connectionString = builder.Configuration.GetConnectionString("SMS_MARKETING")
    ?? throw new InvalidOperationException("Không tìm thấy connection string \"SMS_MARKETING\" trong appsettings.json");

string hangfireConnectionString = builder.Configuration.GetConnectionString("HANGFIRE")
    ?? throw new InvalidOperationException("Không tìm thấy connection string \"HANGFIRE\" trong appsettings.json");

builder.Services.AddDbContext<SmDbContext>(options =>
{
    options.UseSqlServer(connectionString, options =>
    {
        //options.MigrationsAssembly(typeof(Program).Namespace);
        //options.MigrationsHistoryTable(DbSchemas.TableMigrationsHistory, DbSchemas.Core);
    });
    options.UseOpenIddict(); // Register OpenIddict entities
}, ServiceLifetime.Scoped);
#endregion

#region cors
string allowOrigins = builder.Configuration.GetSection("AllowedHosts")!.Value!;
//File.WriteAllText("cors.now.txt", $"CORS: {allowOrigins}");;/'\,
Console.WriteLine($"CORS: {allowOrigins}");
var origins = allowOrigins
    .Split(';')
    .Where(s => !string.IsNullOrWhiteSpace(s))
    .ToArray();
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        ProgramExtensions.CorsPolicy,
        builder =>
        {
            builder
                .WithOrigins(origins)
                .AllowAnyMethod()
                .AllowAnyHeader()
                //.AllowCredentials()
                .WithExposedHeaders("Content-Disposition");
        }
    );
});
#endregion

#region identity
// 2. Add Identity
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<SmDbContext>()
    .AddDefaultTokenProviders();
#endregion

#region auth
string secretKey = builder.Configuration.GetSection("AuthServer:SecretKey").Value!;
string googleClientId = builder.Configuration.GetSection("AuthServer:Google:ClientId").Value!;
string googleClientSecret = builder.Configuration.GetSection("AuthServer:Google:ClientSecret").Value!;
string googleRedirectUri = builder.Configuration.GetSection("AuthServer:Google:RedirectUri").Value!;

string msClientId = builder.Configuration.GetSection("AuthServer:MS:ClientId").Value!;
string msClientSecret = builder.Configuration.GetSection("AuthServer:MS:ClientSecret").Value!;
string msRedirectUri = builder.Configuration.GetSection("AuthServer:MS:RedirectUri").Value!;

builder.Services.Configure<AuthServerSettings>(builder.Configuration.GetSection("AuthServer"));
builder.Services.Configure<AuthGoogleSettings>(builder.Configuration.GetSection("AuthServer:Google"));
builder.Services.Configure<AuthMsSettings>(builder.Configuration.GetSection("AuthServer:MS"));


builder.Services.AddOpenIddict()
    .AddCore(opt =>
    {
        opt.UseEntityFrameworkCore()
           .UseDbContext<SmDbContext>();
    })
    // Register the OpenIddict server components.
    .AddServer(options =>
    {
        // Enable the token endpoint.
        options.SetTokenEndpointUris("connect/token")
            .SetAuthorizationEndpointUris("/connect/authorize")
        ;

        // Enable the client credentials flow.
        options.AllowClientCredentialsFlow()
                .AllowPasswordFlow()
                .AllowRefreshTokenFlow()
                .AllowAuthorizationCodeFlow()
                .RequireProofKeyForCodeExchange()
                ;

        options.AcceptAnonymousClients();
        options.DisableAccessTokenEncryption();

        options.RegisterScopes(OpenIddictConstants.Scopes.OpenId, OpenIddictConstants.Scopes.OfflineAccess, OpenIddictConstants.Scopes.Profile);

        // Register the signing and encryption credentials.
        options.AddDevelopmentEncryptionCertificate()
               .AddDevelopmentSigningCertificate();

        // 🔑 Symmetric signing key
        var secret = Encoding.UTF8.GetBytes(secretKey);
        options.AddEncryptionKey(new SymmetricSecurityKey(secret));
        options.AddSigningKey(new SymmetricSecurityKey(secret));

        // Register the ASP.NET Core host and configure the ASP.NET Core options.
        options.UseAspNetCore()
                .EnableAuthorizationEndpointPassthrough()
               .EnableTokenEndpointPassthrough()
               .DisableTransportSecurityRequirement();

    });

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
    .AddJwtBearer(
        options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,        // ✅ only check exp & nbf
                ClockSkew = TimeSpan.Zero, // no extra leeway

                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                // Symmetric key (HMAC) example
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(secretKey)
                ),

                // 👇 Accept both "JWT" and "at+jwt" as token types
                ValidTypes = new[] { "JWT", "at+jwt" }
            };
            options.RequireHttpsMetadata = false;
        }
    )
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddGoogle(options =>
    {
        options.ClientId = googleClientId;
        options.ClientSecret = googleClientSecret;
        options.ReturnUrlParameter = "redirect_uri";
        options.CallbackPath = googleRedirectUri;
    })
    .AddMicrosoftAccount(options =>
    {
        options.ClientId = msClientId;
        options.ClientSecret = msClientSecret;
    });

builder.Services.AddAuthorization();


builder.Services.AddHostedService<AuthWorker>();
#endregion

#region mapper
// Build mapper configuration
builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile));
#endregion

#region hangfire
builder.Services.ConfigureHangfire(hangfireConnectionString);
#endregion

// Add services to the container.
#region service
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IPermissionsService, PermissionsService>();
builder.Services.AddScoped<IChienDichService, ChienDichService>();
builder.Services.AddScoped<IHopTrucTuyenService, HopTrucTuyenService>();
#endregion

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<GraphServiceClient>(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();

    var options = new ClientSecretCredentialOptions
    {
        AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
    };

    var clientSecretCredential = new ClientSecretCredential(
        configuration["AzureAd:TenantId"],
        configuration["AzureAd:ClientId"],
        configuration["AzureAd:ClientSecret"],
        options);

    return new GraphServiceClient(clientSecretCredential);
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "My API", Version = "v1" });

    // 🔑 Add Bearer JWT Security Definition
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIs...\"",
    });

    // 🔐 Add Security Requirement (apply globally to all endpoints)
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

#region Seed data
// Run seeding inside scope
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    await SeedUser.SeedAsync(userManager, roleManager);

}
#endregion

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(ProgramExtensions.CorsPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseHangfireDashboard();

app.Run();

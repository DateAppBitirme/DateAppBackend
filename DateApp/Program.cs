using DateApp.Data;
using DateApp.Hubs;
using DateApp.Interfaces;
using DateApp.Models;
using DateApp.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using Azure.Identity;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "DateApp API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Lutfen gecerli token giriniz.",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

var keyVaultUri = builder.Configuration["KeyVault:VaultUri"];
if (!string.IsNullOrEmpty(keyVaultUri) && Uri.TryCreate(keyVaultUri, UriKind.Absolute, out var validUri))
{
    builder.Configuration.AddAzureKeyVault(validUri, new DefaultAzureCredential());
    Console.WriteLine($"Successfully loaded configuration from Key Vault: {validUri}");
}
else
{
    Console.WriteLine("Key Vault URI not found or invalid in configuration. Skipping Key Vault configuration.");
}

var connectionString = builder.Configuration["ConnectionStrings:DateAppConnection"];
// VEYA doğrudan Key Vault secret adıyla: builder.Configuration["DatabaseConnectionString"]
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

/*builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DateAppConnection")));*/

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 10;

    options.User.RequireUniqueEmail = true;
    
    options.Tokens.EmailConfirmationTokenProvider = "Default";
})  
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddSingleton<IActiveUserTracker, InMemoryActiveUserTracker>();

var signalRBuilder = builder.Services.AddSignalR();
var signalRConnectionString = builder.Configuration["SignalRConnectionString"]; // Key Vault'taki secret adı
if (!string.IsNullOrEmpty(signalRConnectionString))
{
    signalRBuilder.AddAzureSignalR(signalRConnectionString); // Azure SignalR Service kullanılıyorsa
    Console.WriteLine("Azure SignalR Service configured.");
}
else
{
    Console.WriteLine("SignalRConnectionString not found. Azure SignalR Service NOT configured.");
}

// CORS
/*
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});*/
var jwtSigningKey = builder.Configuration["JWT:SigningKey"];
if (string.IsNullOrWhiteSpace(jwtSigningKey))
    throw new InvalidOperationException("JWT signing key must not be null or empty.");

var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSigningKey));
builder.Services.AddSingleton(securityKey);


builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserBlockService, UserBlockService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
    options.DefaultChallengeScheme =
    options.DefaultForbidScheme =
    options.DefaultScheme =
    options.DefaultSignInScheme =
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = securityKey,
        RoleClaimType = "roles"
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            if (!string.IsNullOrEmpty(accessToken))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    options.AddPolicy("AdminPolicy", policy =>
    {
        policy.RequireAssertion(context =>
            context.User.HasClaim(c =>
                (c.Type == ClaimTypes.Role && c.Value == "admin")
            )
        );
    });

    options.AddPolicy("UserPolicy", policy =>
    {
        policy.RequireAssertion(context =>
            context.User.HasClaim(c =>
                (c.Type == ClaimTypes.Role && c.Value == "admin") ||
                (c.Type == ClaimTypes.Role && c.Value == "user")
            )
        );
    });
});


var app = builder.Build();

app.UseStaticFiles();


if (app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseRouting();


app.UseAuthentication();

app.UseAuthorization();

app.MapHub<ChatHub>("/chatHub");
app.MapHub<LocationChatHub>("/locationchathub");

app.MapControllers();

app.Run();

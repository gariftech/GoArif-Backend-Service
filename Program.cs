using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Goarif.Shared.ViewModels;
using RepositoryPattern.Services.AuthService;
using RepositoryPattern.Services.OtpService;
using SendingEmail;
using RepositoryPattern.Services.AttachmentService;
using RepositoryPattern.Services.ApiSettingService;
using RepositoryPattern.Services.TranscribeService;
using RepositoryPattern.Services.PromptService;

using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<IAttachmentService, AttachmentService>();
builder.Services.AddScoped<IApiSettingService, ApiSettingService>();
builder.Services.AddScoped<ITranscribeService, TranscribeService>();
builder.Services.AddScoped<IPromptService, PromptService>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ConvertJWT>();
builder.Services.AddSingleton<ValidationUserDto>();
builder.Services.AddSingleton<ValidationAuthDto>();
builder.Services.AddHttpClient();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}
).AddJwtBearer(options =>
{
    var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    IConfigurationRoot configuration = builder.Build();
    string secretKey = configuration.GetSection("AppSettings")["JwtKey"];

    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ClockSkew = TimeSpan.Zero,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "Goarif.com",
        ValidAudience = "Goarif.com",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)) // NOTE: THIS SHOULD BE A SECRET KEY NOT TO BE SHARED; A GUID IS RECOMMENDED
    };
});


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Goarif", Version = "v1" });
    c.OperationFilter<SwaggerFileOperationFilter>();
    // Define the "Bearer" security scheme
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer", // The scheme should be "bearer"
        BearerFormat = "JWT"
    });

    // Add the security requirement
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
        });
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 300 * 1024 * 1024; // 50 MB
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 300 * 1024 * 1024; // 200 MB
});

var app = builder.Build();

app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
    context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
    context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");
    await next();
});

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Blazor API V1");
    c.RoutePrefix = "api/v1/swagger";
});

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseStaticFiles();
app.Use(async (context, next) =>
    {
        await next();

        if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized) // 401
        {
            context.Response.ContentType = "application/json";

            var viewModel = new
            {
                code = 401,
                errorMessage = new ErrorDtoVM { error = MessageReport.Unauthorized }
            };
            var json = JsonSerializer.Serialize(viewModel);
            await context.Response.WriteAsync(json);
        }
    });
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.JsonMultipartFormDataSupport.Extensions;
using Swashbuckle.AspNetCore.JsonMultipartFormDataSupport.Integrations;
using System.Text;
using WordWeaver;
using WordWeaver.Attributes;
using WordWeaver.Data;
using WordWeaver.Services.Core.Interfaces;

#pragma warning disable CS8604

var builder = WebApplication.CreateBuilder(args);

Dependencies.RegisterServices(builder.Services);

DinkToPdfAll.LibraryLoader.Load();

builder.Services.AddControllers(options => {
    options.Filters.Add(new AuthorizeFilter(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build()));
});

/*builder.Services.AddMvc().AddJsonOptions(o => {
    o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});*/
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddJsonMultipartFormDataSupport(JsonSerializerChoice.Newtonsoft);
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddDbContext<WordWeaverContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Swagger for API documentation.
builder.Services.AddSwaggerGen(option => {
    option.SwaggerDoc("v1", new OpenApiInfo {
        Title = "WordWeaver",
        Version = "v1"
    });

    // Add security definition for JWT Bearer token.
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    // Add security requirement for JWT Bearer token.
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });

    option.SchemaFilter<SwaggerSkipPropertyFilter>();
});

// Configure CORS policy to allow requests from any origin.
builder.Services.AddCors(options => {
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder
            .WithOrigins("*")
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Configure JWT authentication with options.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ClockSkew = TimeSpan.Zero
        };
    });

var app = builder.Build();

// Configure exception handling middleware.
app.UseExceptionHandler(errorApp => {
    errorApp.Run(async context => {
        // Retrieve exception details
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        // Log the exception
        var errorLogger = context.RequestServices.GetRequiredService<ILoggerService>();
        await errorLogger.Error(exception, false);

        // Set response content type to plain text
        context.Response.ContentType = "text/plain";

        // Write response body with exception and headers
        var msg = $"Error: {exception.Message}\n\n";
        var headers = context.Request.Headers.Select(header => $"{header.Key}: {header.Value}");
        await context.Response.WriteAsync($"{msg}{exception}\n\nHEADERS\n=======\n{string.Join("\n", headers)}");
    });
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowSpecificOrigin");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

using Appel.SharpTemplate.Infrastructure.Application;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Appel.SharpTemplate.API.Configuration;

public static class ApiConfiguration
{
    public static IServiceCollection AddApiConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRazorPages();
        services.AddRouting(options => options.LowercaseUrls = true);
        services.AddOptions<AppSettings>().Bind(configuration.GetSection("AppSettings"));

        services
            .AddControllers()
            .AddFluentValidation(fv =>
            {
                fv.DisableDataAnnotationsValidation = true;
                fv.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
                fv.ValidatorOptions.PropertyNameResolver = CamelCasePropertyNameResolver.ResolvePropertyName;
            });

        services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(jwt =>
    {
        var key = Encoding.ASCII.GetBytes(configuration["AppSettings:AuthTokenSecretKey"]);

        jwt.SaveToken = true;
        jwt.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            RequireExpirationTime = false
        };
    });


        services
            .Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = new Dictionary<string, IEnumerable<string>>();

                    context.ModelState
                    .Select(ms => new
                    {
                        Key = ms.Key,
                        Value = ms.Value?.Errors.Select(x => x.ErrorMessage)
                    })
                    .ToList()
                    .ForEach(x => errors.Add(x.Key, x.Value));

                    return new BadRequestObjectResult(new
                    {
                        Title = "Invalid arguments to the API",
                        Status = 400,
                        Errors = errors
                    });
                };
            });

        services
            .AddCors(options =>
            {
                options.AddPolicy("Total",
                    builder =>
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader());
            });

        services
            .AddHttpsRedirection(options => { options.HttpsPort = 443; });

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });

        return services;
    }

    public static WebApplication UseApiConfiguration(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        if (app.Environment.IsProduction())
        {
            app.UseForwardedHeaders();
            app.UseHttpsRedirection();
        }

        app.UseExceptionHandler(c => c.Run(async context =>
        {
            var exception = context.Features
                .Get<IExceptionHandlerPathFeature>()!
                .Error;
            var response = new { error = exception.Message };
            await context.Response.WriteAsJsonAsync(response);
        }));

        app.UseCors("Total");
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapGraphQL();

        return app;
    }
}

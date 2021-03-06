using System;
using System.Collections.Generic;
using System.Security.Claims;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Authorization.Swagger
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(config =>
            {
                config.AddPolicy("DefaultPolicy",
                    builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Description = "Demo Swagger API v1",
                    Title = "Swagger with IdentityServer4",
                    Version = "1.0.0"
                });

                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Password = new OpenApiOAuthFlow
                        {
                            TokenUrl = new Uri("https://localhost:10001/connect/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                // {"SwaggerAPI", "Swagger API DEMO"}
                            }
                        }
                    }
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "oauth2"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
            })
                //.AddIdentityServerAuthentication(options =>
                //{
                //    options.ApiName = "SwaggerAPI";
                //    options.Authority = "https://localhost:10001";
                //    options.RequireHttpsMetadata = false;
                //});
                .AddJwtBearer("Bearer",
                     options =>
                     {
                         options.Authority = "https://localhost:10001";
                         options.Audience = "SwaggerAPI";
                         options.RequireHttpsMetadata = false;

                         options.TokenValidationParameters = new TokenValidationParameters()
                         {
                             ValidateAudience = false
                         };
                     });
            services.AddAuthorization();
            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("Administrator", builder =>
            //    {
            //        builder.RequireClaim(ClaimTypes.Role, "Administrator1");
            //    });
            //});

            services.AddControllers();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Swagger UI Demo");
                options.DocumentTitle = "Title";

                // ????????? ??????? ? launchSettings.json
                options.RoutePrefix = "docs";
                options.DocExpansion(DocExpansion.List);
                options.OAuthClientId("client_id_swagger");
                options.OAuthScopeSeparator(" ");
                options.OAuthClientSecret("client_secret_swagger");
            });

            app.UseRouting();
            app.UseCors("DefaultPolicy");


            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

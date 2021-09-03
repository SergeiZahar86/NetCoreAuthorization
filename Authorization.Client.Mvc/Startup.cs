using System;
using System.Security.Claims;
using Authorization.Client.Mvc.Infrastructure.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Authorization.Client.Mvc
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(config =>
                {
                    // используем два типа аутентификации
                    // "Cookies"
                    config.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    // "OpenIdConnect"
                    config.DefaultChallengeScheme= OpenIdConnectDefaults.AuthenticationScheme;
                })

                // "Cookies"
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                // "OpenIdConnect"
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, config =>
                {
                    // информаци€ дл€ сервера аутентификации
                    // где будем получать токен (сервер авторизации)
                    config.Authority = "https://localhost:10001";
                    
                    // информаци€ о себе как клиенте
                    config.ClientId = "client_id_mvc";
                    config.ClientSecret = "client_secret_mvc";
                    // сохран€ем токен полученный от серв.аут.
                    config.SaveTokens = true;
                    config.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                    };
                    
                    // способ аутентификации
                    config.ResponseType = "code";

                    config.Scope.Add("OrdersAPI");

                    // дл€ работы Refresh Token
                    config.Scope.Add("offline_access");

                    config.GetClaimsFromUserInfoEndpoint = true;

                    config.ClaimActions.MapJsonKey(ClaimTypes.DateOfBirth, ClaimTypes.DateOfBirth);
                });

            services.AddAuthorization(config =>
            {
                config.AddPolicy("HasDateOfBirth", builder =>
                {
                    builder.RequireClaim(ClaimTypes.DateOfBirth);
                });
            });

            services.AddSingleton<IAuthorizationHandler, OlderThanRequirementHandler>();
            services.AddSingleton<IAuthorizationPolicyProvider, CustomAuthorizationPolicyProvider>();

            services.AddHttpClient();

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name:"Default",
                    pattern: "{controller=Site}/{action=Index}/{id?}");
            });
        }
    }
}

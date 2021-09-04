using System.Collections.Generic;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using static IdentityServer4.IdentityServerConstants;

namespace Authorization.IdentityServer
{
    public static class IdentityServerConfiguration
    {
        /// <summary>
        /// Получает указанных клиентов (настройка подключенных клиентов)
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Client> GetClients() =>
        new List<Client>
        {
            new Client
            {
                ClientId = "client_blazor_web_assembly",
                RequireClientSecret = false,
                RequireConsent = false,
                RequirePkce = true,
                // указание типа авторизации
                AllowedGrantTypes =  GrantTypes.Code,
                AllowedCorsOrigins = { "https://localhost:8001" },
                PostLogoutRedirectUris = { "https://localhost:8001" },
                RedirectUris = { "https://localhost:8001/authentication/login-callback" },
                AllowedScopes =
                {
                    "blazor",
                    "OrdersAPI",
                    StandardScopes.OpenId,
                    StandardScopes.Profile
                }
            },
            new Client
            {
                ClientId = "client_id_js",
                RequireClientSecret = false,
                RequireConsent = false,
                RequirePkce = true,
                AllowedGrantTypes =  GrantTypes.Code,
                AllowedCorsOrigins = { "https://localhost:9001" },
                RedirectUris =
                { "https://localhost:9001/callback.html",
                    "https://localhost:9001/refresh.html" 
                },
                PostLogoutRedirectUris = { "https://localhost:9001/index.html" },
                AllowedScopes =
                {
                    "OrdersAPI",
                    StandardScopes.OpenId,
                    StandardScopes.Profile
                }
            },
            new Client
            {
                ClientId = "client_id_swagger",
                ClientSecrets = { new Secret("client_secret_swagger".ToSha256()) },
                AllowedGrantTypes =  GrantTypes.ResourceOwnerPassword,
                AllowedCorsOrigins = { "https://localhost:7001" },
                AllowedScopes =
                {
                    "SwaggerAPI",
                    StandardScopes.OpenId,
                    StandardScopes.Profile,
                    //"roles"
                },
                AlwaysIncludeUserClaimsInIdToken = true,
                UpdateAccessTokenClaimsOnRefresh = true
            },
            new Client
            {
                // имя клиента
                ClientId = "client_id",
                ClientSecrets = { new Secret("client_secret".ToSha256()) },
                // указание типа авторизации
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                // список scopes, разрешённых именно для данного клиентского приложения
                AllowedScopes =
                {
                    "OrdersAPI",
                    StandardScopes.OpenId,
                    StandardScopes.Profile
                }
            },
            new Client
            {
                ClientId = "client_id_mvc",
                ClientSecrets = { new Secret("client_secret_mvc".ToSha256()) },
                // указание типа авторизации
                AllowedGrantTypes = GrantTypes.Code,
                // список scopes, разрешённых именно для данного клиентского приложения
                AllowedScopes =
                {
                    // возможность обращаться к OrdersAPI
                    "OrdersAPI",
                    StandardScopes.OpenId,
                    StandardScopes.Profile
                },

                // указание того что с этого адреса будет перенаправление 
                // на контроллер авторизации сервера авторизации при попытке
                // достучаться на защищенные маршруты
                RedirectUris = {"https://localhost:2001/signin-oidc"},

                PostLogoutRedirectUris = {"https://localhost:2001/signout-callback-oidc"},

                // вывод информационной страницы после аутентификации
                RequireConsent = false,

                // установка жизни Access Token (секунды)
                AccessTokenLifetime = 5,

                // для работы Refresh Token
                AllowOfflineAccess = true

                // в IdToken мы включаем из userinfo
                // AlwaysIncludeUserClaimsInIdToken = true
            }
        };

        /// <summary>
        /// Получение (указание) API ресурсов которые могут взаимодействовать c 
        /// сервером авторизации. Название ресурсов указываются в GetClients() =>
        /// new Client => AllowedScopes
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ApiResource> GetApiResources()
        {
            //yield return new ApiResource("SwaggerAPI");
            //yield return new ApiResource("OrdersAPI");
            //yield return new ApiResource("roles", "My Roles", new[] { "role", "name" });
            return new List<ApiResource>
            {
                new ApiResource("SwaggerAPI"),
                new ApiResource("OrdersAPI"),
                //new ApiResource("roles", "My Roles", new[] { "role", "name" }),
                //new ApiResource(LocalApi.ScopeName, "Local Api", new [] { JwtClaimTypes.Role }),
            };
        }

        /// <summary> Запрос утверждений (Scopes) о пользователе </summary>
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            // сообщает провайдеру о необходимости возврата утверждения sub (идентификатора
            // субъекта) в токене идентификации.
            yield return new IdentityResources.OpenId();

            // представляет отображаемое имя, адрес электронной почты и утверждение веб-сайта и тд.
            yield return new IdentityResources.Profile();
            //yield return new IdentityResource
            //{
            //    Name = "roles",
            //    DisplayName = "Roles",
            //    UserClaims = { JwtClaimTypes.Role }
            //};
        }

        /// <summary>
        /// IdentityServer4 version 4.x.x changes
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ApiScope> GetApiScopes()
        {
            yield return new ApiScope("SwaggerAPI", "Swagger API");
            yield return new ApiScope("blazor", "Blazor WebAssembly");
            yield return new ApiScope("OrdersAPI", "Orders API");
        }
    }
}

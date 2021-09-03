using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Authorization.IdentityServer.Data
{
    public static class DatabaseInitializer
    {
        public static async Task InitAsync(IServiceProvider scopeServiceProvider)
        {
            var userManager = scopeServiceProvider.GetService<UserManager<IdentityUser>>();
            var roleManager = scopeServiceProvider.GetService<RoleManager<IdentityRole>>();

            //var result = userManager.CreateAsync(user, "123qwe").GetAwaiter().GetResult();
            //if (result.Succeeded)
            //{
            //    userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "Administrator")).GetAwaiter().GetResult();
            //    userManager.AddClaimAsync(user, new Claim(JwtClaimTypes.Scope, "OrdersAPI")).GetAwaiter().GetResult();
            //    userManager.AddClaimAsync(user, new Claim(JwtClaimTypes.Scope, "SwaggerAPI")).GetAwaiter().GetResult();
            //}

            string adminName = "admin";
            if (await roleManager.FindByNameAsync("Administrator") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("Administrator"));
            }
            if (await roleManager.FindByNameAsync("Employee") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("Employee"));
            }
            if (await userManager.FindByNameAsync(adminName) == null)
            {
                var admin = new IdentityUser
                {
                    UserName = adminName
                };
                IdentityResult result = await userManager.CreateAsync(admin, "123qwe");
                if (result.Succeeded)
                {


                    await userManager.AddToRoleAsync(admin, "Administrator");
                    userManager.AddClaimAsync(admin, new Claim(JwtClaimTypes.Scope, "SwaggerAPI")).GetAwaiter().GetResult();
                    userManager.AddClaimAsync(admin, new Claim(ClaimTypes.Role, "Administrator1")).GetAwaiter().GetResult();
                    userManager.AddClaimAsync(admin, new Claim(JwtClaimTypes.Scope, "OrdersAPI")).GetAwaiter().GetResult();

                }
            }























            //scopeServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

            //var context = scopeServiceProvider.GetRequiredService<ConfigurationDbContext>();
            //context.Database.Migrate();
            //if (!context.Clients.Any())
            //{
            //    foreach (var client in IdentityServerConfiguration.GetClients())
            //    {
            //        context.Clients.Add(client.ToEntity());
            //    }
            //    context.SaveChanges();
            //}

            //if (!context.IdentityResources.Any())
            //{
            //    foreach (var resource in IdentityServerConfiguration.GetIdentityResources())
            //    {
            //        context.IdentityResources.Add(resource.ToEntity());
            //    }
            //    context.SaveChanges();
            //}

            //if (!context.ApiResources.Any())
            //{
            //    foreach (var resource in IdentityServerConfiguration.GetApiResources())
            //    {
            //        context.ApiResources.Add(resource.ToEntity());
            //    }
            //    context.SaveChanges();
            //}
        }
    }
}
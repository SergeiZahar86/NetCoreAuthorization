using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authorization.IdentityServer.Data
{
    public class AddRole
    {
        public async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            IdentityResult adminRoleResult;
            bool adminRoleExists = await RoleManager.RoleExistsAsync("Administrator");

            if (!adminRoleExists)
            {
                adminRoleResult = await RoleManager.CreateAsync(new IdentityRole("Administrator"));
            }

            IdentityUser userToMakeAdmin = await UserManager.FindByNameAsync("User");
            await UserManager.AddToRoleAsync(userToMakeAdmin, "Administrator");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity
{
    public class IdentityDataSeed
    {
        public static async Task RolesSeed(RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole(AuthorizationConstants.Roles.ADMINISTRATOR));
            await roleManager.CreateAsync(new IdentityRole(AuthorizationConstants.Roles.CLIENT));
        } 
    }
}

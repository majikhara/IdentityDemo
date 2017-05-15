using IdentityDemo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemo.Data
{
    public class AdministratorSeedData
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdministratorSeedData(UserManager<ApplicationUser> userManager,  RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task EnsureSeedData()
        {
            //Check for existing admin user
            if (await _userManager.FindByEmailAsync("admin@identitydemo.com") == null)
            {
                //user was not found
                //1- create the admin role
                //2- create the admin user
                //3- add admin user to admin role

                //1- Create the admin role
                var adminRole = await _roleManager.FindByNameAsync("admin");
                if(adminRole == null)
                {
                    //Role did not exist - create it
                    adminRole = new IdentityRole("admin");
                    await _roleManager.CreateAsync(adminRole);
                }

                //2- Create the admin user
                ApplicationUser adminUser = new ApplicationUser
                {
                    UserName = "theMaster",
                    Email = "admin@identitydemo.com",
                    FirstName = "Emily",
                    LastName = "Allain",
                    Address = "1700 Mountain Road",
                    City = "Moncton",
                    Province = "NB",
                    PostalCode = "E1G1A6"
                };
                await _userManager.CreateAsync(adminUser, "Admin@123456");
                await _userManager.SetLockoutEnabledAsync(adminUser, false); //no lockout for admin

                //3- add admin user to admin role
                IdentityResult result = await _userManager.AddToRoleAsync(adminUser, "admin"); //can also do "var" instead of "IdentityResult"

            }

            //Add more roles here: 
            var userRole = await _roleManager.FindByNameAsync("user");
            if(userRole == null)
            {
                userRole = new IdentityRole("user");
                await _roleManager.CreateAsync(userRole);
            }
        }
    }
}

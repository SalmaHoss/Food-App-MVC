using FoodApp.Data.Static;
using FoodApp.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace FoodApp.Data
{
    public class AppDbInitializer
    {
        public static async Task SeedUsersAndRolesAsync(IApplicationBuilder applicationBuilder)
        {
            //services 
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {

                //Roles
                //RoleManger control roles
                //we pass Identity Role as it is the default but also like identity user we can add
                // a class that inhirt from it and extend its properties
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                //if admin role not exist in our database create admin role
                if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
                
                //if admin role not exist in our database create User Role
                if (!await roleManager.RoleExistsAsync(UserRoles.User))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

                //Users:admin
                //UserManger control Users
                //we pass ApplicationUSer (our custom User)
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                
                //admin Email
                string adminUserEmail = "admin@etickets.com";

                //create admin User if it is not exist in DB
                var adminUser = await userManager.FindByEmailAsync(adminUserEmail);
                if (adminUser == null)
                {
                    //create adminUser
                    //new AdminUser --> class form identityUser Created
                    var newAdminUser = new ApplicationUser()
                    {
                        FullName = "Admin User",
                        UserName = "admin-user",
                        Email = adminUserEmail,
                        EmailConfirmed = true
                    };
                    //add user to DB 
                    await userManager.CreateAsync(newAdminUser, "Coding@1234?");
                    
                    //add role (admin to the user) to DB 
                    await userManager.AddToRoleAsync(newAdminUser, UserRoles.Admin);
                }

                //Users: application Users
                string appUserEmail = "user@etickets.com";

                //check if user exist in DB
                //if not create new user
                //assign user role to the created user
                var appUser = await userManager.FindByEmailAsync(appUserEmail);
                if (appUser == null)
                {
                    var newAppUser = new ApplicationUser()
                    {
                        FullName = "Application User",
                        UserName = "app-user",
                        Email = appUserEmail,
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(newAppUser, "Coding@1234?");
                    await userManager.AddToRoleAsync(newAppUser, UserRoles.User);
                }
            }
        }
    }
}

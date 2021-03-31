using HotelReservationsManager.Data;
using HotelReservationsManager.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace doc_migs.Controllers
{
    public static class PrepDB
    {
        public static async Task Initialize(ApplicationDbContext ctx,
            UserManager<ApplicationUser> userMgr,
            RoleManager<ApplicationRole> roleMgr)
        {
            //var scope = app.ApplicationServices.CreateScope();
            //var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            //var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            //var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            Console.WriteLine("Updating db...");

            ctx.Database.Migrate();

            Console.WriteLine("Checking for admin role in db...");

            var adminRole = new ApplicationRole("admin");
            var writerRole = new ApplicationRole("user");
            if (!ctx.Roles.Any())
            {
                Console.WriteLine("No roles creating...");
                //create admin role 
                roleMgr.CreateAsync(adminRole).GetAwaiter().GetResult();
                roleMgr.CreateAsync(writerRole).GetAwaiter().GetResult();
            }

            Console.WriteLine("Checking for admin in db...");

            if (!ctx.Users.Any(u => u.UserName == "admin"))
            {

                Console.WriteLine("No admin creating...");

                var adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "a@a.a",
                    EmailConfirmed = true //the policy needs a confirmed email and this is admin acc
                    //name and email must be the same when creating this way idk why
                    //from
                    //https://stackoverflow.com/questions/43731437/how-to-make-a-single-admin-user-for-mvc-net-core-app
                    //look comments
                    /*
                     * I figured it out. Apparently whoever created the default AccountController Login action didn't notice that SignInPasswordAsync uses the Username and Password and not the Email and Password. I had to change that and change the LoginViewModel by removing the dependency attribute 'EmailAddres' – Joe Higley
                     */
                };

                userMgr.CreateAsync(adminUser, "W5c6@Q86s2uj").GetAwaiter().GetResult();
                //userMgr.CreateAsync(userNormal, "W5c6@Q86s2uj").GetAwaiter().GetResult();
                //add role to user
                await userMgr.AddToRoleAsync(adminUser, adminRole.Name);
                //userMgr.AddToRoleAsync(userNormal, writerRole.Name);
            }
        }
    }
}

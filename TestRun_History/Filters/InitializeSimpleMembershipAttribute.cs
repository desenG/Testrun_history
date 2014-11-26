using System;
using System.Web.Mvc;
using System.Threading;
using System.Data.Entity;
using TestRun_History.Models;
using System.Data.Entity.Infrastructure;
using WebMatrix.WebData;
using System.Web.Security;
namespace TestRun_History.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class InitializeSimpleMembershipAttribute : ActionFilterAttribute
    {
        private static SimpleMembershipInitializer _initializer;
        private static object _initializerLock = new object();
        private static bool _isInitialized;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Ensure ASP.NET Simple Membership is initialized only once per app start
            LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
        }

        private class SimpleMembershipInitializer
        {

            public SimpleMembershipInitializer()
            {
                Database.SetInitializer<UsersContext>(null);

                try
                {
                    using (var context = new UsersContext())
                    {
                        if (!context.Database.Exists())
                        {
                            // Create the SimpleMembership database without Entity Framework migration schema
                            ((IObjectContextAdapter)context).ObjectContext.CreateDatabase();
                        }
                    }

                    //WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);
                    WebSecurity.InitializeDatabaseConnection("TestRun_History", "UserProfile", "UserId", "UserName", autoCreateTables: true);





                    //if (!Roles.RoleExists("Administrator"))
                    //    Roles.CreateRole("Administrator");

                    //if (!Roles.RoleExists("Manager"))
                    //    Roles.CreateRole("Manager");

                    //if (!Roles.RoleExists("User"))
                    //    Roles.CreateRole("User");

                    //if (!WebSecurity.UserExists("Admin"))
                    //    WebSecurity.CreateUserAndAccount(
                    //        "Admin",
                    //        "Admin");


                    //if (!WebSecurity.UserExists("carla.gajdecki"))
                    //    WebSecurity.CreateUserAndAccount(
                    //        "carla.gajdecki",
                    //        "Password1");

                    //if (!WebSecurity.UserExists("User"))
                    //    WebSecurity.CreateUserAndAccount(
                    //        "User",
                    //        "User");


                    //if (!Roles.GetRolesForUser("Admin").ToString().Contains("Administrator"))
                    //    Roles.AddUsersToRoles(new[] { "Admin" }, new[] { "Administrator" });


                    //if (!Roles.GetRolesForUser("carla.gajdecki").ToString().Contains("Manager"))
                    //    Roles.AddUsersToRoles(new[] { "carla.gajdecki" }, new[] { "Manager" });

                    //if (!Roles.GetRolesForUser("User").ToString().Contains("User"))
                    //    Roles.AddUsersToRoles(new[] { "User" }, new[] { "User" });
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("The ASP.NET Simple Membership database could not be initialized. For more information, please see http://go.microsoft.com/fwlink/?LinkId=256588", ex);
                }
            }

        }

    }
}

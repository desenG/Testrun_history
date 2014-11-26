using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestRun_History.Models;

namespace TestRun_History.DAL
{
    public class AccountRepository : IAccountRepository, IDisposable
    {
        private UsersContext context;
        public AccountRepository(UsersContext context)
        {
            this.context = context;
        }
        public IEnumerable<UserProfile> GetAllUser()
        {
            //Get all users ordered by name
            return context.UserProfiles.OrderBy(r => r.UserName).ToList(); ;
        }
        public UserProfile GetUserById(int UserID)
        {
            //get user with supplied id
            return context.UserProfiles.Find(UserID);
        }
        public void Save()
        {
            //commit
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestRun_History.Models;

namespace TestRun_History.DAL
{
    public interface IAccountRepository : IDisposable
    {
        IEnumerable<UserProfile> GetAllUser();
        UserProfile GetUserById(int UserID);
        void Save();
    }
}
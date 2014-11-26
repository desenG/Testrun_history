using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestRun_History.Models;

namespace TestRun_History.DAL
{
    public interface ITestRunDataRepository : IDisposable
    {
        IEnumerable<TestRun_Data> GetTestRun_Datas();
        TestRun_Data GetTestRun_DataByID(int TestRun_DataID);
        IEnumerable<TestRun_Data> GetWithRawSqlTestRun_Datas(string query);
        void Save();

    }
}

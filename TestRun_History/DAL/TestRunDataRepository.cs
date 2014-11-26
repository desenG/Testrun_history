using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestRun_History.Models;

namespace TestRun_History.DAL
{
    public class TestRunDataRepository : ITestRunDataRepository, IDisposable
    {
        private TestRun_HistoryEntities context;


        public TestRunDataRepository(TestRun_HistoryEntities context)
        {
            this.context = context;
        }

        public IEnumerable<TestRun_Data> GetTestRun_Datas()
        {
            //Get all test run ordered by test run number
            return context.TestRun_Data.OrderBy(r => r.TR_Num).ToList();
        }

        public IEnumerable<TestRun_Data> GetWithRawSqlTestRun_Datas(string query)
        {
            //get  test runs with supplied query ordered by test run number
            return context.TestRun_Data.SqlQuery(query).OrderBy(r => r.TR_Num).ToList();
        }

        public TestRun_Data GetTestRun_DataByID(int TestRun_DataID)
        {
            //get test run with supplied id
            return context.TestRun_Data.Find(TestRun_DataID);

        }

        public void Save()
        {
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
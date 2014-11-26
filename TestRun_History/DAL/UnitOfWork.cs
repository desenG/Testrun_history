using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestRun_History.DAL;


namespace TestRun_History.Models
{
    public class UnitOfWork : IDisposable
    {
        private TestRun_HistoryEntities TestRunContext;
        private UsersContext UsersContext;
        private AccountRepository accountRepository;
        private GenericRepository<Query> queryRepository;
        private GenericRepository<Clause> clauseRepository;
        private GenericRepository<Step> stepRepository;
        private ITestRunDataRepository testRunDataRepository;

        //private ClauseRepository clauseRepository_old;

        public UnitOfWork()
        {
            this.UsersContext = new UsersContext();
            this.TestRunContext = new TestRun_HistoryEntities();
            
        }

        public AccountRepository AccountRepository
        {
            get
            {

                if (this.accountRepository == null)
                {
                    this.accountRepository = new AccountRepository(UsersContext);
                }
                return accountRepository;
            }
        }

        public GenericRepository<Query> QueryRepository
        {
            get
            {

                if (this.queryRepository == null)
                {
                    this.queryRepository = new GenericRepository<Query>(TestRunContext);
                }
                return queryRepository;
            }
        }

        public GenericRepository<Clause> ClauseRepository
        {
            get
            {

                if (this.clauseRepository == null)
                {
                    this.clauseRepository = new GenericRepository<Clause>(TestRunContext);
                }
                return clauseRepository;
            }
        }
        public GenericRepository<Step> StepRepository
        {
            get
            {

                if (this.stepRepository == null)
                {
                    this.stepRepository = new GenericRepository<Step>(TestRunContext);
                }
                return stepRepository;
            }
        }

        public ITestRunDataRepository TestRunDataRepository
        {
            get
            {

                if (this.testRunDataRepository == null)
                {
                    this.testRunDataRepository = new TestRunDataRepository(TestRunContext);
                }
                return testRunDataRepository;
            }
        }

        public void SaveTestRun()
        {
            TestRunContext.SaveChanges();
        }

        public void SaveAccount()
        {
            UsersContext.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    TestRunContext.Dispose();
                    UsersContext.Dispose();
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
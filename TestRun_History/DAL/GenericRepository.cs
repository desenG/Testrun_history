using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Expressions;
using System.Data.Entity;
using System.Data;

namespace TestRun_History.Models
{
    public class SortExpression<TEntity, TType>
    {
        //Expression<Func<TEntity, TType>> SortProperty;
    }

    public class GenericRepository<TEntity> where TEntity : class
    {
        internal TestRun_HistoryEntities context;
        internal DbSet<TEntity> dbSet;

        public GenericRepository(TestRun_HistoryEntities context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }
        // Expression<Func<TEntity, bool>> filter means the caller will 
        //provide a lambda expression based on the TEntity type, 
        //and this expression will return a Boolean value.
        //Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy also means 
        //the caller will provide a lambda expression. But in this case, 
        //the input to the expression is an IQueryable object for the TEntity type. 
        //The expression will return an ordered version of that IQueryable object.
        //the Get method creates an IQueryable object and then applies the filter expression if there is one
        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }
            //it applies the eager-loading expressions after parsing the comma-delimited list
            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
            //it applies the orderBy expression if there is one and returns the results; 
            //otherwise it returns the results from the unordered query
            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public virtual TEntity GetByID(object id)
        {
            return dbSet.Find(id);
        }

        public virtual void Insert(TEntity entity)
        {
            dbSet.Add(entity);
        }

        public virtual void Delete(object id)
        {
            TEntity entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public virtual IEnumerable<TEntity> GetWithRawSql(string query, params object[] parameters)
        {
            return dbSet.SqlQuery(query, parameters).ToList();
        }
    }
}
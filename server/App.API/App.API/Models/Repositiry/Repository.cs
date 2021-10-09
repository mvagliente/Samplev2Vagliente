using App.API.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace App.API.Models.Repositiry
{
    public class Repository<T> : IRepository<T> where T : Entity, new()
    {
        protected readonly DbContext dbContext;

        public Repository(DataBaseContextFactory dbCtxFact)
        {
            dbContext = dbCtxFact;
        }

        public async void SaveAsync()
        {
            await dbContext.SaveChangesAsync();
        }

        public async Task<T> InsertAsync(T o)
        {
            //var t = dbContext.Set<T>().Create();
            await dbContext.Set<T>().AddAsync(o);
            return o;
        }



        public virtual void Delete(T o)
        {
            dbContext.Set<T>().Remove(o);
        }

        public async Task<T> GetAsync(object id)
        {

            if (id.GetType() == typeof(int))
            {
                var entity = await dbContext.Set<T>().FindAsync((int)id);
                if (entity == null) throw new Exception("this entity doesn't exist anymore");
                return entity;
            }
            if (id.GetType() == typeof(string))
            {
                var entity = await dbContext.Set<T>().FindAsync((string)id);
                if (entity == null) throw new Exception("this entity doesn't exist anymore");
                return entity;
            }

            return null;

        }

        public T Get(object id)
        {

            if (id.GetType() == typeof(int))
            {
                var entity = dbContext.Set<T>().Find((int)id);
                if (entity == null) throw new Exception("this entity doesn't exist anymore");
                return entity;
            }
            if (id.GetType() == typeof(string))
            {
                var entity = dbContext.Set<T>().Find((string)id);
                if (entity == null) throw new Exception("this entity doesn't exist anymore");
                return entity;
            }

            return null;

        }

        public virtual IQueryable<T> Where(Expression<Func<T, bool>> predicate, List<string> includes = null, Boolean notracking = false)
        {

            if (!notracking)
            {
                var query = dbContext.Set<T>().Where(predicate).AsQueryable();

                if (includes != null)
                {
                    foreach (var include in includes)
                    {
                        query = query.Include(include);
                    }
                }

                return query;
            }
            else
            {
                var query = dbContext.Set<T>().AsNoTracking().Where(predicate).AsQueryable();

                if (includes != null)
                {
                    foreach (var include in includes)
                    {
                        query = query.Include(include);
                    }
                }

                return query;
            }
        }

        public virtual IQueryable<T> GetAll(List<string> includes = null)
        {

            var query = dbContext.Set<T>().AsQueryable();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return query;
        }

        public T Update(T o)
        {

            dbContext.Entry<T>(o).State = EntityState.Modified;

            return o;
        }

    }
}

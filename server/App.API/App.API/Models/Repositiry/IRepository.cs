using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace App.API.Models.Repositiry
{
    public interface IRepository<T>
    {
        Task<T> GetAsync(object id);
        T Get(object id);
        IQueryable<T> GetAll(List<string> includes = null);
        IQueryable<T> Where(Expression<Func<T, bool>> predicate, List<string> includes = null, Boolean notracking = false);
        Task<T> InsertAsync(T o);
        void SaveAsync();
        
        void Delete(T o);
        T Update(T o);
    }
}

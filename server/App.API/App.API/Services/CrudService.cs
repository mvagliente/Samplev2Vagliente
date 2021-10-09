using App.API.Models;
using App.API.Models.Repositiry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace App.API.Services
{
    public class CrudService<T> : ICrudService<T> where T : Entity, new()
    {

        protected IRepository<T> repo;

        public CrudService(IRepository<T> repo)
        {
            this.repo = repo;
        }


        public IQueryable<T> GetAll(List<string> includes = null)
        {

            return repo.GetAll(includes);
        }

        public async Task<T> GetAsync(object id)
        {
            return await repo.GetAsync(id);
        }

        public T Get(object id)
        {
            return repo.Get(id);
        }

        public async virtual Task<T> CreateAsync(T item, Boolean autosave = true)
        {
            var newItem = await repo.InsertAsync(item);

            if (autosave)
                repo.SaveAsync();

            return newItem;
        }

      
        public void SaveAsync()
        {
            repo.SaveAsync();
        }


        public virtual void DeleteAsync(T o, Boolean autosave = true)
        {
            repo.Delete(o);
            if (autosave)
                repo.SaveAsync();
        }

      

        public IQueryable<T> Where(Expression<Func<T, bool>> predicate, List<string> includes = null, Boolean notracking = false)
        {
            return repo.Where(predicate, includes, notracking);
        }

        public T Update(T o, Boolean autosave = true)
        {

            var newItem = repo.Update(o);

            if (autosave)
                repo.SaveAsync();

            return newItem;
        }



        //public List<IdentityRole> GetRoles()
        //{
        //    return repo.GetRoles();
        //}

        //public List<ApplicationUser> GetUsers()
        //{
        //    return repo.GetUsers();
        //}
    }
}

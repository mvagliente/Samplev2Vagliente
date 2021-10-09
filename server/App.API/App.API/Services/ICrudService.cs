using App.API.Models;
using App.API.Models.Security;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace App.API.Services
{
    public interface ICrudService<T> where T : Entity, new()
    {
        
        Task<T> CreateAsync(T item, Boolean autosave = true);

        

        void SaveAsync();



        void DeleteAsync(T o, Boolean autosave = true);

        Task<T> GetAsync(object id);

        

        IQueryable<T> GetAll(List<string> includes = null);

        IQueryable<T> Where(Expression<Func<T, bool>> func, List<string> includes = null, Boolean notracking = false);

        //List<IdentityRole> GetRoles();

        //List<ApplicationUser> GetUsers();

        T Update(T o, Boolean autosave = true);

    } 
}

using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace App.API.Models.Security
{
    public class ApplicationUser : IdentityUser
    {
        public Guid? officeId{ get; set; }
        public virtual Office office { get; set; }        
    }

    public static class UserManagerExtensions
    {
        public static List<ApplicationUser> FindByOfficeIdAsync(this UserManager<ApplicationUser> um, Guid officeid)
        {
            return um?.Users?.Where(x => x.officeId == officeid).ToList();
        }
    }
}

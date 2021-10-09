using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App.API.Models.Security
{
    public class UserRegistration
    {

        public UserRegistration(string id, string name, string email, string rol)
        {
            this.Id = id;
            this.Email = email;
            this.Name = name;
            this.rol = rol;
        }
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

        [Required]
        public string rol { get; set; }
    }
}

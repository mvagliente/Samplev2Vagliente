using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.API.Models
{
    public abstract class Entity
    {
        public Guid Id { get; set; }
        public DateTime DateCreated { get; set; }        
    }
}

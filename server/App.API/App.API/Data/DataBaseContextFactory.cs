using App.API.Models;
using App.API.Models.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.API.Data
{
    public class DataBaseContextFactory : IdentityDbContext<ApplicationUser>
    {
        public DataBaseContextFactory(DbContextOptions<DataBaseContextFactory> options) : base(options)
        {
        }

        public DbSet<Office> Offices { get; set; }


        public DbSet<RefreshToken> RefreshToken { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=./app.db");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RefreshToken>().ToTable("RefreshTokens");

            modelBuilder.Entity<ApplicationUser>().ToTable("Office");

            modelBuilder.Entity<ApplicationUser>().ToTable("Users");

            modelBuilder.Entity<IdentityRole>().ToTable("Roles");

            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");

            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");

            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");

            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");

            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");

            modelBuilder.Seed();
        }
    }
}

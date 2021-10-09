using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.API.Models;
using App.API.Models.Security;
using App.API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace App.API.Data
{

    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            var officeNumber = 10;
            var random = new Random(5);
            var usersMin = 2;
            var usersMax = 5;
            var offices = Enumerable.Range(0, officeNumber).SelectMany(o =>
               StreetNames.Select(oo => new Office
               {
                   Id = Guid.NewGuid(),
                   Address = $"{o * 20} {oo}"
               })
            ).ToList();

            offices.ToList().ForEach(t =>
                  modelBuilder.Entity<Office>().HasData(new Office{ Id = t.Id, Address = t.Address, DateCreated = DateTime.Now })
            );

            var roleadmin = new IdentityRole { Id = Guid.NewGuid().ToString(), Name = "Admin", NormalizedName = "Admin" };
            var roleagent = new IdentityRole { Id = Guid.NewGuid().ToString(), Name = "Agent", NormalizedName = "Agent" };
            var rolemanager = new IdentityRole { Id = Guid.NewGuid().ToString(), Name = "OfficeManager", NormalizedName = "OfficeManager" };


            modelBuilder.Entity<IdentityRole>().HasData(
               roleadmin,
               roleagent,
               rolemanager
            );

            var adminuser = new ApplicationUser { UserName = "Admin", NormalizedUserName = "Admin".ToUpper()};
            PasswordHasher<ApplicationUser> ph = new PasswordHasher<ApplicationUser>();
            adminuser.PasswordHash = ph.HashPassword(adminuser, "Pomelo_12");

            modelBuilder.Entity<ApplicationUser>().HasData(adminuser);
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string> { UserId = adminuser.Id, RoleId = roleadmin.Id });

            int j = 0;
            foreach (var office in offices)
            {
                
                var userNumber = random.Next(usersMin, usersMax);
                for (int i = 0; i < userNumber; i++)
                {
                    j++;
                    var user = new ApplicationUser { UserName = $"{FirstNames[random.Next(0, FirstNames.Length - 1)]}{"_"}{LastNames[random.Next(0, LastNames.Length - 1)]}{"_"}{j}", NormalizedUserName = $"{FirstNames[random.Next(0, FirstNames.Length - 1)]}{"_"}{LastNames[random.Next(0, LastNames.Length - 1)]}{"_"}{j}".ToUpper(), officeId = offices[random.Next(0, offices.Count - 1)].Id};
                    
                    modelBuilder.Entity<ApplicationUser>().HasData(user);


                    if (i % 3 == 0)
                        modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string> {UserId = user.Id,RoleId = roleagent.Id });
                    else
                        modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string> { UserId = user.Id, RoleId = rolemanager.Id });

                }
            }

        }

        static string[] StreetNames = new[]
{
            "ROCKAWAY PARKWAY",
            "178TH",
            "GRAND CONCOURSE",
            "CORTELYOU",
            "153RD",
            "MACDONOUGH",
            "HORACE HARDING",
            "BAINBRIDGE",
            "UTOPIA PARKWAY",
            "MACON",
            "VERNON",
            "SEVENTH",
            "SNYDER",
            "ASTORIA",
            "STEINWAY",
            "KENT",
            "BLEECKER",
            "ESSEX",
            "179TH",
            "216TH",
            "EVERGREEN",
            "BRUCKNER",
            "184TH",
            "174TH",
            "198TH",
            "NINTH",
            "MAPLE",
            "AVENUE I",
            "RIDGE",
            "AVENUE X",
            "177TH",
            "187TH",
            "EIGHTH",
            "BAY PARKWAY",
            "BEVERLEY",
            "MURDOCK",
            "SKILLMAN",
            "DITMARS",
            "NEW UTRECHT",
            "182ND",
            "173RD",
            "197TH",
            "HOWARD",
            "TENTH",
            "AVENUE V",
            "STILLWELL",
            "MANOR",
            "OVINGTON",
            "MORRIS",
            "195TH",
            "AVENUE W",
            "TOMPKINS",
            "SOUTHERN",
            "ELDERT",
            "MONTGOMERY",
            "BATH",
            "BEACH",
            "186TH",
            "BLAKE",
            "188TH",
            "COLUMBUS",
            "DAHILL",
            "176TH",
            "DUMONT",
            "192ND",
            "MORGAN",
            "190TH",
            "AVENUE O",
            "PAULDING",
            "CROPSEY",
            "199TH",
            "194TH",
            "GREENWICH",
            "189TH",
            "FREDERICK DOUGLASS",
            "BAYSIDE",
            "154TH",
            "GUY R BREWER",
            "AUSTIN",
            "CLEVELAND",
            "CROWN",
            "UNIVERSITY",
            "CLARKSON",
            "234TH",
            "209TH",
            "ROCKAWAY BEACH",
            "193RD",
            "LITTLE NECK PARKWAY",
            "VERMONT",
            "RIDGEWOOD",
            "FARMERS",
            "JEWEL",
            "AVENUE Y",
            "WALTON",
            "GROVE",
            "BARNES",
            "ARLINGTON",
            "LEONARD",
            "GREENPOINT",
            "GRANT"
            };

        static string[] FirstNames = new[]
        {
            "Dan",
            "Kevin",
            "Tom",
            "Matt",
            "Linda",
            "Rachel",
            "Steven",
            "Stephanie",
            "Mary",
            "Maria",
            "Ben",
            "Julie",
            "Bill",
            "Emily",
            "Sam",
            "Joseph",
            "Jane",
            "Nicole",
            "Anna",
            "Jim",
            "Matthew",
            "Melissa",
            "Andrea",
            "Carol",
            "George",
            "Greg",
            "Alan",
            "Josh",
            "Stephen",
            "Christine",
            "Rebecca",
            "Nick",
            "Kate",
            "Ellen",
            "Anne",
            "Charles",
            "William",
            "Sara",
            "Anthony",
            "Marc",
            "Frank",
            "Bob",
            "Ryan",
            "Amanda",
            "Liz",
            "Jon",
            "Judy",
            "Gary",
            "Patricia",
            "Thomas",
            "Beth",
            "Robin",
            "Wendy",
            "Julia",
            "Patrick",
            "Tim",
            "Christina",
            "Kim",
            "Rob",
            "Christopher",
            "Ann",
            "Justin",
            "Jay",
            "Sharon",
            "Ken",
            "Jack",
            "Diane",
            "Jill",
            "Caroline",
            "Leslie",
            "Katie",
            "Ron",
            "Tony",
            "Alexandra",
            "Allison",
            "Jamie",
            "Jeffrey",
            "Diana",
            "Catherine",
            "Larry",
            "Sean",
            "Deborah",
            "Danielle",
            "Angela",
            "Andy",
            "Heather",
            "Victoria",
            "Joan",
            "Helen",
            "Jenny",
            "Janet",
            "Lori",
            "Ed",
            "Donna",
            "Dana",
            "Kathy",
            "Nina",
            "Jackie",
            "Suzanne",
            "Michele"
        };

        static string[] LastNames = new[]
        {
            "Deutsch",
            "Blum",
            "McDonald",
            "Zhu",
            "Gardner",
            "Lane",
            "Frankel",
            "Freeman",
            "Strauss",
            "Patterson",
            "sutton",
            "Kane",
            "Doe",
            "Simmons",
            "Gottlieb",
            "Jacobson",
            "Myers",
            "Charles",
            "Zhou",
            "Goldsmith",
            "Mason",
            "Abrams",
            "Kay",
            "Baron",
            "Geller",
            "Wolfe",
            "Reid",
            "Wasserman",
            "Rosenfeld",
            "Hart",
            "Baum",
            "Freedman",
            "Morrison",
            "Ellis",
            "Reilly",
            "Zhao",
            "Rao",
            "Brenner",
            "Long",
            "Block",
            "Richardson",
            "Glass",
            "Foley",
            "Alvarez",
            "Shin",
            "Wells",
            "Song",
            "Sanders",
            "Warner",
            "RENTAL",
            "Rich",
            "Meyers",
            "Gill",
            "Barry",
            "Ali",
            "V",
            "Tucker",
            "Lai",
            "Richards",
            "Stark",
            "Leung",
            "Reynolds",
            "Simpson",
            "Jiang",
            "Sales Office",
            "Fishman",
            "Keller",
            "Watson",
            "Sandler",
            "Quinn",
            "Palmer",
            "Fitzgerald",
            "O'Connor",
            "Porter",
            "Leonard",
            "Abraham",
            "Romano",
            "Cohn",
            "Berg",
            "Zimmerman",
            "Jain",
            "Brody",
            "Tran",
            "Dunn",
            "N",
            "Griffin",
            "Tsai",
            "Fine",
            "Matthews",
            "Reed",
            "Lo",
            "Powell",
            "Farrell",
            "Oh",
            "Clarke",
            "Flynn",
            "Mills",
            "Chapman",
            "Ramirez",
            "Mayer"
        };
    }

}

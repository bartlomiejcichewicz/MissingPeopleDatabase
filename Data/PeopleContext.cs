using MissingPeopleDatabase.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MissingPeopleDatabase.Data
{
    public class PeopleContext: IdentityDbContext
    {
        public PeopleContext(DbContextOptions options):base(options)
        {
        }
        public virtual DbSet<Sex> Sexes { get; set; } 
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<Status> Statuses { get; set; }
        public virtual DbSet<Person> Persons { get; set; }
    }
}

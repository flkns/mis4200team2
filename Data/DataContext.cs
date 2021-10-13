using mis4200team2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace mis4200team2.Data
{
    public class DataContext : DbContext
    {
        public DataContext(): base("name=DefaultConnection") { }

        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().ToTable(nameof(Employee));
        }
    }
}
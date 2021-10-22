namespace mis4200team2.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using mis4200team2.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<mis4200team2.Data.DataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(mis4200team2.Data.DataContext context)
        {
            // TO MAKE YOURSELF AN ADMIN, CHANGE BELOW:

            // Guid adminID = context.Employees.Where(e => e.Email == "adminemail@test.com").FirstOrDefault().ID;
            // context.Employees.Find(adminID).BusinessUnit = Employee.BusinessUnits.admin;
        }
    }
}

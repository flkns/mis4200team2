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
            AutomaticMigrationsEnabled = true;
        }

    // http://johnatten.com/2014/04/20/asp-net-mvc-and-identity-2-0-understanding-the-basics/
    // http://johnatten.com/2014/04/20/asp-net-mvc-and-identity-2-0-understanding-the-basics/
    // http://johnatten.com/2014/04/20/asp-net-mvc-and-identity-2-0-understanding-the-basics/

    protected override void Seed(mis4200team2.Data.DataContext context)
        {
            // TO MAKE YOURSELF AN ADMIN, CHANGE BELOW:

            //Guid adminID = context.Employees.Where(e => e.Email == "bf853817@ohio.edu").FirstOrDefault().ID;
            //context.Employees.Find(adminID).Role = Employee.Roles.admin;
            
        }
    }
}

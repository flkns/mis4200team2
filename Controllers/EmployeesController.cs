using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using mis4200team2.Data;
using mis4200team2.Models;

namespace mis4200team2.Controllers
{
  [Authorize]
  public class EmployeesController : Controller
  {
    private DataContext db = new DataContext();

    // GET: Employees
    public ActionResult Index()
    {
      return View(db.Employees.ToList());
    }

    // GET: Employees/Details/5
    public ActionResult Details(Guid? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Employee employee = db.Employees.Find(id);
      if (employee == null)
      {
        return HttpNotFound();
      }
      return View(employee);
    }

    // GET: Employees/Create
    public ActionResult Create()
    {
      return View();
    }

    // POST: Employees/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(Include = "ID,Role,FirstName,LastName,Email,Phone,RegisteredDate,HireDate,Title,BusinessUnit,Version")] Employee employee)
    {
      if (ModelState.IsValid)
      {
        Guid.TryParse(User.Identity.GetUserId(), out Guid employeeID);

        employee.ID = employeeID;
        employee.Email = User.Identity.GetUserName();
        employee.RegisteredDate = DateTime.Now;
        employee.Role = Employee.Roles.user;

        db.Employees.Add(employee);

        try
        {
          db.SaveChanges();
        }
        catch (Exception ex)
        {
          ViewBag.Error = "Unable to save changes. Try again, and if the problem persists see your system administrator." + "\b" + ex.Message;
          return View("Error");
        }


        return RedirectToAction("Index");
      }

      return View(employee);
    }

    // GET: Employees/Edit/5
    public ActionResult Edit(Guid? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }

      Employee employee = db.Employees.Find(id);

      if (employee == null)
      {
        return HttpNotFound();
      }

      return View(employee);
    }

    // POST: Employees/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include = "ID,Role,FirstName,LastName,Email,Phone,RegisteredDate,HireDate,Title,BusinessUnit,Version")] Employee employee)
    {
      if (ModelState.IsValid)
      {
        string currentEmployeeEmail = User.Identity.GetUserName().ToString();
        Employee currentEmployee = db.Employees.Where(e => e.Email.ToString().Equals(currentEmployeeEmail)).First();

        if (currentEmployee != null)
        {
          bool isAdmin = currentEmployee.Role == Employee.Roles.admin;

          if ((currentEmployee.ID == employee.ID) || isAdmin)
          {
              try
              {
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
              }
              catch (Exception ex)
              {
                ViewBag.Error = "Unable to save changes. Try again, and if the problem persists see your system administrator." + "\b" + ex.Message;
                return View("Error");
              }
            
            
          }
          else
          {
            ViewBag.error = "Not authorized to make changes to this user.";
            return View("Error");
          }
        }
        else
        {
          ViewBag.Error = "Unable to find employee. Try again, and if the problem persists see your system administrator.";
          return View("Error");
        }


      }
      return View(employee);
    }

    // GET: Employees/Delete/5
    public ActionResult Delete(Guid? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }

      Employee employee = db.Employees.Find(id);

      if (employee == null)
      {
        return HttpNotFound();
      }

      Guid.TryParse(User.Identity.GetUserId(), out Guid employeeID);

      string currentEmployeeEmail = User.Identity.GetUserName().ToString();
      Employee currentEmployee = db.Employees.Where(e => e.Email.ToString().Equals(currentEmployeeEmail)).First();

      if (currentEmployee != null)
      {
        bool isAdmin = currentEmployee.Role == Employee.Roles.admin;

        if (isAdmin)
        {
          return View(employee);
        }
        else
        {
          ViewBag.error = "Not authorized to delete employee user data.";
          return View("Error");
        }
      }
      else
      {
        ViewBag.Error = "Unable to find employee. Try again, and if the problem persists see your system administrator.";
        return View("Error");
      }
    }

    // POST: Employees/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(Guid id)
    {
      Employee employee = db.Employees.Find(id);
      db.Employees.Remove(employee);
      db.SaveChanges();
      return RedirectToAction("Index");
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        db.Dispose();
      }
      base.Dispose(disposing);
    }
  }
}

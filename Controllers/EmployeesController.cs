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
    private readonly DataContext db = new DataContext();

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
    public ActionResult Create([Bind(Include = "EmployeeID,Role,FirstName,LastName,Email,Phone,RegisteredDate,HireDate,Title,BusinessUnit")] Employee employee)
    {
      if (ModelState.IsValid)
      {
        Guid employeeID;
        Guid.TryParse(User.Identity.GetUserId(), out employeeID);

        employee.EmployeeID = employeeID;
        employee.Email = User.Identity.GetUserName();
        employee.RegisteredDate = DateTime.Now;
        employee.LastUpdateDateTime = DateTime.Now;

        try
        {
          db.Employees.Add(employee);
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

    [Authorize(Roles = "admin")]
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
    [Authorize(Roles = "admin")]
    [HttpPost, ActionName("Edit")]
    [ValidateAntiForgeryToken]
    public ActionResult EditConfirmed(Guid? id)
    {
      if(id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }

      var employeeToUpdate = db.Employees.Find(id);

      Guid currentEmployeeID;
      Guid.TryParse(User.Identity.GetUserId(), out currentEmployeeID);

      if ((id == currentEmployeeID) || User.IsInRole("admin"))
      {
        if (TryUpdateModel(employeeToUpdate, "", new string[] { "Role", "FirstName", "LastName", "Email", "Phone", "HireDate", "Title", "BusinessUnit", "LastUpdateDateTime" }))
        {
          try
          {
            employeeToUpdate.LastUpdateDateTime = DateTime.Now;
            db.Entry(employeeToUpdate).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
          }
          catch (DataException /* dex */)
          {
            ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
          }
        }
      }
      else
      {
        ViewBag.error = "Not authorized to make changes to this user.";
        return View("Error");
      }

      return View(employeeToUpdate);
    }

    // GET: Employees/Delete/5
    [Authorize(Roles = "admin")]
    public ActionResult Delete(Guid? id, bool? saveChangesError = false)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }

      if (saveChangesError.GetValueOrDefault())
      {
        ViewBag.Error = "Delete failed. Try again, and if the problem persists see your system administrator.";
        return View("Error");
      }

      Employee employee = db.Employees.Find(id);

      if (employee == null)
      {
        return HttpNotFound();
      }
      
      return View(employee);
    }

    // POST: Employees/Delete/5
    [Authorize(Roles = "admin")]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(Guid id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }

     

      var currentEmployeeEmail = User.Identity.GetUserName().ToString();
      var currentEmployee = db.Employees.Where(e => e.Email.ToString().Equals(currentEmployeeEmail)).First();

      if (currentEmployee == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }

      bool IsAdmin = User.IsInRole("admin");

      if (IsAdmin)
      {
        try
        {
          var employeeToDelete = new Employee() { EmployeeID = id };
          db.Entry(employeeToDelete).State = EntityState.Deleted;

          db.Employees.Remove(employeeToDelete);
          db.SaveChanges();
        }
        catch (DataException /* dex */)
        {
          return RedirectToAction("Delete", new { EmployeeID = id, saveChangesError = true });
        }

        return RedirectToAction("Index");
      }
      else
      {
        ViewBag.error = "Not authorized to delete this user.";
        return View("Error");
      }
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

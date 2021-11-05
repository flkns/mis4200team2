using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
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
    public async Task<ActionResult> Index(int? page, string searchEmail, string searchName)
    {
      int pageSize = 10;
      int pageNumber = (page ?? 1);

      ViewBag.searchEmail = String.IsNullOrEmpty(searchEmail) ? "" : searchEmail;
      ViewBag.searchName = String.IsNullOrEmpty(searchName) ? "" : searchName;



      var employees = await db.Employees.ToListAsync();
      var employee = from e in employees select e;

      employee = db.Employees.OrderByDescending(e => e.RegisteredDate).ThenBy(e => e.LastName).ThenBy(e => e.FirstName);

      if (!String.IsNullOrEmpty(searchEmail))
      {
        employee = employee.Where(e => e.Email.Contains(searchEmail));
      }

      if (!String.IsNullOrEmpty(searchName))
      {
        string[] renterNames;
        renterNames = searchName.Split(' ');

        if (renterNames.Count() == 1)
        {
          employee = employee.Where(e => e.FirstName.Contains(searchName) || e.LastName.Contains(searchName));
        }
        else
        {
          string r1 = renterNames[0];
          string r2 = renterNames[1];
          employee = employee.Where(e => e.FirstName.Contains(r1) && e.LastName.Contains(r2));
        }
      }

      var employeesList = employee.ToPagedList(pageNumber, pageSize);

      return View(employeesList);
    }

    // GET: Employees/Details/5
    public async Task<ActionResult> Details(Guid? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Employee employee = await db.Employees.FindAsync(id);
      if (employee == null)
      {
        return HttpNotFound();
      }
      return View(employee);
    }

    // GET: Employees/Create
    [Authorize]
    public ActionResult Create()
    {
      return View();
    }

    // POST: Employees/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create([Bind(Include = "ID,Role,FirstName,LastName,Email,RegisteredDate,HireDate,BusinessUnit,Title")] Employee employee)
    {
      try
      {
        if (ModelState.IsValid)
        {
          Guid employeeID;
          Guid.TryParse(User.Identity.GetUserId(), out employeeID);

          employee.ID = employeeID;
          employee.Email = User.Identity.GetUserName();

          employee.RegisteredDate = DateTime.Now;
          employee.Role = Employee.Roles.user;

          db.Employees.Add(employee);

          try
          {
            await db.SaveChangesAsync();
          }
          catch (Exception ex)
          {
            ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            ViewBag.error = ex.Message;
            return View("Error");
          }

          return RedirectToAction("Index");
        }
      }
      catch (RetryLimitExceededException /* dex */)
      {
        //Log the error (uncomment dex variable name and add a line here to write a log.
        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
      }

      return View(employee);
    }

    // GET: Employees/Edit/5
    public async Task<ActionResult> Edit(Guid? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }

      Employee employee = await db.Employees.FindAsync(id);

      if (employee == null)
      {
        return HttpNotFound();
      }

      string currentEmployeeEmail = User.Identity.GetUserName().ToString();
      Employee currentEmployee = await db.Employees.Where(e => e.Email.ToString().Equals(currentEmployeeEmail)).FirstAsync();
      
      if (currentEmployee != null)
      {
        bool isAuthorized = currentEmployee.Role == Employee.Roles.admin;
        
        if (currentEmployee.ID == employee.ID || isAuthorized)
        {
          return View(employee);
        }
        else
        {
          ViewBag.error = "Not authorized to make changes to this user.";
          return View("Error");
        }
      }
      else
      {
        ViewBag.error = "User not found.";
        return View("Error");
      }
    }

    // POST: Employees/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit([Bind(Include = "ID,Role,FirstName,LastName,Email,RegisteredDate,HireDate,BusinessUnit,Title,Version")] Employee employee) {
     if (ModelState.IsValid)
        {
          db.Entry(employee).State = EntityState.Modified;
          try
          {
            await db.SaveChangesAsync();
  }
          catch (Exception ex)
          {
            ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            ViewBag.error = ex.Message;
            return View("Error");
}
return RedirectToAction("Index");
        }
      
      return View(employee);
    }

    /*public async Task<ActionResult> Edit(Guid? id, byte[] version)
    {
      string[] bindFields = new string[] { "ID", "Role", "FirstName", "LastName", "Email", "RegisteredDate", "HireDate", "BusinessUnit", "Title", "Version" };
      if(id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      var employeeToUpdate = await db.Employees.FindAsync(id);
      if(employeeToUpdate == null)
      {
        Employee deletedEmployee = new Employee();
        TryUpdateModel(deletedEmployee, bindFields);
        ModelState.AddModelError(nameof(deletedEmployee.Email), "Unable to save changes. The employee was deleted by another user.");
        return View(deletedEmployee);
      }

      if(TryUpdateModel(employeeToUpdate, bindFields))
      {
        try
        {
          db.Entry(employeeToUpdate).OriginalValues["Version"] = version;
          await db.SaveChangesAsync();

          return RedirectToAction("Index");
        } catch (DbUpdateConcurrencyException ex)
        {
          var entry = ex.Entries.Single();
          var values = (Employee)entry.Entity;
          var dbEntry = await entry.GetDatabaseValuesAsync();
          if(dbEntry == null)
          {
            ModelState.AddModelError(nameof(values.Email), "Unable to save changes. The employee was deleted by another user.");
          } else
          {
            var dbValues = (Employee)dbEntry.ToObject();

            if(dbValues.Role != values.Role)
            {
              ModelState.AddModelError("Role", "Current value: "+string.Format("{0:c}", dbValues.Role));
            }
            if (dbValues.FirstName != values.FirstName)
            {
              ModelState.AddModelError("FirstName", "Current value: " + string.Format("{0:c}", dbValues.FirstName));
            }
            if (dbValues.LastName != values.LastName)
            {
              ModelState.AddModelError("LastName", "Current value: " + string.Format("{0:c}", dbValues.LastName));
            }
            if (dbValues.Email != values.Email)
            {
              ModelState.AddModelError("Email", "Current value: " + string.Format("{0:c}", dbValues.Email));
            }
            if (dbValues.HireDate != values.HireDate)
            {
              ModelState.AddModelError("HireDate", "Current value: " + string.Format("{0:c}", dbValues.HireDate));
            }
            if (dbValues.BusinessUnit != values.BusinessUnit)
            {
              ModelState.AddModelError("BusinessUnit", "Current value: " + string.Format("{0:c}", dbValues.BusinessUnit));
            }
            if (dbValues.Title != values.Title)
            {
              ModelState.AddModelError("Title", "Current value: " + string.Format("{0:c}", dbValues.Title));
            }
            ModelState.AddModelError(nameof(employeeToUpdate.Email), "The record you attempted to edit "
                            + "was modified by another user after you got the original value. The "
                            + "edit operation was canceled and the current values in the database "
                            + "have been displayed. If you still want to edit this record, click "
                            + "the Save button again. Otherwise click the Back to List hyperlink.");
            employeeToUpdate.Version = dbValues.Version;
          }
        } catch (RetryLimitExceededException /* dex *//*)
        {
          ModelState.AddModelError(nameof(employeeToUpdate.Email), "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
        }
      }
      return View(employeeToUpdate);
*/

       

    // GET: Employees/Delete/5
    public async Task<ActionResult> Delete(Guid? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Employee employee = await db.Employees.FindAsync(id);
      if (employee == null)
      {
        return HttpNotFound();
      }

      Guid employeeID;
      Guid.TryParse(User.Identity.GetUserId(), out employeeID);

      Employee currentUser = await db.Employees.FindAsync(employeeID);
      bool isAuthorized = currentUser.Role == Employee.Roles.admin;

      if (isAuthorized)
      {
        return View(employee);
      }
      else
      {
        ViewBag.error = "Only administrators are allowed to delete employee user data.";
        return View("Error");
      }
    }

    // POST: Employees/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> DeleteConfirmed(Guid id)
    {
      try
      {
        Employee employee = await db.Employees.FindAsync(id);
        db.Employees.Remove(employee);
        try
        {
          await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
          ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
          ViewBag.error = ex.Message;
          return View("Error");
        }
      }
      catch (RetryLimitExceededException /* dex */)
      {
        //Log the error (uncomment dex variable name and add a line here to write a log.
        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
      }
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

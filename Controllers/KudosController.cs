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
using PagedList;

namespace mis4200team2.Controllers
{
  [Authorize]
  public class KudosController : Controller
  {
    private DataContext db = new DataContext();

    // GET: Kudos
    public ActionResult Index()
    {
      var kudosDB = db.KudosDB.Include(k => k.ReceiverEmployee).Include(k => k.SenderEmployee);
      return View(kudosDB.ToList());
    }

    public ActionResult Leaderboard(string currentSort, string currentFilter, string searchString, int? page)
    {
      ViewBag.ReceiveSort = String.IsNullOrEmpty(currentSort) ? "receive_asc" : "";
      ViewBag.SentSort = currentSort == "sent_asc" ? "sent_desc" : "sent_asc";
      ViewBag.CurrentSort = currentSort;

      if (searchString != null)
      {
        page = 1;
      }
      else { searchString = currentFilter; }

      ViewBag.CurrentFilter = searchString;

      var employees = from e in db.Employees select e;

      if (!String.IsNullOrEmpty(searchString))
      {
        employees = employees.Where(e => e.FirstName.ToUpper().Contains(searchString.ToUpper()) || e.LastName.ToUpper().Contains(searchString.ToUpper()));
      }

      switch (currentSort)
      {
        case "receive_asc":
          employees = employees.OrderBy(e => e.ReceivedKudos.Count());
          break;
        case "sent_asc":
          employees = employees.OrderBy(e => e.SentKudos.Count());
          break;
        case "sent_desc":
          employees = employees.OrderByDescending(e => e.SentKudos.Count());
          break;
        default:
          employees = employees.OrderByDescending(e => e.ReceivedKudos.Count());
          break;
      }

      int pageSize = 15;
      int pageNumber = (page ?? 1);

      return View(employees.AsNoTracking().ToPagedList(pageNumber, pageSize));
    }

    // GET: Kudos/Details/5
    public ActionResult Details(Guid? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Kudos kudos = db.KudosDB.Find(id);
      if (kudos == null)
      {
        return HttpNotFound();
      }
      return View(kudos);
    }

    // GET: Kudos/Create
    public ActionResult Create()
    {
      Guid currentEmployeeID;
      Guid.TryParse(User.Identity.GetUserId(), out currentEmployeeID);


      ViewBag.ReceiverID = new SelectList(db.Employees.Where(e => e.EmployeeID != currentEmployeeID).OrderBy(e => e.LastName), "EmployeeID", "FullName");
      //ViewBag.SenderID = new SelectList(db.Employees, "ID", "FullName");
      return View();
    }

    // POST: Kudos/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(Include = "KudosID,SenderID,ReceiverID,SendTime,Type")] Kudos kudos)
    {
      Guid currentEmployeeID;
      Guid.TryParse(User.Identity.GetUserId(), out currentEmployeeID);
      if (ModelState.IsValid)
      {
        kudos.KudosID = Guid.NewGuid();

        var currentEmployee = db.Employees.Find(currentEmployeeID);
        var receiverEmployee = db.Employees.Find(kudos.ReceiverID);

        kudos.SenderEmployee = currentEmployee;
        kudos.ReceiverEmployee = receiverEmployee;

        kudos.SenderID = currentEmployeeID;
        kudos.SendTime = DateTime.Now;

        if ((kudos.ReceiverEmployee.GetType() == null) || (kudos.ReceiverEmployee == null))
        {
          ViewBag.Error = "You cannot send kudos to no one!";
          return View("Error");
        }

        if ((kudos.SenderID == kudos.ReceiverID) || (kudos.SenderEmployee == kudos.ReceiverEmployee))
        {
          ViewBag.Error = "You cannot send kudos to yourself!";
          return View("Error");
        }

        try
        {
          db.KudosDB.Add(kudos);
          db.SaveChanges();
        }
        catch (Exception ex)
        {
          ViewBag.Error = "Unable to save changes. Try again, and if the problem persists see your system administrator." + "\b" + ex.Message;
          return View("Error");
        }
        return RedirectToAction("Index");
      }

      ViewBag.ReceiverID = new SelectList(db.Employees.Where(e => e.EmployeeID != currentEmployeeID).OrderBy(e => e.LastName), "EmployeeID", "FullName", kudos.ReceiverID);
      //ViewBag.SenderID = new SelectList(db.Employees, "ID", "FullName", kudos.SenderID);
      return View(kudos);
    }

    // GET: Kudos/Edit/5
    [Authorize(Roles = "admin")]
    public ActionResult Edit(Guid? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Kudos kudos = db.KudosDB.Find(id);
      if (kudos == null)
      {
        return HttpNotFound();
      }
      ViewBag.ReceiverID = new SelectList(db.Employees, "EmployeeID", "Role", kudos.ReceiverID);
      ViewBag.SenderID = new SelectList(db.Employees, "EmployeeID", "Role", kudos.SenderID);
      return View(kudos);
    }

    // POST: Kudos/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin")]
    public ActionResult Edit([Bind(Include = "KudosID,SenderID,ReceiverID,SendTime,Type")] Kudos kudos)
    {
      if (ModelState.IsValid)
      {
        db.Entry(kudos).State = EntityState.Modified;
        db.SaveChanges();
        return RedirectToAction("Index");
      }
      ViewBag.ReceiverID = new SelectList(db.Employees, "EmployeeID", "Role", kudos.ReceiverID);
      ViewBag.SenderID = new SelectList(db.Employees, "EmployeeID", "Role", kudos.SenderID);
      return View(kudos);
    }

    // GET: Kudos/Delete/5
    [Authorize(Roles = "admin")]
    public ActionResult Delete(Guid? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Kudos kudos = db.KudosDB.Find(id);
      if (kudos == null)
      {
        return HttpNotFound();
      }
      return View(kudos);
    }

    // POST: Kudos/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin")]
    public ActionResult DeleteConfirmed(Guid id)
    {
      Kudos kudos = db.KudosDB.Find(id);
      db.KudosDB.Remove(kudos);
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



/*
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
  public class KudosController : Controller
  {
    private readonly DataContext db = new DataContext();

    // GET: Kudos
    public ActionResult Index()
    {
      var kudos = db.KudosDB.Include(k => k.ReceiverEmployee).Include(k => k.SenderEmployee);
      return View(kudos.ToList());
    }

    public ActionResult Leaderboard()
    {
      var kudos = db.KudosDB.Include(k => k.ReceiverEmployee).Include(k => k.SenderEmployee);
      return View(kudos.ToList());
    }

    // GET: Kudos/Details/5
    public ActionResult Details(Guid? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Kudos kudos = db.KudosDB.Find(id);
      if (kudos == null)
      {
        return HttpNotFound();
      }
      return View(kudos);
    }

    // GET: Kudos/Create
    public ActionResult Create()
    {
      //ViewBag.SenderID = new SelectList(db.Employees, "ID", "FullName");
      ViewBag.ReceiverID = new SelectList(db.Employees, "ID", "FullName");
      return View();
    }

    // POST: Kudos/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(Include = "ID,SenderID,ReceiverID,SendTime,Type")] Kudos kudos)
    {
      if (ModelState.IsValid)
      {
        //kudos.ID = Guid.NewGuid();

        //Guid currentEmployeeID;
        //Guid.TryParse(User.Identity.GetUserId(), out currentEmployeeID);

        //kudos.SenderID = currentEmployeeID;
        //kudos.SendTime = DateTime.Now;


        try
        {
          db.KudosDB.Add(kudos);
          db.SaveChanges();
        }
        catch (Exception ex)
        {
          ViewBag.Error = "Unable to save changes. Try again, and if the problem persists see your system administrator." + "\b" + ex.Message;
          return View("Error");
        }

        return RedirectToAction("Index");
      }


      //ViewBag.SenderID = new SelectList(db.Employees, "ID", "FullName", kudos.SenderID);
      //ViewBag.ReceiverID = new SelectList(db.Employees, "ID", "FullName", kudos.ReceiverID);
      return View(kudos);
    }

    // GET: Kudos/Edit/5
    public ActionResult Edit(Guid? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Kudos kudos = db.KudosDB.Find(id);
      if (kudos == null)
      {
        return HttpNotFound();
      }
      ViewBag.SenderID = new SelectList(db.Employees, "ID", "FirstName", kudos.SenderID);
      ViewBag.ReceiverID = new SelectList(db.Employees, "ID", "FirstName", kudos.ReceiverID);
      return View(kudos);
    }

    // POST: Kudos/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include = "ID,SenderID,ReceiverID,SendTime,Type")] Kudos kudos)
    {
      if (ModelState.IsValid)
      {
        db.Entry(kudos).State = EntityState.Modified;
        db.SaveChanges();
        return RedirectToAction("Index");
      }
      ViewBag.SenderID = new SelectList(db.Employees, "ID", "FirstName", kudos.SenderID);
      ViewBag.ReceiverID = new SelectList(db.Employees, "ID", "FirstName", kudos.ReceiverID);
      return View(kudos);
    }

    // GET: Kudos/Delete/5
    public ActionResult Delete(Guid? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Kudos kudos = db.KudosDB.Find(id);
      if (kudos == null)
      {
        return HttpNotFound();
      }
      return View(kudos);
    }

    // POST: Kudos/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(Guid id)
    {
      Kudos kudos = db.KudosDB.Find(id);
      db.KudosDB.Remove(kudos);
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
*/
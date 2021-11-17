using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using mis4200team2.Data;
using mis4200team2.Models;

namespace mis4200team2.Controllers
{
    public class KudosController : Controller
    {
        private DataContext db = new DataContext();

        // GET: Kudos
        public ActionResult Index()
        {
            var kudos = db.Kudos.Include(k => k.Employee).Include(k => k.Employees);
            return View(kudos.ToList());
        }

        // GET: Kudos/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kudos kudos = db.Kudos.Find(id);
            if (kudos == null)
            {
                return HttpNotFound();
            }
            return View(kudos);
        }

        // GET: Kudos/Create
        public ActionResult Create()
        {
            ViewBag.SenderID = new SelectList(db.Employees, "ID", "FirstName");
            ViewBag.ReceiverID = new SelectList(db.Employees, "ID", "FirstName");
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
                kudos.ID = Guid.NewGuid();
                db.Kudos.Add(kudos);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SenderID = new SelectList(db.Employees, "ID", "FirstName", kudos.SenderID);
            ViewBag.ReceiverID = new SelectList(db.Employees, "ID", "FirstName", kudos.ReceiverID);
            return View(kudos);
        }

        // GET: Kudos/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kudos kudos = db.Kudos.Find(id);
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
            Kudos kudos = db.Kudos.Find(id);
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
            Kudos kudos = db.Kudos.Find(id);
            db.Kudos.Remove(kudos);
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

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplicationMVC_EmailConf.Models;

namespace WebApplicationMVC_EmailConf.Controllers
{
    public class Sub_StepController : Controller
    {
        private SpeechSageDBEntities db = new SpeechSageDBEntities();

        // GET: Sub_Step
        public ActionResult Index()
        {
            var sub_Step = db.Sub_Step.Include(s => s.Step).Where(m => m.Active == true).OrderBy(o => new { o.Step_ID, o.Order_By });
            return View(sub_Step.ToList());
        }

        // GET: Sub_Step/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sub_Step sub_Step = db.Sub_Step.Find(id);
            if (sub_Step == null)
            {
                return HttpNotFound();
            }
            return View(sub_Step);
        }

        // GET: Sub_Step/Create
        public ActionResult Create()
        {
            ViewBag.Step_ID = new SelectList(db.Steps, "Step_ID", "Name");
            return View();
        }

        // POST: Sub_Step/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SubStep_ID,Name,Description,Step_ID,Order_By")] Sub_Step sub_Step)
        {
            if (ModelState.IsValid)
            {
                sub_Step.Active = true;
                db.Sub_Step.Add(sub_Step);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Step_ID = new SelectList(db.Steps, "Step_ID", "Name", sub_Step.Step_ID);
            return View(sub_Step);
        }

        // GET: Sub_Step/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sub_Step sub_Step = db.Sub_Step.Find(id);
            if (sub_Step == null)
            {
                return HttpNotFound();
            }
            ViewBag.Step_ID = new SelectList(db.Steps, "Step_ID", "Name", sub_Step.Step_ID);
            return View(sub_Step);
        }

        // POST: Sub_Step/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SubStep_ID,Name,Description,Step_ID,Order_By")] Sub_Step sub_Step)
        {
            if (ModelState.IsValid)
            {
                sub_Step.Active = true;
                db.Entry(sub_Step).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Step_ID = new SelectList(db.Steps, "Step_ID", "Name", sub_Step.Step_ID);
            return View(sub_Step);
        }

        // GET: Sub_Step/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sub_Step sub_Step = db.Sub_Step.Find(id);
            if (sub_Step == null)
            {
                return HttpNotFound();
            }
            return View(sub_Step);
        }

        // POST: Sub_Step/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Sub_Step sub_Step = db.Sub_Step.Find(id);
            sub_Step.Active = false;
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
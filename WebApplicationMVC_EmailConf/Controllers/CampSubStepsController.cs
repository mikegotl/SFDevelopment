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
    public class CampSubStepsController : Controller
    {
        private SpeechSageDBEntities db = new SpeechSageDBEntities();

        // GET: CampSubSteps
        public ActionResult Index()
        {
            var campSubSteps = db.CampSubSteps.Include(c => c.CampStep);
            return View(campSubSteps.ToList());
        }

        // GET: CampSubSteps/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CampSubStep campSubStep = db.CampSubSteps.Find(id);
            if (campSubStep == null)
            {
                return HttpNotFound();
            }
            return View(campSubStep);
        }

        // GET: CampSubSteps/Create
        public ActionResult Create()
        {
            ViewBag.StepID = new SelectList(db.CampSteps, "StepID", "Name");
            return View();
        }

        // POST: CampSubSteps/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "SubStepID,Name,StepID,SubStepContent")] CampSubStep campSubStep)
        {
            if (ModelState.IsValid)
            {
                db.CampSubSteps.Add(campSubStep);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.StepID = new SelectList(db.CampSteps, "StepID", "Name", campSubStep.StepID);
            return View(campSubStep);
        }

        // GET: CampSubSteps/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CampSubStep campSubStep = db.CampSubSteps.Find(id);
            if (campSubStep == null)
            {
                return HttpNotFound();
            }
            ViewBag.StepID = new SelectList(db.CampSteps, "StepID", "Name", campSubStep.StepID);
            return View(campSubStep);
        }

        // POST: CampSubSteps/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "SubStepID,Name,StepID,SubStepContent")] CampSubStep campSubStep)
        {
            if (ModelState.IsValid)
            {
                db.Entry(campSubStep).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.StepID = new SelectList(db.CampSteps, "StepID", "Name", campSubStep.StepID);
            return View(campSubStep);
        }

        // GET: CampSubSteps/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CampSubStep campSubStep = db.CampSubSteps.Find(id);
            if (campSubStep == null)
            {
                return HttpNotFound();
            }
            return View(campSubStep);
        }

        // POST: CampSubSteps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CampSubStep campSubStep = db.CampSubSteps.Find(id);
            db.CampSubSteps.Remove(campSubStep);
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
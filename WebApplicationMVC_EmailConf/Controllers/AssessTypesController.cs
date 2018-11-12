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
    public class AssessTypesController : Controller
    {
        private SpeechSageDBEntities db = new SpeechSageDBEntities();

        // GET: AssessTypes
        public ActionResult Index()
        {
            return View(db.AssessTypes.ToList());
        }

        // GET: AssessTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AssessType assessType = db.AssessTypes.Find(id);
            if (assessType == null)
            {
                return HttpNotFound();
            }
            return View(assessType);
        }

        // GET: AssessTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AssessTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AssessTypeID,Name,Description")] AssessType assessType)
        {
            if (ModelState.IsValid)
            {
                db.AssessTypes.Add(assessType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(assessType);
        }

        // GET: AssessTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AssessType assessType = db.AssessTypes.Find(id);
            if (assessType == null)
            {
                return HttpNotFound();
            }
            return View(assessType);
        }

        // POST: AssessTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AssessTypeID,Name,Description")] AssessType assessType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(assessType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(assessType);
        }

        // GET: AssessTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AssessType assessType = db.AssessTypes.Find(id);
            if (assessType == null)
            {
                return HttpNotFound();
            }
            return View(assessType);
        }

        // POST: AssessTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AssessType assessType = db.AssessTypes.Find(id);
            db.AssessTypes.Remove(assessType);
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

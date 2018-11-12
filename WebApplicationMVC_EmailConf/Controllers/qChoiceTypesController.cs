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
    public class qChoiceTypesController : Controller
    {
        private SpeechSageDBEntities db = new SpeechSageDBEntities();

        // GET: qChoiceTypes
        public ActionResult Index()
        {
            return View(db.qChoiceTypes.ToList());
        }

        // GET: qChoiceTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            qChoiceType qChoiceType = db.qChoiceTypes.Find(id);
            if (qChoiceType == null)
            {
                return HttpNotFound();
            }
            return View(qChoiceType);
        }

        // GET: qChoiceTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: qChoiceTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "choiceTypeID,name,description")] qChoiceType qChoiceType)
        {
            if (ModelState.IsValid)
            {
                db.qChoiceTypes.Add(qChoiceType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(qChoiceType);
        }

        // GET: qChoiceTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            qChoiceType qChoiceType = db.qChoiceTypes.Find(id);
            if (qChoiceType == null)
            {
                return HttpNotFound();
            }
            return View(qChoiceType);
        }

        // POST: qChoiceTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "choiceTypeID,name,description")] qChoiceType qChoiceType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(qChoiceType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(qChoiceType);
        }

        // GET: qChoiceTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            qChoiceType qChoiceType = db.qChoiceTypes.Find(id);
            if (qChoiceType == null)
            {
                return HttpNotFound();
            }
            return View(qChoiceType);
        }

        // POST: qChoiceTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            qChoiceType qChoiceType = db.qChoiceTypes.Find(id);
            db.qChoiceTypes.Remove(qChoiceType);
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

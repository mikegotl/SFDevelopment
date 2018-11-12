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
    public class qChoicesController : Controller
    {
        private SpeechSageDBEntities db = new SpeechSageDBEntities();

        // GET: qChoices
        public ActionResult Index()
        {
            return View(db.qChoices.ToList());
        }

        // GET: qChoices/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            qChoice qChoice = db.qChoices.Find(id);
            if (qChoice == null)
            {
                return HttpNotFound();
            }
            return View(qChoice);
        }

        // GET: qChoices/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: qChoices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "choiceID,name,description")] qChoice qChoice)
        {
            if (ModelState.IsValid)
            {
                db.qChoices.Add(qChoice);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(qChoice);
        }

        // GET: qChoices/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            qChoice qChoice = db.qChoices.Find(id);
            if (qChoice == null)
            {
                return HttpNotFound();
            }
            return View(qChoice);
        }

        // POST: qChoices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "choiceID,name,description")] qChoice qChoice)
        {
            if (ModelState.IsValid)
            {
                db.Entry(qChoice).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(qChoice);
        }

        // GET: qChoices/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            qChoice qChoice = db.qChoices.Find(id);
            if (qChoice == null)
            {
                return HttpNotFound();
            }
            return View(qChoice);
        }

        // POST: qChoices/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            qChoice qChoice = db.qChoices.Find(id);
            db.qChoices.Remove(qChoice);
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

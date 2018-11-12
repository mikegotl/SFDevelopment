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
    public class qQuestionsController : Controller
    {
        private SpeechSageDBEntities db = new SpeechSageDBEntities();

        // GET: qQuestions
        public ActionResult Index()
        {
            return View(db.qQuestions.ToList());
        }

        // GET: qQuestions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            qQuestion qQuestion = db.qQuestions.Find(id);
            if (qQuestion == null)
            {
                return HttpNotFound();
            }
            return View(qQuestion);
        }

        // GET: qQuestions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: qQuestions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "questionID,Name,Description")] qQuestion qQuestion)
        {
            if (ModelState.IsValid)
            {
                db.qQuestions.Add(qQuestion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(qQuestion);
        }

        // GET: qQuestions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            qQuestion qQuestion = db.qQuestions.Find(id);
            if (qQuestion == null)
            {
                return HttpNotFound();
            }
            return View(qQuestion);
        }

        // POST: qQuestions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "questionID,Name,Description")] qQuestion qQuestion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(qQuestion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(qQuestion);
        }

        // GET: qQuestions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            qQuestion qQuestion = db.qQuestions.Find(id);
            if (qQuestion == null)
            {
                return HttpNotFound();
            }
            return View(qQuestion);
        }

        // POST: qQuestions/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            qQuestion qQuestion = db.qQuestions.Find(id);
            db.qQuestions.Remove(qQuestion);
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

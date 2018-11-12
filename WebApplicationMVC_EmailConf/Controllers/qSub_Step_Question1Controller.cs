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
    public class qSub_Step_Question1Controller : Controller
    {
        private SpeechSageDBEntities db = new SpeechSageDBEntities();

        // GET: qSub_Step_Question1
        public ActionResult Index()
        {
            var qSub_Step_Question = db.qSub_Step_Question.Include(q => q.qQuestion).Include(q => q.Sub_Step);
            return View(qSub_Step_Question.ToList());
        }

        // GET: qSub_Step_Question1/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            qSub_Step_Question qSub_Step_Question = db.qSub_Step_Question.Find(id);
            if (qSub_Step_Question == null)
            {
                return HttpNotFound();
            }
            return View(qSub_Step_Question);
        }

        // GET: qSub_Step_Question1/Create
        public ActionResult Create()
        {
            ViewBag.questionID = new SelectList(db.qQuestions, "questionID", "Name");
            ViewBag.subStep_ID = new SelectList(db.Sub_Step, "SubStep_ID", "Name");
            return View();
        }

        // POST: qSub_Step_Question1/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "subStepQuestionID,questionID,subStep_ID,Active")] qSub_Step_Question qSub_Step_Question)
        {
            if (ModelState.IsValid)
            {
                db.qSub_Step_Question.Add(qSub_Step_Question);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.questionID = new SelectList(db.qQuestions, "questionID", "Name", qSub_Step_Question.questionID);
            ViewBag.subStep_ID = new SelectList(db.Sub_Step, "SubStep_ID", "Name", qSub_Step_Question.subStep_ID);
            return View(qSub_Step_Question);
        }

        // GET: qSub_Step_Question1/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            qSub_Step_Question qSub_Step_Question = db.qSub_Step_Question.Find(id);
            if (qSub_Step_Question == null)
            {
                return HttpNotFound();
            }
            ViewBag.questionID = new SelectList(db.qQuestions, "questionID", "Name", qSub_Step_Question.questionID);
            ViewBag.subStep_ID = new SelectList(db.Sub_Step, "SubStep_ID", "Name", qSub_Step_Question.subStep_ID);
            return View(qSub_Step_Question);
        }

        // POST: qSub_Step_Question1/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "subStepQuestionID,questionID,subStep_ID,Active")] qSub_Step_Question qSub_Step_Question)
        {
            if (ModelState.IsValid)
            {
                db.Entry(qSub_Step_Question).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.questionID = new SelectList(db.qQuestions, "questionID", "Name", qSub_Step_Question.questionID);
            ViewBag.subStep_ID = new SelectList(db.Sub_Step, "SubStep_ID", "Name", qSub_Step_Question.subStep_ID);
            return View(qSub_Step_Question);
        }

        // GET: qSub_Step_Question1/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            qSub_Step_Question qSub_Step_Question = db.qSub_Step_Question.Find(id);
            if (qSub_Step_Question == null)
            {
                return HttpNotFound();
            }
            return View(qSub_Step_Question);
        }

        // POST: qSub_Step_Question1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            qSub_Step_Question qSub_Step_Question = db.qSub_Step_Question.Find(id);
            db.qSub_Step_Question.Remove(qSub_Step_Question);
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

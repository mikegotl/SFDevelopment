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
    public class qSub_Step_QuestionController : Controller
    {
        private SpeechSageDBEntities db = new SpeechSageDBEntities();

        // GET: qSub_Step_Question
        public ActionResult Index(int? ss_id)
        {
            List<qSub_Step_Question> qSub_Step_Questions = new List<Models.qSub_Step_Question>();
            if (ss_id != null)
            {
                qSub_Step_Questions = db.qSub_Step_Question.Include(q => q.qQuestion).Include(q => q.Sub_Step).Where(m => m.subStep_ID == ss_id).ToList<qSub_Step_Question>();
                ViewBag.Scratch = false;
            }
            else {
                qSub_Step_Questions = db.qSub_Step_Question.Include(q => q.qQuestion).Include(q => q.Sub_Step).ToList<qSub_Step_Question>();
                ViewBag.Scratch = true;
            }
            ViewBag.ss_id = ss_id;
            return View(qSub_Step_Questions);
        }

        // GET: qSub_Step_Question/Details/5
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

        // GET: qSub_Step_Question/Create
        public ActionResult Create(int? ss_id)
        {
            if (ss_id != null)
            {
                ViewBag.questionID = new SelectList(db.qQuestions, "questionID", "Name");
                ViewBag.subStepID = ss_id;
                ViewBag.SubStepName = db.Sub_Step.Find(ss_id).Name;

                ViewBag.Scratch = false;
            }
            else {
                ViewBag.questionID = new SelectList(db.qQuestions, "questionID", "Name");
                ViewBag.subStep_ID = new SelectList(db.Sub_Step, "SubStep_ID", "Name");

                ViewBag.Scratch = true;
            }

            return View();
        }

        // POST: qSub_Step_Question/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "subStepQuestionID,questionID,subStep_ID")] qSub_Step_Question qSub_Step_Question, FormCollection fc)
        {
            int _ss_id = 0;

            if (ModelState.IsValid)
            {
                _ss_id = Convert.ToInt16(fc["subStepID"]);

                if (_ss_id != null && _ss_id > 0)
                {
                    qSub_Step_Question.subStep_ID = _ss_id;
                    qSub_Step_Question.Active = true;
                    db.qSub_Step_Question.Add(qSub_Step_Question);
                    db.SaveChanges();
                    return RedirectToAction("Index", new { ss_id = _ss_id });
                }
                else {
                    db.qSub_Step_Question.Add(qSub_Step_Question);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            ViewBag.questionID = new SelectList(db.qQuestions, "questionID", "Name", qSub_Step_Question.questionID);
            ViewBag.subStep_ID = new SelectList(db.Sub_Step, "SubStep_ID", "Name", qSub_Step_Question.subStep_ID);
            return View("Index/" + _ss_id);
        }

        // GET: qSub_Step_Question/Edit/5
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

        // POST: qSub_Step_Question/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
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

        // GET: qSub_Step_Question/Delete/5
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

        // POST: qSub_Step_Question/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
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
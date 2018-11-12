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
    public class qQuestionChoicesController : Controller
    {
        private SpeechSageDBEntities db = new SpeechSageDBEntities();

        // GET: qQuestionChoices
        public ActionResult Index(int? ssq_id)
        {
            List<qQuestionChoice> qQuestionChoices = new List<qQuestionChoice>();
            if (ssq_id != null)
            {
                ViewBag.ssq_id = ssq_id;

                qQuestionChoices = db.qQuestionChoices.Include(q => q.qChoice).Include(q => q.qChoiceType).Include(q => q.qSub_Step_Question).Where(m => m.qSub_Step_Question.subStepQuestionID == ssq_id).ToList<qQuestionChoice>();
                if (qQuestionChoices.Count == 0)
                {
                    ViewBag.NonFound = true;
                    qQuestionChoices = db.qQuestionChoices.Include(q => q.qChoice).Include(q => q.qChoiceType).Include(q => q.qSub_Step_Question).ToList<qQuestionChoice>();
                }
                return View(qQuestionChoices);
            }
            else {
                qQuestionChoices = db.qQuestionChoices.Include(q => q.qChoice).Include(q => q.qChoiceType).Include(q => q.qSub_Step_Question).ToList<qQuestionChoice>();
            }

            return View(qQuestionChoices);
        }

        // GET: qQuestionChoices/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            qQuestionChoice qQuestionChoice = db.qQuestionChoices.Find(id);
            if (qQuestionChoice == null)
            {
                return HttpNotFound();
            }
            return View(qQuestionChoice);
        }

        // GET: qQuestionChoices/Create
        public ActionResult Create(int? ssq_id)
        {
            ViewBag.choiceID = new SelectList(db.qChoices, "choiceID", "name");
            ViewBag.choiceTypeID = new SelectList(db.qChoiceTypes, "choiceTypeID", "name");

            List<SelectListItem> ssqItems = new List<SelectListItem>();

            //List<qSub_Step_Question> ssqs = db.qSub_Step_Question.ToList<qSub_Step_Question>();

            //foreach (qSub_Step_Question q in ssqs)
            //{
            //    SelectListItem i = new SelectListItem();
            //    i.Text = "(SS) " + q.Sub_Step.Name + " (Question) " + q.qQuestion.Name;
            //    i.Value = q.subStepQuestionID.ToString();

            //    ssqItems.Add(i);
            //}

            //ViewBag.subStepQuestionID = ssqItems;
            ViewBag.ssq_id = ssq_id;
            ViewBag.subStepQuestionName = db.qSub_Step_Question.Where(m => m.subStepQuestionID == ssq_id).FirstOrDefault().qQuestion.Name;
            return View();
        }

        // POST: qQuestionChoices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "questionChoiceID,choiceID,subStepQuestionID,choiceTypeID")] qQuestionChoice qQuestionChoice, FormCollection fc)
        {
            int _ssq_id = 0;

            if (ModelState.IsValid)
            {
                _ssq_id = Convert.ToInt16(fc["ssq_id"]);
                qQuestionChoice.subStepQuestionID = _ssq_id;

                db.qQuestionChoices.Add(qQuestionChoice);
                db.SaveChanges();
                return RedirectToAction("Index", new { ssq_id = _ssq_id });
            }

            ViewBag.choiceID = new SelectList(db.qChoices, "choiceID", "name", qQuestionChoice.choiceID);
            ViewBag.choiceTypeID = new SelectList(db.qChoiceTypes, "choiceTypeID", "name", qQuestionChoice.choiceTypeID);
            ViewBag.subStepQuestionID = new SelectList(db.qSub_Step_Question, "subStepQuestionID", "subStepQuestionID", qQuestionChoice.subStepQuestionID);
            return View(qQuestionChoice);
        }

        // GET: qQuestionChoices/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            qQuestionChoice qQuestionChoice = db.qQuestionChoices.Find(id);
            if (qQuestionChoice == null)
            {
                return HttpNotFound();
            }
            ViewBag.choiceID = new SelectList(db.qChoices, "choiceID", "name", qQuestionChoice.choiceID);
            ViewBag.choiceTypeID = new SelectList(db.qChoiceTypes, "choiceTypeID", "name", qQuestionChoice.choiceTypeID);
            ViewBag.subStepQuestionID = new SelectList(db.qSub_Step_Question, "subStepQuestionID", "qQuestion.Name", qQuestionChoice.subStepQuestionID);
            return View(qQuestionChoice);
        }

        // POST: qQuestionChoices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]-
        public ActionResult Edit([Bind(Include = "questionChoiceID,choiceID,subStepQuestionID,choiceTypeID")] qQuestionChoice qQuestionChoice)
        {
            if (ModelState.IsValid)
            {
                db.Entry(qQuestionChoice).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.choiceID = new SelectList(db.qChoices, "choiceID", "name", qQuestionChoice.choiceID);
            ViewBag.choiceTypeID = new SelectList(db.qChoiceTypes, "choiceTypeID", "name", qQuestionChoice.choiceTypeID);
            ViewBag.subStepQuestionID = new SelectList(db.qSub_Step_Question, "subStepQuestionID", "subStepQuestionID", qQuestionChoice.subStepQuestionID);
            return View(qQuestionChoice);
        }

        // GET: qQuestionChoices/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            qQuestionChoice qQuestionChoice = db.qQuestionChoices.Find(id);
            if (qQuestionChoice == null)
            {
                return HttpNotFound();
            }
            return View(qQuestionChoice);
        }

        // POST: qQuestionChoices/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            qQuestionChoice qQuestionChoice = db.qQuestionChoices.Find(id);
            db.qQuestionChoices.Remove(qQuestionChoice);
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
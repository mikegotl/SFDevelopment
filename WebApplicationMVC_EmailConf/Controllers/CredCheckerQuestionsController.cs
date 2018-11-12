using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebApplicationMVC_EmailConf.Models;

namespace WebApplicationMVC_EmailConf.Controllers
{
    public class CredCheckerQuestionsController : Controller
    {
        private SpeechSageDBEntities db = new SpeechSageDBEntities();

        #region READS

        // GET: CredCheckerQuestions
        public ActionResult Index()
        {
            return View(db.CredCheckerQuestions.OrderBy(o => o.QuestionNumber).ToList());
        }

        // GET: CredCheckerQuestions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CredCheckerQuestion credCheckerQuestion = db.CredCheckerQuestions.Find(id);
            if (credCheckerQuestion == null)
            {
                return HttpNotFound();
            }
            return View(credCheckerQuestion);
        }

        #endregion READS

        #region CREATES

        // GET: CredCheckerQuestions/Create
        public ActionResult Create()
        {
            List<CredCheckerQuestion> questions = db.CredCheckerQuestions.ToList<CredCheckerQuestion>();
            CredCheckerQuestion defQ = new CredCheckerQuestion();
            defQ.CCQID = 0;
            defQ.Question = "None";

            questions.Insert(0, defQ);

            ViewBag.ParentQID = new SelectList(questions, "CCQID", "Question");

            return View();
        }

        // POST: CredCheckerQuestions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CCQID,Question,QuestionNumber,ParentQID")] CredCheckerQuestion credCheckerQuestion)
        {
            if (ModelState.IsValid)
            {
                if (credCheckerQuestion.ParentQID == 0) { credCheckerQuestion.ParentQID = null; }

                db.CredCheckerQuestions.Add(credCheckerQuestion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(credCheckerQuestion);
        }

        #endregion CREATES

        #region EDITS

        // GET: CredCheckerQuestions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CredCheckerQuestion credCheckerQuestion = db.CredCheckerQuestions.Find(id);
            if (credCheckerQuestion == null)
            {
                return HttpNotFound();
            }

            List<CredCheckerQuestion> questions = db.CredCheckerQuestions.ToList<CredCheckerQuestion>();
            CredCheckerQuestion defQ = new CredCheckerQuestion();
            defQ.CCQID = 0;
            defQ.Question = "None";

            questions.Insert(0, defQ);

            ViewBag.ParentQID = new SelectList(questions, "CCQID", "Question", credCheckerQuestion.ParentQID);

            //ViewBag.ParentQID =

            return View(credCheckerQuestion);
        }

        // POST: CredCheckerQuestions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CCQID,Question,QuestionNumber,ParentQID")] CredCheckerQuestion credCheckerQuestion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(credCheckerQuestion).State = EntityState.Modified;

                if (credCheckerQuestion.ParentQID == 0) { credCheckerQuestion.ParentQID = null; }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(credCheckerQuestion);
        }

        #endregion EDITS

        #region DELETES

        // GET: CredCheckerQuestions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CredCheckerQuestion credCheckerQuestion = db.CredCheckerQuestions.Find(id);
            if (credCheckerQuestion == null)
            {
                return HttpNotFound();
            }
            return View(credCheckerQuestion);
        }

        // POST: CredCheckerQuestions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CredCheckerQuestion credCheckerQuestion = db.CredCheckerQuestions.Find(id);
            db.CredCheckerQuestions.Remove(credCheckerQuestion);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        #endregion DELETES

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
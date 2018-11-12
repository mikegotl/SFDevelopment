using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebApplicationMVC_EmailConf.Models;

namespace WebApplicationMVC_EmailConf.Controllers
{
    public class CredCheckerChoicesController : Controller
    {
        private SpeechSageDBEntities db = new SpeechSageDBEntities();

        #region LISTS

        // GET: CredCheckerChoices
        public ActionResult Index()
        {
            var credCheckerChoices = db.CredCheckerChoices.Include(c => c.CredCheckerQuestion).OrderBy(o => o.CredCheckerQuestion.CCQID);
            return View(credCheckerChoices.ToList());
        }

        // GET: CredCheckerChoices/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CredCheckerChoice credCheckerChoice = db.CredCheckerChoices.Find(id);
            if (credCheckerChoice == null)
            {
                return HttpNotFound();
            }
            return View(credCheckerChoice);
        }

        #endregion LISTS

        #region CREATES

        // GET: CredCheckerChoices/Create
        public ActionResult Create()
        {
            ViewBag.CCQID = new SelectList(db.CredCheckerQuestions, "CCQID", "Question");
            return View();
        }

        // POST: CredCheckerChoices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CCCID,Choice,CCQID,Points")] CredCheckerChoice credCheckerChoice)
        {
            if (ModelState.IsValid)
            {
                db.CredCheckerChoices.Add(credCheckerChoice);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CCQID = new SelectList(db.CredCheckerQuestions, "CCQID", "Question", credCheckerChoice.CCQID);
            return View(credCheckerChoice);
        }

        #endregion CREATES

        #region EDITS

        // GET: CredCheckerChoices/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CredCheckerChoice credCheckerChoice = db.CredCheckerChoices.Find(id);
            if (credCheckerChoice == null)
            {
                return HttpNotFound();
            }
            ViewBag.CCQID = new SelectList(db.CredCheckerQuestions, "CCQID", "Question", credCheckerChoice.CCQID);
            return View(credCheckerChoice);
        }

        // POST: CredCheckerChoices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CCCID,Choice,CCQID,Points")] CredCheckerChoice credCheckerChoice)
        {
            if (ModelState.IsValid)
            {
                db.Entry(credCheckerChoice).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CCQID = new SelectList(db.CredCheckerQuestions, "CCQID", "Question", credCheckerChoice.CCQID);
            return View(credCheckerChoice);
        }

        #endregion EDITS

        #region DELETES

        // GET: CredCheckerChoices/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CredCheckerChoice credCheckerChoice = db.CredCheckerChoices.Find(id);
            if (credCheckerChoice == null)
            {
                return HttpNotFound();
            }
            return View(credCheckerChoice);
        }

        // POST: CredCheckerChoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CredCheckerChoice credCheckerChoice = db.CredCheckerChoices.Find(id);
            db.CredCheckerChoices.Remove(credCheckerChoice);
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
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebApplicationMVC_EmailConf.Models;

namespace WebApplicationMVC_EmailConf.Controllers
{
    public class CredCheckersController : Controller
    {
        private SpeechSageDBEntities db = new SpeechSageDBEntities();

        #region READ

        // GET: CredCheckers
        public ActionResult Index()
        {
            return View(db.CredCheckers.ToList());
        }

        // GET: CredCheckers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CredChecker credChecker = db.CredCheckers.Find(id);
            if (credChecker == null)
            {
                return HttpNotFound();
            }
            return View(credChecker);
        }

        #endregion READ

        #region CREATE

        // GET: CredCheckers/Create
        public ActionResult Create()
        {
            ViewBag.TopQuestions = db.CredCheckerQuestions.Where(m => m.ParentQID == null).OrderBy(o => o.QuestionNumber).ToList<CredCheckerQuestion>();

            return View();
        }

        // POST: CredCheckers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CCID,SpeechID")] CredChecker credChecker, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                db.CredCheckers.Add(credChecker);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(credChecker);
        }

        #endregion CREATE

        #region EDIT

        // GET: CredCheckers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CredChecker credChecker = db.CredCheckers.Find(id);
            if (credChecker == null)
            {
                return HttpNotFound();
            }
            return View(credChecker);
        }

        // POST: CredCheckers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CCID,SpeechID")] CredChecker credChecker)
        {
            if (ModelState.IsValid)
            {
                db.Entry(credChecker).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(credChecker);
        }

        #endregion EDIT

        #region DELETE

        // GET: CredCheckers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CredChecker credChecker = db.CredCheckers.Find(id);
            if (credChecker == null)
            {
                return HttpNotFound();
            }
            return View(credChecker);
        }

        // POST: CredCheckers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CredChecker credChecker = db.CredCheckers.Find(id);
            db.CredCheckers.Remove(credChecker);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        #endregion DELETE

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
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebApplicationMVC_EmailConf.Models;

namespace WebApplicationMVC_EmailConf.Controllers
{
    public class SourcesController : Controller
    {
        private SpeechSageDBEntities db = new SpeechSageDBEntities();


        // GET: Sources
        public ActionResult Index()
        {
            var sources = db.Sources.Include(s => s.Speech).Include(s => s.SourceType);
            return View(sources.ToList());
        }

        public ActionResult GetSourceList(int SpeechID)
        {
            List<Source> sources = db.Sources.Include(s => s.Speech).Include(s => s.SourceType).Where(m => m.SpeechID == SpeechID).ToList<Source>();

            //ViewBag.TopQuestions = db.CredCheckerQuestions.Where(m => m.ParentQID == null).OrderBy(o => o.QuestionNumber).ToList<CredCheckerQuestion>();

            return PartialView("_Sources", sources);
        }

        // GET: Sources/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Source source = db.Sources.Find(id);
            if (source == null)
            {
                return HttpNotFound();
            }
            return View(source);
        }

        // GET: Sources/Create
        public ActionResult Create()
        {
            ViewBag.SourceTypeID = new SelectList(db.SourceTypes, "SourceTypeID", "Name");
            return View();
        }

        // POST: Sources/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SourceID,When,Who,What,SpeechID,CreateDate,SourceTypeID")] Source source)
        {
            if (ModelState.IsValid)
            {
                db.Sources.Add(source);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SourceTypeID = new SelectList(db.SourceTypes, "SourceTypeID", "Name", source.SourceTypeID);
            return View(source);
        }

        // GET: Sources/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Source source = db.Sources.Find(id);
            if (source == null)
            {
                return HttpNotFound();
            }
            ViewBag.SourceTypeID = new SelectList(db.SourceTypes, "SourceTypeID", "Name", source.SourceTypeID);
            return View(source);
        }

        // POST: Sources/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SourceID,When,Who,What,SpeechID,CreateDate,SourceTypeID")] Source source)
        {
            if (ModelState.IsValid)
            {
                db.Entry(source).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SourceTypeID = new SelectList(db.SourceTypes, "SourceTypeID", "Name", source.SourceTypeID);
            return View(source);
        }

        // GET: Sources/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Source source = db.Sources.Find(id);
            if (source == null)
            {
                return HttpNotFound();
            }
            return View(source);
        }

        public string DeleteSource(int SourceID)
        {
            Source s = db.Sources.Find(SourceID);
            db.Sources.Remove(s);
            db.SaveChanges();
            return "Successful";
        }

        public string EditSource(int SourceID, string Who, string When, string What, decimal cScore) {
            Source s = db.Sources.Find(SourceID);
            s.Who = Who;
            s.When = When;
            s.What = What;
            s.CredScore = cScore;
            db.SaveChanges();
            return "Successful";
        }

        // POST: Sources/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Source source = db.Sources.Find(id);
            db.Sources.Remove(source);
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
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
    public class Speeches1Controller : Controller
    {
        private SpeechSageDBEntities db = new SpeechSageDBEntities();

        // GET: Speeches1
        public ActionResult Index()
        {
            var speeches = db.Speeches.Include(s => s.Citation_Styles).Include(s => s.Member).Include(s => s.Organizational_Type).Include(s => s.Source_Reference_Type);
            return View(speeches.ToList());
        }

        // GET: Speeches1/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Speech speech = db.Speeches.Find(id);
            if (speech == null)
            {
                return HttpNotFound();
            }
            return View(speech);
        }

        // GET: Speeches1/Create
        public ActionResult Create()
        {
            ViewBag.CitationStyle_ID = new SelectList(db.Citation_Styles, "CitationStyle_ID", "Name");
            ViewBag.memberID = new SelectList(db.Members, "MemberID", "UserID");
            ViewBag.OrgType_ID = new SelectList(db.Organizational_Type, "OrgType_ID", "Name");
            ViewBag.SourceReferenceType_ID = new SelectList(db.Source_Reference_Type, "SourceReferenceType_ID", "Name");
            return View();
        }

        // POST: Speeches1/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Speech_ID,Topic,SourceReferenceType_ID,CitationStyle_ID,OrgType_ID,Research_Total,Entered_Date,Updated_Date,Filename,memberID")] Speech speech)
        {
            if (ModelState.IsValid)
            {
                db.Speeches.Add(speech);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CitationStyle_ID = new SelectList(db.Citation_Styles, "CitationStyle_ID", "Name", speech.CitationStyle_ID);
            ViewBag.memberID = new SelectList(db.Members, "MemberID", "UserID", speech.memberID);
            ViewBag.OrgType_ID = new SelectList(db.Organizational_Type, "OrgType_ID", "Name", speech.OrgType_ID);
            ViewBag.SourceReferenceType_ID = new SelectList(db.Source_Reference_Type, "SourceReferenceType_ID", "Name", speech.SourceReferenceType_ID);
            return View(speech);
        }

        // GET: Speeches1/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Speech speech = db.Speeches.Find(id);
            if (speech == null)
            {
                return HttpNotFound();
            }
            ViewBag.CitationStyle_ID = new SelectList(db.Citation_Styles, "CitationStyle_ID", "Name", speech.CitationStyle_ID);
            ViewBag.memberID = new SelectList(db.Members, "MemberID", "UserID", speech.memberID);
            ViewBag.OrgType_ID = new SelectList(db.Organizational_Type, "OrgType_ID", "Name", speech.OrgType_ID);
            ViewBag.SourceReferenceType_ID = new SelectList(db.Source_Reference_Type, "SourceReferenceType_ID", "Name", speech.SourceReferenceType_ID);
            return View(speech);
        }

        // POST: Speeches1/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Speech_ID,Topic,SourceReferenceType_ID,CitationStyle_ID,OrgType_ID,Research_Total,Entered_Date,Updated_Date,Filename,memberID")] Speech speech)
        {
            if (ModelState.IsValid)
            {
                db.Entry(speech).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CitationStyle_ID = new SelectList(db.Citation_Styles, "CitationStyle_ID", "Name", speech.CitationStyle_ID);
            ViewBag.memberID = new SelectList(db.Members, "MemberID", "UserID", speech.memberID);
            ViewBag.OrgType_ID = new SelectList(db.Organizational_Type, "OrgType_ID", "Name", speech.OrgType_ID);
            ViewBag.SourceReferenceType_ID = new SelectList(db.Source_Reference_Type, "SourceReferenceType_ID", "Name", speech.SourceReferenceType_ID);
            return View(speech);
        }

        // GET: Speeches1/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Speech speech = db.Speeches.Find(id);
            if (speech == null)
            {
                return HttpNotFound();
            }
            return View(speech);
        }

        // POST: Speeches1/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Speech speech = db.Speeches.Find(id);
            db.Speeches.Remove(speech);
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

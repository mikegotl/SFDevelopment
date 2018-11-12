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
    public class AssessesController : Controller
    {
        private SpeechSageDBEntities db = new SpeechSageDBEntities();

        // GET: Assesses
        public ActionResult Index()
        {
            var assesses = db.Assesses.Include(a => a.AssessType);
            return View(assesses.ToList());
        }

        // GET: Assesses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assess assess = db.Assesses.Find(id);
            if (assess == null)
            {
                return HttpNotFound();
            }
            return View(assess);
        }

        // GET: Assesses/Create
        public ActionResult Create()
        {
            ViewBag.AssessTypeID = new SelectList(db.AssessTypes, "AssessTypeID", "Name");
            return View();
        }

        // POST: Assesses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AssessID,Name,AssessTypeID")] Assess assess)
        {
            if (ModelState.IsValid)
            {
                db.Assesses.Add(assess);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AssessTypeID = new SelectList(db.AssessTypes, "AssessTypeID", "Name", assess.AssessTypeID);
            return View(assess);
        }

        // GET: Assesses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assess assess = db.Assesses.Find(id);
            if (assess == null)
            {
                return HttpNotFound();
            }
            ViewBag.AssessTypeID = new SelectList(db.AssessTypes, "AssessTypeID", "Name", assess.AssessTypeID);
            return View(assess);
        }

        // POST: Assesses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AssessID,Name,AssessTypeID")] Assess assess)
        {
            if (ModelState.IsValid)
            {
                db.Entry(assess).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AssessTypeID = new SelectList(db.AssessTypes, "AssessTypeID", "Name", assess.AssessTypeID);
            return View(assess);
        }

        // GET: Assesses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assess assess = db.Assesses.Find(id);
            if (assess == null)
            {
                return HttpNotFound();
            }
            return View(assess);
        }

        // POST: Assesses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Assess assess = db.Assesses.Find(id);
            db.Assesses.Remove(assess);
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

        [HttpPost]
        public string SaveMemberAss(int AssessID, int SelectedValue) {
            int MemberID = MyHelpers.LoggedInMember.MemberID;

            //see if member record exists for this assess
            var found = db.MemberAssesses.Where(m => m.MemberID == MemberID && m.AssessID == AssessID);
            if (found.Count() > 0)
            {
                //exists, update
                switch (SelectedValue)
                {
                    case 1:
                        found.First().Strength = true;
                        found.First().Weakness = false;
                        found.First().Unknown = false;
                        break;
                    case 2:
                        found.First().Strength = false;
                        found.First().Weakness = true;
                        found.First().Unknown = false;
                        break;

                    case 3:
                        found.First().Strength = false;
                        found.First().Weakness = false;
                        found.First().Unknown = true;
                        break;
                    default:
                        break;
                }
                db.SaveChanges();
                return "Saved";
            }
            else {
                //not exists, create for selection
                MemberAssess ma = new MemberAssess();
                ma.MemberID = MemberID;
                ma.AssessID = AssessID;
                switch (SelectedValue)
                {
                    case 1:
                        ma.Strength = true;
                        ma.Weakness = false;
                        ma.Unknown = false;
                        break;
                    case 2:
                        ma.Strength = false;
                        ma.Weakness = true;
                        ma.Unknown = false;
                        break;

                    case 3:
                        ma.Strength = false;
                        ma.Weakness = false;
                        ma.Unknown = true;
                        break;
                    default:
                        break;
                }
                db.MemberAssesses.Add(ma);
                db.SaveChanges();
                return "Saved";
            }

        }
    }
}

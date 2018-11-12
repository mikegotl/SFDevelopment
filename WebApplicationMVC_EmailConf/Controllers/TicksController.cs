using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebApplicationMVC_EmailConf.Models;

namespace WebApplicationMVC_EmailConf.Controllers
{
    public class TicksController : Controller
    {
        private SpeechSageDBEntities db = new SpeechSageDBEntities();

        // GET: Ticks
        public ActionResult Index()
        {
            var ticks = db.Ticks.Include(t => t.TickType);
            return View(ticks.ToList());
        }

        // GET: Ticks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tick tick = db.Ticks.Find(id);
            if (tick == null)
            {
                return HttpNotFound();
            }
            return View(tick);
        }

        // GET: Ticks/Create
        public ActionResult Create()
        {
            ViewBag.TickTypeID = new SelectList(db.TickTypes, "TickTypeID", "Name");
            return View();
        }

        // POST: Ticks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TickID,Name,TickTypeID")] Tick tick)
        {
            if (ModelState.IsValid)
            {
                db.Ticks.Add(tick);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TickTypeID = new SelectList(db.TickTypes, "TickTypeID", "Name", tick.TickTypeID);
            return View(tick);
        }

        // GET: Ticks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tick tick = db.Ticks.Find(id);
            if (tick == null)
            {
                return HttpNotFound();
            }
            ViewBag.TickTypeID = new SelectList(db.TickTypes, "TickTypeID", "Name", tick.TickTypeID);
            return View(tick);
        }

        // POST: Ticks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TickID,Name,TickTypeID")] Tick tick)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tick).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TickTypeID = new SelectList(db.TickTypes, "TickTypeID", "Name", tick.TickTypeID);
            return View(tick);
        }

        // GET: Ticks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tick tick = db.Ticks.Find(id);
            if (tick == null)
            {
                return HttpNotFound();
            }
            return View(tick);
        }

        // POST: Ticks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tick tick = db.Ticks.Find(id);
            db.Ticks.Remove(tick);
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
        public string AddToMember(int MemberID, int TickID)
        {
            var found = db.MemberTicks.Where(m => m.MemberID == MemberID && m.TickID == TickID);
            if (found.Count() > 0)
            {
                //Found, do nothing
                return "Tick already saved for member.";
            }
            else {
                //Not Found, add to db
                MemberTick mt = new MemberTick();
                mt.MemberID = MemberID;
                mt.TickID = TickID;

                db.MemberTicks.Add(mt);
                db.SaveChanges();

                return "Successfully saved tick to member";
            }
        }

        [HttpPost]
        public string RemoveFromMember(int MemberID, int TickID) {
            var found = db.MemberTicks.Where(m => m.MemberID == MemberID && m.TickID == TickID);

            if (found.Count() > 0)
            {
                //delete
                db.MemberTicks.Remove(found.First());
                db.SaveChanges();

                return "Successfully deleted tick from member";
            }
            else {
                return "No tick found for member to delete";
            }
        }
    }
}
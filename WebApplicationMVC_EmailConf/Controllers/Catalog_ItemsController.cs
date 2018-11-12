using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.UI;
using WebApplicationMVC_EmailConf.Models;

namespace WebApplicationMVC_EmailConf.Controllers
{
    public class Catalog_ItemsController : Controller
    {
        private SpeechSageDBEntities db = new SpeechSageDBEntities();

        // GET: Catalog_Items
        [Authorize]
        public ActionResult Index()
        {
            var catalog_Items = db.Catalog_Items.Include(c => c.Catalog);
            return View(catalog_Items.ToList());
        }

        // GET: Catalog_Items
        //[OutputCache(Duration = 1000000, VaryByParam = "none", Location = OutputCacheLocation.Server)]
        [Authorize]
        public ActionResult SpeechCatalog()
        {
            var catalog_Items = db.Catalog_Items.Include(c => c.Catalog).Where(c => c.Catalog.Name == "Site");
            return View(catalog_Items.ToList());
        }
        // GET: Catalog_Items
        //[OutputCache(Duration = 1000000, VaryByParam = "none", Location = OutputCacheLocation.Server)]
        [Authorize]
        public ActionResult SpeechCatalogNoSubscriptionItems()
        {
            var catalog_Items = db.Catalog_Items.Include(c => c.Catalog).Where(c=>c.Catalog.Name != "Site");
            return View("SpeechCatalog", catalog_Items.ToList());
        }

        // GET: Catalog_Items/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Catalog_Items catalog_Items = db.Catalog_Items.Find(id);
            if (catalog_Items == null)
            {
                return HttpNotFound();
            }
            return View(catalog_Items);
        }

        // GET: Catalog_Items/Create
        public ActionResult Create()
        {
            ViewBag.CatalogID = new SelectList(db.Catalogs, "CatalogID", "Name");
            return View();
        }

        // POST: Catalog_Items/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Catalog_ItemID,Name,Description,CatalogID,Price,ImageURI")] Catalog_Items catalog_Items)
        {
            if (ModelState.IsValid)
            {
                db.Catalog_Items.Add(catalog_Items);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CatalogID = new SelectList(db.Catalogs, "CatalogID", "Name", catalog_Items.CatalogID);
            return View(catalog_Items);
        }

        // GET: Catalog_Items/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Catalog_Items catalog_Items = db.Catalog_Items.Find(id);
            if (catalog_Items == null)
            {
                return HttpNotFound();
            }
            ViewBag.CatalogID = new SelectList(db.Catalogs, "CatalogID", "Name", catalog_Items.CatalogID);
            return View(catalog_Items);
        }

        // POST: Catalog_Items/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Catalog_ItemID,Name,Description,CatalogID,Price,ImageURI")] Catalog_Items catalog_Items)
        {
            if (ModelState.IsValid)
            {
                db.Entry(catalog_Items).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CatalogID = new SelectList(db.Catalogs, "CatalogID", "Name", catalog_Items.CatalogID);
            return View(catalog_Items);
        }

        // GET: Catalog_Items/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Catalog_Items catalog_Items = db.Catalog_Items.Find(id);
            if (catalog_Items == null)
            {
                return HttpNotFound();
            }
            return View(catalog_Items);
        }

        // POST: Catalog_Items/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Catalog_Items catalog_Items = db.Catalog_Items.Find(id);
            db.Catalog_Items.Remove(catalog_Items);
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
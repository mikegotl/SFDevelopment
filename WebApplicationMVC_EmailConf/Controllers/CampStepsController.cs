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
    public class CampStepsController : Controller
    {
        private SpeechSageDBEntities db = new SpeechSageDBEntities();

        // GET: CampSteps
        public ActionResult Index()
        {
            return View(db.CampSteps.ToList());
        }

        public ActionResult CampView()
        {
            return View(db.CampSteps.ToList());
        }

        // GET: CampSteps/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CampStep campStep = db.CampSteps.Find(id);
            if (campStep == null)
            {
                return HttpNotFound();
            }
            return View(campStep);
        }

        // GET: CampSteps/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CampSteps/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StepID,Name")] CampStep campStep)
        {
            if (ModelState.IsValid)
            {
                db.CampSteps.Add(campStep);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(campStep);
        }

        // GET: CampSteps/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CampStep campStep = db.CampSteps.Find(id);
            if (campStep == null)
            {
                return HttpNotFound();
            }
            return View(campStep);
        }

        // POST: CampSteps/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StepID,Name")] CampStep campStep)
        {
            if (ModelState.IsValid)
            {
                db.Entry(campStep).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(campStep);
        }

        // GET: CampSteps/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CampStep campStep = db.CampSteps.Find(id);
            if (campStep == null)
            {
                return HttpNotFound();
            }
            return View(campStep);
        }

        // POST: CampSteps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CampStep campStep = db.CampSteps.Find(id);
            db.CampSteps.Remove(campStep);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public JsonResult GetTreeData()
        {
            List<CampStep> dbsteps = db.CampSteps.ToList<CampStep>();
            List<Tree_Node> treeNodes = new List<Tree_Node>();
            int nid = 0;

            foreach (CampStep s in dbsteps)
            {
                Tree_Node TN = new Tree_Node();

                TN.Text = s.Name;
                TN.Idx = s.StepID;
                TN.nid = nid;

                //create Nodes Obj for Tree Node
                List<Tree_SubNodes> treeSubNodes = new List<Tree_SubNodes>();

                int sCounter = 0;
                foreach (CampSubStep ss in s.CampSubSteps.ToList<CampSubStep>())
                {
                    sCounter += 1;
                    nid += 1;

                    Tree_SubNodes TsN = new Tree_SubNodes();
                    TsN.text = ss.Name;
                    TsN.Idx = ss.SubStepID;
                    TsN.href = "tabPanel" + ss.StepID.ToString() + "-" + ss.SubStepID.ToString();
                    TsN.nid = nid;
                    treeSubNodes.Add(TsN);
                }

                TN.Nodes = treeSubNodes;

                treeNodes.Add(TN);
                nid += 1;
            }

            return Json(treeNodes, JsonRequestBehavior.AllowGet);
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
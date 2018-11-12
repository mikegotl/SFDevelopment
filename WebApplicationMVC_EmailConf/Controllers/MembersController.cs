using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplicationMVC_EmailConf.Models;

namespace WebApplicationMVC_EmailConf.Controllers
{
    public class MembersController : Controller
    {
        private SpeechSageDBEntities db = new SpeechSageDBEntities();

        #region LISTS

        // GET: Members
        public ActionResult Index()
        {
            var members = db.Members.Include(m => m.Gender1).Include(m => m.AspNetUser);
            return View(members.ToList());
        }

        // GET: Members/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Member member = db.Members.Find(id);
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }

        #endregion LISTS

        #region CREATES

        // GET: Members/Create
        public ActionResult Create()
        {
            ViewBag.Gender = new SelectList(db.Genders, "GenderID", "Name");
            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Email");
            return View();
        }

        // POST: Members/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MemberID,UserID,FirstName,LastName,MiddleInitial,Gender,DOB")] Member member)
        {
            if (ModelState.IsValid)
            {
                db.Members.Add(member);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Gender = new SelectList(db.Genders, "GenderID", "Name", member.Gender);
            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Email", member.UserID);
            return View(member);
        }

        #endregion CREATES

        #region EDITS

        [HttpPost]
        public JsonResult InstitutionSearch(string term)
        {
            var result = db.Institutions.Where(m => m.Name.Contains(term));

            var newResult = from q in result select new { q.Name, q.InstitutionID };

            return Json(newResult, JsonRequestBehavior.AllowGet);
        }

        // GET: Members/Edit/5
        [Authorize]
        public ActionResult Edit()
        {
            Member member = new Member();
            member = db.Members.Include("MemberMedias").Where(m => m.UserID == MyHelpers.UID).First();

            if (member == null)
            {
                return HttpNotFound();
            }

            List<SelectListItem> Levels = new List<SelectListItem>(){
                new SelectListItem() { Text="Please Select", Value="0"},
                new SelectListItem() { Text="Beginner: 1-2 Speeches", Value="1"},
        new SelectListItem() { Text="In Training: 5 + Speeches", Value="2"},
        new SelectListItem() { Text="Intermediate: 10 + Speeches", Value="3"},
        new SelectListItem() { Text="Advanced: 25 + Speeches", Value="4"},
        new SelectListItem() {Text="Expert: 50 + Speeches", Value="5"}
        };

            ViewBag.ExperienceLevel = new SelectList(Levels, "Value", "Text", member.ExperienceLevel);
            ViewBag.Gender = new SelectList(db.Genders, "GenderID", "Name", member.Gender);
            ViewBag.HighestEducationID = new SelectList(db.HighestEducations, "ID", "Name", member.HighestEducationID);

            //Institution
            if (member.Institution != null)
            {
                ViewBag.InstitutionName = member.Institution.Name;
            }

            //Anxiety
            var aQuestions = db.AnxietyQuestions.OrderBy(o => o.ID);
            ViewBag.aQuestions = aQuestions;

            if (MyHelpers.LoggedInMember.MemberType.Name.Contains("Academic"))
            {
                ViewBag.MemberTypeIsAcademic = true;
            }
            else {
                ViewBag.MemberTypeIsAcademic = false;
            } 
            return View(member);
        }

        // POST: Members/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]

        [Authorize]
        public ActionResult Edit([Bind(Include = "MemberID,UserID,FirstName,LastName,Gender,DOB,AnxietyLevel,ExperienceLevel,FirstExperience,SpeakerStrengths,SpeakerPhysicalTicks,ThreeCommunicationGoals,AboutMe,HighSchool,HSGradYear,University,UGradYear,FavJob,FavJobWhy,ProfilePic,Location,State,HighestEducationID,CanShare,Anonymous,MostMemorableStory,MemberTypeID")] Member member, HttpPostedFileBase file, FormCollection fc)
        {
            int year1 = Convert.ToInt16(fc.GetValues("DOB.years")[0]);
            var month1 = Convert.ToInt16(fc.GetValues("DOB.months")[0]);
            var day1 = Convert.ToInt16(fc.GetValues("DOB.days")[0]);

            DateTime dt1 = new DateTime(year1, month1, day1);
            member.DOB = dt1;

            //get institutionID
            var institutionName = fc["ilookup"];
            if (!string.IsNullOrEmpty(institutionName))
            {
                var foundIns = db.Institutions.Where(m => m.Name == institutionName);
                if (foundIns.Count() > 0)
                {
                    member.InstitutionID = foundIns.First().InstitutionID;
                }
                else
                {
                    member.InstitutionID = member.InstitutionID;
                }
            }
            else
            {
                member.InstitutionID = MyHelpers.LoggedInMember.InstitutionID;
            }

            //Save Member (Profile)
            if (ModelState.IsValid)
            {
                db.Entry(member).State = EntityState.Modified;
                db.SaveChanges();

                //save physical pic to images location
                string resultMessage = ProcessPicture(member, file);
                ViewBag.result = resultMessage;
            }
            //return View();
            return RedirectToAction("Edit", "Members");
        }

        #region Profile Picture Functions

        private string ProcessPicture(Member tblMember, HttpPostedFileBase file)
        {
            string resultMessage = "";

            if (file != null)
            {
                string _path = "";
                string _serverPath = Server.MapPath("~/Content/Images/members/profilepics");
                string _fileName = Path.GetFileName(file.FileName);
                string _fileExt = _fileName.Substring(_fileName.Length - 4);

                //from http://www.codeproject.com/Tips/481015/Rename-Resize-Upload-Image-ASP-NET-MVC
                ImageUpload imageUpload = new ImageUpload { Width = 600 }; //set Width here
                ImageResult imageResult = imageUpload.RenameUploadFile(file);

                if (imageResult.Success)
                {
                    //TODO: write the filename to the db
                    //save path for image to membermedia in db
                    MemberMedia mm = new MemberMedia();
                    mm.MemberID = tblMember.MemberID;
                    _path = "/Content/Images/members/profilepics/" + imageResult.ImageName;
                    mm.Path = _path;

                    int countExistingMM = db.MemberMedias.Where(m => m.MemberID == tblMember.MemberID).Count();
                    if (countExistingMM == 0) { mm.PrimaryPic = true; }

                    tblMember.MemberMedias.Add(mm);

                    resultMessage = "Successfully Uploaded Image";
                }
                else
                {
                    // use imageResult.ErrorMessage to show the error
                    resultMessage = "Successfully Uploaded Image";
                }
                db.SaveChanges();
            }
            return resultMessage;
        }

        private void SavePicToImages(HttpPostedFileBase file, long _memberID, ref string _path, string _serverPath, ref string _fileName, string _fileExt)
        {
            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    _fileName = _memberID.ToString() + "_pImage_1" + _fileExt;

                    _path = Path.Combine(_serverPath, _fileName);
                    int i = 1;

                    while (System.IO.File.Exists(_path))
                    {
                        //file with this name already exists
                        int imgIDStart = _fileName.IndexOf("_pImage_") + 8;
                        _fileName = _fileName.Substring(0, imgIDStart) + i.ToString() + _fileExt;

                        //_fileName = _fileName.Substring()
                        _path = Path.Combine(_serverPath, _fileName);

                        i += 1;
                    }

                    file.SaveAs(_path);
                    ViewBag.Message = "Pic uploaded successfully";
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Error:" + ex.Message.ToString();
                }
            }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }
        }

        #endregion Profile Picture Functions

        #endregion EDITS

        #region DELETES

        // GET: Members/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Member member = db.Members.Find(id);
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }

        // POST: Members/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Member member = db.Members.Find(id);
            db.Members.Remove(member);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult DeleteMyProfPic(int id)
        {
            MemberMedia mm = db.MemberMedias.Find(id);
            db.MemberMedias.Remove(mm);
            db.SaveChanges();

            return RedirectToAction("Edit", "Members");
        }

        #endregion DELETES

        public ActionResult AssignPrimary(int id)
        {
            MemberMedia mm = db.MemberMedias.Find(id);
            mm.PrimaryPic = true;

            long memberID = (long)mm.MemberID;

            List<MemberMedia> mms = db.MemberMedias.Where(m => m.MemberID == memberID && m.MemberMediaID != id).ToList<MemberMedia>();

            foreach (MemberMedia i in mms)
            {
                i.PrimaryPic = false;
            }

            db.SaveChanges();
            return RedirectToAction("Edit", "Members");
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
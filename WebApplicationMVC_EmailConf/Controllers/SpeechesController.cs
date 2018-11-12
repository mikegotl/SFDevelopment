using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using WebApplicationMVC_EmailConf.Models;

namespace WebApplicationMVC_EmailConf.Controllers
{
    public class SpeechesController : Controller
    {
        #region Variables

        private SpeechSageDBEntities db = new SpeechSageDBEntities();
        private int _blank_TransitionID;
        private bool _dbChanged = true;

        public int SpeechID
        {
            get
            {
                int id = (int)Session["SpeechID"];
                return id;
            }
            set
            {
                Session["SpeechID"] = value;
            }
        }

        public bool isTest
        {
            get
            {
                return false;
            }
        }

        public int? bestStrat
        {
            get
            {
                int best;
                var b = WholeSpeech.SpeechStrategies.Where(m => m.Best == true);
                if (b != null)
                {
                    if (b.FirstOrDefault() != null)
                    {
                        best = b.FirstOrDefault().StrategyID;
                        return best;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public int Blank_TransitionID
        {
            get
            {
                if (_blank_TransitionID > 0)
                {
                    return _blank_TransitionID;
                }
                else
                {
                    return db.Transitions.Where(m => m.Name == "").FirstOrDefault().TransitionID;
                }
            }
            set
            {
                _blank_TransitionID = value;
            }
        }

        public List<qAnswer> SpeechAnswers
        {
            get
            {
                var answers = WholeSpeech.qAnswers;
                return answers.ToList<qAnswer>();
            }
        }

        public string SavedPurposeStatement
        {
            get
            {
                var purposeStatementAnswer = WholeSpeech.qAnswers.Where(m => m.qQuestionChoice.qSub_Step_Question.qQuestion.Name == "By the end of my speech, my audience will...").FirstOrDefault();

                if (purposeStatementAnswer != null)
                {
                    return purposeStatementAnswer.free_text;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        private string GetAttentionGrabber(int? id)
        {
            string AttnGrabTxt = string.Empty;
            var AttnGrab = WholeSpeech.qAnswers.Where(m => m.qQuestionChoice.qSub_Step_Question.Sub_Step.Name.Contains("Attention Grabber")).FirstOrDefault();
            if (AttnGrab != null)
            {
                AttnGrabTxt = AttnGrab.free_text;
            }
            return AttnGrabTxt;
        }

        public string ThesisEthos
        {
            get
            {
                string ethosAnswer = string.Empty;
                var e = GetEthosAnswer(SpeechID);
                if (e != null)
                {
                    ethosAnswer = e.free_text;
                }
                else
                {
                    ethosAnswer = "";
                }
                return ethosAnswer;
            }
        }

        public string ThesisRelatePathos
        {
            get
            {
                string pathosAnswer = string.Empty;
                var e = GetPathosAnswer(SpeechID);
                if (e != null)
                {
                    pathosAnswer = e.free_text;
                }
                else
                {
                    pathosAnswer = "";
                }
                return pathosAnswer;
            }
        }

        public string ThesisSupport
        {
            get
            {
                string supportAnswer = string.Empty;
                var e = GetThesisSupportAnswer(SpeechID);
                if (e != null)
                {
                    supportAnswer = e.free_text;
                }
                else
                {
                    supportAnswer = "";
                }
                return supportAnswer;
            }
        }

        public int _fulfilledStatusID
        {
            get
            {
                return db.Status.Where(m => m.Name.ToLower() == "fulfilled").FirstOrDefault().StatusID;
            }
        }

        public int _activeStatusID
        {
            get
            {
                return db.Status.Where(m => m.Name.ToLower() == "active").FirstOrDefault().StatusID;
            }
        }

        public Speech WholeSpeech
        {
            get
            {
                SpeechSageDBEntities dbw = new SpeechSageDBEntities();

                if (Session["WholeSpeech"] == null || _dbChanged)
                {
                    Speech _speech = new Models.Speech();
                    _speech = dbw.Speeches.Find(SpeechID);

                    Session["WholeSpeech"] = _speech;
                    _dbChanged = false;
                }
                return (Speech)Session["WholeSpeech"];
            }
        }

        #endregion Variables

        #region CRUD

        [CheckSessionOut]
        [Authorize]
        [ActivePaidMember]
        public ActionResult Speech(int id)
        {
            SpeechID = id;
            _dbChanged = true;

            ViewBag.TopQuestions = db.CredCheckerQuestions.Where(m => m.ParentQID == null).OrderBy(o => o.QuestionNumber).ToList<CredCheckerQuestion>();

            return View(WholeSpeech);
        }

        [Authorize]
        [CheckSessionOut]
        public ActionResult Index()
        {
            int memberID = MyHelpers.LoggedInMember.MemberID;

            List<Speech> speeches = db.Speeches.Include(s => s.Member).Where(m => m.memberID == MyHelpers.LoggedInMember.MemberID).OrderByDescending(m => m.Updated_Date).ToList<Speech>();

            return View(speeches.ToList());
        }

        [Authorize]
        [CheckSessionOut]
        public ActionResult List()
        {
            List<Speech> speeches = db.Speeches.Include(s => s.Member).ToList<Speech>();

            return View(speeches.ToList());
        }

        [Authorize]
        [CheckSessionOut]
        public ActionResult List_Prof()
        {
            List<Speech> myStudentsSpeeches = new List<Models.Speech>();
            var MyCourses = db.Courses.Where(m => m.professor_memberID == MyHelpers.LoggedInMember.MemberID).ToList<Course>();

            foreach (Course c in MyCourses)
            {
                try
                {
                    List<Speech> speeches = db.Speeches.Include(s => s.Member).Where(m => m.courseID == c.courseID && m.memberID != MyHelpers.LoggedInMember.MemberID).ToList<Speech>();
                    myStudentsSpeeches.AddRange(speeches);
                }
                catch (Exception)
                {
                }
            }

            return View(myStudentsSpeeches);
        }

        [Authorize]
        [CheckSessionOut]
        public ActionResult Students()
        {
            var model = db.Members.ToList<Member>();
            return View(model);
        }

        [Authorize]
        [CheckSessionOut]
        public ActionResult Students_Prof()
        {
            List<Member> _myStudents = new List<Member>();
            var MyCourses = db.Courses.Where(m => m.professor_memberID == MyHelpers.LoggedInMember.MemberID).ToList<Course>();

            foreach (Course c in MyCourses)
            {
                var speeches = db.Speeches.Where(m => m.courseID == c.courseID);

                foreach (Speech s in speeches)
                {
                    try
                    {
                        Member member = db.Members.Where(m => m.MemberID == s.memberID && m.MemberID != MyHelpers.LoggedInMember.MemberID).First();
                        _myStudents.Add(member);
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            return View(_myStudents);
        }

        // GET: Speeches/Details/5
        [CheckSessionOut]
        public ActionResult Details(int id)
        {
            Speech speech = SpeechDetails((int)id);
            return View(speech);
        }

        [WordDocument]
        public ActionResult PreviewDocument()
        {
            Speech speech = SpeechDetails(SpeechID);
            ViewBag.WordDocumentFilename = "MySpeechPreview";
            return View("Details", speech);
        }

        [WordDocument]
        public ActionResult ExportCards()
        {
            var mps = getSpeechMainPoints();
            ViewBag.AttnGrab = GetAttentionGrabber(SpeechID);
            ViewBag.Ethos = ThesisEthos;
            ViewBag.ThesisSupport = ThesisSupport;
            ViewBag.ThesisRelatePathos = ThesisRelatePathos;
            ViewBag.WordDocumentFilename = "MyCards";
            return View("Cards", mps);
        }

        private Speech SpeechDetails(int id)
        {
            Speech speech = WholeSpeech;
            //Get speech parts
            string RunningHead = speech.Topic.ToUpper();
            string Topic = speech.Topic.ToString();
            string StudentName = MyHelpers.LoggedInMember.FirstName + " " + MyHelpers.LoggedInMember.LastName;

            //AudienceAnalysis
            List<string> choicesPrimary = new List<string>();
            var audAnalanswersPrimary = WholeSpeech.qAnswers.Where(m => m.free_text == "true" && m.qQuestionChoice.qSub_Step_Question.Sub_Step.Name.Contains("Primary -")).ToList<qAnswer>();

            foreach (qAnswer a in audAnalanswersPrimary)
            {
                string choiceName = a.qQuestionChoice.qChoice.name;
                choicesPrimary.Add(choiceName);
            }

            List<string> choicesSecondary = new List<string>();
            var audAnalanswersSecondary = WholeSpeech.qAnswers.Where(m => m.qQuestionChoice.qSub_Step_Question.Sub_Step.Name.Contains("Secondary")).ToList<qAnswer>();

            var Conc_Thesis_Support_Ans = WholeSpeech.qAnswers.Where(m => m.questionChoiceID == 75);

            var Conc_Thesis_State_Ans = WholeSpeech.qAnswers.Where(m => m.questionChoiceID == 76);

            var Conc_Thesis_Relate_Ans = WholeSpeech.qAnswers.Where(m => m.questionChoiceID == 77);

            ViewBag.SpeechID = SpeechID;
            ViewBag.PurposeStatement = SavedPurposeStatement;
            ViewBag.AAPrimary = choicesPrimary;
            ViewBag.AASecondary = audAnalanswersSecondary;
            ViewBag.RunningHead = RunningHead;
            ViewBag.Topic = Topic;
            ViewBag.StudentName = StudentName;
            ViewBag.MainPoints = getSpeechMainPoints();
            ViewBag.connections = db.SpeechSubpointConnections.Where(m => m.SpeechSubPoint.SpeechMainPoint.SpeechID == id).ToList<SpeechSubpointConnection>();
            ViewBag.AttnGrab = GetAttentionGrabber(id);
            ViewBag.Ethos = ThesisEthos;
            ViewBag.ThesisSupport = ThesisSupport;
            ViewBag.ThesisRelatePathos = ThesisRelatePathos;

            if (Conc_Thesis_Support_Ans.Count() > 0)
            {
                ViewBag.Conc_Thesis_Support_Ans = Conc_Thesis_Support_Ans.FirstOrDefault().free_text;
            }

            if (Conc_Thesis_State_Ans.Count() > 0)
            {
                ViewBag.Conc_Thesis_State_Ans = Conc_Thesis_State_Ans.FirstOrDefault().free_text;
            }

            if (Conc_Thesis_Relate_Ans.Count() > 0)
            {
                ViewBag.Conc_Thesis_Relate_Ans = Conc_Thesis_Relate_Ans.FirstOrDefault().free_text;
            }
            return speech;
        }

        //Get: Speeches
        [ActivePaidMember]
        public ActionResult Create()
        {
            ViewBag.hasCourses = false;

            try
            {
                //If Student or Professor, Display Professor questions and Course List
                int studentMemberTypeID = db.MemberTypes.Where(m => m.Name.Contains("Student")).First().MemberTypeID;
                int professorMemberTypeID = db.MemberTypes.Where(m => m.Name.Contains("Professor")).First().MemberTypeID;

                if (MyHelpers.LoggedInMember.MemberType.MemberTypeID == studentMemberTypeID || MyHelpers.LoggedInMember.MemberTypeID == professorMemberTypeID)
                {
                    ViewBag.isStudentOrProfessor = true;

                    List<Member> _professors = db.Members.Where(m => m.MemberType.Name.Contains("Professor")).ToList<Member>();

                    Member newSelectPlease = new Member();
                    newSelectPlease.LastName = "Select an Instructor";
                    newSelectPlease.MemberID = 0;

                    _professors.Insert(0, newSelectPlease);

                    var selSource = (from p in _professors.ToList()
                                     select new
                                     {
                                         MemberID = p.MemberID,
                                         FullName = p.FirstName + " " + p.LastName
                                     });

                    var profSelectList = new SelectList(selSource, "MemberID", "FullName");

                    ViewBag.professors = profSelectList;
                }
            }
            catch (Exception)
            {
            }

            return View();
        }

        public JsonResult GetCourses(int professorID)
        {
            List<Course> _courses = null;

            _courses = db.Courses.Where(m => m.professor_memberID == professorID).ToList<Course>();

            SelectList sl = new SelectList(_courses, "courseID", "courseCode");
            return Json(sl);
        }

        // POST: Speeches/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Topic, CourseID, courseName, instrEmail")] Speech speech)
        {
            if (ModelState.IsValid)
            {
                //Creates New Speech record
                speech.memberID = MyHelpers.LoggedInMember.MemberID;
                speech.Updated_Date = DateTime.Now;
                speech.Entered_Date = DateTime.Now;
                db.Speeches.Add(speech);
                SaveDBChanges();

                qAnswer topicAnswer = new qAnswer();
                topicAnswer.speechID = speech.Speech_ID;
                topicAnswer.free_text = speech.Topic;
                topicAnswer.questionChoiceID = 1;
                db.qAnswers.Add(topicAnswer);

                SubStepStatu sss = new SubStepStatu();
                sss.SpeechID = speech.Speech_ID;
                sss.StatusID = 2;
                sss.SubStepID = 1;
                sss.CreateDate = DateTime.Now;

                db.SubStepStatus.Add(sss);
                SaveDBChanges();

                return Redirect(Url.Action("Speech", "Speeches", new { id = speech.Speech_ID }));
            }
            return View(speech);
        }

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

        // POST: Speeches/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //find answers for this speech first and delete all
            List<qAnswer> speech_answers = db.qAnswers.Where(a => a.speechID == id).ToList<qAnswer>();
            db.qAnswers.RemoveRange(speech_answers);
            SaveDBChanges();

            //find speech and delete from db
            Speech speech = db.Speeches.Find(id);
            db.Speeches.Remove(speech);
            SaveDBChanges();
            return RedirectToAction("Index");
        }

        #endregion CRUD

        #region Save Methods

        //POST auto-save
        [CheckSessionOut]
        [HttpPost]
        public string AutoSave(int questionChoiceID, string freeText)
        {
            qAnswer _answer = new qAnswer();

            _answer.questionChoiceID = questionChoiceID;
            _answer.speechID = SpeechID;

            if (String.IsNullOrEmpty(freeText))
            {
                _answer.free_text = null;
            }
            else
            {
                _answer.free_text = freeText.Trim();
            }

            try
            {
                qAnswer existingAnswer = db.qAnswers.Where(m => m.speechID == SpeechID && m.questionChoiceID == questionChoiceID).FirstOrDefault();
                if (existingAnswer != null)
                {
                    existingAnswer.free_text = freeText;
                }
                else
                {
                    db.qAnswers.Add(_answer);
                }

                //save topic to speech table is questionchoiceID = 1
                if (questionChoiceID == 1)
                {
                    Speech s = db.Speeches.Find(SpeechID);
                    if (existingAnswer != null)
                    {
                        s.Topic = existingAnswer.free_text;
                    }
                    else
                    {
                        s.Topic = _answer.free_text;
                    }
                }
                SaveDBChanges();
            }
            catch (Exception ex)
            {
                string msg = string.Empty;

                if (ex.InnerException != null)
                {
                    msg = ex.Message + ": " + ex.InnerException;
                }
                else
                {
                    msg = ex.Message;
                }
                return "SpeechID:" + SpeechID + msg;
            }

            //update last updated date in speech record
            var sp = db.Speeches.Find(SpeechID);
            if (sp != null)
            {
                sp.Updated_Date = DateTime.Now.AddHours(2);
                SaveDBChanges();
            }

            return "Changes have been autosaved to speech.";
        }

        [CheckSessionOut]
        [HttpPost]
        public string AutoSaveStrat(int strategyID, string freeText)
        {
            //save checked strat to speech_strategies table
            List<SpeechStrategy> existingSpeechStrategies = new List<SpeechStrategy>();
            existingSpeechStrategies = db.SpeechStrategies.Where(m => m.SpeechID == SpeechID && m.StrategyID == strategyID).ToList<SpeechStrategy>();
            if (existingSpeechStrategies.Count > 0)
            {
                if (freeText == "false")
                {
                    //delete
                    db.SpeechStrategies.RemoveRange(existingSpeechStrategies);
                }
            }
            else
            {
                //db.SpeechStrategies.Add()
                if (freeText == "true")
                {
                    //add
                    SpeechStrategy ss = new SpeechStrategy();
                    ss.CreateDate = DateTime.Now;
                    ss.StrategyID = strategyID;
                    ss.SpeechID = SpeechID;

                    db.SpeechStrategies.Add(ss);
                }
            }
            SaveDBChanges();

            return "Changes have been autosaved to speech strategies table.";
        }

        [CheckSessionOut]
        [HttpPost]
        public string AutoSaveBestStrat(int SpeechStrategyID, string freeText)
        {
            List<SpeechStrategy> strats = db.SpeechStrategies.Where(m => m.SpeechID == SpeechID).ToList<SpeechStrategy>();

            foreach (SpeechStrategy ss in strats)
            {
                if (ss.SpeechStrategyID == SpeechStrategyID)
                {
                    if (freeText == "false")
                    {
                        ss.Best = false;
                    }
                    else
                    {
                        ss.Best = true;
                    }
                }
                else
                {
                    ss.Best = false;
                }
            }

            SaveDBChanges();

            //Add Strategy Main Points to Speech_Main Points table
            //Get Speech Best Strategy
            List<SpeechStrategy> sstrats = db.SpeechStrategies.Where(m => m.SpeechID == SpeechID && m.Best == true).ToList<SpeechStrategy>();
            if (sstrats.Count > 0)
            {
                SpeechStrategy bestStrat = sstrats.FirstOrDefault();
                List<MainPoint> bestStratMainPoints = bestStrat.Strategy.MainPoints.ToList<MainPoint>();
                foreach (MainPoint mp in bestStratMainPoints)
                {
                    SpeechMainPoint existingSpeechMP = db.SpeechMainPoints.Where(m => m.MainPointID == mp.MainPointID && m.SpeechID == SpeechID).FirstOrDefault();
                    if (existingSpeechMP == null)
                    {
                        //does not exist, add new Speech Main Point from this Best Strat Main Point
                        SpeechMainPoint newMP = new SpeechMainPoint();
                        newMP.MainPointID = mp.MainPointID;
                        newMP.SpeechID = SpeechID;

                        int maxOrd = 0;
                        try
                        {
                            maxOrd = (int)db.SpeechMainPoints.Where(m => m.SpeechID == SpeechID).Max(m => m.Ordinal);
                        }
                        catch (Exception ex)
                        {
                        }
                        newMP.Ordinal = maxOrd + 1;
                        db.SpeechMainPoints.Add(newMP);
                        SaveDBChanges();
                    }
                }
            }

            return "Changes have been autosaved to speech strategies table.";
        }

        [CheckSessionOut]
        [HttpPost]
        public string AutoSaveSpeechNotes(int SpeechID, string Notes)
        {
            Speech s = db.Speeches.Where(m => m.Speech_ID == SpeechID).First();
            s.Notes = Notes;
            db.SaveChanges();

            return "Changes have been autosaved.";
        }

        [CheckSessionOut]
        [HttpPost]
        public string AutoSaveSpeechSlides(int slideId, string slideContent, string label)
        {
            try
            {
                //Find existing slide record
                var existingSlide = db.SpeechSlideSaves.Where(x =>
                                                    x.SpeechID == SpeechID && x.SlideNbr == slideId);
                if (!existingSlide.Any())
                {
                    //Add new slide records
                    SpeechSlideSave newSlide = new SpeechSlideSave()
                    {
                        SpeechID = SpeechID,
                        SlideNbr = slideId,
                        Text = slideContent,
                        Label = label
                    };

                    db.SpeechSlideSaves.Add(newSlide);
                }
                else
                {
                    existingSlide.FirstOrDefault().Label = label;
                    existingSlide.FirstOrDefault().Text = slideContent;
                }

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "Changes have been autosaved.";
        }

        [CheckSessionOut]
        [HttpPost]
        public string ResetSaveSlides()
        {
            //Find existing slide record
            var slides = db.SpeechSlideSaves.Where(x => x.SpeechID == SpeechID);
            if (slides.Any())
            {
                db.SpeechSlideSaves.RemoveRange(slides);
                db.SaveChanges();
            }
            return "Speech slides have been deleted.";
        }

        [CheckSessionOut]
        public string AddMainPoint(string strMainPoint)
        {
            if (!string.IsNullOrEmpty(strMainPoint))
            {
                //add main point to custom main points table
                SpeechMainPoint sMP = new SpeechMainPoint();
                sMP.MainPointFreeText = strMainPoint;
                sMP.SpeechID = SpeechID;

                int maxOrd = 0;
                try
                {
                    maxOrd = (int)db.SpeechMainPoints.Where(m => m.SpeechID == SpeechID).Max(m => m.Ordinal);
                }
                catch (Exception ex)
                {
                }

                sMP.Ordinal = maxOrd + 1;
                db.SpeechMainPoints.Add(sMP);
                SaveDBChanges();
                return "Successfully added new main point";
            }
            else
            {
                return "No text was entered for new main point";
            }
        }

        [CheckSessionOut]
        public string UpdateSpeechMainPointText(int speechMainPointID, string val)
        {
            SpeechMainPoint foundSMP = db.SpeechMainPoints.Find(speechMainPointID);
            foundSMP.MainPointFreeText = val;
            SaveDBChanges();

            return "Successful";
        }

        [CheckSessionOut]
        public string UpdateSpeechSubPointText(int SpeechSubPointsID, string val)
        {
            SpeechSubPoint foundSSP = db.SpeechSubPoints.Find(SpeechSubPointsID);
            foundSSP.SubPointFreeText = val;
            SaveDBChanges();

            return "Successful";
        }

        public string MoveSMP_up(int speechMainPointID)
        {
            //Get current smp and its ordinal
            SpeechMainPoint foundSMP = db.SpeechMainPoints.Find(speechMainPointID);
            int currOrd = (int)foundSMP.Ordinal;

            //Get smps previous to current smp and find the max ordinal and max smp
            List<SpeechMainPoint> SMPs = getSpeechMainPoints();
            int prevMaxOrd = SMPs.Where(m => m.Ordinal < currOrd).Max(m => m.Ordinal).Value;
            SpeechMainPoint prevMaxSMP = SMPs.Where(o => o.Ordinal == prevMaxOrd).FirstOrDefault();

            //switch ordinals between current smp and previous smp
            prevMaxSMP.Ordinal = currOrd;
            foundSMP.Ordinal = prevMaxOrd;
            SaveDBChanges();
            return "Successful";
        }

        public string MoveSMP_down(int speechMainPointID)
        {
            //Get current smp and its ordinal
            SpeechMainPoint foundSMP = db.SpeechMainPoints.Find(speechMainPointID);
            int currOrd = (int)foundSMP.Ordinal;

            //Get smps previous to current smp and find the max ordinal and max smp
            List<SpeechMainPoint> SMPs = getSpeechMainPoints();
            int nextMinOrd = SMPs.Where(m => m.Ordinal > currOrd).Min(m => m.Ordinal).Value;
            SpeechMainPoint nextMinSMP = SMPs.Where(o => o.Ordinal == nextMinOrd).FirstOrDefault();

            //switch ordinals between current smp and previous smp
            nextMinSMP.Ordinal = currOrd;
            foundSMP.Ordinal = nextMinOrd;
            SaveDBChanges();
            return "Successful";
        }

        public string MoveSSP(int SpeechSubPointsID, int direction)
        {
            //Get current smp and its ordinal
            SpeechSubPoint foundSSP = db.SpeechSubPoints.Find(SpeechSubPointsID);
            List<SpeechSubPoint> SSPs = db.SpeechSubPoints.Where(m => m.SpeechMainPoint.SpeechID == SpeechID && m.SpeechMainPointID == foundSSP.SpeechMainPointID).ToList<SpeechSubPoint>();
            int maxOrd = SSPs.Max(m => m.Ordinal).Value;
            int minOrd = SSPs.Min(m => m.Ordinal).Value;
            int currOrd = 0;

            if (foundSSP.Ordinal != null)
            {
                currOrd = (int)foundSSP.Ordinal;
            }
            else
            {
                foundSSP.Ordinal = 100;
            }

            if (direction == 1)//up
            {
                //Get smps previous to current smp and find the max ordinal and max smp
                int prevMaxOrd = SSPs.Where(m => m.Ordinal < currOrd).Max(m => m.Ordinal).Value;
                SpeechSubPoint prevMaxSSP = SSPs.Where(o => o.Ordinal == prevMaxOrd).FirstOrDefault();

                //switch ordinals between current smp and previous smp
                prevMaxSSP.Ordinal = currOrd;
                foundSSP.Ordinal = prevMaxOrd;
            }
            else
            {//down
                //Get smps previous to current smp and find the max ordinal and max smp
                int nextMinOrd = SSPs.Where(m => m.Ordinal > currOrd).Min(m => m.Ordinal).Value;
                SpeechSubPoint nextMinSMP = SSPs.Where(o => o.Ordinal == nextMinOrd).FirstOrDefault();

                //switch ordinals between current smp and previous smp
                nextMinSMP.Ordinal = currOrd;
                foundSSP.Ordinal = nextMinOrd;
            }

            SaveDBChanges();
            return "Successful";
        }

        public string DeleteSMP(int speechMainPointID)
        {
            SpeechMainPoint foundSMP = db.SpeechMainPoints.Find(speechMainPointID);
            db.SpeechMainPoints.Remove(foundSMP);
            SaveDBChanges();
            return "Successful";
        }

        [CheckSessionOut]
        public string SaveSourceSummary(int SpeechSubPointsID, string summary)
        {
            SpeechSubpointResearchSummary existingSSP_RS = new SpeechSubpointResearchSummary();
            existingSSP_RS = db.SpeechSubpointResearchSummaries.Where(m => m.SpeechSubPointsID == SpeechSubPointsID).FirstOrDefault();
            if (existingSSP_RS != null)
            {
                existingSSP_RS.SummaryText = summary;
            }
            else
            {
                SpeechSubpointResearchSummary newSSP_RS = new SpeechSubpointResearchSummary();
                newSSP_RS.SpeechSubPointsID = SpeechSubPointsID;
                newSSP_RS.SummaryText = summary;
                db.SpeechSubpointResearchSummaries.Add(newSSP_RS);
            }
            SaveDBChanges();
            return "Successful";
        }

        public string AddPersonalSource(string when, string participants, string what)
        {
            try
            {
                //add to db
                Source newSource = new Source();
                newSource.CreateDate = DateTime.Now;
                newSource.SourceTypeID = (int)db.SourceTypes.Where(m => m.Name.Contains("Personal")).FirstOrDefault().SourceTypeID;
                newSource.SpeechID = SpeechID;
                newSource.When = when;
                newSource.What = what;
                newSource.Who = participants;

                db.Sources.Add(newSource);
                SaveDBChanges();
            }
            catch (Exception ex)
            {
                return "Error saving record:" + ex.Message;
            }

            return "Successfully added. Click close.";
        }

        public string AddPublishedSource(string citation, string what, string cScore)
        {
            try
            {
                //add to db
                Source newSource = new Source();
                newSource.CreateDate = DateTime.Now;
                newSource.SourceTypeID = (int)db.SourceTypes.Where(m => m.Name.Contains("Published")).FirstOrDefault().SourceTypeID;
                newSource.SpeechID = SpeechID;
                newSource.When = "";
                newSource.What = what;
                newSource.Who = citation;
                if (!String.IsNullOrEmpty(cScore))
                {
                    newSource.CredScore = Convert.ToDecimal(cScore);
                }

                db.Sources.Add(newSource);
                SaveDBChanges();
            }
            catch (Exception ex)
            {
                return "Error Saving Record:" + ex.Message;
            }
            return "Successfully added. Click close.";
        }

        private void SaveDBChanges()
        {
            db.SaveChanges();
            _dbChanged = true;
        }

        #endregion Save Methods

        #region Speech Methods

        [Authorize]
        private void LoadEthosAnswer(int id)
        {
            //check if answer exists yet for state ethos. if not, build answer from
            //purpose statement and prepend statement then save to State Ethos answer
            string prependStatement = "So today, I would like to discuss with you how to ";
            qAnswer savedAnswerStateEthos = GetEthosAnswer(id);

            if (savedAnswerStateEthos == null)
            {
                int questionChoiceID = 71;
                qAnswer ethosAnswer = new qAnswer();
                ethosAnswer.questionChoiceID = questionChoiceID;
                ethosAnswer.free_text = prependStatement + SavedPurposeStatement;
                ethosAnswer.speechID = id;
                db.qAnswers.Add(ethosAnswer);
                SaveDBChanges();
            }
            else
            {
                //There is a saved answer for ethos
                //check if is empty or null
                if (String.IsNullOrEmpty(savedAnswerStateEthos.free_text))
                {
                    //is null or empty answer for ethos
                    //fill with answer needed
                    savedAnswerStateEthos.free_text = prependStatement + SavedPurposeStatement;
                    SaveDBChanges();
                }
            }
        }

        private qAnswer GetEthosAnswer(int? id)
        {
            return WholeSpeech.qAnswers.Where(m => m.qQuestionChoice.qSub_Step_Question.qQuestion.Name == "State:").FirstOrDefault();
        }

        private qAnswer GetThesisSupportAnswer(int? id)
        {
            return WholeSpeech.qAnswers.Where(m => m.qQuestionChoice.qSub_Step_Question.qQuestion.Name == "Support:").FirstOrDefault();
        }

        private qAnswer GetPathosAnswer(int? id)
        {
            return WholeSpeech.qAnswers.Where(m => m.qQuestionChoice.qSub_Step_Question.qQuestion.Name == "Relate:").FirstOrDefault();
        }

        private List<qAnswer> GetAnswersBySubStep_QuestionName(int speechID, string subStepName, string questionName)
        {
            var answers = WholeSpeech.qAnswers.Where(m => m.qQuestionChoice.qSub_Step_Question.qQuestion.Name == questionName && m.qQuestionChoice.qSub_Step_Question.Sub_Step.Name == subStepName).ToList<qAnswer>();

            return answers;
        }

        public JsonResult GetTreeData()
        {
            var dbsteps = db.Steps.Where(m => m.Active == true).ToList<Step>();

            List<Tree_Node> treeNodes = new List<Tree_Node>();

            var nid = 0;

            foreach (Step s in dbsteps)
            {
                Tree_Node TN = new Tree_Node();

                TN.Text = s.Name;
                TN.Idx = s.Step_ID;
                TN.nid = nid;

                //create Nodes Obj for Tree Node
                List<Tree_SubNodes> treeSubNodes = new List<Tree_SubNodes>();

                int sCounter = 0;
                foreach (Sub_Step ss in s.Sub_Step.Where(m => m.Active == true).OrderBy(o => o.Order_By).ToList<Sub_Step>())
                {
                    sCounter += 1;
                    nid += 1;

                    Tree_SubNodes TsN = new Tree_SubNodes();
                    TsN.text = ss.Name;
                    TsN.Idx = ss.SubStep_ID;
                    TsN.href = "dropdown" + ss.Step_ID.ToString() + "-" + sCounter.ToString();
                    TsN.nid = nid;
                    treeSubNodes.Add(TsN);
                }

                TN.Nodes = treeSubNodes;

                treeNodes.Add(TN);
                nid += 1;
            }

            return Json(treeNodes, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllCompletedSubSteps()
        {
            List<subStep> subSteps = new List<subStep>();

            List<SubStepStatu> completedSubSteps = WholeSpeech.SubStepStatus.Where(m => m.Status.Name == "Fulfilled").ToList<SubStepStatu>();

            foreach (SubStepStatu ss in completedSubSteps)
            {
                subStep subStep = new subStep();
                subStep.sid = ss.Sub_Step.Step_ID;
                subStep.ssid = (int)ss.SubStepID;

                subSteps.Add(subStep);
            }

            return Json(subSteps, JsonRequestBehavior.AllowGet);
        }

        public int GetIndexOfStepID(int stepID)
        {
            int idx = 0;

            foreach (Step s in db.Steps.ToList<Step>())
            {
                if (stepID == s.Step_ID) { break; }

                int ssCnt = s.Sub_Step.Count() + 1;
                idx = idx + ssCnt;
            }

            return idx;
        }

        protected string RenderActionResultToString(ActionResult result)
        {
            // Create memory writer.
            var sb = new StringBuilder();
            var memWriter = new StringWriter(sb);

            // Create fake http context to render the view.
            var fakeResponse = new HttpResponse(memWriter);
            var fakeContext = new HttpContext(System.Web.HttpContext.Current.Request, fakeResponse);
            var fakeControllerContext = new ControllerContext(
                new HttpContextWrapper(fakeContext),
                this.ControllerContext.RouteData,
                this.ControllerContext.Controller);
            var oldContext = System.Web.HttpContext.Current;
            System.Web.HttpContext.Current = fakeContext;

            // Render the view.
            result.ExecuteResult(fakeControllerContext);

            // Restore data.
            System.Web.HttpContext.Current = oldContext;

            // Flush memory and return output.
            memWriter.Flush();
            return sb.ToString();
        }

        public bool CheckIfSubStepCompleted(int SubStepID)
        {
            bool _completed = false;

            if (isSubStepWithQuestion(SubStepID))
            {
                //Sub step answer is recorded in qAnswers table and can checked for completeness with
                //vwCheckSubStep
                var vwCheckSS = db.vwCheckSubSteps.Where(m => m.speechID == SpeechID && m.SubStep_ID == SubStepID);
                if (vwCheckSS.Count() > 0)
                {
                    //SS is completely answered and can have status recorded as fulfilled
                    UpdateSubStepStatus(SubStepID, _fulfilledStatusID);
                    _completed = true;
                }
                else
                {
                    //SS is not complete. must mark status table
                    UpdateSubStepStatus(SubStepID, _activeStatusID);
                    _completed = false;
                }
            }
            else
            {
                //is not a substep with answer that would be saved in qAnswers table
                //must check different tables for answer for this SS
                var vwCheckSSNoQ = db.vwCheckSubStepsWithoutQuestions.Where(m => m.speechid == SpeechID && m.substepID == SubStepID);
                if (vwCheckSSNoQ.Count() > 0)
                {
                    //SS is completely answered and can have status recorded as fulfilled
                    UpdateSubStepStatus(SubStepID, _fulfilledStatusID);
                    _completed = true;
                }
                else
                {
                    //SS is not complete. must mark status table
                    UpdateSubStepStatus(SubStepID, _activeStatusID);
                    _completed = false;
                }
            }

            return _completed;
        }

        [CheckSessionOut]
        private void UpdateSubStepStatus(int SubStepID, int statusID)
        {
            var existing_sss = db.SubStepStatus.Where(m => m.SpeechID == SpeechID && m.SubStepID == SubStepID);
            if (existing_sss.Count() > 0)
            {
                //SS has existing status, update
                existing_sss.FirstOrDefault().StatusID = statusID;
            }
            else
            {
                //SS does not have existing status, create new
                SubStepStatu newsss = new SubStepStatu();
                newsss.SpeechID = SpeechID;
                newsss.StatusID = statusID;
                newsss.CreateDate = DateTime.Now;
                newsss.SubStepID = SubStepID;

                db.SubStepStatus.Add(newsss);
            }
            SaveDBChanges();
        }

        public bool isSubStepWithQuestion(int SubStepID)
        {
            int count = db.vwSubStepsWithQuestions.Where(m => m.SubStep_ID == SubStepID).Count();
            if (count > 0) { return true; } else { return false; }
        }

        public DateTime? GetDeliveryDate()
        {
            DateTime? _delDateDT = new DateTime();

            var _delDate = WholeSpeech.qAnswers.Where(m => m.questionChoiceID == 84);
            if (_delDate.Count() > 0)
            {
                _delDateDT = Convert.ToDateTime(_delDate.First().free_text);
            }
            else
            {
                _delDateDT = null;
            }
            return _delDateDT;
        }

        public double GetCountDownDays()
        {
            DateTime _now = DateTime.Now;
            double _daysDiff = 0.00;

            var gdd = GetDeliveryDate();
            if (gdd != null)
            {
                DateTime _dd = (DateTime)GetDeliveryDate();

                if (_dd != null)
                {
                    _daysDiff = _dd.Subtract((DateTime)_now).Days;
                }
                return _daysDiff;
            }
            else
            {
                return _daysDiff;
            }
        }

        public decimal GetPercComplete()
        {
            decimal _percComplete = 0.00M;

            //Percentage Completion
            List<SubStepStatu> completedSubSteps = WholeSpeech.SubStepStatus.Where(m => m.Status.Name == "Fulfilled").Distinct().ToList<SubStepStatu>();

            decimal _countCompl = completedSubSteps.Select(g => new
            {
                g.SubStepID
            }).Distinct().Count();

            decimal _countTtlSS = db.Sub_Step.Where(m => m.Active == true).Count();

            if (_countCompl > 0 && _countTtlSS > 0)
            {
                decimal _percCompl = _countCompl / _countTtlSS;
                _percComplete = Math.Round(_percCompl * 100, 2);

                return _percComplete;
            }
            else
            {
                return 0.00M;
            }
        }

        public int GetSpeechWords()
        {
            int _totalSpeechWords = 0;

            return _totalSpeechWords;
        }

        public decimal GetCredScoreAvg()
        {
            decimal _getCredScoreAvg = 0;

            _getCredScoreAvg = (decimal)db.Sources.Where(m => m.SpeechID == WholeSpeech.Speech_ID).Average(m => m.CredScore);

            return _getCredScoreAvg;
        }

        public string GetUserID(int SpeechID)
        {
            string userID = db.Speeches.Find(SpeechID).Member.UserID;

            return userID;
        }

        #region Subpoints

        public string DeleteSSP(int SpeechSubPointsID)
        {
            SpeechSubPoint foundSSP = db.SpeechSubPoints.Find(SpeechSubPointsID);
            db.SpeechSubPoints.Remove(foundSSP);
            SaveDBChanges();

            return "Successful";
        }

        public string DeleteSSPR(int SpeechSubpointResearchID)
        {
            SpeechSubpointResearch foundSSPR = db.SpeechSubpointResearches.Find(SpeechSubpointResearchID);
            db.SpeechSubpointResearches.Remove(foundSSPR);
            SaveDBChanges();

            return "Successful";
        }

        public string AddSubPoint(int StrategyID, int SpeechMainPointID)
        {
            //Get Strategy by Parameter
            Strategy strat = db.Strategies.Find(StrategyID);
            //Add new subpoints for each strat main point if any. if no main points then add new subpoint with strategyID and freetext value = strat name
            if (strat.MainPoints.Count > 0)
            {
                foreach (var mp in strat.MainPoints)
                {
                    SpeechSubPoint newSP = new SpeechSubPoint();
                    newSP.SpeechMainPointID = SpeechMainPointID;
                    newSP.StrategyID = StrategyID;
                    newSP.SubPointFreeText = mp.Name;
                    newSP.Ordinal = GetMaxSubpointOrdinal(SpeechMainPointID) + 100;

                    db.SpeechSubPoints.Add(newSP);
                    SaveDBChanges();
                }
            }
            else
            {
                SpeechSubPoint newSP = new SpeechSubPoint();
                newSP.SpeechMainPointID = SpeechMainPointID;
                newSP.StrategyID = StrategyID;
                newSP.SubPointFreeText = strat.Name;
                newSP.Ordinal = GetMaxSubpointOrdinal(SpeechMainPointID) + 100;

                db.SpeechSubPoints.Add(newSP);
                SaveDBChanges();
            }
            return "Successful";
        }

        public string GetAnswer(int questionChoiceID)
        {
            var a = db.qAnswers.Where(m => m.speechID == SpeechID && m.questionChoiceID == questionChoiceID).FirstOrDefault();
            if (a != null)
            {
                return a.free_text;
            }
            else
            {
                return "";
            }
        }

        public string AddSubpointCustom(string customSPtxt, int SpeechMainPointID)
        {
            SpeechSubPoint newSP = new SpeechSubPoint();
            newSP.SubPointFreeText = customSPtxt;
            newSP.StrategyID = db.Strategies.Where(m => m.Name == "Topical").FirstOrDefault().StrategyID;
            newSP.SpeechMainPointID = SpeechMainPointID;
            newSP.Ordinal = GetMaxSubpointOrdinal(SpeechMainPointID);

            db.SpeechSubPoints.Add(newSP);
            SaveDBChanges();
            return "Successful";
        }

        public string AddConnection(int SpeechSubPointID, int ConnectionTypeID, string Txt)
        {
            SpeechSubpointConnection sspc = new SpeechSubpointConnection();
            sspc.ConnectionTypeID = ConnectionTypeID;
            sspc.Text = Txt;
            sspc.SpeechSubpointID = SpeechSubPointID;
            db.SpeechSubpointConnections.Add(sspc);
            SaveDBChanges();
            return "Successful";
        }

        public string EditConnection(int sspConnID, int connTypeID, string txt)
        {
            SpeechSubpointConnection editConn = db.SpeechSubpointConnections.Find(sspConnID);
            editConn.ConnectionTypeID = connTypeID;
            editConn.Text = txt;
            SaveDBChanges();
            return "Successful";
        }

        public string DeleteConnection(int id)
        {
            SpeechSubpointConnection deleteConn = db.SpeechSubpointConnections.Find(id);
            db.SpeechSubpointConnections.Remove(deleteConn);
            SaveDBChanges();
            return "Successful";
        }

        private int GetMaxSubpointOrdinal(int SpeechMainPointID)
        {
            var SSP_MPs = db.SpeechSubPoints.Where(m => m.SpeechMainPointID == SpeechMainPointID).ToList<SpeechSubPoint>();

            if (SSP_MPs.Max(m => m.Ordinal) == null)
            {
                return 0;
            }
            else
            {
                return SSP_MPs.Max(m => m.Ordinal).Value;
            }
        }

        public string AddSourceToSubPoint(int SpeechSubPointID, int SourceID)
        {
            SpeechSubpointResearch sspr = new SpeechSubpointResearch();
            sspr.SourceID = SourceID;
            sspr.SpeechSubpointID = SpeechSubPointID;

            db.SpeechSubpointResearches.Add(sspr);
            SaveDBChanges();

            return "Successful";
        }

        public string saveSSP_DDL_Transition(int SpeechSubPointID, int TransitionID, string TransitionText)
        {
            SpeechSubPoint ssp = db.SpeechSubPoints.Find(SpeechSubPointID);
            ssp.TransitionID = TransitionID;
            ssp.SpeechSubPointsID = SpeechSubPointID;
            ssp.TransitionText = TransitionText;
            SaveDBChanges();
            return "Successful";
        }

        public string EditTransition(int SpeechSubPointsID, string txt)
        {
            SpeechSubPoint ssp = db.SpeechSubPoints.Find(SpeechSubPointsID);
            ssp.TransitionText = txt;
            SaveDBChanges();
            return "Successful";
        }

        //public string saveSSP_Custom_Transition(int SpeechSubPointID, string TransitionText)
        //{
        //    SpeechSubPoint ssp = db.SpeechSubPoints.Find(SpeechSubPointID);
        //    ssp.TransitionID = Blank_TransitionID;//blank
        //    ssp.SpeechSubPointsID = SpeechSubPointID;
        //    ssp.TransitionText = TransitionText;
        //    SaveDBChanges();
        //    return "Successful";
        //}

        #endregion Subpoints

        /// <summary>
        /// Returns a PDF action result. This method renders the view to a string then
        /// use that string to generate a PDF file. The generated PDF file is then
        /// returned to the browser as binary content. The view associated with this
        /// action should render an XML compatible with iTextSharp xml format.
        /// </summary>
        /// <param name="model">The model to send to the view.</param>
        /// <returns>The resulted BinaryContentResult.</returns>
        protected ActionResult ViewPdf(object model)
        {
            // Create the iTextSharp document.
            Document doc = new Document();
            // Set the document to write to memory.
            MemoryStream memStream = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, memStream);
            writer.CloseStream = false;
            doc.Open();

            // Render the view xml to a string, then parse that string into an XML dom.
            string htmlText = this.RenderActionResultToString(this.View(model));

            htmlText = Regex.Replace(htmlText, @"(/Content/.*\.png)", m => HttpContext.Server.MapPath(m.Groups[1].Value));

            HTMLWorker worker = new HTMLWorker(doc);
            worker.Parse(new StringReader(htmlText));

            // Close and get the resulted binary data.
            doc.Close();
            var buf = new byte[memStream.Position];
            memStream.Position = 0;
            memStream.Read(buf, 0, buf.Length);

            // Send the binary data to the browser.
            return new BinaryContentResult(buf, "application/pdf");
        }

        //public ActionResult FullOutline()
        //{
        //    Speech speech = db.Speeches.FirstOrDefault();
        //    if (speech == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    return View(speech);
        //}

        private List<SpeechMainPoint> getSpeechMainPoints()
        {
            //List<SpeechMainPoint> smpList = WholeSpeech.SpeechMainPoints.Where(m => (m.MainPoint.StrategyID == bestStrat || m.MainPoint == null)).OrderBy(o => o.Ordinal).ToList<SpeechMainPoint>();

            //List<vwSpeechMainPoint> v_smpList = db.vwSpeechMainPoints.Where(m => m.SpeechID == SpeechID).ToList<vwSpeechMainPoint>();

            //List<SpeechMainPoint> smps = new List<SpeechMainPoint>();

            //foreach (vwSpeechMainPoint v in v_smpList)
            //{
            //    SpeechMainPoint smp = new SpeechMainPoint();
            //    smp.SpeechMainPointID = v.SpeechMainPointID;
            //    smp.MainPointID = v.MainPointID;
            //    smp.MainPointFreeText = v.MainPointFreeText;
            //    smp.Ordinal = v.Ordinal;
            //    smp.SpeechID = v.SpeechID;

            //    smps.Add(smp);
            //}

            List<SpeechMainPoint> smpList = WholeSpeech.SpeechMainPoints.OrderBy(o => o.Ordinal).ToList<SpeechMainPoint>();

            List<SpeechMainPoint> smps = new List<SpeechMainPoint>();

            foreach (SpeechMainPoint smp in smpList)
            {
                if (smp.MainPoint != null)
                {
                    if (smp.MainPoint.StrategyID == bestStrat)
                    {
                        smps.Add(smp);
                    }
                }
                else
                {
                    smps.Add(smp);
                }
            }

            return smps;
        }

        public string getSpeechMainPoints_Str()
        {
            string mpstring = string.Empty;

            List<SpeechMainPoint> smpList = WholeSpeech.SpeechMainPoints.Where(m => (m.MainPoint.StrategyID == bestStrat || m.MainPoint == null)).OrderBy(o => o.Ordinal).ToList<SpeechMainPoint>();

            int counter = 1;

            foreach (SpeechMainPoint mp in smpList)
            {
                if (!string.IsNullOrEmpty(mp.MainPointFreeText))
                {
                    mpstring = mpstring + counter + ". " + mp.MainPointFreeText + ". ";
                }
                else
                {
                    if (mp.MainPoint != null)
                    {
                        mpstring = mpstring + counter + ". " + mp.MainPoint.Name + ". ";
                    }
                }
                counter += 1;
            }
            return mpstring;
        }

        #endregion Speech Methods

        #region Render Partial Views

        public ActionResult RenderBestStrategy()
        {
            var ps = WholeSpeech.qAnswers.Where(m => m.qQuestionChoice.qSub_Step_Question.subStepQuestionID == 18).FirstOrDefault();
            if (ps != null)
            {
                ViewBag.purposeStatement = ps.free_text;
            }

            ViewBag.speechStrats = WholeSpeech.SpeechStrategies.ToList<SpeechStrategy>();

            return PartialView("_BestStrategy");
        }

        public ActionResult RenderGraph()
        {
            qAnswer emotAnswer = WholeSpeech.qAnswers.Where(m => m.free_text == "true"
            && m.qQuestionChoice.qSub_Step_Question.qQuestion.Name.Contains("emotional appeal")).FirstOrDefault();

            qAnswer motivAnswer = WholeSpeech.qAnswers.Where(m => m.free_text == "true"
            && m.qQuestionChoice.qSub_Step_Question.qQuestion.Name.Contains("motivate your")).FirstOrDefault();

            string concat = string.Empty;
            string emotInt = string.Empty;
            string motivInt = string.Empty;

            if (emotAnswer.qQuestionChoice.qChoice.name != null)
            {
                switch (emotAnswer.qQuestionChoice.qChoice.name)
                {
                    case "low":
                        emotInt = "1";
                        break;

                    case "medium":
                        emotInt = "2";
                        break;

                    case "high":
                        emotInt = "3";
                        break;

                    default:
                        break;
                }

                switch (motivAnswer.qQuestionChoice.qChoice.name)
                {
                    case "low":
                        motivInt = "1";
                        break;

                    case "medium":
                        motivInt = "2";
                        break;

                    case "high":
                        motivInt = "3";
                        break;

                    default:
                        break;
                }
            }

            ViewBag.concat = emotInt + motivInt;

            return PartialView("_Graph");
        }

        public ActionResult RenderSkeleton_MP()
        {
            var model = getSpeechMainPoints();

            return PartialView("_Skeleton_MP", model);
        }

        public ActionResult RenderSkeleton_SP()
        {
            List<SpeechMainPoint> model = getSpeechMainPoints();

            return PartialView("_Skeleton_SP", model);
        }

        public ActionResult RenderSkeleton_SP_Out()
        {
            List<SpeechMainPoint> model = getSpeechMainPoints();
            return PartialView("_Skeleton_SP_Out", model);
        }

        public PartialViewResult _Skeleton_SP_Out()
        {
            List<SpeechMainPoint> model = getSpeechMainPoints();
            return PartialView("_Skeleton_SP_Out", model);
        }

        public ActionResult RenderConnection()
        {
            List<SpeechMainPoint> model = getSpeechMainPoints();

            ViewBag.connections = db.SpeechSubpointConnections.Where(m => m.SpeechSubPoint.SpeechMainPoint.SpeechID == SpeechID).ToList<SpeechSubpointConnection>();

            return PartialView("_Connection", model);
        }

        public ActionResult RenderTransition()
        {
            List<SpeechMainPoint> model = getSpeechMainPoints();

            ViewBag.connections = db.SpeechSubpointConnections.Where(m => m.SpeechSubPoint.SpeechMainPoint.SpeechID == SpeechID).ToList<SpeechSubpointConnection>();

            var t = db.Transitions.OrderBy(o => o.Name);
            ViewBag.Transitions = new SelectList(t, "TransitionID", "Name", null);

            return PartialView("_Transition", model);
        }

        public ActionResult RenderResearch()
        {
            ViewBag.sources = WholeSpeech.Sources.ToList<Source>();
            var model = getSpeechMainPoints();
            return PartialView("_Research", model);
        }

        public ActionResult RenderSlides()
        {
            List<SpeechSlideSave> Slides = new List<SpeechSlideSave>();
            List<SpeechSlideSave> savedSlides = db.SpeechSlideSaves.Where(x => x.SpeechID == SpeechID).ToList();
            SpeechSlideSave slide = new SpeechSlideSave();
            string slideText = "";

            var mps = getSpeechMainPoints();
            int slideNbr = 1;
            string lb = "\n";

            //Build First Slide
            if (savedSlides.Where(x => x.SlideNbr == slideNbr).Any())
            {
                slide = savedSlides.Where(x => x.SlideNbr == slideNbr).First();
            }
            else
            {
                slideText = "Main Points" + lb + lb;
                int counter = 1;
                foreach (SpeechMainPoint smp in mps)
                {
                    var mpStr = string.Empty;
                    if (smp.MainPointID == null || !string.IsNullOrEmpty(smp.MainPointFreeText))
                    {
                        mpStr = smp.MainPointFreeText.Trim();
                    }
                    else
                    {
                        mpStr = smp.MainPoint.Name.Trim();
                    }
                    slideText += counter.ToString() + ". " + mpStr + lb;
                    counter += 1;
                }

                slide = new SpeechSlideSave()
                {
                    Label = "Preview Main Points",
                    SlideNbr = slideNbr,
                    Text = slideText,
                    SpeechID = SpeechID
                };
            }

            Slides.Add(slide);
            slideNbr += 1;

            //Build Main Points Slides
            foreach (SpeechMainPoint smp in mps)
            {
                foreach (SpeechSubPoint ssp in smp.SpeechSubPoints)
                {
                    if (savedSlides.Where(x => x.SlideNbr == slideNbr).Any())
                    {
                        slide = savedSlides.Where(x => x.SlideNbr == slideNbr).First();
                    }
                    else
                    {
                        var rs = ssp.SpeechSubpointResearchSummaries.FirstOrDefault();
                        string rsStr = string.Empty;

                        if (rs != null)
                        {
                            rsStr = rs.SummaryText;
                        }

                        slide = new SpeechSlideSave()
                        {
                            Label = "Slide #" + slideNbr.ToString(),
                            SlideNbr = slideNbr,
                            Text = ssp.SubPointFreeText + lb + lb + rsStr,
                            SpeechID = SpeechID
                        };
                    }

                    Slides.Add(slide);
                    slideNbr += 1;
                }

                //Transitional Slide after each Main Point slide except last one
                if (savedSlides.Where(x => x.SlideNbr == slideNbr).Any())
                {
                    slide = savedSlides.Where(x => x.SlideNbr == slideNbr).First();
                }
                else
                {
                    string tta1 = "Main Points" + lb;
                    int tcounter = 1;
                    foreach (SpeechMainPoint tsmp in mps)
                    {
                        var mpStr = string.Empty;
                        if (tsmp.MainPointID == null || !string.IsNullOrEmpty(tsmp.MainPointFreeText))
                        {
                            mpStr = tsmp.MainPointFreeText;
                        }
                        else
                        {
                            mpStr = tsmp.MainPoint.Name;
                        }
                        tta1 += tcounter.ToString() + ". " + mpStr + lb;
                        tcounter += 1;
                    }

                    slide = new SpeechSlideSave()
                    {
                        Label = "Main Point Transition",
                        SlideNbr = slideNbr,
                        Text = slideText,
                        SpeechID = SpeechID
                    };
                }

                Slides.Add(slide);
                slideNbr += 1;
            }

            //Build References Slide
            if (savedSlides.Where(x => x.SlideNbr == slideNbr).Any())
            {
                slide = savedSlides.Where(x => x.SlideNbr == slideNbr).First();
            }
            else
            {
                string taRefs = "References" + lb;

                foreach (var ss in mps.FirstOrDefault().Speech.Sources.Where(m => m.SourceTypeID == 2))
                {
                    taRefs += ss.Who + lb + lb;
                }

                slide = new SpeechSlideSave()
                {
                    Label = "References",
                    SlideNbr = slideNbr,
                    Text = taRefs,
                    SpeechID = SpeechID
                };
            }
            Slides.Add(slide);
            slideNbr += 1;

            //Last Slide
            slideNbr += 1;
            if (savedSlides.Where(x => x.SlideNbr == slideNbr).Any())
            {
                slide = savedSlides.Where(x => x.SlideNbr == slideNbr).First();
            }
            else
            {
                string lta1 = "Main Points" + lb + lb;
                int lcounter = 1;
                foreach (SpeechMainPoint lsmp in mps)
                {
                    var mpStr = string.Empty;
                    if (lsmp.MainPointID == null || !string.IsNullOrEmpty(lsmp.MainPointFreeText))
                    {
                        mpStr = lsmp.MainPointFreeText;
                    }
                    else
                    {
                        mpStr = lsmp.MainPoint.Name;
                    }
                    lta1 += lcounter.ToString() + ". " + mpStr + lb;
                    lcounter += 1;
                }

                slide = new SpeechSlideSave()
                {
                    Label = "Review Main Points",
                    SlideNbr = slideNbr,
                    Text = slideText,
                    SpeechID = SpeechID
                };
            }

            Slides.Add(slide);
            slideNbr += 1;
            return PartialView("_Slides", Slides);
        }

        public void ExportPPT()
        {
            List<SpeechMainPoint> mps = getSpeechMainPoints();

            Application pptApplication = new Application();

            Slides slides;
            // Create the Presentation File
            Presentation pptPresentation = pptApplication.Presentations.Add(MsoTriState.msoTrue);

            Microsoft.Office.Interop.PowerPoint.CustomLayout customLayout = pptPresentation.SlideMaster.CustomLayouts[Microsoft.Office.Interop.PowerPoint.PpSlideLayout.ppLayoutText];

            slides = pptPresentation.Slides;

            #region Slide 1

            //Create Slide 1 - Preview Main Points
            string lb = "\n";
            int slideNbr = 1;
            string _content;

            string ta1 = "Main Points" + lb + lb;
            int counter = 1;
            foreach (SpeechMainPoint smp in mps)
            {
                var mpStr = string.Empty;
                if (smp.MainPointID == null || !string.IsNullOrEmpty(smp.MainPointFreeText))
                {
                    mpStr = smp.MainPointFreeText;
                }
                else
                {
                    mpStr = smp.MainPoint.Name;
                }
                ta1 += counter.ToString() + ". " + mpStr + lb;
                counter += 1;
            }
            // Create new Slide
            CreateSlide(slides, customLayout, slideNbr, "Preview Main Points", ta1, "Notes");

            slideNbr += 1;

            #endregion Slide 1

            #region MP Slides

            foreach (SpeechMainPoint smp in mps)
            {
                foreach (SpeechSubPoint ssp in smp.SpeechSubPoints)
                {
                    var rs = ssp.SpeechSubpointResearchSummaries.FirstOrDefault();
                    string rsStr = string.Empty;

                    if (rs != null)
                    {
                        rsStr = rs.SummaryText;
                    }

                    _content = ssp.SubPointFreeText + " " + lb + lb + rsStr;

                    CreateSlide(slides, customLayout, slideNbr, "Slide #" + slideNbr.ToString(), _content, "Notes");

                    slideNbr += 1;
                }
                //TRANSITIONAL SLIDE FOR MAIN POINT
                string tta1 = "Main Points" + lb;
                int tcounter = 1;
                foreach (SpeechMainPoint tsmp in mps)
                {
                    var mpStr = string.Empty;
                    if (tsmp.MainPointID == null || !string.IsNullOrEmpty(tsmp.MainPointFreeText))
                    {
                        mpStr = tsmp.MainPointFreeText;
                    }
                    else
                    {
                        mpStr = tsmp.MainPoint.Name;
                    }
                    tta1 += tcounter.ToString() + ". " + mpStr + lb;
                    tcounter += 1;
                }

                _content = tta1;

                CreateSlide(slides, customLayout, slideNbr, "Slide #" + slideNbr.ToString(), _content, "Notes");
            }

            #endregion MP Slides

            //Finalize and Save PPTX File
            pptPresentation.SaveAs(@"c:\temp\fppt.pptx", Microsoft.Office.Interop.PowerPoint.PpSaveAsFileType.ppSaveAsDefault, MsoTriState.msoTrue);
            pptPresentation.Close();
            pptApplication.Quit();
        }

        private static void CreateSlide(Slides slides, CustomLayout customLayout, int index, string SlideTitle, string SlideContent, string SlideNotes)
        {
            _Slide slide;
            TextRange objText;

            slide = slides.AddSlide(index, customLayout);

            // Add title
            objText = slide.Shapes[1].TextFrame.TextRange;
            objText.Text = SlideTitle;
            objText.Font.Name = "Arial";
            objText.Font.Size = 32;

            objText = slide.Shapes[2].TextFrame.TextRange;
            objText.Text = SlideContent;

            slide.NotesPage.Shapes[2].TextFrame.TextRange.Text = SlideNotes;
        }

        public ActionResult RenderCards()
        {
            var mps = getSpeechMainPoints();

            ViewBag.AttnGrab = GetAttentionGrabber(SpeechID);
            ViewBag.Ethos = ThesisEthos;
            ViewBag.ThesisSupport = ThesisSupport;
            ViewBag.ThesisRelatePathos = ThesisRelatePathos;

            return PartialView("_Cards", mps);
        }

        #endregion Render Partial Views

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    internal class subStep
    {
        public int ssid;
        public int sid;
        public string ssname;
        public string sname;
    }

    internal class Tree_Node
    {
        public String Text;
        public int Idx;
        public int nid;
        public List<Tree_SubNodes> Nodes;
    }

    internal class Tree_SubNodes
    {
        public String text;
        public String href;
        public int Idx;
        public int nid;
    }

    public class BinaryContentResult : ActionResult
    {
        private readonly string ContentType;
        private readonly byte[] ContentBytes;

        public BinaryContentResult(byte[] contentBytes, string contentType)
        {
            this.ContentBytes = contentBytes;
            this.ContentType = contentType;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;
            response.Clear();
            response.Cache.SetCacheability(HttpCacheability.NoCache);
            response.ContentType = this.ContentType;

            var stream = new MemoryStream(this.ContentBytes);
            stream.WriteTo(response.OutputStream);
            stream.Dispose();
        }
    }
}
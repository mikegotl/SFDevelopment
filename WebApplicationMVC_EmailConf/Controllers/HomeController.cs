using System.Data;
using System.Linq;
using System.Web.Mvc;
using WebApplicationMVC_EmailConf.Models;

namespace WebApplicationMVC_EmailConf.Controllers
{
    public class HomeController : Controller
    {
        private SpeechSageDBEntities db = new SpeechSageDBEntities();

        [Authorize]
        [AllowAnonymous]
        public ActionResult Index(string rtn, string token)
        {
            if (token != null) {
                return RedirectToAction("Register", "Account", new {token= token });
            }

            if (User.Identity.IsAuthenticated)
            {
                //find memberID
                var member = MyHelpers.LoggedInMember;
                if (member != null)
                {
                    var memberID = member.MemberID;
                    var speeches = db.Speeches.Where(m => m.memberID == memberID).ToList<Speech>();
                    ViewBag.speeches = speeches;

                    if (string.IsNullOrEmpty(member.FirstName))
                    {
                        //incomplete profile, redirect to Profile Edit Page
                        return RedirectToAction("Edit", "Members");
                    }
                    return View();
                }
                else
                {
                    if (Request.QueryString["Pass"] == "True")
                    {
                        return View();
                    }
                    else
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(rtn))
                {
                    return Redirect("~/Landing/");
                }
            }

            return View();
        }

        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [Authorize]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
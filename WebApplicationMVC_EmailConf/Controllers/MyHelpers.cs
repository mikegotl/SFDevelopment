using Microsoft.AspNet.Identity;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebApplicationMVC_EmailConf.Models;

namespace WebApplicationMVC_EmailConf.Controllers
{
    [Authorize]
    public static class MyHelpers
    {
        public static SpeechSageDBEntities db = new SpeechSageDBEntities();

        public static string UID
        {
            get
            {
                return System.Web.HttpContext.Current.User.Identity.GetUserId();
            }
        }

        public static bool IsAdministrator
        {
            get
            {
                try
                {
                    string adminRoleID = db.AspNetRoles.Where(m => m.Name == "Administrator-Site").First().Id;

                    if (!string.IsNullOrEmpty(adminRoleID))
                    {
                        var adminUser = db.AspNetUserRoles.Where(m => m.UserId == LoggedInMember.UserID && m.RoleId == adminRoleID);
                        if (adminUser.Count() > 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        private static bool _isAdmin;

        public static Member LoggedInMember
        {
            get
            {
                return db.Members.Include("MemberMedias").Include("MemberType").Where(m => m.UserID == UID).FirstOrDefault();
            }
        }

        public static bool isActivePaidMember()
        {
            bool _isActive = false;

            DateTime DaysAgo7 = DateTime.Now.Date.AddDays(-7);
            DateTime DaysAgo30 = DateTime.Now.Date.AddDays(-30);
            DateTime DaysAgo90 = DateTime.Now.Date.AddDays(-90);
            DateTime DaysAgo112 = DateTime.Now.Date.AddDays(-112);
            DateTime DaysAgo365 = DateTime.Now.Date.AddDays(-365);

            Semester SpringSem = new Semester("Spring", 1, 1, 5, 15);
            Semester SummerSem = new Semester("Summer", 5, 1, 8, 20);
            Semester FallSem = new Semester("Fall", 8, 1, 12, 31);

            try
            {
                if (LoggedInMember.MemberType.MemberTypeID == db.MemberTypes.Where(x => x.Name == "Academic - Professor").First().MemberTypeID)
                {
                    return true;
                }
            }
            catch (Exception ex) { }

            try
            {
                if (db.AspNetUserRoles.Where(x => x.UserId == LoggedInMember.UserID).First().RoleId == db.AspNetRoles.Where(x => x.Name.Contains("Administrator")).First().Id)
                {
                    return true;
                }
            }
            catch (Exception ex) { }
            try
            {
                List<Purchase> memberPurchases = db.Purchases.Where(p => p.MemberID == LoggedInMember.MemberID).ToList<Purchase>();

                foreach (Purchase p in memberPurchases)
                {
                    DateTime today = DateTime.Now;
                    DateTime startDate;
                    DateTime endDate;
                    DateTime pCreateDate = (DateTime)p.CreateDate;
                    string catItemName = p.Catalog_Items.Name;

                    switch (catItemName)
                    {
                        case "7 Day Promo":
                            if (pCreateDate >= DaysAgo7)
                            {
                                _isActive = true;
                            }
                            break;

                        case "30 Day Package":
                            if (pCreateDate >= DaysAgo30)
                            {
                                _isActive = true;
                            }
                            break;

                        case "90 Day Package":
                            if (pCreateDate >= DaysAgo90)
                            {
                                _isActive = true;
                            }
                            break;

                        case "Annual Package":
                            if (pCreateDate >= DaysAgo365)
                            {
                                _isActive = true;
                            }
                            break;

                        case "Spring Package":
                            startDate = new DateTime(DateTime.Now.Year, SpringSem._startMonth, SpringSem._startDay + 1);
                            endDate = new DateTime(DateTime.Now.Year, SpringSem._endMonth, SpringSem._endDay);

                            if (today >= startDate && today <= endDate)
                            {
                                _isActive = true;
                            }
                            break;

                        case "Summer Package":
                            startDate = new DateTime(DateTime.Now.Year, SummerSem._startMonth, SummerSem._startDay + 1);
                            endDate = new DateTime(DateTime.Now.Year, SpringSem._endMonth, SpringSem._endDay);

                            if (today >= startDate && today <= endDate)
                            {
                                _isActive = true;
                            }
                            break;

                        case "Fall Package":
                            startDate = new DateTime(DateTime.Now.Year, FallSem._startMonth, FallSem._startDay + 1);
                            endDate = new DateTime(DateTime.Now.Year, FallSem._endMonth, FallSem._endDay);

                            if (today >= startDate && today <= endDate)
                            {
                                _isActive = true;
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return _isActive;
        }

        public static int cartCount()
        {
            try
            {
                var cartitems = db.Cart_Items.Where(m => m.Cart.MemberID == LoggedInMember.MemberID && m.Cart.StatusID == 1);
                var c = cartitems.Count();
                return c;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static List<SelectListItem> Months = new List<SelectListItem>()
        {
              new SelectListItem() {Text="January", Value="1"},
              new SelectListItem() {Text="February", Value="2"},
              new SelectListItem() {Text="March", Value="3"},
              new SelectListItem() {Text="April", Value="4"},
              new SelectListItem() {Text="May", Value="5"},
              new SelectListItem() {Text="June", Value="6"},
              new SelectListItem() {Text="July", Value="7"},
              new SelectListItem() {Text="August", Value="8"},
              new SelectListItem() {Text="September", Value="9"},
              new SelectListItem() {Text="October", Value="10"},
              new SelectListItem() {Text="November", Value="11"},
              new SelectListItem() {Text="December", Value="12"}
        };

        public static List<SelectListItem> Years(int prevYearCount, int nextYearCount)
        {
            List<SelectListItem> _years = new List<SelectListItem>();

            for (int i = DateTime.Now.Year - prevYearCount; i < DateTime.Now.Year + nextYearCount; i++)
            {
                _years.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
            }
            return _years;
        }

        #region States

        public static List<SelectListItem> States = new List<SelectListItem>()
    {
        new SelectListItem() {Text="Alabama", Value="AL"},
        new SelectListItem() { Text="Alaska", Value="AK"},
        new SelectListItem() { Text="Arizona", Value="AZ"},
        new SelectListItem() { Text="Arkansas", Value="AR"},
        new SelectListItem() { Text="California", Value="CA"},
        new SelectListItem() { Text="Colorado", Value="CO"},
        new SelectListItem() { Text="Connecticut", Value="CT"},
        new SelectListItem() { Text="District of Columbia", Value="DC"},
        new SelectListItem() { Text="Delaware", Value="DE"},
        new SelectListItem() { Text="Florida", Value="FL"},
        new SelectListItem() { Text="Georgia", Value="GA"},
        new SelectListItem() { Text="Hawaii", Value="HI"},
        new SelectListItem() { Text="Idaho", Value="ID"},
        new SelectListItem() { Text="Illinois", Value="IL"},
        new SelectListItem() { Text="Indiana", Value="IN"},
        new SelectListItem() { Text="Iowa", Value="IA"},
        new SelectListItem() { Text="Kansas", Value="KS"},
        new SelectListItem() { Text="Kentucky", Value="KY"},
        new SelectListItem() { Text="Louisiana", Value="LA"},
        new SelectListItem() { Text="Maine", Value="ME"},
        new SelectListItem() { Text="Maryland", Value="MD"},
        new SelectListItem() { Text="Massachusetts", Value="MA"},
        new SelectListItem() { Text="Michigan", Value="MI"},
        new SelectListItem() { Text="Minnesota", Value="MN"},
        new SelectListItem() { Text="Mississippi", Value="MS"},
        new SelectListItem() { Text="Missouri", Value="MO"},
        new SelectListItem() { Text="Montana", Value="MT"},
        new SelectListItem() { Text="Nebraska", Value="NE"},
        new SelectListItem() { Text="Nevada", Value="NV"},
        new SelectListItem() { Text="New Hampshire", Value="NH"},
        new SelectListItem() { Text="New Jersey", Value="NJ"},
        new SelectListItem() { Text="New Mexico", Value="NM"},
        new SelectListItem() { Text="New York", Value="NY"},
        new SelectListItem() { Text="North Carolina", Value="NC"},
        new SelectListItem() { Text="North Dakota", Value="ND"},
        new SelectListItem() { Text="Ohio", Value="OH"},
        new SelectListItem() { Text="Oklahoma", Value="OK"},
        new SelectListItem() { Text="Oregon", Value="OR"},
        new SelectListItem() { Text="Pennsylvania", Value="PA"},
        new SelectListItem() { Text="Rhode Island", Value="RI"},
        new SelectListItem() { Text="South Carolina", Value="SC"},
        new SelectListItem() { Text="South Dakota", Value="SD"},
        new SelectListItem() { Text="Tennessee", Value="TN"},
        new SelectListItem() { Text="Texas", Value="TX"},
        new SelectListItem() { Text="Utah", Value="UT"},
        new SelectListItem() { Text="Vermont", Value="VT"},
        new SelectListItem() { Text="Virginia", Value="VA"},
        new SelectListItem() { Text="Washington", Value="WA"},
        new SelectListItem() { Text="West Virginia", Value="WV"},
        new SelectListItem() { Text="Wisconsin", Value="WI"},
        new SelectListItem() { Text="Wyoming", Value="WY"}
    };

        #endregion States
    }

    public enum RomanNumerals
    {
        I = 1,
        II = 2,
        III = 3,
        IV = 4,
        V = 5,
        VI = 6,
        VII = 7,
        VIII = 8,
        VIIII = 9,
        X = 10
    }

    internal class Semester
    {
        public Semester(string Name, int StartMonth, int StartDay, int EndMonth, int EndDay)
        {
            _name = Name;
            _startMonth = StartMonth;
            _startDay = StartDay;
            _endMonth = EndMonth;
            _endDay = EndDay;
        }

        public string _name { get; set; }
        public int _startMonth { get; set; }
        public int _startDay { get; set; }
        public int _endMonth { get; set; }
        public int _endDay { get; set; }
    }

    public static class Configuration
    {
        //these variables will store the clientID and clientSecret
        //by reading them from the web.config
        public readonly static string ClientId;

        public readonly static string ClientSecret;

        static Configuration()
        {
            var config = GetConfig();
            ClientId = config["clientId"];
            ClientSecret = config["clientSecret"];
        }

        // getting properties from the web.config
        public static Dictionary<string, string> GetConfig()
        {
            return PayPal.Api.ConfigManager.Instance.GetProperties();
        }

        private static string GetAccessToken()
        {
            // getting accesstocken from paypal
            string accessToken = new OAuthTokenCredential(ClientId, ClientSecret, GetConfig()).GetAccessToken();

            return accessToken;
        }

        public static APIContext GetAPIContext()
        {
            // return apicontext object by invoking it with the accesstoken
            APIContext apiContext = new APIContext(GetAccessToken());
            apiContext.Config = GetConfig();
            return apiContext;
        }
    }
}
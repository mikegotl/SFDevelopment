using System;
using System.Linq;
using System.Web.Mvc;
using WebApplicationMVC_EmailConf.Models;

namespace WebApplicationMVC_EmailConf.Controllers
{
    public class CheckoutController : Controller
    {
        private SpeechSageDBEntities db = new SpeechSageDBEntities();

        // GET: Checkout
        public ActionResult checkout()
        {
            //Cart_ItemsController cic = new Cart_ItemsController();
            //List<cartItemListed> cartList = cic.GetCartItems();

            //update  subtotal, tax, and total in cart
            var cart = db.Carts.Where(m => m.MemberID == MyHelpers.LoggedInMember.MemberID);
            if (cart.Count() > 0)
            {
                try
                {
                    var c = cart.First();
                    c.SubTotal = c.Cart_Items.Sum(s => s.Catalog_Items.Price);
                    c.Tax = 0;
                    c.Total = c.SubTotal;

                    db.SaveChanges();
                    return View("checkout", c);
                }
                catch (Exception ex)
                {
                    ViewBag.error = ex.Message;
                    return View("error");
                }
            }
            else
            {
                ViewBag.error = "";
                return View("error");
            }
        }

        public ActionResult Purchase()
        {
            Cart activeCart = db.Carts.Where(m => m.MemberID == MyHelpers.LoggedInMember.MemberID && m.StatusID == 1).FirstOrDefault();

            //Create purchase records for each cart item
            foreach (Cart_Items ci in activeCart.Cart_Items)
            {
                Purchase newP = new Purchase();
                newP.CreateDate = DateTime.Now;
                newP.MemberID = MyHelpers.LoggedInMember.MemberID;
                newP.Catalog_ItemID = ci.Catalog_ItemID;
                newP.Price = ci.Catalog_Items.Price;

                db.Purchases.Add(newP);
            }

            //empty from cart
            db.Carts.Remove(activeCart);

            db.SaveChanges();

            return View("Confirmation");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebApplicationMVC_EmailConf.Models;

namespace WebApplicationMVC_EmailConf.Controllers
{
    public class Cart_ItemsController : Controller
    {
        private SpeechSageDBEntities db = new SpeechSageDBEntities();

        // GET: Cart_Items
        public ActionResult Index()
        {
            var cart_Items = db.Cart_Items.Include(c => c.Catalog_Items).Include(c => c.Cart);
            return View(cart_Items.ToList());
        }

        public ActionResult CartItems()
        {
            var m = GetCartItems();
            return View("CartItems", m);
        }

        // GET: Cart_Items/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart_Items cart_Items = db.Cart_Items.Find(id);
            if (cart_Items == null)
            {
                return HttpNotFound();
            }
            return View(cart_Items);
        }

        // GET: Cart_Items/Create
        public ActionResult Create()
        {
            ViewBag.Catalog_ItemID = new SelectList(db.Catalog_Items, "Catalog_ItemID", "Name");
            ViewBag.CartID = new SelectList(db.Carts, "CartID", "CartID");
            return View();
        }

        // POST: Cart_Items/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Cart_ItemID,Catalog_ItemID,CartID,CreateDate")] Cart_Items cart_Items)
        {
            if (ModelState.IsValid)
            {
                db.Cart_Items.Add(cart_Items);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Catalog_ItemID = new SelectList(db.Catalog_Items, "Catalog_ItemID", "Name", cart_Items.Catalog_ItemID);
            ViewBag.CartID = new SelectList(db.Carts, "CartID", "CartID", cart_Items.CartID);
            return View(cart_Items);
        }

        // GET: Cart_Items/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart_Items cart_Items = db.Cart_Items.Find(id);
            if (cart_Items == null)
            {
                return HttpNotFound();
            }
            ViewBag.Catalog_ItemID = new SelectList(db.Catalog_Items, "Catalog_ItemID", "Name", cart_Items.Catalog_ItemID);
            ViewBag.CartID = new SelectList(db.Carts, "CartID", "CartID", cart_Items.CartID);
            return View(cart_Items);
        }

        // POST: Cart_Items/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Cart_ItemID,Catalog_ItemID,CartID,CreateDate")] Cart_Items cart_Items)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cart_Items).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Catalog_ItemID = new SelectList(db.Catalog_Items, "Catalog_ItemID", "Name", cart_Items.Catalog_ItemID);
            ViewBag.CartID = new SelectList(db.Carts, "CartID", "CartID", cart_Items.CartID);
            return View(cart_Items);
        }

        // GET: Cart_Items/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart_Items cart_Items = db.Cart_Items.Find(id);
            if (cart_Items == null)
            {
                return HttpNotFound();
            }
            return View(cart_Items);
        }

        // POST: Cart_Items/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Cart_Items cart_Items = db.Cart_Items.Find(id);
            db.Cart_Items.Remove(cart_Items);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult AddToCart(int Catalog_ItemID)
        {
            //check if active cart exists for member
            List<Cart> activeCart = db.Carts.Where(m => m.MemberID == MyHelpers.LoggedInMember.MemberID && m.StatusID == 1).ToList<Cart>();

            if (activeCart.Count > 0)
            {
                //add item to active cart found
                var c = activeCart.FirstOrDefault();

                Cart_Items item = new Cart_Items();
                item.Catalog_ItemID = Catalog_ItemID;
                item.CreateDate = DateTime.Now;
                item.CartID = c.CartID;

                db.Cart_Items.Add(item);
                db.SaveChanges();
            }
            else
            {
                //create new cart then add item
                Cart newCart = new Cart();
                newCart.MemberID = MyHelpers.LoggedInMember.MemberID;
                newCart.StatusID = 1;
                newCart.CreateDate = DateTime.Now;

                db.Carts.Add(newCart);
                db.SaveChanges();

                int newCartID = newCart.CartID;

                Cart_Items item = new Cart_Items();
                item.Catalog_ItemID = Catalog_ItemID;
                item.CreateDate = DateTime.Now;
                item.CartID = newCartID;

                db.Cart_Items.Add(item);
                db.SaveChanges();
            }

            var model = GetCartItems();
            return View("CartItems", model);
        }

        public ActionResult ProcessPromo(FormCollection fc)
        {
            int memID = MyHelpers.LoggedInMember.MemberID;
            Promo promo = new Promo();

            string viewNameIfExists = "DuplicatePurchase";
            string viewNameIfNotExists = "Confirmation";

            string promoCodeInLower = fc["promo"].ToString().ToLower();
            switch (promoCodeInLower)
            {
                case "mybestspeech":
                    promo.packageName = "7 Day Promo";
                    promo.messageIfNotExists = "Thank you for beginning 7 Day Promo. Enjoy.";
                    promo.messageIfExists = "Thank you for exploring Speech Formula. Your 7 day free promotion is now over. Please select the package that works best for you to continue writing your best speech ever.";
                    break;

                case "formula365":
                    promo.packageName = "Annual Package";
                    promo.messageIfNotExists = "You have unlocked 365 days of access. Enjoy";
                    promo.messageIfExists = "Your Annual package has now expired. Please select the package that works best for you to continue writing your best speech ever";
                    break;

                case "barryspecial":
                    promo.packageName = "90 Day Package";
                    promo.messageIfNotExists = "You have unlocked 90 days of access. Enjoy";
                    promo.messageIfExists = "Your 90 day package has now expired. Please select the package that works best for you to continue writing your best speech ever";

                    break;
                case "WEXSpecial":
                    if (DateTime.Now.Year > 2018)
                    {
                        ViewBag.error = "The Promo Code has expired.";
                        return View("error");

                    }
                    promo.packageName = "90 Day Package";
                    promo.messageIfNotExists = "You have unlocked 90 days of access. Enjoy";
                    promo.messageIfExists = "Your 90 day package has now expired. Please select the package that works best for you to continue writing your best speech ever";
                    break;

                default:
                    ViewBag.error = "The Promo Code entered does not exist.";
                    return View("error");
            }

            List<Purchase> existingPurchase = db.Purchases.Where(p => p.MemberID == memID
                                              && p.Catalog_Items.Name == promo.packageName).ToList<Purchase>();

            if (existingPurchase.Count() == 0)
            {
                Purchase newP = new Purchase();
                newP.CreateDate = DateTime.Now;
                newP.Price = 0;
                newP.MemberID = memID;
                newP.Catalog_ItemID = db.Catalog_Items.Where(m => m.Name == promo.packageName).First().Catalog_ItemID;

                db.Purchases.Add(newP);
                db.SaveChanges();

                ViewBag.Message = promo.messageIfNotExists;
                return View(viewNameIfNotExists);
            }
            else
            {
                ViewBag.Message = promo.messageIfExists;
                return View(viewNameIfExists);
            }
        }

        public List<cartItemListed> GetCartItems()
        {
            //get model of cart items to return to cart items view for user
            var model = db.Cart_Items.Where(m => m.Cart.Status.Name.ToLower() == "active" && m.Cart.MemberID == MyHelpers.LoggedInMember.MemberID).Include("Catalog_Items").OrderBy(o => o.Catalog_ItemID).ToList<Cart_Items>();

            List<cartItemListed> cartItemsList = new List<cartItemListed>();

            foreach (var i in model)
            {
                cartItemListed ci = new cartItemListed();
                ci.catalogItem = i.Catalog_Items;
                ci.quantity = model.Where(m => m.Catalog_ItemID == i.Catalog_ItemID).Count();

                //check if item exists in list
                var e = cartItemsList.Where(c => c.catalogItem.Catalog_ItemID == i.Catalog_ItemID).FirstOrDefault();
                if (e == null)
                {
                    cartItemsList.Add(ci);
                }
            }

            return cartItemsList;
        }

        public ActionResult RemoveFromCart(int id)
        {
            //get cartitem to delete
            Cart_Items item = db.Cart_Items.Where(m => m.Catalog_ItemID == id
            && m.Cart.MemberID == MyHelpers.LoggedInMember.MemberID
            && m.Cart.StatusID == 1).FirstOrDefault();

            //delete item
            db.Cart_Items.Remove(item);
            db.SaveChanges();

            //show cart
            var model = GetCartItems();
            return View("CartItems", model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        internal class Promo
        {
            public string packageName { get; set; }
            public string messageIfExists { get; set; }
            public string messageIfNotExists { get; set; }
        }
    }

    public class cartItemListed
    {
        public Catalog_Items catalogItem;
        public int quantity;
    }
}
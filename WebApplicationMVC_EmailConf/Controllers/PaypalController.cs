using Newtonsoft.Json;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebApplicationMVC_EmailConf.Models;

namespace WebApplicationMVC_EmailConf.Controllers
{
    [Authorize]
    public class PaypalController : Controller
    {
        private SpeechSageDBEntities db = new SpeechSageDBEntities();

        // GET: Paypal
        public ActionResult Index()
        {
            PayPal.Api.CreditCard model = new CreditCard();
            model.billing_address = new Address();
            model.billing_address.country_code = "US";
            return View(model);
        }

        [HttpPost]
        public ActionResult PaymentWithCreditCard([Bind(Include = "first_name,last_name,billing_address,number,expire_month,expire_year,cvv2")] PayPal.Api.CreditCard creditCard, FormCollection fc)
        {
            creditCard.type = fc["type"];

            //Now make a List of Item and add the above item to it
            //you can create as many items as you want and add to this list
            List<Item> itms = new List<Item>();

            //Get cart items and each to itms list below
            var cart = db.Carts.Where(m => m.MemberID == MyHelpers.LoggedInMember.MemberID && m.Status.Name.ToLower() == "active");

            if (cart.Count() > 0)
            {
                foreach (Cart_Items ci in cart.First().Cart_Items)
                {
                    ////create an item for which you are taking payment
                    Item item = new Item();
                    item.name = ci.Catalog_Items.Name;
                    item.currency = "USD";
                    item.price = String.Format("{0:0.00}", (decimal)ci.Catalog_Items.Price);
                    item.quantity = "1";
                    item.sku = "sku";
                    item.tax = "0";

                    itms.Add(item);
                }
                ItemList itemList = new ItemList();
                itemList.items = itms;

                // Specify details of your payment amount.
                string subtotal = cart.First().Cart_Items.Sum(m => m.Catalog_Items.Price).ToString();

                Details details = new Details();
                details.shipping = "0";
                details.subtotal = String.Format("{0:0.00}", Convert.ToDecimal(subtotal));
                details.tax = "0";

                // Specify your total payment amount and assign the details object
                Amount amnt = new Amount();
                amnt.currency = "USD";
                // Total = shipping tax + subtotal.
                amnt.total = String.Format("{0:0.00}", Convert.ToDecimal(subtotal));
                amnt.details = details;

                // Now make a transaction object and assign the Amount object
                Transaction tran = new Transaction();
                tran.amount = amnt;
                tran.description = "Description about the payment amount.";
                tran.item_list = itemList;

                //generate Invoice Number
                DateTime _now = DateTime.Now;
                string invNbr = MyHelpers.LoggedInMember.MemberID.ToString() + _now.Year.ToString() + _now.Month.ToString() + _now.Day.ToString() + _now.Minute.ToString() + _now.Second.ToString();

                tran.invoice_number = invNbr;

                // Now, we have to make a list of transaction and add the transactions object
                // to this list. You can create one or more object as per your requirements

                List<Transaction> transactions = new List<Transaction>();
                transactions.Add(tran);

                // Now we need to specify the FundingInstrument of the Payer
                // for credit card payments, set the CreditCard which we made above

                FundingInstrument fundInstrument = new FundingInstrument();
                fundInstrument.credit_card = creditCard;

                // The Payment creation API requires a list of FundingIntrument

                List<FundingInstrument> fundingInstrumentList = new List<FundingInstrument>();
                fundingInstrumentList.Add(fundInstrument);

                // Now create Payer object and assign the fundinginstrument list to the object
                Payer payr = new Payer();
                payr.funding_instruments = fundingInstrumentList;
                payr.payment_method = "credit_card";

                // finally create the payment object and assign the payer object & transaction list to it
                Payment pymnt = new Payment();
                pymnt.intent = "sale";
                pymnt.payer = payr;
                pymnt.transactions = transactions;

                try
                {
                    //getting context from the paypal
                    //basically we are sending the clientID and clientSecret key in this function
                    //to the get the context from the paypal API to make the payment
                    //for which we have created the object above.

                    //Basically, apiContext object has a accesstoken which is sent by the paypal
                    //to authenticate the payment to facilitator account.
                    //An access token could be an alphanumeric string

                    APIContext apiContext = Configuration.GetAPIContext();

                    //Create is a Payment class function which actually sends the payment details
                    //to the paypal API for the payment. The function is passed with the ApiContext
                    //which we received above.

                    Payment createdPayment = pymnt.Create(apiContext);

                    //if the createdPayment.state is "approved" it means the payment was successful else not

                    if (createdPayment.state.ToLower() != "approved")
                    {
                        ViewBag.message = "Card was not approved: " + createdPayment.state.ToString();
                        return View("Index", creditCard);
                    }
                    else
                    {
                        //SUCCESSFUL
                        //create purchase record for member for each cart item. Then clear cart or mark inactive
                        try
                        {
                            foreach (Cart_Items ci in cart.First().Cart_Items)
                            {
                                Purchase purch = new Purchase();
                                purch.MemberID = MyHelpers.LoggedInMember.MemberID;
                                purch.Price = ci.Catalog_Items.Price;
                                purch.Catalog_ItemID = ci.Catalog_ItemID;
                                purch.CreateDate = DateTime.Now;

                                db.Purchases.Add(purch);
                                db.SaveChanges();
                            }
                            //clear cart
                            db.Carts.First().StatusID = db.Status.Where(m => m.Name.ToLower() == "fulfilled").First().StatusID;
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            //could not complete purchase/clear cart operation
                            ViewBag.message = "Could not complete purchase/clear cart operation. " + ex.Message;
                            return View("Index", creditCard);
                        }

                        ViewBag.message = createdPayment.state.ToString().ToUpper();
                        return View("SuccessView");
                    }
                }
                catch (PayPal.PayPalException ex)
                {
                    Logger.Log("Error: " + ex.Message);
                    ViewBag.error = JsonConvert.DeserializeObject<PayPal.Api.Error>(((PayPal.ConnectionException)ex).Response);

                    ViewBag.cnnerror = ((PayPal.ConnectionException)ex).ToString();

                    return View("Index", creditCard);
                }
            }
            else
            {
                //cart is empty
                ViewBag.message = "Cart is empty";
                return View("Index", creditCard);
            }
        }
    }
}
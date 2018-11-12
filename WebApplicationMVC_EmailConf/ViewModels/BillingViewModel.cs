using WebApplicationMVC_EmailConf.Models;

namespace WebApplicationMVC_EmailConf.ViewModels
{
    public class BillingViewModel
    {
        public PayPal.Api.CreditCard CreditCard { get; set; }

        public BillingViewModel()
        {
            BillingAddress billingaddress = new BillingAddress();
            CreditCard = new PayPal.Api.CreditCard();
        }
    }
}
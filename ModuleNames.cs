using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace PKCommon
{
    public struct ModuleNames
    {
        public const string Vendor = "Vendor";

        public const string Customer = "Customer";

        public const string User = "PKUsers";

        public const string RetailCustomer = "RetailCustomer";

        public const string Tour = "Tour";
        public const string TourAgent = "TourAgent";
        public const string TourGuide = "TourGuide";

        public const string Employee = "Employee";

        public const string Location = "Location";

        public const string Department = "Department";
        public const string Category = "Category";
        public const string Product = "Product";

        public const string Price = "Price";
        public const string ProductTax = "PKProductTax";
        public const string POSProductTax = "ProductTax";

        public const string CustomerDiscountPolicy = "CustomerDiscountPolicy";
        //reversed synchronize (synchronize product data from frank database to new database)
        public const string PKProduct = "PKProduct";

        //new version POS Transactions==================================
        public const string POSTransaction = "POSTransaction";
        public const string TransactionItem = "TransactionItem";
        public const string TransactionItemTax = "TransactionItemTax";
        public const string TransactionTax = "TransactionTax";
        public const string TransactionItemTaxExemption = "TransactionItemTaxExemption";
        public const string Payment = "Payment";
        public const string POSPayment = "POSPayment";//new version payment is renamed to POSPayment on the enterprise 
        public const string CustomerTransaction = "CustomerTransaction";
        public const string TourTransaction = "TourTransaction";
        public const string UserTransaction = "UserTransaction";
        //promotion
        public const string TransactionPromotion = "TransactionPromotion";
        public const string TransactionPromotionItem = "TransactionPromotionItem";

        public const string ClockInOut = "ClockInOut";
        //==============================================================

        //Promotion=====================================================
        public const string PKPromotion = "PKPromotion";
        public const string PKPromotionPrice = "PKPromotionPrice";
        public const string PKPromotionPriceTax = "PKPromotionPriceTax";
        public const string PKPromotionProduct = "PKPromotionProduct";
        //==============================================================

        //Coupon========================================================
        public const string PKCouponPolicy = "PKCouponPolicy";
        public const string PKCoupon = "Coupon";
        //==============================================================

        //PKTax in Enterprise==========================================================
        public const string TaxTable = "PKTax";
        //==============================================================


    }
}

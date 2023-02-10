using System;
using System.Collections.Generic;
using System.Text;

namespace PKCommon
{
    public class CouponInfo
    {

        public string ID { get; set; }
        public string DisplayText { get; set; }
        public string CouponCode { get; set; }
        public decimal DiscountPercentage { get; set; }
        public string ExpireDate { get; set; }
        public string Status { get; set; }
        public string CreateDateTime { get; set; }
        public string UpdateDateTime { get; set; }
        public string TransactionID { get; set; }
        public string UpdateBy { get; set; }
        public string ApplyTransactionID { get; set; }
        public Byte[] CouponCodeImage { get; set; }


    }
}

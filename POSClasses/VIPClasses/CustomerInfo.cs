using System;
using System.Collections.Generic;

using System.Text;


namespace PKCommon
{
    public class CustomerInfo
    {
        public string ID { get; set; }
        public string CustomerNo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string TEL { get; set; }
        public string FAX { get; set; }
        public string IDCardNo { get; set; }
        public string EMail { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public string Postal { get; set; }
        public string CreateDate { get; set; }
        public string UpdateDate { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public decimal TotalPurchaseAmount { get; set; }
        public string LockTotalPurchaseAmount { get; set; }
        public decimal DiscountPercentage { get; set; }
        public string LockDiscountPercentage { get; set; }
        public decimal Points { get; set; }
        public string LockPoints { get; set; }
        public decimal MaxDiscountAmount { get; set; }
        public decimal DiscountAmountLeft { get; set; }
        public string CustomerType { get; set; }
        public DateTime? Birthday { get; set; }
        public string Gender { get; set; }
        //Kevin  20140924
        public string VIPType { get; set; }
        public decimal Balance { get; set; }
        public string RegLocation { get; set; }
        public string updater { get; set; }
        //public decimal NextDiscountPercentage;
        //public decimal NextDiscountTotalPurchaseAmount;
        //public decimal TotalPurchaseAmountThistime;
    }
}


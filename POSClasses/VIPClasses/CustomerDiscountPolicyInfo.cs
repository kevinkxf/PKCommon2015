using System;
using System.Collections.Generic;
using System.Text;

namespace PKCommon
{
    public class CustomerDiscountPolicyInfo
    {
        public string ID { get; set; }
        public decimal TotalPurchaseAmount { get; set; }
        public decimal DiscountPercentage { get; set; }
        public string CreateTime { get; set; }
        public string UpdateTime { get; set; }
        public string Status { get; set; }
    }
}


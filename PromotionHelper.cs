using System;
using System.Collections.Generic;
using System.Text;

namespace PKCommon
{
    public class PromotionHelper
    {
        public string PromotionConnectionString { get; set; }
        public PromotionHelper(string connectionString)
        {
            PromotionConnectionString = connectionString;
        }


    }
}

using System;
using System.Collections.Generic;
using System.Text;

    public class TransactionItemInfo
    {
        public string ID { get; set; }
        public string TransactionID { get; set; }
        public string ProductID { get; set; }
        public string Barcode { get; set; }
        public string PLU { get; set; }
        public int ShowSequence { get; set; }
        public string Name1 { get; set; }
        public string Name2 { get; set; }
        public decimal Qty { get; set; }
        public string UnitName { get; set; }
        public string Weigh { get; set; }
        public decimal UnitCost { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal ItemDiscountAmount { get; set; }
        public decimal ItemTaxTotalAmount { get; set; }
        public decimal ItemSubTotal { get; set; }
        public string Status { get; set; }
        public string StatusDateTime { get; set; }

        public string Type { get; set; }
        public string PriceOverrideFlag { get; set; }

        public decimal Amount
        {
            get
            {
                decimal amount = UnitPrice * Qty;
                return decimal.Round(amount, 2);
            }
        }

        public List<TransactionItemTaxInfo> TransactionItemTaxList { get; set; }
    }



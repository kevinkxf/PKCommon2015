using System;
using System.Collections.Generic;
using System.Text;

    public class TransactionInfo
    {
        public string ID { get; set; }
        public string StoreName { get; set; }
        public string ComputerName { get; set; }
        public string Cashier { get; set; }
        public string TransactionNo { get; set; }
        public string Type { get; set; }
        public int TotalItemCount { get; set; }
        public decimal ItemTotalCost { get; set; }
        public decimal ItemDiscountTotalAmount { get; set; }
        public decimal SubTotalAmount { get; set; }
        public decimal SubTotalDiscount { get; set; }
        public decimal DollarDiscount { get; set; }
        public decimal AllTaxTotalAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string StatusDateTime { get; set; }
        public string ReferenceTransactionNo { get; set; }

        public List<TransactionTaxInfo> TransactionTaxList { get; set; }

        public List<TransactionItemInfo> TransactionItemList { get; set; }
    }



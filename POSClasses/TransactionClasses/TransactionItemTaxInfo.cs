using System;
using System.Collections.Generic;
using System.Text;


public class TransactionItemTaxInfo
{
    public string TransactionID { get; set; }
    public string TransactionItemID { get; set; }
    public string TaxID { get; set; }
    public string TaxName { get; set; }
    public decimal ItemTaxAmount { get; set; }
    public string Status { get; set; }
}


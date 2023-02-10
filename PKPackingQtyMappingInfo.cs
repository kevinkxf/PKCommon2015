using System;
using System.Collections.Generic;
using System.Web;

namespace PKCommon
{
    public class PKPackingQtyMappingInfo
    {
        private int id;

        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        private string productID;

        public string ProductID
        {
            get { return productID; }
            set { productID = value; }
        }

        private string baseProductID;

        public string BaseProductID
        {
            get { return baseProductID; }
            set { baseProductID = value; }
        }

        private decimal packingQty;

        public decimal PackingQty
        {
            get { return packingQty; }
            set { packingQty = value; }
        }

        private int unitID;

        public int UnitID
        {
            get { return unitID; }
            set { unitID = value; }
        }
    }
}
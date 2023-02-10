using System;
using System.Collections.Generic;
using System.Text;

namespace PKCommon
{
    public enum UnitName
    {
        each = 0,
        lb = 1,
        kg = 2,
        g = 3
    }

    public class UnitConvertor
    {
        public static decimal convertKG2LB(decimal qtyKG)
        {
            decimal qtyLB = 0;
            qtyLB = Math.Round((qtyKG * 2.20462m), 2);
            return qtyLB;
        }

        public static decimal convertG2LB(decimal qtyKG)
        {
            decimal qtyLB = 0;
            qtyLB = Math.Round(((qtyKG / 1000m) * 2.20462m), 2);
            return qtyLB;
        }

        public static decimal getConvertedQty(decimal qty, int unitID)
        {
            decimal convertedQty = 0.00m;

            switch (unitID)
            {
                case (int)UnitName.each:
                    convertedQty = qty;
                    break;
                case (int)UnitName.lb:
                    convertedQty = qty;
                    break;
                case (int)UnitName.kg:
                    convertedQty = qty * 2.20462m;
                    break;
                case (int)UnitName.g:
                    convertedQty = (qty / 1000) * 2.20462m;
                    break;
            }

            return decimal.Round(convertedQty, 3);

        }
    }
}

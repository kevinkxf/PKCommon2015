using System;
using System.Collections.Generic;
using System.Text;



namespace PKCommon
{
    public class CustomerSearchCriteria
    {
        public CustomerSearchCriteria()
        {
            ID = string.Empty;
            Name = string.Empty;
            IDNumber = string.Empty;
            Phone = string.Empty;

        }

        public CustomerSearchCriteria(string strID, string strName, string strIDNumber, string strPhone)
        {
            ID = strID;
            Name = strName;
            IDNumber = strIDNumber;
            Phone = strPhone;
        }

        public string ID { get; set; }
        public string Name { get; set; }
        public string IDNumber { get; set; }
        public string Phone { get; set; }

    }
}

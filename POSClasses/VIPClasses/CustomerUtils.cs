using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;

namespace PKCommon
{
    public class CustomerUtils
    {
        private string strConnectionString = string.Empty;// ConfigurationManager.ConnectionStrings["RomeStation.Properties.Settings.PKRomeStnConnectionString"].ToString();

        public CustomerUtils(string connectionString)
        {
            strConnectionString = connectionString;
        }

        public bool SaveCustomer(CustomerInfo newCustomer)
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = strConnectionString;
            try
            {
                SqlCommand cmd = new SqlCommand();
                string strSQL = @"INSERT INTO Customer                              
                                    (ID, CustomerNo, FirstName, LastName, Phone, IDCardNo, EMail, Address, City, Province, Country, Postal, CreateDate, UpdateDate, Status, Remarks, TEL, FAX, 
                                    TotalPurchaseAmount, LockTotalPurchaseAmount, DiscountPercentage, LockDiscountPercentage, Points, LockPoints, MaxDiscountamount, DiscountAmountLeft, BirthDay, Gender, CustomerType, VIPType, RegLocation, updater) 
                                    VALUES 
                                    (@ID, @CustomerNo, @FirstName, @LastName, @Phone, @IDCardNo, @EMail, @Address, @City, @Province, @Country, @Postal, 
                                        @CreateDate, @UpdateDate, @Status, @Remarks, @TEL, @FAX, 
                                    @TotalPurchaseAmount, @LockTotalPurchaseAmount, @DiscountPercentage, @LockDiscountPercentage, @Points, @LockPoints,@MaxDiscountamount, @DiscountAmountLeft, @BirthDay, @Gender, @CustomerType, @VIPType , @RegLocation, @updater)";

                cmd.Connection = cn;
                cmd.CommandText = strSQL;
                AddCmdParameters(newCustomer, cmd);

                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();
                return true;

            }
            catch (Exception ex)
            {
                string strError = ex.Message;
                //Kevin  20140925: Add log for test 
                PKCommon.Logger.AddToFile("PKCommon->CustomerUtils->SaveCustomer:" + ex.Message.ToString());
                Global.DATABASE_ACCESS_ERROR_MESSAGE = "Database Save Customer Error! Please contact your system provider.";
                return false;
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
            }
        }

        private void AddCmdParameters(CustomerInfo Customer, SqlCommand cmd)
        {
            cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 50).Value = Customer.ID;
            cmd.Parameters.Add("@CustomerNo", SqlDbType.NVarChar, 50).Value = Customer.CustomerNo;
            cmd.Parameters.Add("@FirstName", SqlDbType.NVarChar, 50).Value = Customer.FirstName;
            cmd.Parameters.Add("@LastName", SqlDbType.NVarChar, 50).Value = Customer.LastName;
            cmd.Parameters.Add("@Phone", SqlDbType.NVarChar, 50).Value = Customer.Phone;
            cmd.Parameters.Add("@IDCardNo", SqlDbType.NVarChar, 50).Value = Customer.IDCardNo;

            cmd.Parameters.Add("@EMail", SqlDbType.NVarChar, 200).Value = Customer.EMail;
            cmd.Parameters.Add("@Address", SqlDbType.NVarChar, 200).Value = Customer.Address;
            cmd.Parameters.Add("@City", SqlDbType.NVarChar, 50).Value = Customer.City;
            cmd.Parameters.Add("@Province", SqlDbType.NVarChar, 50).Value = Customer.Province;
            cmd.Parameters.Add("@Country", SqlDbType.NVarChar, 50).Value = Customer.Country;
            cmd.Parameters.Add("@Postal", SqlDbType.NVarChar, 50).Value = Customer.Postal;

            cmd.Parameters.Add("@CreateDate", SqlDbType.NVarChar, 50).Value = Customer.CreateDate;
            cmd.Parameters.Add("@UpdateDate", SqlDbType.NVarChar, 50).Value = Customer.UpdateDate;
            cmd.Parameters.Add("@Status", SqlDbType.NVarChar, 50).Value = Customer.Status;
            cmd.Parameters.Add("@Remarks", SqlDbType.NVarChar, 500).Value = Customer.Remarks;
            cmd.Parameters.Add("@TEL", SqlDbType.NVarChar, 50).Value = Customer.TEL;
            cmd.Parameters.Add("@FAX", SqlDbType.NVarChar, 50).Value = Customer.FAX;

            cmd.Parameters.Add("@TotalPurchaseAmount", SqlDbType.Decimal).Value = Customer.TotalPurchaseAmount;
            cmd.Parameters.Add("@LockTotalPurchaseAmount", SqlDbType.NVarChar, 50).Value = Customer.LockTotalPurchaseAmount;
            cmd.Parameters.Add("@DiscountPercentage", SqlDbType.Decimal).Value = Customer.DiscountPercentage;
            cmd.Parameters.Add("@LockDiscountPercentage", SqlDbType.NVarChar, 50).Value = Customer.LockDiscountPercentage;
            cmd.Parameters.Add("@Points", SqlDbType.Decimal).Value = Customer.Points;
            cmd.Parameters.Add("@LockPoints", SqlDbType.NVarChar, 50).Value = Customer.LockPoints;

            cmd.Parameters.Add("@MaxDiscountAmount", SqlDbType.Decimal).Value = Customer.MaxDiscountAmount;
            cmd.Parameters.Add("@DiscountAmountLeft", SqlDbType.Decimal).Value = Customer.DiscountAmountLeft;
            if (Customer.CustomerType == null)
            {
                cmd.Parameters.Add("@CustomerType", SqlDbType.NVarChar, 50).Value = string.Empty;   // Customer.CustomerType;
            }
            else
            {
                cmd.Parameters.Add("@CustomerType", SqlDbType.NVarChar, 50).Value = Customer.CustomerType;
            }

            if (Customer.Birthday == null)
                cmd.Parameters.Add("@Birthday", SqlDbType.DateTime).Value = DBNull.Value;
            else
                cmd.Parameters.Add("@Birthday", SqlDbType.DateTime).Value = Customer.Birthday;

            if (Customer.Gender == null)
            {
                cmd.Parameters.Add("@Gender", SqlDbType.NVarChar, 50).Value = string.Empty;
            }
            else
            {
                cmd.Parameters.Add("@Gender", SqlDbType.NVarChar, 50).Value = Customer.Gender;
            }

            if (Customer.VIPType == null)
            {
                cmd.Parameters.Add("@VIPType", SqlDbType.NVarChar, 50).Value = string.Empty;
            }
            else
            {
                cmd.Parameters.Add("@VIPType", SqlDbType.NVarChar, 50).Value = Customer.VIPType;
            }

            if (Customer.RegLocation == null)
            {
                cmd.Parameters.Add("@RegLocation", SqlDbType.VarChar, 50).Value = string.Empty;
            }
            else
            {
                cmd.Parameters.Add("@RegLocation", SqlDbType.VarChar, 50).Value = Customer.RegLocation;
            }

            if (Customer.updater == null)
            {
                cmd.Parameters.Add("@updater", SqlDbType.VarChar, 50).Value = string.Empty;
            }
            else
            {
                cmd.Parameters.Add("@updater", SqlDbType.VarChar, 50).Value = Customer.updater;
            }

        }
        public bool UpdateCustomerFromPOS(CustomerInfo Customer)
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = strConnectionString;
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"Update Customer
                                    SET  UpdateDate=@UpdateDate, TotalPurchaseAmount=@TotalPurchaseAmount, Points=@Points, DiscountAmountLeft=@DiscountAmountLeft WHERE ID=@ID";
                cmd.Connection = cn;
                AddCmdParameters(Customer, cmd);
                PKCommon.Logger.AddToFile(cmd.CommandText); //Kevin  20141003

                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();

                return true;

            }
            catch (Exception ex)
            {
                string strError = ex.Message;
                Global.DATABASE_ACCESS_ERROR_MESSAGE = "Database Update Customer Error! Please contact your system provider.";
                return false;
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
            }
        }

        public bool UpdateCustomer(CustomerInfo Customer)
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = strConnectionString;
            try
            {
                SqlCommand cmd = new SqlCommand();

                cmd.CommandText = @"Update Customer
                                    SET ID=@ID, CustomerNo=@CustomerNo, FirstName=@FirstName, LastName=@LastName, Phone=@Phone, IDCardNo=@IDCardNo, EMail=@EMail, 
                                        Address=@Address, City=@City, Province=@Province, Country=@Country, Postal=@Postal, CreateDate=@CreateDate, UpdateDate=@UpdateDate, Status=@Status, Remarks=@Remarks,
                                        TEL=@TEL, FAX=@FAX, 
                                        TotalPurchaseAmount=@TotalPurchaseAmount, LockTotalPurchaseAmount=@LockTotalPurchaseAmount, DiscountPercentage=@DiscountPercentage, 
                                        LockDiscountPercentage=@LockDiscountPercentage, Points=@Points, LockPoints=@LockPoints, MaxDiscountAmount = @MaxDiscountAmount, 
                                        DiscountAmountLeft=@DiscountAmountLeft, CustomerType=@CustomerType, Birthday=@Birthday, 
                                        Gender=@gender, VIPType=@VIPType, updater=@updater WHERE ID=@ID";

                cmd.Connection = cn;
                AddCmdParameters(Customer, cmd);
                PKCommon.Logger.AddToFile(cmd.CommandText); //Kevin  20141003

                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();

                return true;

            }
            catch (Exception ex)
            {
                string strError = ex.Message;
                Global.DATABASE_ACCESS_ERROR_MESSAGE = "Database Update Customer Error! Please contact your system provider.";
                return false;
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
            }
        }

        public bool DeleteCustomer(string CustomerID)
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = strConnectionString;
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"Update Customer
                                    SET Status=@Status, CustomerNo=@CustomerNo                           
                                    WHERE ID=@ID";
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@ID", CustomerID);
                cmd.Parameters.AddWithValue("@Status", Global.CUSTOMER_STATUS_Deleted);
                cmd.Parameters.AddWithValue("@CustomerNo", "C000000");

                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();

                return true;

            }
            catch (Exception ex)
            {
                string strError = ex.Message;
                Global.DATABASE_ACCESS_ERROR_MESSAGE = "Database Update Customer Error! Please contact your system provider.";
                return false;
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
            }
        }

        public CustomerInfo GetCustomerInfo(string CustomerID)
        {
            SqlConnection cn = new SqlConnection();
            CustomerInfo ci = new CustomerInfo();

            try
            {
                string strSQL = @"SELECT ID, CustomerNo, FirstName, LastName, Phone, IDCardNo, EMail, Address, City, Province, Country, Postal, CreateDate, UpdateDate, Status, Remarks, TEL, FAX, 
                                  TotalPurchaseAmount, LockTotalPurchaseAmount, DiscountPercentage, LockDiscountPercentage, Points, LockPoints, MaxDiscountAmount, DiscountAmountLeft,CustomerType,Birthday,Gender, CustomerType, VIPType,isnull(updater,'') as updater
                                  FROM Customer
                                  WHERE ID=@ID";

                cn.ConnectionString = strConnectionString;
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@ID", CustomerID);
                cmd.CommandText = strSQL;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        ci.ID = dr["ID"].ToString();
                        ci.CustomerNo = dr["CustomerNo"].ToString();
                        ci.FirstName = dr["FirstName"].ToString();
                        ci.LastName = dr["LastName"].ToString();
                        ci.Phone = dr["Phone"].ToString();
                        ci.EMail = dr["EMail"].ToString();
                        ci.IDCardNo = dr["IDCardNo"].ToString();
                        ci.Address = dr["Address"].ToString();
                        ci.City = dr["City"].ToString();
                        ci.Province = dr["Province"].ToString();
                        ci.Country = dr["Country"].ToString();
                        ci.Postal = dr["Postal"].ToString();
                        if (dr["Birthday"] == null || dr["Birthday"].ToString() == string.Empty)
                            ci.Birthday = null;
                        else
                            ci.Birthday = (DateTime)dr["Birthday"];
                        ci.Gender = dr["Gender"].ToString();
                        if (dr["CreateDate"] != null || dr["CreateDate"].ToString() != string.Empty)
                        {
                            ci.CreateDate = ((DateTime)dr["CreateDate"]).ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        else
                        {
                            ci.CreateDate = "";
                        }

                        if (dr["UpdateDate"] != null || dr["UpdateDate"].ToString() != string.Empty)
                        {
                            ci.UpdateDate = ((DateTime)dr["UpdateDate"]).ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        else
                        {
                            ci.UpdateDate = "";
                        }
                        ci.Status = dr["Status"].ToString();
                        ci.Remarks = dr["Remarks"].ToString();
                        ci.TEL = dr["TEL"].ToString();
                        ci.FAX = dr["FAX"].ToString();
                        ci.CustomerType = dr["CustomerType"].ToString();
                        if (dr["TotalPurchaseAmount"] != null && dr["TotalPurchaseAmount"].ToString() != string.Empty)
                        {
                            ci.TotalPurchaseAmount = (decimal)dr["TotalPurchaseAmount"];
                        }
                        else
                        {
                            ci.TotalPurchaseAmount = 0.00m;
                        }
                        ci.LockTotalPurchaseAmount = dr["LockTotalPurchaseAmount"].ToString();

                        if (dr["DiscountPercentage"] != null && dr["DiscountPercentage"].ToString() != string.Empty)
                        {
                            ci.DiscountPercentage = (decimal)dr["DiscountPercentage"];
                        }
                        else
                        {
                            ci.DiscountPercentage = 0.00m;
                        }
                        ci.LockDiscountPercentage = dr["LockDiscountPercentage"].ToString();

                        if (dr["Points"] != null && dr["Points"].ToString() != string.Empty)
                        {
                            ci.Points = (decimal)dr["Points"];
                        }
                        else
                        {
                            ci.Points = 0.00m;
                        }
                        ci.LockPoints = dr["LockPoints"].ToString();
                        decimal dMDA = 0;
                        decimal.TryParse(dr["MaxDiscountAmount"].ToString(), out dMDA);
                        ci.MaxDiscountAmount = dMDA;
                        decimal dDAL = 0;
                        decimal.TryParse(dr["DiscountAmountLeft"].ToString(), out dDAL);
                        ci.DiscountAmountLeft = dDAL;
                        ci.CustomerType = dr["CustomerType"].ToString();
                        ci.VIPType = dr["VIPType"].ToString();

                    }
                }
                cn.Close();
                return ci;
            }
            catch (Exception ex)
            {
                string strError = ex.Message;
                Global.DATABASE_ACCESS_ERROR_MESSAGE = "Database Get Customer Information Error! Please contact your system provider.";
                return null;
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
            }
        }

        public string GenerateCustomerNo()
        {
            try
            {
                SqlConnection cnn = new SqlConnection();
                cnn.ConnectionString = strConnectionString;
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT max(CustomerNo) from Customer where customerNo like 'C%'";

                cmd.Connection = cnn;
                cnn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                string strMAXNo;
                int intMaxNo = 0;
                while (dr.Read())
                {
                    if (dr[0] != null && dr[0].ToString() != string.Empty)
                    {
                        strMAXNo = dr[0].ToString();
                        intMaxNo = Convert.ToInt32(strMAXNo.Substring(1));
                        intMaxNo++;
                        string strNo = string.Empty;
                        if (intMaxNo < 999999)
                        {
                            strNo = intMaxNo.ToString();
                            if (strNo.Length == 1)
                            {
                                strNo = "C00000" + strNo;
                            }
                            else if (strNo.Length == 2)
                            {
                                strNo = "C0000" + strNo;
                            }
                            else if (strNo.Length == 3)
                            {
                                strNo = "C000" + strNo;
                            }
                            else if (strNo.Length == 4)
                            {
                                strNo = "C00" + strNo;
                            }
                            else if (strNo.Length == 5)
                            {
                                strNo = "C0" + strNo;
                            }
                            else if (strNo.Length == 6)
                            {
                                strNo = "C" + strNo;
                            }
                        }
                        else
                        {
                            int intNo = 0;
                            for (int count = 1; count <= 999999; count++)
                            {
                                intNo++;
                                string strAvailabeNo = intNo.ToString();
                                if (strAvailabeNo.Length == 1)
                                {
                                    strAvailabeNo = "C00000" + strAvailabeNo;
                                }
                                else if (strAvailabeNo.Length == 2)
                                {
                                    strAvailabeNo = "C0000" + strAvailabeNo;
                                }
                                else if (strAvailabeNo.Length == 3)
                                {
                                    strAvailabeNo = "C000" + strAvailabeNo;
                                }
                                else if (strAvailabeNo.Length == 4)
                                {
                                    strAvailabeNo = "C00" + strAvailabeNo;
                                }
                                else if (strAvailabeNo.Length == 5)
                                {
                                    strAvailabeNo = "C0" + strAvailabeNo;
                                }
                                else if (strAvailabeNo.Length == 6)
                                {
                                    strAvailabeNo = "C" + strAvailabeNo;
                                }

                                PKCommon.CustomerUtils PKCUtils = new CustomerUtils(strConnectionString);
                                if (PKCUtils.SelectNonUseCustomerNo(strAvailabeNo))
                                {
                                    strNo = strAvailabeNo;
                                    break;
                                }
                                else
                                {
                                    strNo = "Error";
                                }
                            }
                        }
                        cnn.Close();
                        return strNo;
                    }
                }
                cnn.Close();
                return "C000001";
            }
            catch (Exception ex)
            {
                string strError = ex.Message;
                return strError;
            }
        }

        public bool SelectNonUseCustomerNo(string CustomerNo)
        {
            SqlConnection cn = new SqlConnection();
            CustomerInfo ci = new CustomerInfo();

            try
            {
                string strsql = @"SELECT ID, CustomerNo FROM Customer
                                where CustomerNo=@CustomerNo";

                cn.ConnectionString = strConnectionString;
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = cn;
                    cmd.CommandText = strsql;
                    cmd.Parameters.AddWithValue("@CustomerNo", CustomerNo);
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ci.ID = dr["ID"].ToString();
                            ci.CustomerNo = dr["CustomerNo"].ToString();
                            break;
                        }
                        if (!dr.HasRows)
                        {
                            cn.Close();
                            return true;
                        }
                        else
                        {
                            cn.Close();
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO log 
                string error = ex.Message;
                return false;
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
            }
        }


        public List<TextValuePair> GetCustomerNameList(CustomerSearchCriteria CustomerSearchCriteria)
        {
            SqlConnection cn = new SqlConnection();
            List<TextValuePair> condignorList = new List<TextValuePair>();
            string strsql = string.Empty;
            try
            {
                strsql = @"SELECT ID, CustomerNo, FirstName, LastName, Phone, IDCardNo, EMail, Address, City, Province, Country, Postal, CreateDate, UpdateDate, Status, Remarks, TEL, FAX
                             FROM Customer";

                string strwhere = GetCriteriaString(CustomerSearchCriteria);
                strsql += strwhere;
                strsql += " Order by FirstName";

                cn.ConnectionString = strConnectionString;
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.CommandText = strsql;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        TextValuePair tvp = new TextValuePair();
                        string phone = string.Empty;
                        if (dr["Phone"].ToString().Length == 0)
                        {
                            phone = string.Empty;
                        }
                        else if (dr["Phone"].ToString().Length > 0 && dr["Phone"].ToString().Length <= 3)
                        {
                            phone = "(" + dr["Phone"].ToString().Substring(0, dr["Phone"].ToString().Length) + ")";
                        }
                        else if (dr["Phone"].ToString().Length > 3 && dr["Phone"].ToString().Length <= 6)
                        {
                            phone = "(" + dr["Phone"].ToString().Substring(0, 3) + ") " + dr["Phone"].ToString().Substring(3, dr["Phone"].ToString().Length - 3) + "-";
                        }
                        else if (dr["Phone"].ToString().Length > 6)
                        {
                            phone = "(" + dr["Phone"].ToString().Substring(0, 3) + ") " + dr["Phone"].ToString().Substring(3, 3) + "-" + dr["Phone"].ToString().Substring(6, dr["Phone"].ToString().Length - 6);
                        }
                        tvp.MyText = dr["FirstName"].ToString() + " " + dr["LastName"].ToString() + " " + phone;
                        tvp.MyValue = dr["ID"].ToString();

                        condignorList.Add(tvp);
                    }
                }
                cn.Close();
                return condignorList;
            }
            catch (Exception ex)
            {
                string strError = ex.Message;
                Global.DATABASE_ACCESS_ERROR_MESSAGE = "Database Get Customer List Error! Please contact your system provider.";
                return null;
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
            }
        }

        private string GetCriteriaString(CustomerSearchCriteria CustomerSearchCriteria)
        {
            if (CustomerSearchCriteria.ID != string.Empty)
            {
                return " where ID='" + CustomerSearchCriteria.ID + "' and Status !='" + Global.CUSTOMER_STATUS_Deleted + "'";
            }
            string strwhere = " where Status !='" + Global.CUSTOMER_STATUS_Deleted + "'";
            bool isCriteriaSet = true;
            if (CustomerSearchCriteria.Name != string.Empty)
            {
                if (isCriteriaSet)
                {
                    strwhere += " and ";
                }
                strwhere += " (FirstName like '%" + CustomerSearchCriteria.Name + "%' OR LastName like '%" + CustomerSearchCriteria.Name + "%')";
                isCriteriaSet = true;
            }
            if (CustomerSearchCriteria.IDNumber != string.Empty)
            {
                if (isCriteriaSet)
                {
                    strwhere += " and ";
                }
                strwhere += " IDCardNo like'%" + CustomerSearchCriteria.IDNumber + "%' ";
                isCriteriaSet = true;
            }
            if (CustomerSearchCriteria.Phone != string.Empty)
            {
                if (isCriteriaSet)
                {
                    strwhere += " and ";
                }
                strwhere += " Phone like'%" + CustomerSearchCriteria.Phone + "%' OR TEL like '%" + CustomerSearchCriteria.Phone + "%'";
                isCriteriaSet = true;
            }

            return strwhere;
            //if (isCriteriaSet)
            //{
            //    return strwhere;
            //}
            //else
            //{
            //    return string.Empty;
            //}
        }

        public bool SaveCustomerTransaction(CustomerTransactionInfo newCustomerTransaction)
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = strConnectionString;
            try
            {
                SqlCommand cmd = new SqlCommand();
                string strSQL = @"INSERT INTO CustomerTransaction                              
                                    (CustomerID, TransactionID ) 
                                    VALUES 
                                    (@CustomerID, @TransactionID)";

                cmd.Connection = cn;
                cmd.CommandText = strSQL;
                AddCmdParameters(newCustomerTransaction, cmd);

                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();

                return true;

            }
            catch (Exception ex)
            {
                string strError = ex.Message;
                Global.DATABASE_ACCESS_ERROR_MESSAGE = "Database Save CustomerTransaction Error! Please contact your system provider.";
                return false;
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
            }
        }

        private void AddCmdParameters(CustomerTransactionInfo CustomerTransaction, SqlCommand cmd)
        {
            cmd.Parameters.Add("@CustomerID", SqlDbType.NVarChar, 50).Value = CustomerTransaction.CustomerID;
            cmd.Parameters.Add("@TransactionID", SqlDbType.NVarChar, 50).Value = CustomerTransaction.TransactionID;
        }

        public List<TransactionInfo> GetTransactionListByCustomerID(string customerID)
        {
            SqlConnection cn = new SqlConnection();
            List<TransactionInfo> tiList = new List<TransactionInfo>();

            try
            {
                string strSQL = @"SELECT post.ID, StoreName, ComputerName, Cashier, TransactionNo, Type, TotalItemCount, ItemTotalCost, ItemDiscountTotalAmount, SubTotalAmount, SubTotalDiscount, DollarDiscount, 
                                        AllTaxTotalAmount, TotalAmount, post.Status, StatusDateTime, ReferenceTransactionNo
                                  FROM POSTransaction AS post RIGHT JOIN CustomerTransaction AS ct ON post.ID = ct.TransactionID
                                       LEFT JOIN Customer AS c ON ct.CustomerID = c.ID
                                  WHERE c.ID=@CustomerID";

                cn.ConnectionString = strConnectionString;
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@CustomerID", customerID);
                cmd.CommandText = strSQL;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        TransactionInfo ti = new TransactionInfo();
                        ti.ID = dr["ID"].ToString();
                        ti.StoreName = dr["StoreName"].ToString();
                        ti.ComputerName = dr["ComputerName"].ToString();
                        ti.Cashier = dr["Cashier"].ToString();
                        ti.TransactionNo = dr["TransactionNo"].ToString();
                        ti.Type = dr["Type"].ToString();
                        if (dr["TotalItemCount"] != null && dr["TotalItemCount"].ToString() != string.Empty)
                        {
                            ti.TotalItemCount = (int)dr["TotalItemCount"];
                        }
                        else
                        {
                            ti.TotalItemCount = 0;
                        }

                        if (dr["ItemTotalCost"] != null && dr["ItemTotalCost"].ToString() != string.Empty)
                        {
                            ti.ItemTotalCost = (decimal)dr["ItemTotalCost"];
                        }
                        else
                        {
                            ti.ItemTotalCost = 0.00m;
                        }

                        if (dr["ItemDiscountTotalAmount"] != null && dr["ItemDiscountTotalAmount"].ToString() != string.Empty)
                        {
                            ti.ItemDiscountTotalAmount = (decimal)dr["ItemDiscountTotalAmount"];
                        }
                        else
                        {
                            ti.ItemDiscountTotalAmount = 0.00m;
                        }

                        if (dr["SubTotalAmount"] != null && dr["SubTotalAmount"].ToString() != string.Empty)
                        {
                            ti.SubTotalAmount = (decimal)dr["SubTotalAmount"];
                        }
                        else
                        {
                            ti.SubTotalAmount = 0.00m;
                        }

                        if (dr["SubTotalDiscount"] != null && dr["SubTotalDiscount"].ToString() != string.Empty)
                        {
                            ti.SubTotalDiscount = (decimal)dr["SubTotalDiscount"];
                        }
                        else
                        {
                            ti.SubTotalDiscount = 0.00m;
                        }

                        if (dr["DollarDiscount"] != null && dr["DollarDiscount"].ToString() != string.Empty)
                        {
                            ti.DollarDiscount = (decimal)dr["DollarDiscount"];
                        }
                        else
                        {
                            ti.DollarDiscount = 0.00m;
                        }

                        if (dr["AllTaxTotalAmount"] != null && dr["AllTaxTotalAmount"].ToString() != string.Empty)
                        {
                            ti.AllTaxTotalAmount = (decimal)dr["AllTaxTotalAmount"];
                        }
                        else
                        {
                            ti.AllTaxTotalAmount = 0.00m;
                        }

                        if (dr["TotalAmount"] != null && dr["TotalAmount"].ToString() != string.Empty)
                        {
                            ti.TotalAmount = (decimal)dr["TotalAmount"];
                        }
                        else
                        {
                            ti.TotalAmount = 0.00m;
                        }
                        ti.Status = dr["Status"].ToString();
                        ti.StatusDateTime = ((DateTime)dr["StatusDateTime"]).ToString("yyyy-MM-dd HH:mm:ss");
                        ti.ReferenceTransactionNo = dr["ReferenceTransactionNo"].ToString();

                        tiList.Add(ti);
                    }
                }
                cn.Close();
                return tiList;
            }
            catch (Exception ex)
            {
                string strError = ex.Message;
                Global.DATABASE_ACCESS_ERROR_MESSAGE = "Database Get Customer Transaction List Error! Please contact your system provider.";
                return null;
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
            }
        }

        public List<TransactionInfo> GetTransactionListByCustomerID(string customerID, string strDateTimeFrom, string strDateTimeTo)
        {


                List<TransactionInfo> tiList = new List<TransactionInfo>();
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["PKRetailConnectionString"].ConnectionString);
                DataSet ds = new DataSet();
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("PK_GetTransactionListByCustomerID", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CustomerID", customerID);
                    cmd.Parameters.AddWithValue("@DateTimeFrom", strDateTimeFrom);
                    cmd.Parameters.AddWithValue("@DateTimeTo", strDateTimeTo);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);


                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {

                            DataRow dr = ds.Tables[0].Rows[i];

                            TransactionInfo ti = new TransactionInfo();
                            ti.ID = dr["ID"].ToString();
                            ti.StoreName = dr["StoreName"].ToString();
                            ti.ComputerName = dr["ComputerName"].ToString();
                            ti.Cashier = dr["Cashier"].ToString();
                            ti.TransactionNo = dr["TransactionNo"].ToString();
                            ti.Type = dr["Type"].ToString();
                            if (dr["TotalItemCount"] != null && dr["TotalItemCount"].ToString() != string.Empty)
                            {
                                ti.TotalItemCount = (int)dr["TotalItemCount"];
                            }
                            else
                            {
                                ti.TotalItemCount = 0;
                            }

                            if (dr["ItemTotalCost"] != null && dr["ItemTotalCost"].ToString() != string.Empty)
                            {
                                ti.ItemTotalCost = (decimal)dr["ItemTotalCost"];
                            }
                            else
                            {
                                ti.ItemTotalCost = 0.00m;
                            }

                            if (dr["ItemDiscountTotalAmount"] != null && dr["ItemDiscountTotalAmount"].ToString() != string.Empty)
                            {
                                ti.ItemDiscountTotalAmount = (decimal)dr["ItemDiscountTotalAmount"];
                            }
                            else
                            {
                                ti.ItemDiscountTotalAmount = 0.00m;
                            }

                            if (dr["SubTotalAmount"] != null && dr["SubTotalAmount"].ToString() != string.Empty)
                            {
                                ti.SubTotalAmount = (decimal)dr["SubTotalAmount"];
                            }
                            else
                            {
                                ti.SubTotalAmount = 0.00m;
                            }

                            if (dr["SubTotalDiscount"] != null && dr["SubTotalDiscount"].ToString() != string.Empty)
                            {
                                ti.SubTotalDiscount = (decimal)dr["SubTotalDiscount"];
                            }
                            else
                            {
                                ti.SubTotalDiscount = 0.00m;
                            }

                            if (dr["DollarDiscount"] != null && dr["DollarDiscount"].ToString() != string.Empty)
                            {
                                ti.DollarDiscount = (decimal)dr["DollarDiscount"];
                            }
                            else
                            {
                                ti.DollarDiscount = 0.00m;
                            }

                            if (dr["AllTaxTotalAmount"] != null && dr["AllTaxTotalAmount"].ToString() != string.Empty)
                            {
                                ti.AllTaxTotalAmount = (decimal)dr["AllTaxTotalAmount"];
                            }
                            else
                            {
                                ti.AllTaxTotalAmount = 0.00m;
                            }

                            if (dr["TotalAmount"] != null && dr["TotalAmount"].ToString() != string.Empty)
                            {
                                ti.TotalAmount = (decimal)dr["TotalAmount"];
                            }
                            else
                            {
                                ti.TotalAmount = 0.00m;
                            }
                            ti.Status = dr["Status"].ToString();
                            ti.StatusDateTime = ((DateTime)dr["StatusDateTime"]).ToString("yyyy-MM-dd HH:mm:ss");
                            ti.ReferenceTransactionNo = dr["ReferenceTransactionNo"].ToString();

                            tiList.Add(ti);

                        }
                    }
                }
                catch (Exception ex)
                {
                    throw (new Exception("Error in GetDataTable", ex));
                }
                finally
                {
                    if (con != null) con.Close();
                    if (ds != null) ds.Dispose();
                }

                
                return tiList;
            
        }

        public CustomerInfo GetCustomerInfoByTransactionNo(string transactionNo)
        {
            SqlConnection cn = new SqlConnection();
            CustomerInfo ci = new CustomerInfo();

            try
            {
                string strSQL = @"SELECT c.ID, CustomerNo, FirstName, LastName, Phone, IDCardNo, EMail, Address, City, Province, Country, Postal, CreateDate, UpdateDate, c.Status, c.Remarks, c.TEL, c.FAX,
                                    c.TotalPurchaseAmount, c.LockTotalPurchaseAmount, c.DiscountPercentage, c.LockDiscountPercentage, c.Points, c.LockPoints
                                  FROM POSTransaction AS post RIGHT JOIN CustomerTransaction AS ct ON post.ID = ct.TransactionID
                                       LEFT JOIN Customer AS c ON ct.CustomerID = c.ID
                                  WHERE TransactionNo Like @TransactionNo";

                cn.ConnectionString = strConnectionString;
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@TransactionNo", transactionNo);
                cmd.CommandText = strSQL;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        ci.ID = dr["ID"].ToString();
                        ci.CustomerNo = dr["CustomerNo"].ToString();
                        ci.FirstName = dr["FirstName"].ToString();
                        ci.LastName = dr["LastName"].ToString();
                        ci.Phone = dr["Phone"].ToString();
                        ci.EMail = dr["EMail"].ToString();
                        ci.IDCardNo = dr["IDCardNo"].ToString();
                        ci.Address = dr["Address"].ToString();
                        ci.City = dr["City"].ToString();
                        ci.Province = dr["Province"].ToString();
                        ci.Country = dr["Country"].ToString();
                        ci.Postal = dr["Postal"].ToString();
                        if (dr["CreateDate"] != null && dr["CreateDate"].ToString() != null && dr["CreateDate"].ToString() != string.Empty && dr["CreateDate"].ToString() != "")
                        {
                            ci.CreateDate = ((DateTime)dr["CreateDate"]).ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        else
                        {
                            ci.CreateDate = "";
                        }

                        if (dr["UpdateDate"] != null && dr["UpdateDate"].ToString() != null && dr["UpdateDate"].ToString() != string.Empty && dr["UpdateDate"].ToString() != "")
                        {
                            ci.UpdateDate = ((DateTime)dr["UpdateDate"]).ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        else
                        {
                            ci.UpdateDate = "";
                        }
                        ci.Status = dr["Status"].ToString();
                        ci.Remarks = dr["Remarks"].ToString();
                        ci.TEL = dr["TEL"].ToString();
                        ci.FAX = dr["FAX"].ToString();

                        if (dr["TotalPurchaseAmount"] != null && dr["TotalPurchaseAmount"].ToString() != string.Empty)
                        {
                            ci.TotalPurchaseAmount = (decimal)dr["TotalPurchaseAmount"];
                        }
                        else
                        {
                            ci.TotalPurchaseAmount = 0.00m;
                        }
                        ci.LockTotalPurchaseAmount = dr["LockTotalPurchaseAmount"].ToString();

                        if (dr["DiscountPercentage"] != null && dr["DiscountPercentage"].ToString() != string.Empty)
                        {
                            ci.DiscountPercentage = (decimal)dr["DiscountPercentage"];
                        }
                        else
                        {
                            ci.DiscountPercentage = 0.00m;
                        }
                        ci.LockDiscountPercentage = dr["LockDiscountPercentage"].ToString();

                        if (dr["Points"] != null && dr["Points"].ToString() != string.Empty)
                        {
                            ci.Points = (decimal)dr["Points"];
                        }
                        else
                        {
                            ci.Points = 0.00m;
                        }
                        ci.LockPoints = dr["LockPoints"].ToString();
                    }
                }

                cn.Close();
                return ci;
            }
            catch (Exception ex)
            {
                string strError = ex.Message;
                Global.DATABASE_ACCESS_ERROR_MESSAGE = "Database Get Customer Info Error! Please contact your system provider.";
                return null;
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
            }
        }

        public decimal GetReferenceTotalPurchaseAmount(string customerID)
        {
            decimal decTotalPurchaseAmount = 0.00m;

            SqlConnection cn = new SqlConnection();
            List<TransactionInfo> tiList = new List<TransactionInfo>();

            try
            {
                string strSQL = @"SELECT SUM(SubTotalAmount) AS TotalPurchaseAmount
                                  FROM POSTransaction AS post RIGHT JOIN CustomerTransaction AS ct ON post.ID = ct.TransactionID
                                       LEFT JOIN Customer AS c ON ct.CustomerID = c.ID
                                  WHERE c.ID=@CustomerID AND post.Status=@Status";

                cn.ConnectionString = strConnectionString;
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@CustomerID", customerID);
                cmd.Parameters.AddWithValue("@Status", Global.TRANSACTION_STATUS_Confirmed);
                cmd.CommandText = strSQL;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        if (dr["TotalPurchaseAmount"] != null && dr["TotalPurchaseAmount"].ToString() != string.Empty)
                        {
                            decTotalPurchaseAmount = (decimal)dr["TotalPurchaseAmount"];
                        }
                        else
                        {
                            decTotalPurchaseAmount = 0.00m;
                        }
                    }
                }
                cn.Close();
                return decTotalPurchaseAmount;
            }
            catch (Exception ex)
            {
                string strError = ex.Message;
                //Global.DATABASE_ACCESS_ERROR_MESSAGE = "Database Get Customer SUM(SubTotalAmount) Error! Please contact your system provider.";
                return decTotalPurchaseAmount;
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
            }
        }

        public decimal GetReferenceTotalPurchaseAmountBeforeTax(string customerID)
        {
            decimal decTotalAmount = 0.00m;
            decimal decSubDiscountAmount = 0.00m;
            decimal decTotalPurchaseAmount = 0.00m;

            SqlConnection cn = new SqlConnection();
            List<TransactionInfo> tiList = new List<TransactionInfo>();

            try
            {
                string strSQL = @"SELECT SUM(SubTotalDiscount) AS SubDiscountAmount, SUM(SubTotalAmount) AS TotalAmount
                                  FROM POSTransaction AS post RIGHT JOIN CustomerTransaction AS ct ON post.ID = ct.TransactionID
                                       LEFT JOIN Customer AS c ON ct.CustomerID = c.ID
                                  WHERE c.ID=@CustomerID AND post.Status=@Status";

                cn.ConnectionString = strConnectionString;
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@CustomerID", customerID);
                cmd.Parameters.AddWithValue("@Status", Global.TRANSACTION_STATUS_Confirmed);
                cmd.CommandText = strSQL;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        if (dr["TotalAmount"] != null && dr["TotalAmount"].ToString() != string.Empty)
                        {
                            decTotalAmount = (decimal)dr["TotalAmount"];
                        }
                        else
                        {
                            decTotalAmount = 0.00m;
                        }
                        if (dr["SubDiscountAmount"] != null && dr["SubDiscountAmount"].ToString() != string.Empty)
                        {
                            decSubDiscountAmount = (decimal)dr["SubDiscountAmount"];
                        }
                        else
                        {
                            decSubDiscountAmount = 0.00m;
                        }
                    }

                    decTotalPurchaseAmount = decTotalAmount + decSubDiscountAmount;
                }
                cn.Close();
                return decTotalPurchaseAmount;
            }
            catch (Exception ex)
            {
                string strError = ex.Message;
                //Global.DATABASE_ACCESS_ERROR_MESSAGE = "Database Get Customer SUM(SubTotalAmount) Error! Please contact your system provider.";
                return decTotalPurchaseAmount;
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
            }
        }

        public decimal GetReferenceTotalPurchaseAmountAfterTax(string customerID)
        {
            decimal decTotalPurchaseAmount = 0.00m;

            SqlConnection cn = new SqlConnection();
            List<TransactionInfo> tiList = new List<TransactionInfo>();

            try
            {
                string strSQL = @"SELECT SUM(TotalAmount) AS TotalPurchaseAmount
                                  FROM POSTransaction AS post RIGHT JOIN CustomerTransaction AS ct ON post.ID = ct.TransactionID
                                       LEFT JOIN Customer AS c ON ct.CustomerID = c.ID
                                  WHERE c.ID=@CustomerID AND post.Status=@Status";

                cn.ConnectionString = strConnectionString;
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@CustomerID", customerID);
                cmd.Parameters.AddWithValue("@Status", Global.TRANSACTION_STATUS_Confirmed);
                cmd.CommandText = strSQL;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        if (dr["TotalPurchaseAmount"] != null && dr["TotalPurchaseAmount"].ToString() != string.Empty)
                        {
                            decTotalPurchaseAmount = (decimal)dr["TotalPurchaseAmount"];
                        }
                        else
                        {
                            decTotalPurchaseAmount = 0.00m;
                        }
                    }
                }
                cn.Close();
                return decTotalPurchaseAmount;
            }
            catch (Exception ex)
            {
                string strError = ex.Message;
                //Global.DATABASE_ACCESS_ERROR_MESSAGE = "Database Get Customer SUM(SubTotalAmount) Error! Please contact your system provider.";
                return decTotalPurchaseAmount;
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
            }
        }


        public bool UpdateCustomerTotalPurchaseAmount(string customerID, decimal totalPurchaseAmount)
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = strConnectionString;
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"Update Customer
                                    SET ID=@ID, UpdateDate=@UpdateDate, TotalPurchaseAmount=@TotalPurchaseAmount                             
                                    WHERE ID=@ID";
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@ID", customerID);
                cmd.Parameters.AddWithValue("@TotalPurchaseAmount", totalPurchaseAmount);
                cmd.Parameters.AddWithValue("@UpdateDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();

                return true;

            }
            catch (Exception ex)
            {
                string strError = ex.Message;
                Global.DATABASE_ACCESS_ERROR_MESSAGE = "Database Update CustomerTotalPurchaseAmount Error! Please contact your system provider.";
                return false;
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
            }
        }
        /*
        public bool UpdateCustomerDiscountPercentage(string customerID, decimal discountPercentage)
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = strConnectionString;
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"Update Customer
                                    SET ID=@ID, UpdateDate=@UpdateDate, DiscountPercentage=@DiscountPercentage                             
                                    WHERE ID=@ID";
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@ID", customerID);
                cmd.Parameters.AddWithValue("@DiscountPercentage", discountPercentage);
                cmd.Parameters.AddWithValue("@UpdateDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();

                return true;

            }
            catch (Exception ex)
            {
                string strError = ex.Message;
                Global.DATABASE_ACCESS_ERROR_MESSAGE = "Database Update CustomerDiscountPercentage Error! Please contact your system provider.";
                return false;
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
            }
        }
        */
        public bool UpdateCustomerPoints(string customerID, decimal points)
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = strConnectionString;
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"Update Customer
                                    SET ID=@ID, UpdateDate=@UpdateDate, Points=@Points                             
                                    WHERE ID=@ID";
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@ID", customerID);
                cmd.Parameters.AddWithValue("@Points", points);
                cmd.Parameters.AddWithValue("@UpdateDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();

                return true;

            }
            catch (Exception ex)
            {
                string strError = ex.Message;
                Global.DATABASE_ACCESS_ERROR_MESSAGE = "Database Update CustomerPoints Error! Please contact your system provider.";
                return false;
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
            }
        }

        public bool UpdateCustomerEmail(string customerID, string email)
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = strConnectionString;
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"Update Customer SET EMail=@EMail, UpdateDate=@UpdateDate WHERE ID=@ID";
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@ID", customerID);
                cmd.Parameters.AddWithValue("@EMail", email);
                cmd.Parameters.AddWithValue("@UpdateDate", DateTime.Now);

                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();

                return true;

            }
            catch (Exception ex)
            {
                string strError = ex.Message;
                Global.DATABASE_ACCESS_ERROR_MESSAGE = "Database Update CustomerEmail Error! Please contact your system provider.";
                return false;
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
            }
        }
        

    }




}
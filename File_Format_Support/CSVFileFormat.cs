using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//--Kevin  20140324
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace PKCommon.File_Format_Support
{
//--Kevin  20140324: Create new class to support CSV file 
    public class CSVFileFormat
    {


        /// <summary>
        /// Kevin  20140330: Export customized csv file for Digi scale SM100
        /// </summary>
        /// <param name="strCSVfileOut"></param>
        /// <param name="strConnectionString"></param>
        /// <returns></returns>
        public static bool exportToCSVfile(string strCSVfileOut, string strConnectionString)
        {
            Logger.AddToFile("PKCommon.File_Format_Support->exportToCSVfile: Start to export CSV");
            try
            {
                //string CSVconnectionString = ConfigurationManager.ConnectionStrings["RomeStation.Properties.Settings.PKRomeStnConnectionString"].ConnectionString;
                string CSVconnectionString = strConnectionString;
                string strSeparator = ",";

                //--Kevin  20140710: Using select count(*) to compare number of certified records--------------
                SqlConnection conCount = new SqlConnection(CSVconnectionString);
                string sqlQueryCount = "select count(*) from product where len(barcode) >=3 and len(barcode) <=5";
                SqlCommand commandCount = new SqlCommand(sqlQueryCount, conCount);
                conCount.Open();
                int countRecords = System.Convert.ToInt32(commandCount.ExecuteScalar());
                Logger.AddToFile(sqlQueryCount + "----------" + countRecords.ToString());
                conCount.Close();
                //--Kevin  20140710: Using select count(*) to compare number of certified records----End of----

                // Connects to the database, and makes the select command
                // Export 8 fields into csv file-PLU/Barcode/Name1/Name2/Weight/Price/Best Before/status
                SqlConnection conn = new SqlConnection(CSVconnectionString);
                //--Kevin  20140731: Add Size into sql
                string sqlQuery = "select ID, plu, barcode, name1, name2, weigh, price, status, Attribute1, Size from product where len(barcode) >=3 and len(barcode) <=5"; // and weigh = 'True' //--Kevin  20140505: Add ID of product to get weekly special price    

                



                SqlCommand command = new SqlCommand(sqlQuery, conn);
                conn.Open();

                // Creates a SqlDataReader instance to read data from the table.
                SqlDataReader dr = command.ExecuteReader();

                // Retrieves the schema of the table.
                DataTable dtSchema = dr.GetSchemaTable();

                // Creates the CSV file as a stream, using the given encoding.
                //--Kevin  20140320: confirmed with Digi Canada for encoding.UTF8
                StreamWriter sw = new StreamWriter(strCSVfileOut, false, Encoding.UTF8);

                // represents a full row
                string strRow;
                string strName1;
                string strName2;
                string strProductStatus;
                //--Kevin  20140505: Price 
                string strPrice;
                string strProductID;
                //--Kevin  20150731: Size for realfood
                string strSize;

                int i = 0;  //--Kevin  20140708: count records

                // Reads the rows one by one from the SqlDataReader
                // transfers them to a string with the given separator character and
                // writes it to the file.
                while (dr.Read())
                {
                    i++;
                    Logger.AddToFile("i=" + i.ToString());
                    strRow = string.Empty;
                    strName1 = string.Empty;
                    strName2 = string.Empty;

                    //for (int i = 0; i < dr.FieldCount; i++)
                    //{
                        // Detect 3/4/5 digits' length of barcode (3<= Length <= 5)
                        // Detect weigh is True(false), if true, the unit name is LB
                        // not support 100g per unit 
                        //if ((dr["barcode"].ToString().Length <= 5)
                        //    && (dr["barcode"].ToString().Length >= 3)
                        //    && ((dr["weigh"].ToString().Trim() == "True")))
                        //{
                            //strRow = dr["plu"].ToString();

                    //--Kevin  20140705: 
                            strRow = "1";    //1 is only department in WINFIS application. Please set it.
                            strRow += strSeparator;
                            strRow += dr["barcode"].ToString();
                            strRow += strSeparator;

                            // Remove comma and blank space in name1 and name2
                            strName1 = strTrimForCSV(dr["name1"].ToString().Trim());
                            if (strName1.Length > 24)
                            {
                                //PKCommon.Logger.AddToFile(strName1);
                                strName1 = strName1.Substring(0, 24);
                                //PKCommon.Logger.AddToFile(strName1);
                            }
                            //strname1 = dr["name1"].ToString().Trim();
                            strRow += strName1;
                            strRow += strSeparator;

                            strName2 = strTrimForCSV(dr["name2"].ToString().Trim());
                            if (strName2.Length > 24)
                            {
                                //PKCommon.Logger.AddToFile(strName2);
                                strName2 = strName2.Substring(0, 24);
                                //PKCommon.Logger.AddToFile(strName2);
                            }
                            //strname2 = dr["name2"].ToString().Trim();
                            strRow += strName2;
                            strRow += strSeparator;

                            if (dr["weigh"].ToString().Trim() == "True")
                            {
                                strRow += "2";  // weigh is true->2 for lb
                                strRow += strSeparator;
                            }
                            else
                            {
                                //EACH in UME column is 3
                                strRow += "3";  // weigh is true->2 for lb
                                strRow += strSeparator;
                            }



                            //--Kevin  20140505: if it is weekly speical product, we need to search PKPromotionProduct\PKPromotion\PKPromotionPrice to get price B
                            strProductID = dr["ID"].ToString(); //PKCommon.Logger.AddToFile(strProductID); 
                            strPrice = dr["price"].ToString();

                            Logger.AddToFile(strRow + "---A Price: " + strPrice);   //--Kevin  20140705: Catch the original price before searching weekly special promotion

                            string strPriceTemp = strGetWeeklySpecialPrice(strProductID, CSVconnectionString, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), false);

                            //Logger.AddToFile(strRow);

                            if (strPriceTemp != string.Empty)
                            {
                                strPrice = strPriceTemp; 
                            }


                            strRow += strPrice;
                            strRow += strSeparator;

                            //--Kevin  20140401: Attribute1 equals best-before-day. If it is empty, the value will be set to 5. 
                            //--Kevin  20140627: Attribute1 equals best-before-day. If it is empty, the value of "BBP Best Before Print Flag" will be set to 0. 
                            //it would be very similar to how it was done with the kg/ea product, u need a flag (0 or 1), then you link it to "BBP Best Before Print Flag" with the marco "1=Y;0=N"
                            //so if the flag is 0, that means it won't print the best before day, if the flag is 1, then it will print best before day.

                            if (dr["Attribute1"].ToString().Trim() == string.Empty)
                            {
                                strRow += "0";
                                strRow += strSeparator;
                                strRow += "0";
                            }
                            else
                            {
                                strRow += dr["Attribute1"].ToString();  //Best before->Expiration Date. The default value is "5".
                                strRow += strSeparator;
                                strRow += "1";
                            }
                            strRow += strSeparator;

                            //--Kevin  20140331: "D" in column 8 means this product was deleted. PLU will not shown in Digi Scale SM100 
                            strProductStatus = dr["status"].ToString();
                            if (strProductStatus == "Deleted")
                            {
                                strRow += "D";
                            }
                            else
                            {
                                strRow += "N";
                            }
                            strRow += strSeparator;

                            //PKCommon.Logger.AddToFile(strProductID + strPrice);

                        //}
                    //}

                    ////Detect if strRow is empty
                    if (strRow != string.Empty)
                    {
                        sw.WriteLine(strRow);
                        PKCommon.Logger.AddToFile(strRow);
                    }
                }

                // Closes the text stream and the database connection.
                sw.Close();
                conn.Close();

                //--Kevin  20140710: Compare first count resul with the latest i. If they are different, return false and you should check the reason.
                if (countRecords == i)
                {
                    // Notifies the user.
                    PKCommon.Logger.AddToFile("CountRecords:" + countRecords.ToString());
                    PKCommon.Logger.AddToFile("i in CSV file:" + i.ToString());  
                    PKCommon.Logger.AddToFile("PKcommon->Class CSVFileFormat: " + "End of exporting CSV! " + strCSVfileOut + " has been exported successfully from Product Table!");
                    return true;
                }
                else
                {
                    PKCommon.Logger.AddToFile("CountRecords:" + countRecords.ToString());
                    PKCommon.Logger.AddToFile("i in CSV file:" + i.ToString());                    
                    PKCommon.Logger.AddToFile("PKcommon->Class CSVFileFormat: The exported csv file is incorrect! Please export it again. ");
                    return false;
                }
            }

            //--Access to the path 'C:\WINFIE' is denied.??? How to do it??
            catch (Exception ex)
            {
                PKCommon.Logger.AddToFile("Error:PKCommon.File_Format_Support->exportToCSVfile->" + ex.Message);
                return false;
            }
            //// Notifies the user.
            //PKCommon.Logger.AddToFile("PKcommon->Class CSVFileFormat: " + strCSVfileOut + " has been exported successfully from Product Table!");
            //return true;
        }


        /// <summary>
        /// Kevin  20140330: Remove comma and blank space in string->name1 or name2
        /// </summary>
        /// <param name="strName"></param>
        /// <returns></returns>
       private static string strTrimForCSV(string strName)
        {
            string strNewName = string.Empty;

            strNewName = strName.Replace(" ", string.Empty);
            strNewName = strName.Replace(",", string.Empty);

            return strNewName;
        }

       private static string strGetWeeklySpecialPrice(string strProductID, string strConnectionString, string strDateTimeNow, bool FromServer)
       {
           int intProductQTY = 0; 
           string strProductWeeklySpecialPrice = string.Empty;
           string queryStringForProductID;
           if (FromServer == false)
           {
               queryStringForProductID = @"select p.ID, p.PLU, pkpp.PromotionID, pkpp.PromotionUnitPrice from Product as p 
                                left join PKPromotionProduct as pkpp on p.ID = pkpp.ProductID 
                                left join PKPromotion as pkpro on pkpro.ID = pkpp.PromotionID 
                                where p.ID = @productID and pkpro.Type = @Type and pkpro.Status = @Status 
                                and (pkpro.StartDate <= @DateTimeNow or pkpro.StartDate is null) and (pkpro.ExpireDate >= @DateTimeNow or pkpro.ExpireDate is null)";
           }
           else
           {
               queryStringForProductID = @"select p.ID, p.PLU, pkpp.PromotionID, pkpp.PromotionUnitPrice from PKProduct as p 
                                left join PKPromotionProduct as pkpp on p.ID = pkpp.ProductID 
                                left join PKPromotion as pkpro on pkpro.ID = pkpp.PromotionID 
                                where p.ID = @productID and pkpro.Type = @Type and pkpro.Status = @Status 
                                and (pkpro.StartDate <= @DateTimeNow or pkpro.StartDate is null) and (pkpro.ExpireDate >= @DateTimeNow or pkpro.ExpireDate is null)";
           }

           using  (SqlConnection connectionString = new SqlConnection(strConnectionString))
               try
               {
                   SqlCommand command = new SqlCommand(queryStringForProductID, connectionString);
                   command.Parameters.AddWithValue("@productID", strProductID);
                   command.Parameters.AddWithValue("@Type", "4");
                   command.Parameters.AddWithValue("@Status", "Active");
                   command.Parameters.AddWithValue("@DateTimeNow", strDateTimeNow);

                   connectionString.Open();

                   ////if (command.ExecuteNonQuery() <= 0)
                   //int intRowAffected = command.ExecuteNonQuery();
                   //if (intRowAffected == 1)
                   //{
                   //    //connectionString.Close();
                   //    //return string.Empty;
                   //    SqlDataReader reader = command.ExecuteReader();
                   //    while (reader.Read())
                   //    {
                   //        strProductWeeklySpecialPrice = reader["PromotionUnitPrice"].ToString();
                   //        //--Kevin  20140705: Add log for productID and weeklyspecialprice
                   //        PKCommon.Logger.AddToFile(strProductID + "|" + strProductWeeklySpecialPrice);
                   //    }
                   //    reader.Close();
                   //}

                   SqlDataReader reader = command.ExecuteReader();
                   while (reader.Read())
                   {
                       strProductWeeklySpecialPrice = reader["PromotionUnitPrice"].ToString();
                       //--Kevin  20140705: Add log for productID and weeklyspecialprice
                       PKCommon.Logger.AddToFile(strProductID + "|" + strProductWeeklySpecialPrice);
                       intProductQTY++;  
                   }

                   if (intProductQTY > 1)
                   {
                       Logger.AddToFile("strGetWeeklySpecialPrice()->Same barcode in the table");
                   }

                   reader.Close();
                   connectionString.Close();

                   return strProductWeeklySpecialPrice;
               }
               catch (Exception ex)
               {
                   Logger.AddToFile(ex.Message);
                   return strProductWeeklySpecialPrice = string.Empty;
               }
               finally
               {
                   if (connectionString.State == ConnectionState.Open)
                   {
                       connectionString.Close();
                   }
               }

       }

        
//       private static string strGetWeeklySpecialPrice(string strProductID, string strConnectionString, string strDateTimeNow)
//       {
//           string strProductWeeklySpecialPrice = string.Empty;
//           string strPromotionID = string.Empty;  //return strProductWeeklySpecialPrice;

//           //////1 Find product which has the promotion price 
//           SqlConnection connectionString = new SqlConnection(strConnectionString);
//           string queryStringForProductID = @"select p.ID, p.PLU, pkpp.PromotionID, pkpp.PromotionUnitPrice from Product as p 
//                                left join PKPromotionProduct as pkpp on p.ID = pkpp.ProductID 
//                                where p.ID = @productID"; 

//           SqlCommand command = new SqlCommand(queryStringForProductID, connectionString);
//           command.Parameters.AddWithValue("@productID", strProductID);
//           connectionString.Open();

//           SqlDataReader reader = command.ExecuteReader();
//           while (reader.Read())
//           {
//               strProductWeeklySpecialPrice = reader["PromotionUnitPrice"].ToString();
//               strPromotionID = reader["PromotionID"].ToString();
//           }
//           reader.Close();

//           //////2 Check weekly-speical, status, StartDate and ExpireDate
//           //--Kevin  20140512: Check weekly-speical price of promotion product
//           string strType = string.Empty;
//           if (strProductWeeklySpecialPrice == string.Empty)
//           {
//               //skip
//           }
//           else
//           {
//               string queryStringForPKPromotion = @"select * from  PKPromotion where ID = @promotionID";
//               command = new SqlCommand(queryStringForPKPromotion, connectionString);
//               command.Parameters.AddWithValue("@promotionID", strPromotionID);
//               //command.Parameters.AddWithValue("", "4");
//               //command.Parameters.AddWithValue("", "2014-05-08 00:00:00.000");

//               if (command.ExecuteNonQuery() > 0)
//               {
//                   //skip
//               }
//               else
//               {
//                   strProductWeeklySpecialPrice = string.Empty;
//               }
//           }

//           //reader.Close();
//           connectionString.Close();

//           return strProductWeeklySpecialPrice;
           

//           //--Kevin  20140507: test 
//           //using (SqlConnection con = new SqlConnection(strConnectionString))
//           //{
//           //    string sqlQuery = "Select * from PKPromotionProduct where ProductID = @productid";// + strProductID;
//           //    SqlCommand cmd = new SqlCommand(sqlQuery, con);
//           //    cmd.Parameters.AddWithValue("@productid", strProductID);
//           //    con.Open();
//           //    try
//           //    {
//           //        SqlDataReader dr = cmd.ExecuteReader();
//           //        //strProductWeeklySpecialPrice = dr["PromotionID"].ToString();
//           //        while (dr.Read())
//           //        {
//           //            strProductWeeklySpecialPrice = dr["PromotionID"].ToString();
//           //        }
//           //        dr.Close();
//           //        con.Close();
//           //        return strProductWeeklySpecialPrice;
//           //    }
//           //    catch (Exception ex)
//           //    {
//           //        PKCommon.Logger.AddToFile(ex.Message);
//           //        return string.Empty;
//           //    }
//           //}

//           //--Kevin  20140507: join Product, PKPromotionProduct, PKPromotion to find product price in weekly-special  
//           //    public static PKPromotionInfo GetPromotionInfoByProductInfo(string productID, string strDateTimeNow)

//       }
       public static bool exportToCSVfileFromServer(string strCSVfileOut, string strConnectionString)
       {
           Logger.AddToFile("PKCommon.File_Format_Support->exportToCSVfile: Start to export CSV");
           try
           {
               //string CSVconnectionString = ConfigurationManager.ConnectionStrings["RomeStation.Properties.Settings.PKRomeStnConnectionString"].ConnectionString;
               string CSVconnectionString = strConnectionString;
               string strSeparator = ",";

               //--Kevin  20140710: Using select count(*) to compare number of certified records--------------
               SqlConnection conCount = new SqlConnection(CSVconnectionString);
               string sqlQueryCount = "select count(*) from PKProduct where len(barcode) >=3 and len(barcode) <=5";
               SqlCommand commandCount = new SqlCommand(sqlQueryCount, conCount);
               conCount.Open();
               int countRecords = System.Convert.ToInt32(commandCount.ExecuteScalar());
               Logger.AddToFile(sqlQueryCount + "----------" + countRecords.ToString());
               conCount.Close();
               //--Kevin  20140710: Using select count(*) to compare number of certified records----End of----

               // Connects to the database, and makes the select command
               // Export 8 fields into csv file-PLU/Barcode/Name1/Name2/Weight/Price/Best Before/status
               SqlConnection conn = new SqlConnection(CSVconnectionString);
               //--Kevin  20140731: Add Size into sql
               string sqlQuery = @"select PKProduct.ID, plu, barcode, name1, name2, weigh, PKPrice.A as price, status, Attribute1, Size from PKProduct 
                Left Join PKPrice on PKProduct.ID = PKPrice.ProductID 
                where len(barcode) >=3 and len(barcode) <=5"; // and weigh = 'True' //--Kevin  20140505: Add ID of product to get weekly special price    





               SqlCommand command = new SqlCommand(sqlQuery, conn);
               conn.Open();

               // Creates a SqlDataReader instance to read data from the table.
               SqlDataReader dr = command.ExecuteReader();

               // Retrieves the schema of the table.
               DataTable dtSchema = dr.GetSchemaTable();

               // Creates the CSV file as a stream, using the given encoding.
               //--Kevin  20140320: confirmed with Digi Canada for encoding.UTF8
               StreamWriter sw = new StreamWriter(strCSVfileOut, false, Encoding.UTF8);

               // represents a full row
               string strRow;
               string strName1;
               string strName2;
               string strProductStatus;
               //--Kevin  20140505: Price 
               string strPrice;
               string strProductID;
               //--Kevin  20150731: Size for realfood
               string strSize;

               int i = 0;  //--Kevin  20140708: count records

               // Reads the rows one by one from the SqlDataReader
               // transfers them to a string with the given separator character and
               // writes it to the file.
               while (dr.Read())
               {
                   i++;
                   Logger.AddToFile("i=" + i.ToString());
                   strRow = string.Empty;
                   strName1 = string.Empty;
                   strName2 = string.Empty;

                   //for (int i = 0; i < dr.FieldCount; i++)
                   //{
                   // Detect 3/4/5 digits' length of barcode (3<= Length <= 5)
                   // Detect weigh is True(false), if true, the unit name is LB
                   // not support 100g per unit 
                   //if ((dr["barcode"].ToString().Length <= 5)
                   //    && (dr["barcode"].ToString().Length >= 3)
                   //    && ((dr["weigh"].ToString().Trim() == "True")))
                   //{
                   //strRow = dr["plu"].ToString();

                   //--Kevin  20140705: 
                   strRow = "1";    //1 is only department in WINFIS application. Please set it.
                   strRow += strSeparator;
                   strRow += dr["barcode"].ToString();
                   strRow += strSeparator;

                   // Remove comma and blank space in name1 and name2
                   strName1 = strTrimForCSV(dr["name1"].ToString().Trim());
                   if (strName1.Length > 24)
                   {
                       //PKCommon.Logger.AddToFile(strName1);
                       strName1 = strName1.Substring(0, 24);
                       //PKCommon.Logger.AddToFile(strName1);
                   }
                   //strname1 = dr["name1"].ToString().Trim();
                   strRow += strName1;
                   strRow += strSeparator;

                   strName2 = strTrimForCSV(dr["name2"].ToString().Trim());
                   if (strName2.Length > 24)
                   {
                       //PKCommon.Logger.AddToFile(strName2);
                       strName2 = strName2.Substring(0, 24);
                       //PKCommon.Logger.AddToFile(strName2);
                   }
                   //strname2 = dr["name2"].ToString().Trim();
                   strRow += strName2;
                   strRow += strSeparator;

                   if (dr["weigh"].ToString().Trim() == "Y")
                   {
                       strRow += "2";  // weigh is true->2 for lb
                       strRow += strSeparator;
                   }
                   else
                   {
                       //EACH in UME column is 3
                       strRow += "3";  // weigh is true->2 for lb
                       strRow += strSeparator;
                   }



                   //--Kevin  20140505: if it is weekly speical product, we need to search PKPromotionProduct\PKPromotion\PKPromotionPrice to get price B
                   strProductID = dr["ID"].ToString(); //PKCommon.Logger.AddToFile(strProductID); 
                   strPrice = dr["price"].ToString();

                   Logger.AddToFile(strRow + "---A Price: " + strPrice);   //--Kevin  20140705: Catch the original price before searching weekly special promotion

                   string strPriceTemp = strGetWeeklySpecialPrice(strProductID, CSVconnectionString, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), true);

                   //Logger.AddToFile(strRow);

                   if (strPriceTemp != string.Empty)
       {
                       strPrice = strPriceTemp;
                   }


                   strRow += strPrice;
                   strRow += strSeparator;

                   //--Kevin  20140401: Attribute1 equals best-before-day. If it is empty, the value will be set to 5. 
                   //--Kevin  20140627: Attribute1 equals best-before-day. If it is empty, the value of "BBP Best Before Print Flag" will be set to 0. 
                   //it would be very similar to how it was done with the kg/ea product, u need a flag (0 or 1), then you link it to "BBP Best Before Print Flag" with the marco "1=Y;0=N"
                   //so if the flag is 0, that means it won't print the best before day, if the flag is 1, then it will print best before day.

                   if (dr["Attribute1"].ToString().Trim() == string.Empty)
                   {
                       strRow += "0";
                       strRow += strSeparator;
                       strRow += "0";
                   }
                   else
                   {
                       strRow += dr["Attribute1"].ToString();  //Best before->Expiration Date. The default value is "5".
                       strRow += strSeparator;
                       strRow += "1";
                   }
                   strRow += strSeparator;

                   //--Kevin  20140331: "D" in column 8 means this product was deleted. PLU will not shown in Digi Scale SM100 
                   strProductStatus = dr["status"].ToString();
                   if (strProductStatus == "Deleted")
                   {
                       strRow += "D";
                   }
                   else
           {
                       strRow += "N";
           }
                   strRow += strSeparator;

                   //PKCommon.Logger.AddToFile(strProductID + strPrice);

                   //}
                   //}

                   ////Detect if strRow is empty
                   if (strRow != string.Empty)
           {
                       sw.WriteLine(strRow);
                       PKCommon.Logger.AddToFile(strRow);
                   }
           }

               // Closes the text stream and the database connection.
               sw.Close();
               conn.Close();

               //--Kevin  20140710: Compare first count resul with the latest i. If they are different, return false and you should check the reason.
               if (countRecords == i)
               {
                   // Notifies the user.
                   PKCommon.Logger.AddToFile("CountRecords:" + countRecords.ToString());
                   PKCommon.Logger.AddToFile("i in CSV file:" + i.ToString());
                   PKCommon.Logger.AddToFile("PKcommon->Class CSVFileFormat: " + "End of exporting CSV! " + strCSVfileOut + " has been exported successfully from Product Table!");
                   return true;
               }
               else
               {
                   PKCommon.Logger.AddToFile("CountRecords:" + countRecords.ToString());
                   PKCommon.Logger.AddToFile("i in CSV file:" + i.ToString());
                   PKCommon.Logger.AddToFile("PKcommon->Class CSVFileFormat: The exported csv file is incorrect! Please export it again. ");
                   return false;
               }
           }

           //--Access to the path 'C:\WINFIE' is denied.??? How to do it??
           catch (Exception ex)
           {
               PKCommon.Logger.AddToFile("Error:PKCommon.File_Format_Support->exportToCSVfile->" + ex.Message);
               return false;
           }
           //// Notifies the user.
           //PKCommon.Logger.AddToFile("PKcommon->Class CSVFileFormat: " + strCSVfileOut + " has been exported successfully from Product Table!");
           //return true;
       } 


    }
}

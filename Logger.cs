using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Configuration;
using System.Data.SqlClient;

namespace PKCommon
{
    public class Logger
    {
        public static void AddToFile(string contents)
        {
            AddToFile(contents, string.Empty);
        }

        //--Kevin  20140314: Move log file to \log folder
        //--strpath will be changed and log folder will be created
        //http://msdn.microsoft.com/en-us/library/as2f1fez.aspx 
        public static void AddToFile(string contents, string storeid)
        {
            string strPath = string.Empty;
            string strSubfolder = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "log");
            
            //Check if log sub-folder does exist
            if (!System.IO.Directory.Exists(strSubfolder))
            {
                System.IO.Directory.CreateDirectory(strSubfolder);
            }

            try
            {
                string strLogTarget = ConfigurationManager.AppSettings["LogTarget"];
                if (string.Equals(strLogTarget, "DB"))
                {
                    Add2DB(contents, storeid);
                }
                else
                {
                    strPath = strSubfolder + "\\PKLog" + storeid + DateTime.Now.Date.ToString("yyyyMMdd") + ".txt";
                    Add2File(contents, strPath);
                }

            }
            catch (Exception ex)
            {
                strPath = strSubfolder + "\\PKLogEX" + storeid + DateTime.Now.Date.ToString("yyyyMMdd") + ".txt";
                    try
                    {
                        Add2File(ex.Message, strPath);
                        Add2File(contents, strPath);
                    }
                    catch (Exception)
                    {

                    }
            }
        }

        private static void Add2DB(string contents, string strStoreID)
        {
            SqlConnection cn = new SqlConnection();
            try
            {
                cn.ConnectionString = ConfigurationManager.ConnectionStrings["SystemLogConnectionString"].ConnectionString;
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = cn;
                    cmd.CommandText = "insert into PKSystemLogs (TimeStamp, LogContent, LocationID) values (@TimeStamp, @LogContent, @LocationID)";
                    cmd.Parameters.AddWithValue("@TimeStamp", DateTime.Now.ToString(GlobalConst.strTimeStampFormat));
                    if (contents.Length > 500)
                    {
                        contents = contents.Substring(0, 500);
                    }
                    cmd.Parameters.AddWithValue("@LogContent", contents);
                    cmd.Parameters.AddWithValue("@LocationID", strStoreID);
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == System.Data.ConnectionState.Open)
                {
                    cn.Close();
                }
            }


        }

        private static void Add2File(string contents, string strPath)
        {
            //set up a filestream   
            FileStream fs = new FileStream(strPath, FileMode.OpenOrCreate, FileAccess.Write);

            //set up a streamwriter for adding text
            StreamWriter sw = new StreamWriter(fs);

            //find the end of the underlying filestream
            sw.BaseStream.Seek(0, SeekOrigin.End);

            //add the text 
            sw.WriteLine(DateTime.Now.ToString() + ": " + contents);
            //add the text to the underlying filestream

            sw.Flush();
            //close the writer
            sw.Close();
        }
    }
}


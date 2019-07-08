using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using System.IO;
using System.Web;
using Liteon.ICM.DataCore;
using LiteOn.EA.Borg.Utility;
using System.Security.Cryptography;
using System.Net;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
//using LiteOn.EA.BLL;

namespace GreenPower
{
    class Program
    {
        static ArrayList opc = new ArrayList();
        static String sToken = "";
        static SqlDB sdb = null;
        static void Main(string[] args)
        {
            try
            {
                sdb = new SqlDB(DataPara.GetDbConnectionString("GreenPower"));
                //10秒後關閉
                System.Timers.Timer t = new System.Timers.Timer(10000);
                t.Elapsed += new System.Timers.ElapsedEventHandler(exitProgram);
                t.AutoReset = false;
                t.Enabled = true;

                getToken();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                WriteLog(true, ex.Message);
            }
        }

        /// <summary>
        /// 取得API TOKEN
        /// </summary>
        static void getToken()
        {
            WebClient wc = new WebClient();
            var URI = new Uri("http://www/token");
            string sPROXY = System.Configuration.ConfigurationManager.AppSettings["PROXY"].ToString();
            int sPORT = int.Parse( System.Configuration.ConfigurationManager.AppSettings["PORT"].ToString());
            string sUID = System.Configuration.ConfigurationManager.AppSettings["UID"].ToString();
            string sUPW = System.Configuration.ConfigurationManager.AppSettings["UPW"].ToString();
            WebProxy wp = new WebProxy(sPROXY, sPORT);
            wp.Credentials = new NetworkCredential(sUID, sUPW);
            wc.Proxy = wp;
            wc.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            //wc.Headers["KEY"] = "Your_Key_Goes_Here";

            wc.UploadStringCompleted += new UploadStringCompletedEventHandler(wc_UploadStringCompleted_getToken);
            wc.UploadStringAsync(URI, "POST", "client_id=web_pyd&client_secret=web_pyd_2016&grant_type=client_credentials");
            Console.ReadLine();
        }

        static void wc_UploadStringCompleted_getToken(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                Console.WriteLine(e.Result);
                JObject joPara = JObject.Parse(e.Result);
                sToken = joPara.GetValue("access_token").ToString();
                getCompanyID();   
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString());
            }
        }

        /// <summary>
        /// 取得公司ID
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void wc_UploadStringCompleted_getCompanyID(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                Console.WriteLine(e.Result);
                getMeterArchiveByCompany();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString());
            }
        }

        /// <summary>
        /// 實時電量獲取完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void wc_UploadStringCompleted_getMeterPW(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                Console.WriteLine(e.Result);
                System.Diagnostics.Debug.WriteLine(e.Result);
                
                //寫入即時電量
                try
                {
                    JObject joR = JObject.Parse(e.Result);
                    JArray ja = JArray.Parse(joR.GetValue("body").ToString());
                    for (int i = 0; i < ja.Count; i++)
                    {
                        try
                        {
                            sdb = new SqlDB(DataPara.GetDbConnectionString("GreenPower"));
                            JObject jo2 = (JObject)ja[i];
                            String sTotal = jo2.GetValue("elc_fa").ToString();
                            String sDt = jo2.GetValue("dt").ToString();
                            String sDID = jo2.GetValue("did").ToString();
                            //寫入DB
                            StringBuilder sbSql = new StringBuilder();
                            sbSql.Append("INSERT INTO KWH SELECT @did,'R',@dt,@total,getDate(),'N','A1'");
                            opc.Clear();
                            opc.Add(DataPara.CreateDataParameter("@did", DbType.String, sDID, ParameterDirection.Input));
                            opc.Add(DataPara.CreateDataParameter("@dt", DbType.String, sDt, ParameterDirection.Input));
                            opc.Add(DataPara.CreateDataParameter("@total", DbType.String, sTotal, ParameterDirection.Input));
                            DataTable dt = sdb.TransactionExecute(sbSql.ToString(), opc);
                            //sdb.Commit();
                        }
                        catch (Exception ex)
                        {
                            
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    
                }

                String sURL = "http://www/restful/Meter/GetMeterElectricDayData";
                JObject jo = new JObject();
                jo.Add("did", System.Configuration.ConfigurationSettings.AppSettings["METERDID"].ToString());
                jo.Add("sdt", DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));
                jo.Add("edt", DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));
                GetData(sURL, wc_UploadStringCompleted_getKWHDay, jo);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString());
            }
        }

        /// <summary>
        /// 點日電量獲取完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void wc_UploadStringCompleted_getKWHDay(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                Console.WriteLine(e.Result);
                System.Diagnostics.Debug.WriteLine(e.Result);
                
                try
                {
                    JObject jo = JObject.Parse(e.Result);
                    JArray ja = JArray.Parse(jo.GetValue("body").ToString());
                    for (int i = 0; i < ja.Count; i++)
                    {
                        try
                        {
                            sdb = new SqlDB(DataPara.GetDbConnectionString("GreenPower"));
                            JObject jo2 = (JObject)ja[i];
                            String sTotal = jo2.GetValue("to").ToString();
                            String sDID = jo2.GetValue("did").ToString();

                            //寫入DB
                            StringBuilder sbSql = new StringBuilder();
                            sbSql.Append("INSERT INTO KWH SELECT @did,'D',@dt,@total,getDate(),'N','A1'");

                            opc.Clear();
                            opc.Add(DataPara.CreateDataParameter("@did", DbType.String, sDID, ParameterDirection.Input));
                            opc.Add(DataPara.CreateDataParameter("@dt", DbType.String, DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"), ParameterDirection.Input));
                            opc.Add(DataPara.CreateDataParameter("@total", DbType.String, sTotal, ParameterDirection.Input));
                            DataTable dt = sdb.TransactionExecute(sbSql.ToString(), opc);
                            sdb.Commit();
                            System.Diagnostics.Debug.WriteLine("====");
                            System.Diagnostics.Debug.WriteLine(sTotal);
                            System.Diagnostics.Debug.WriteLine("====");
                        }
                        catch (Exception ex)
                        {
                            
                        }
                    }
                    
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    Console.WriteLine("END");
                    Console.ReadLine();
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString());
            }
        }

        public static void exitProgram(object source, System.Timers.ElapsedEventArgs e)
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// 取得點位實時資訊
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void wc_UploadStringCompleted_getMeter(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                Console.WriteLine(e.Result);
                System.Diagnostics.Debug.WriteLine(e.Result);
                String sURL = "http://www/restful/Meter/GetMeterRealData";
                JObject jo = new JObject();
                jo.Add("did", System.Configuration.ConfigurationSettings.AppSettings["METERDID"].ToString());
                jo.Add("di", "1,2,3,4");
                GetData(sURL, wc_UploadStringCompleted_getMeterPW, jo);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString());
            }
        }

        static void getCompanyID()
        {
            if (String.IsNullOrEmpty(sToken))
            {
                return;
            }
            JObject jo = new JObject();
            jo.Add("name", System.Configuration.ConfigurationSettings.AppSettings["CHINADNY_ID"].ToString());
            jo.Add("pwd", System.Configuration.ConfigurationSettings.AppSettings["CHINADNY_PW"].ToString());
            GetData("http://www/restful/Account/Login", wc_UploadStringCompleted_getCompanyID, jo);

            Console.ReadLine();
        }

        static void getMeterArchiveByCompany()
        {
            if (String.IsNullOrEmpty(sToken))
            {
                return;
            }
            JObject jo = new JObject();
            jo.Add("uid", System.Configuration.ConfigurationSettings.AppSettings["COMPANYID"].ToString());//公司ID
            GetData("http://www/restful/Archive/GetMeterArchiveByCompany", wc_UploadStringCompleted_getMeter, jo);

            Console.ReadLine();
        }
        

        static void GetData(String sURL,UploadStringCompletedEventHandler usceh, JObject jo)
        {
            WebClient wc = new WebClient();
            wc.Encoding = System.Text.Encoding.UTF8;
            var URI = new Uri(sURL);
            //WebProxy wp = System.Net.WebProxy.GetDefaultProxy();
            string sPROXY = System.Configuration.ConfigurationSettings.AppSettings["PROXY"].ToString();
            int sPORT = int.Parse(System.Configuration.ConfigurationSettings.AppSettings["PORT"].ToString());
            string sUID = System.Configuration.ConfigurationSettings.AppSettings["UID"].ToString();
            string sUPW = System.Configuration.ConfigurationSettings.AppSettings["UPW"].ToString();
            WebProxy wp = new WebProxy(sPROXY, sPORT);
            wp.Credentials = new NetworkCredential(sUID, sUPW);
            wc.Proxy = wp;
            String sTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            wc.Headers["Content-Type"] = "application/json";
            wc.Headers["time"] = sTime;
            wc.Headers["token"] = sToken;
            wc.Headers["key"] = GetMd5Hash("web_pyd" + sTime + sToken);
            //wc.Headers["KEY"] = "Your_Key_Goes_Here";

            wc.UploadStringCompleted += new UploadStringCompletedEventHandler(usceh);
            wc.UploadStringAsync(URI, "POST", jo.ToString());
        }

        static string GetMd5Hash(string input)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        static void SendMail(String sMSG,String MailTo, String MailCC)
        {
            ArrayList To = new ArrayList();
            ArrayList CC = new ArrayList();

            string subject = "";
            StringBuilder Mbody = new StringBuilder();
            Borg_Mail oBorg_Mail = new Borg_Mail();
            Mbody.AppendFormat("Dear All:<BR/>");
            Mbody.AppendFormat(sMSG);
            string[] ToList = MailTo.Split(';');
            string[] CCList = MailCC.Split(';');
            foreach (string list in ToList)
            {
                To.Add(list);
            }
            foreach (string listCC in CCList)
            {
                CC.Add(listCC);
            }
            try
            {
                oBorg_Mail.SendMail_Normal(To, CC, subject, Mbody.ToString(), true);
                WriteLog(true, "Message sent successfully");
            }
            catch (Exception ex)
            {
                WriteLog(false, ex.Message.ToString());
            }
        }

        /// <summary>
        /// 寫本機LOG
        /// </summary>
        /// <param name="msg">LOG信息</param>
        /// <param name="errflag">TRUE:異常：FALSE:正常</param>
        static private void WriteLog(bool errFlag, string msg)
        {

            if (errFlag)
            {
                msg = "[ERROR] " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + msg;
            }
            else
            {
                msg = "[     ] " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + msg;
            }

            string errLogPath = AppDomain.CurrentDomain.BaseDirectory + "\\Log\\";

            string logFile = errLogPath + DateTime.Today.ToString("yyyyMMdd") + ".txt";
            //路徑不存在則建立
            if (!Directory.Exists(errLogPath))
            {
                Directory.CreateDirectory(errLogPath);
            }

            //檢查文件存在
            if (!File.Exists(logFile))
            {
                //文件不存在則建立
                StreamWriter sw = File.CreateText(logFile);
                try
                {
                    sw.WriteLine(msg);
                }
                catch (Exception)
                {
                }
                finally
                {
                    sw.Flush();
                    sw.Close();
                }
            }
            else
            {
                //文件存在則複寫
                StreamWriter sw = File.AppendText(logFile);
                try
                {
                    sw.WriteLine(msg);
                }
                catch (Exception)
                {
                }
                finally
                {
                    sw.Flush();
                    sw.Close();
                }
            }

        }

    }
}

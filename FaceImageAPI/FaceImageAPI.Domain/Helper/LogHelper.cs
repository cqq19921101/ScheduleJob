using System;
using System.IO;

namespace FaceImageAPI.Domain.Helper
{
    public class LogHelper
    {
        /// <summary>
        /// 写本机Log
        /// </summary>
        /// <param name="errFlag"></param>
        /// <param name="msg"></param>
        public static void WriteErrorLog(string msg)
        {
            string errLogPath;
            string guid = Guid.NewGuid().ToString();
            msg = "[ERROR] " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + msg;
            errLogPath = AppDomain.CurrentDomain.BaseDirectory + "\\ErrLog\\";
            string logFile = errLogPath + DateTime.Today.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HH") + ".txt";


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

using FaceImageAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FaceImageAPI.Repository.Repository
{
    /// <summary>
    /// 员工管理仓储接口实现
    /// </summary>
    public class StaffManagementRepository : IStaffManagementRepository
    {

        /// <summary>
        /// Post方式创建用户并上传人脸库图片
        /// </summary>
        /// <param name="url">Server Address</param>
        /// <param name="Token">Token</param>
        /// <param name="timeOut">设定超时时间</param>
        /// <param name="FileName">图片名称</param>
        /// <param name="FilePath">图片路径</param>
        /// <param name="strdic">Body参数</param>
        /// <returns></returns>
        public string PostCreateUpLoadUser(string url, string Token, int timeOut, string FileName, string FilePath, Dictionary<string, string> strdic)
        {
            string ResponseResult;
            MemoryStream ms = new MemoryStream();
            string boundary = "---------------" + DateTime.Now.Ticks.ToString("x");

            //开始标识
            var beginBoundary = Encoding.ASCII.GetBytes("--" + boundary + "\r\n");
            //结束标识
            var endBoundary = Encoding.ASCII.GetBytes("--" + boundary + "--\r\n");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            var Headers = request.Headers;
            Headers["Authorization"] = Token;//Token认证
            request.Method = "POST";
            request.Timeout = timeOut;
            request.ContentType = "multipart/form-data; boundary=" + boundary;

            // 写入图片
            var fileStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
            const string filePartHeader = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" + "Content-Type: application/octet-stream\r\n\r\n";
            var header = string.Format(filePartHeader, FileName, FilePath);
            var headerbytes = Encoding.UTF8.GetBytes(header);

            ms.Write(beginBoundary, 0, beginBoundary.Length);
            ms.Write(headerbytes, 0, headerbytes.Length);

            var buffer = new byte[1024];
            int bytesRead; // =0
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                ms.Write(buffer, 0, bytesRead);
            }
            //var str = ms.ToArray();
            //string strstr = Encoding.UTF8.GetString(str);

            // 字符串拼接
            var stringKeyHeader = "\r\n--" + boundary +
                                   "\r\nContent-Disposition: form-data; name=\"{0}\"" +
                                   "\r\n\r\n{1}\r\n";

            foreach (byte[] formitembytes in from string key in strdic.Keys
                                             select string.Format(stringKeyHeader, key, strdic[key])
                                                 into formitem
                                             select Encoding.UTF8.GetBytes(formitem))
            {
                ms.Write(formitembytes, 0, formitembytes.Length);
            }

            // 结束
            ms.Write(endBoundary, 0, endBoundary.Length);
            request.ContentLength = ms.Length;
            var requestStream = request.GetRequestStream();
            ms.Position = 0;
            var tempBuffer = new byte[ms.Length];
            ms.Read(tempBuffer, 0, tempBuffer.Length);
            requestStream.Write(tempBuffer, 0, tempBuffer.Length);

            var httpWebResponse = (HttpWebResponse)request.GetResponse();
            using (StreamReader StreamReader = new StreamReader(httpWebResponse.GetResponseStream(),
                                                            Encoding.UTF8))
            {
                ResponseResult = StreamReader.ReadToEnd();
            }
            requestStream.Close();
            ms.Close();
            fileStream.Close();
            httpWebResponse.Close();
            request.Abort();
            return ResponseResult;
        }
    }
}

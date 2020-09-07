using FaceImageAPI.Entity;
using FaceImageAPI.Repository.IRepository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FaceImageAPI.Services
{
    /// <summary>
    /// 员工管理服务
    /// </summary>
    public class StaffManagementService : IStaffManagementService
    {
        private readonly IStaffManagementRepository _StaffManagementRepository;

        /// <summary>
        /// 构造函数注入
        /// </summary>
        public StaffManagementService(IStaffManagementRepository StaffManagementRepository)
        {
            _StaffManagementRepository = StaffManagementRepository;
        }

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
        public string PostCreateUpLoadUser(string url, string Token, int timeOut, string FileName, string FilePath, Dictionary<string, object> strdic)
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

        /// <summary>
        /// 根据工号获取用户的Subject_id
        /// </summary>
        /// <param name="url"></param>
        /// <param name="Token"></param>
        /// <param name="EmpNo">工号</param>
        /// <returns>subject_id</returns>
        public string GetUserDataByEmpNo(string url, string Token, string EmpNo)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 根据Subjectid删除单个用户数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="Token"></param>
        /// <param name="subjectid"></param>
        /// <returns>ResponseResult</returns>
        public string DeleteBySubjectId(string url, string Token, string subjectid)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 根据工号获取对应的subjectid集合
        /// </summary>
        /// <param name="url"></param>
        /// <param name="Token"></param>
        /// <returns></returns>
        public ArrayList GetSubListByEmpNo(string url, string Token)
        {
            ArrayList sublist = new ArrayList();

            string subjectid = _StaffManagementRepository.GetSubjectID(url,Token,"12200473");


            //List<v_smartpark_emp> LEmplist = _StaffManagementRepository.GetLeaveEmp();
            //if (LEmplist != null && LEmplist.Count > 0)
            //{
            //    foreach (v_smartpark_emp item in LEmplist)
            //    {

            //    }
            //}
            //else
            //{
            //    return sublist;
            //}


            return sublist;
        }


        #region Excute Function

        /// <summary>
        /// 执行创建用户并上传图片至底库的方法 Before PRD
        /// </summary>
        /// <param name="CreateUserUrl"></param>
        /// <param name="Token"></param>
        /// <returns>ResposeResult</returns>
        public string ExcutePostUpload(string CreateUserUrl, string Token)
        {
            string ResponseResult = string.Empty;
            List<v_smartpark_emp> lst = _StaffManagementRepository.GetUserDataBeforePRD();
            foreach (v_smartpark_emp item in lst)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("subject_type", "0");
                dic.Add("group_ids", "0");
                dic.Add("extra_id", item.EmpNumber);
                dic.Add("name", item.EmpName);

                Stream stream = new MemoryStream(item.FileData);
                Bitmap img = new Bitmap(stream);
                img.Save(AppDomain.CurrentDomain.BaseDirectory + $@"\Photo\{item.EmpName}.jpg");
                string filepath = AppDomain.CurrentDomain.BaseDirectory + $@"\Photo\{item.EmpName}.jpg";
                ResponseResult = PostCreateUpLoadUser(CreateUserUrl, Token, 30000, "photo", filepath, dic);
            }
            return "OK";
        }

        /// <summary>
        /// 执行新增每天新入职员工的方法
        /// </summary>
        /// <param name="CreateEntryEmpUrl"></param>
        /// <param name="Token"></param>
        /// <returns></returns>
        public string ExcutePostAddEntryEmp(string CreateEntryEmpUrl, string Token)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 执行删除每天离职员工的方法
        /// </summary>
        /// <param name="DelLeaveEmpUrl"></param>
        /// <param name="Token"></param>
        /// <param name="subject_id"></param>
        /// <returns></returns>
        public string ExcutePostDelLeaveEmp(string DelLeaveEmpUrl, string Token, ArrayList sublist)
        {
            throw new NotImplementedException();
        }

        #endregion

        /// <summary>
        /// 16进制转byte[]类型
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public byte[] ConvertHexStringToBytes(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException("参数长度不正确");
            }

            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return returnBytes;
        }

    }
}

using FaceImageAPI.Entity;
using FaceImageAPI.Repository.IRepository;
using FaceImageAPI.Services.IService;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FaceImageAPI.Services.Service
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

        #region 
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
        /// 根据离职工号获取对应的subjectid集合
        /// </summary>
        /// <param name="url"></param>
        /// <param name="Token"></param>
        /// <returns></returns>
        public ArrayList GetSubListByLeavingEmpNo(string url, string Token)
        {
            ArrayList sublist = new ArrayList();
            List<v_smartpark_emp> LEmplist = _StaffManagementRepository.GetLeaveEmp();
            if (LEmplist != null && LEmplist.Count > 0)
            {
                foreach (v_smartpark_emp item in LEmplist)
                {
                    string subjectid = _StaffManagementRepository.GetSubjectID(url, Token, item.EmpNumber);
                    sublist.Add(subjectid);
                }
            }
            else
            {
                return null;
            }
            return sublist;
        }

        /// <summary>
        /// 根据当天更新过资料的工号获取对应的subjectid集合
        /// </summary>
        /// <param name="url"></param>
        /// <param name="Token"></param>
        /// <returns></returns>
        public ArrayList GetSubListByUpdatingEmpNo(string url, string Token)
        {
            ArrayList sublist = new ArrayList();
            List<v_smartpark_emp> UEmplist = _StaffManagementRepository.GetUpdateEmp();
            if (UEmplist != null && UEmplist.Count > 0)
            {
                foreach (v_smartpark_emp item in UEmplist)
                {
                    string subjectid = _StaffManagementRepository.GetSubjectID(url, Token, item.EmpNumber);
                    sublist.Add(subjectid);
                }
            }
            else
            {
                return null;
            }
            return sublist;

        }
        #endregion

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
            if (lst != null && lst.Count > 0)
            {
                foreach (v_smartpark_emp item in lst)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("subject_type", "0");
                    dic.Add("group_ids", "0");
                    dic.Add("extra_id", item.EmpNumber);
                    dic.Add("name", item.EmpName);

                    Stream stream = new MemoryStream(item.FileData);
                    Bitmap img = new Bitmap(stream);
                    string FilePath = AppDomain.CurrentDomain.BaseDirectory + $@"\Photo\{item.EmpName}.jpg";
                    img.Save(FilePath);
                    string filepath = AppDomain.CurrentDomain.BaseDirectory + $@"\Photo\{item.EmpName}.jpg";
                    ResponseResult = PostCreateUpLoadUser(CreateUserUrl, Token, 30000, "photo", filepath, dic);
                }
            }
            return ResponseResult;
        }

        /// <summary>
        /// 执行新增每天新入职员工的方法
        /// </summary>
        /// <param name="CreateEntryEmpUrl"></param>
        /// <param name="Token"></param>
        /// <returns></returns>
        public string ExcutePostAddEntryEmp(string CreateEntryEmpUrl, string Token)
        {
            string ResponseResult = string.Empty;
            List<v_smartpark_emp> lst = _StaffManagementRepository.GetEntryEmp();
            if (lst != null && lst.Count > 0)
            {
                foreach (v_smartpark_emp item in lst)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("subject_type", "0");
                    dic.Add("group_ids", "0");
                    dic.Add("extra_id", item.EmpNumber);
                    dic.Add("name", item.EmpName);

                    Stream stream = new MemoryStream(item.FileData);
                    Bitmap img = new Bitmap(stream);
                    string FilePath = AppDomain.CurrentDomain.BaseDirectory + $@"\Photo\{item.EmpName}.jpg";
                    img.Save(FilePath);
                    string filepath = AppDomain.CurrentDomain.BaseDirectory + $@"\Photo\{item.EmpName}.jpg";
                    ResponseResult = PostCreateUpLoadUser(CreateEntryEmpUrl, Token, 30000, "photo", filepath, dic);
                }
            }
            else
            {

            }
            return ResponseResult;
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
            string result = string.Empty;
            if (sublist != null && sublist.Count > 0)
            {
                foreach (string parameter in sublist)
                {
                    DelLeaveEmpUrl = DelLeaveEmpUrl + int.Parse(parameter);
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(DelLeaveEmpUrl));
                    request.Timeout = 20 * 1000;//设置30s的超时
                    request.ContentType = "application/x-www-form-urlencoded";
                    var Headers = request.Headers;
                    Headers["Authorization"] = Token;//Token认证
                    request.Method = "DELETE";

                    HttpWebResponse httpWebResponse = (HttpWebResponse)request.GetResponse();
                    StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
                    result = streamReader.ReadToEnd();
                    httpWebResponse.Close();
                    streamReader.Close();
                }
            }
            return result;
        }

        /// <summary>
        /// 执行更新每天资料变动的人员
        /// </summary>
        /// <param name="UpdateEmpUrl"></param>
        /// <param name="Token"></param>
        /// <param name="sublist"></param>
        /// <returns></returns>
        public string ExcutePostUpdateEmp(string UpdateEmpUrl, string Token, ArrayList sublist)
        {
            string result = string.Empty;
            if (sublist != null && sublist.Count > 0)
            {
                foreach (string parameter in sublist)
                {
                    UpdateEmpUrl = UpdateEmpUrl + parameter;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(UpdateEmpUrl));
                    request.Timeout = 20 * 1000;//设置30s的超时
                    request.ContentType = "application/json";
                    var Headers = request.Headers;
                    Headers["Authorization"] = Token;//Token认证
                    request.Method = "PUT";
                    //var temp = new
                    //{
                    //    username = LoginID,
                    //    password = Password,
                    //    auth_token = true
                    //};

                    //var postData = JsonHelper.ObjectToString(temp);
                    //byte[] data = Encoding.UTF8.GetBytes(postData);
                    //request.ContentLength = data.Length;
                    //Stream postStream = request.GetRequestStream();
                    //postStream.Write(data, 0, data.Length);
                    //using (var res = request.GetResponse() as HttpWebResponse)
                    //{
                    //    if (res.StatusCode == HttpStatusCode.OK)
                    //    {
                    //        StreamReader reader = new StreamReader(res.GetResponseStream(), Encoding.UTF8);
                    //        result = reader.ReadToEnd();
                    //        reader.Close();
                    //    }
                    //}
                    //postStream.Close();
                    //request.Abort();

                    HttpWebResponse httpWebResponse = (HttpWebResponse)request.GetResponse();
                    StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
                    result = streamReader.ReadToEnd();
                    httpWebResponse.Close();
                    streamReader.Close();
                }
            }
            return result;
        }

        #endregion

        #region Common
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
        #endregion

    }
}

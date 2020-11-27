using FaceImageAPI.Domain.Helper;
using FaceImageAPI.Entity;
using FaceImageAPI.Repository.IRepository;
using FaceImageAPI.Services.IService;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

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
                    if (subjectid != null && subjectid.Length > 0)
                    {
                        sublist.Add(subjectid);
                    }
                }
            }
            else
            {
                return null;
            }
            return sublist;
        }

        /// <summary>
        /// 根据当天更新过资料的工号获取对应的subjectid集合和员工实体集合
        /// </summary>
        /// <param name="url"></param>
        /// <param name="Token"></param>
        /// <returns></returns>
        public void GetSubjectidandEmpNumber(string url, string Token, out ArrayList Usublist, out List<v_smartpark_emp> UEmplist)
        {
            string subjectid;
            string CreateUserUrl = "http://10.170.3.75/subject/file";
            //Usublist = null;
            Usublist = new ArrayList();
            UEmplist = _StaffManagementRepository.GetUpdateEmp();
            if (UEmplist != null && UEmplist.Count > 0)
            {
                foreach (v_smartpark_emp item in UEmplist)
                {
                    subjectid = _StaffManagementRepository.GetSubjectID(url, Token, item.EmpNumber);
                    if (subjectid != null && subjectid.Length > 0)
                    {
                        Usublist.Add(subjectid);
                    }
                    else if(item.LDate == null )
                    {
                        CreateUploadUser(CreateUserUrl,item,Token);
                    }
                }
            }
            else
            {

            }
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
                CreateUploadUser(CreateUserUrl, lst, Token);
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
                CreateUploadUser(CreateEntryEmpUrl, lst, Token);
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
                    result = PostDeleteFunction(DelLeaveEmpUrl, Token, parameter);
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
        public string ExcutePostUpdateEmp(string UpdateEmpUrl, string CreateEntryEmpUrl, string Token, ArrayList sublist, List<v_smartpark_emp> EmpList)
        {
            //因缺少更新图片的接口  先删除 后新增
            string result;
            if (sublist != null && sublist.Count > 0)
            {
                //先删除Subjectid人员
                foreach (string parameter in sublist)
                {
                    result = PostDeleteFunction(UpdateEmpUrl, Token, parameter);
                }
            }
            //新增相关人员
            if (EmpList != null && EmpList.Count > 0)
            {
                CreateUploadUser(CreateEntryEmpUrl, EmpList, Token);
            }
            return "OK";
        }

        #endregion

        #region Common
        /// <summary>
        /// 创建用户并上传底库图片 实体集合
        /// </summary>
        /// <param name="EmpList"></param>
        /// <param name="url"></param>
        /// <param name="Token"></param>
        /// <param name="timeOut"></param>
        /// <param name="FileName"></param>
        /// <param name="FilePath"></param>
        /// <param name="strdic"></param>
        /// <returns></returns>
        public string CreateUploadUser(string CreateEntryEmpUrl, List<v_smartpark_emp> EmpList, string Token)
        {
            string ResponseResult;
            foreach (v_smartpark_emp item in EmpList)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("subject_type", "0");
                dic.Add("group_ids", "0");
                dic.Add("extra_id", item.EmpNumber);
                dic.Add("name", item.EmpName);

                Stream stream = new MemoryStream(item.FileData);
                Bitmap img = new Bitmap(stream);
                string filepath = AppDomain.CurrentDomain.BaseDirectory + $@"\Photo\{item.EmpName}.jpg";
                img.Save(filepath);
                ResponseResult = PostCreateUpLoadUser(CreateEntryEmpUrl, Token, 30000, "photo", filepath, dic);

                ExceptionEntity.Root da = JsonConvert.DeserializeObject<ExceptionEntity.Root>(ResponseResult);
                if (da.desc != null && da.desc.Length > 0)
                {
                    if (da.desc != "唯一标识重复")
                    {
                        string ErrprPhoto = AppDomain.CurrentDomain.BaseDirectory + $@"\ErrorPhoto\{item.EmpName}.jpg";
                        img.Save(ErrprPhoto);
                        //Write Exception log
                        LogHelper.WriteErrorLog($"工号 : {item.EmpNumber} 姓名 ：{item.EmpName} 异常信息 ： {da.desc}");
                    }
                }

            }
            return "OK";
        }

        /// <summary>
        /// 创建用户并上传底库图片 单个实体
        /// </summary>
        /// <param name="EmpList"></param>
        /// <param name="url"></param>
        /// <param name="Token"></param>
        /// <param name="timeOut"></param>
        /// <param name="FileName"></param>
        /// <param name="FilePath"></param>
        /// <param name="strdic"></param>
        /// <returns></returns>
        public string CreateUploadUser(string CreateEntryEmpUrl, v_smartpark_emp Emp, string Token)
        {
            string ResponseResult;
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("subject_type", "0");
            dic.Add("group_ids", "0");
            dic.Add("extra_id", Emp.EmpNumber);
            dic.Add("name", Emp.EmpName);

            Stream stream = new MemoryStream(Emp.FileData);
            Bitmap img = new Bitmap(stream);
            string filepath = AppDomain.CurrentDomain.BaseDirectory + $@"\Photo\{Emp.EmpName}.jpg";
            img.Save(filepath);
            ResponseResult = PostCreateUpLoadUser(CreateEntryEmpUrl, Token, 30000, "photo", filepath, dic);

            ExceptionEntity.Root da = JsonConvert.DeserializeObject<ExceptionEntity.Root>(ResponseResult);
            if (da.desc != null && da.desc.Length > 0 )
            {
                //Write Exception Log
                LogHelper.WriteErrorLog($"工号 : {Emp.EmpNumber} 姓名 ：{Emp.EmpName} 异常信息 ： {da.desc}");
            }
            return "OK";
        }


        /// <summary>
        /// Post方式创建用户并上传人脸库图片 接口创建方法
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
        /// 接口删除方法
        /// </summary>
        /// <param name="url"></param>
        /// <param name="Token"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public string PostDeleteFunction(string url, string Token, string parameter)
        {
            string result;
            url = url + int.Parse(parameter);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));
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
            return result;
        }

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

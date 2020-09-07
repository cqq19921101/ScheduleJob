using FaceImageAPI.Domain.DB;
using FaceImageAPI.Entity;
using FaceImageAPI.Helper;
using FaceImageAPI.Repository.IRepository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace FaceImageAPI.Repository.Repository
{
    /// <summary>
    /// 员工管理仓储接口实现
    /// </summary>
    public class StaffManagementRepository : IStaffManagementRepository
    {
        /// <summary>
        /// 上正式环境前 同步人脸库中的所有符合条件的数据
        /// </summary>
        /// <returns></returns>
        public List<v_smartpark_emp> GetUserDataBeforePRD()
        {

            StringBuilder sb = new StringBuilder();
            sb.Append(@"select  EmpNumber,EmpName,JDate,FileData from v_smartpark_emp
                            where EmpName = '陈乾乾'
                            and LDate IS NULL and FileData IS Not Null");
            using (var db = new DBContext())
            {
                return db.Database.SqlQuery<v_smartpark_emp>(sb.ToString()).ToList();//
            }
        }

        /// <summary>
        /// 抓取当天的入职员工
        /// </summary>
        /// <returns></returns>
        public List<v_smartpark_emp> GetEntryEmp()
        {
            string NDate = DateTime.Now.ToString("yyyy-MM-dd");
            StringBuilder sb = new StringBuilder();
            sb.Append($@"select  EmpNumber,EmpName,JDate,FileData from v_smartpark_emp
                            where CONVERT(varchar(10),JDate,120) = {NDate}
                            and FileData is not null    ");
            using (var db = new DBContext())
            {
                return  db.Database.SqlQuery<v_smartpark_emp>(sb.ToString()).ToList();//
            }
        }

        /// <summary>
        /// 抓取当天的离职员工
        /// </summary>
        /// <returns></returns>
        public List<v_smartpark_emp> GetLeaveEmp()
        {
            string NDate = DateTime.Now.ToString("yyyy -MM-dd");
            StringBuilder sb = new StringBuilder();
            //sb.Append($@"select  EmpNumber,EmpName,JDate,FileData from v_smartpark_emp
            //             where CONVERT(varchar(10),LDate,120) = {NDate} ");
            sb.Append($@"select  EmpNumber,EmpName,JDate,FileData from v_smartpark_emp
                         where EmpName = '陈乾乾' ");
            using (var db = new DBContext())
            {
                return db.Database.SqlQuery<v_smartpark_emp>(sb.ToString()).ToList();//
            }
        }


        /// <summary>
        /// Http请求 获取传入工号对应的subjectID
        /// </summary>
        /// <param name="url"></param>
        /// <param name="Token"></param>
        /// <param name="EmpNo"></param>
        /// <returns></returns>
        public string GetSubjectID(string url,string Token,string EmpNo)
        {
            url = url + $"category=employee&name=&department=&interviewee=&start_time=&end_time=&filterstr=&remark=&extra_id={EmpNo}"; 
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            request.Timeout = 20 * 1000;//设置30s的超时
            request.ContentType = "application/json";
            var Headers = request.Headers;
            Headers["Authorization"] = Token;//Token认证
            request.Method = "Get";

            HttpWebResponse httpWebResponse = (HttpWebResponse)request.GetResponse();
            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
            string result = streamReader.ReadToEnd();
            httpWebResponse.Close();
            streamReader.Close();

            Root da = JsonConvert.DeserializeObject<Root>(result);
            return da.data.Select(d => d.photos);
        }

    }
}

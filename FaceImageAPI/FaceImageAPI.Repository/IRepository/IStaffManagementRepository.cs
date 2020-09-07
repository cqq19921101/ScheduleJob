using FaceImageAPI.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceImageAPI.Repository.IRepository
{
    /// <summary>
    /// 员工管理仓储接口
    /// </summary>
    public interface IStaffManagementRepository
    {
        ///// <summary>
        ///// Post方式创建用户并上传人脸库图片
        ///// </summary>
        ///// <param name="url">Server Address</param>
        ///// <param name="Token">Token</param>
        ///// <param name="timeOut">设定超时时间</param>
        ///// <param name="FileName">图片名称</param>
        ///// <param name="FilePath">图片路径</param>
        ///// <param name="strdic">Body参数</param>
        ///// <returns></returns>
        //string PostCreateUpLoadUser(string url, string Token, int timeOut, string FileName, string FilePath, Dictionary<string, object> strdic);
      
        /// <summary>
        /// 上正式环境前 同步人脸库所有数据
        /// </summary>
        /// <returns></returns>
        List<v_smartpark_emp> GetUserDataBeforePRD();

        /// <summary>
        /// 抓取当天的入职员工
        /// </summary>
        /// <returns></returns>
        List<v_smartpark_emp> GetEntryEmp();

        /// <summary>
        /// 抓取当天的离职员工
        /// </summary>
        /// <returns></returns>
        List<v_smartpark_emp> GetLeaveEmp();

        /// <summary>
        /// Http请求 获取传入工号对应的subjectID
        /// </summary>
        /// <param name="url"></param>
        /// <param name="Token"></param>
        /// <param name="EmpNo"></param>
        /// <returns></returns>
        string GetSubjectID(string url,string Token, string EmpNo);
    }
}

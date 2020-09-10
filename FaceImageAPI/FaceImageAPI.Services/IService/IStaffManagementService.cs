using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceImageAPI.Services.IService
{
    /// <summary>
    /// 员工管理接口
    /// </summary>
    public interface IStaffManagementService
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
        string PostCreateUpLoadUser(string url, string Token, int timeOut, string FileName, string FilePath, Dictionary<string, object> strdic);

        ///// <summary>
        ///// 根据Subjectid删除单个用户数据
        ///// </summary>
        ///// <param name="url"></param>
        ///// <param name="Token"></param>
        ///// <param name="subjectid"></param>
        ///// <returns></returns>
        //string DeleteBySubjectId(string url, string Token, string subjectid);

        /// <summary>
        /// 根据离职工号获取对应的subjectid集合
        /// </summary>
        /// <param name="url"></param>
        /// <param name="Token"></param>
        /// <returns></returns>
        ArrayList GetSubListByLeavingEmpNo(string url, string Token);

        /// <summary>
        /// 根据当天更新过资料的工号获取对应的subjectid集合
        /// </summary>
        /// <param name="url"></param>
        /// <param name="Token"></param>
        /// <returns></returns>
        ArrayList GetSubListByUpdatingEmpNo(string url, string Token);

        /// <summary>
        /// 执行创建用户并上传图片至底库的方法
        /// </summary>
        /// <param name="CreateUserUrl"></param>
        /// <param name="Token"></param>
        /// <returns>ResposeResult</returns>
        string ExcutePostUpload(string CreateUserUrl, string Token);

        /// <summary>
        /// 执行新增每天新入职员工的方法
        /// </summary>
        /// <param name="CreateEntryEmpUrl"></param>
        /// <param name="Token"></param>
        /// <returns></returns>
        string ExcutePostAddEntryEmp(string CreateUserUrl, string Token);

        /// <summary>
        /// 执行删除每天离职员工的方法
        /// </summary>
        /// <param name="DelLeaveEmpUrl"></param>
        /// <param name="Token"></param>
        /// <param name="subject_id"></param>
        /// <returns></returns>
        string ExcutePostDelLeaveEmp(string DelLeaveEmpUrl, string Token, ArrayList sublist);

        /// <summary>
        /// 执行更新每天资料变动的人员
        /// </summary>
        /// <param name="UpdateEmpUrl"></param>
        /// <param name="Token"></param>
        /// <param name="subject_id"></param>
        /// <returns></returns>
        string ExcutePostUpdateEmp(string UpdateEmpUrl, string Token, ArrayList sublist);

        /// <summary>
        /// 16进制转换Byte[]类型
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        byte[] ConvertHexStringToBytes(string hexString);
    }
}

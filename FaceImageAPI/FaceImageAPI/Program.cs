using Autofac;
using FaceImageAPI.Entity;
using FaceImageAPI.Ioc;
using FaceImageAPI.Services.IService;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceImageAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            ArrayList Lsublist = new ArrayList();
            string Token = string.Empty;

            #region Declare Parameter
            string TokenUrl = ConfigurationManager.AppSettings["TokenUrl"].ToString();
            string CreateUserUrl = ConfigurationManager.AppSettings["CreateUserUrl"].ToString();
            string DelLeaveEmpUrl = ConfigurationManager.AppSettings["DelLeaveEmpUrl"].ToString();
            string GetSubjectIDUrl = ConfigurationManager.AppSettings["GetSubjectIDUrl"].ToString();
            string UpdateEmpUrl = ConfigurationManager.AppSettings["UpdateEmpUrl"].ToString();
            string LoginId = ConfigurationManager.AppSettings["LoginId"].ToString();
            string LoginPsd = ConfigurationManager.AppSettings["LoginPsd"].ToString();
            #endregion

            #region Ioc Init
            //初始化Ioc容器
            Container.Init();
            //Ioc 注入
            IAuthorityService AuthorityService = Container.Instance.Resolve<IAuthorityService>();
            IStaffManagementService StaffManagementService = Container.Instance.Resolve<IStaffManagementService>();
            #endregion

            #region Get Token
            //获取Token
            Token = AuthorityService.GetToken(TokenUrl, LoginId, LoginPsd);
            #endregion

            #region Insert
            //Add 执行创建用户并上传图片至底库的方法 Before PRD
            //StaffManagementService.ExcutePostUpload(CreateUserUrl, Token);

            //Add 执行新增每天新入职员工的方法
            StaffManagementService.ExcutePostAddEntryEmp(CreateUserUrl, Token);
            #endregion

            #region Update
            //Update 执行更新每天资料变动的人员
            StaffManagementService.GetSubjectidandEmpNumber(GetSubjectIDUrl, Token, out ArrayList Usublist, out List<v_smartpark_emp> EmpList);//更新过资料的人员集合
            if (Usublist != null && Usublist.Count > 0)
            {
                StaffManagementService.ExcutePostUpdateEmp(UpdateEmpUrl, CreateUserUrl, Token, Usublist, EmpList);
            }
            #endregion

            #region Delete 
            //Delete 执行删除每天离职员工的方法
            Lsublist = StaffManagementService.GetSubListByLeavingEmpNo(GetSubjectIDUrl, Token);//离职人员集合
            //if (Lsublist.Count > 0 && Lsublist != null)
            if (Lsublist != null)
            {
                StaffManagementService.ExcutePostDelLeaveEmp(DelLeaveEmpUrl, Token, Lsublist);
            }
            #endregion

        }
    }
}

using Autofac;
using FaceImageAPI.Entity;
using FaceImageAPI.Ioc;
using FaceImageAPI.Services.IService;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;


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
            Console.WriteLine("Get Token Start!");
            //获取Token
            Token = AuthorityService.GetToken(TokenUrl, LoginId, LoginPsd);
            Console.WriteLine("Get Token End!");
            #endregion

            #region Insert
            Console.WriteLine("Insert Function Start!");

            //StaffManagementService.ExcutePostUpload(CreateUserUrl, Token);

            StaffManagementService.ExcutePostAddEntryEmp(CreateUserUrl, Token);
            Console.WriteLine("Insert Function End!");

            #endregion

            #region Update
            Console.WriteLine("Update Function Start!");
            StaffManagementService.GetSubjectidandEmpNumber(GetSubjectIDUrl, Token, out ArrayList Usublist, out List<v_smartpark_emp> EmpList);//更新过资料的人员集合
            Console.WriteLine($"当天更新人脸数据的人数" + Usublist.Count.ToString());
            if (Usublist != null && Usublist.Count > 0)
            {
                StaffManagementService.ExcutePostUpdateEmp(UpdateEmpUrl, CreateUserUrl, Token, Usublist, EmpList);
            }
            Console.WriteLine("Update Function End!");
            #endregion

            #region Delete 
            Console.WriteLine("Delete Function Start!");
            //Delete 执行删除离职员工的方法
            Lsublist = StaffManagementService.GetSubListByLeavingEmpNo(GetSubjectIDUrl, Token);//离职人员集合
            Console.WriteLine($"近三月离职的人数" + Lsublist.Count.ToString());
            if (Lsublist != null)
            {
                StaffManagementService.ExcutePostDelLeaveEmp(DelLeaveEmpUrl, Token, Lsublist);
            }
            Console.WriteLine("Delete Function End!");
            #endregion

        }
    }
}

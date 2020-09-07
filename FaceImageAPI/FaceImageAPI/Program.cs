using Autofac;
using FaceImageAPI.Entity;
using FaceImageAPI.Ioc;
using FaceImageAPI.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;

namespace FaceImageAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            ArrayList sublist = new ArrayList();
            string ReturnMessage1 = string.Empty;
            string ReturnMessage2 = string.Empty;
            string ReturnMessage3 = string.Empty;
            string Token = string.Empty;
            string TokenUrl = ConfigurationManager.AppSettings["TokenUrl"].ToString();
            string CreateUserUrl = ConfigurationManager.AppSettings["CreateUserUrl"].ToString();
            string DelLeaveEmpUrl = ConfigurationManager.AppSettings["DelLeaveEmpUrl"].ToString();
            string GetSubjectIDUrl = ConfigurationManager.AppSettings["GetSubjectIDUrl"].ToString();
            string LoginId = ConfigurationManager.AppSettings["LoginId"].ToString();
            string LoginPsd = ConfigurationManager.AppSettings["LoginPsd"].ToString();

            Container.Init();//初始化Ioc容器

            //Ioc 注入
            IAuthorityService AuthorityService = Container.Instance.Resolve<IAuthorityService>();
            IStaffManagementService StaffManagementService = Container.Instance.Resolve<IStaffManagementService>();

            //获取Token
            Token = AuthorityService.GetToken(TokenUrl, LoginId, LoginPsd);


            //执行创建用户并上传图片至底库的方法 Before PRD
            //ReturnMessage1 = StaffManagementService.ExcutePostUpload(CreateUserUrl, Token);

            ////执行新增每天新入职员工的方法
            //ReturnMessage2 = StaffManagementService.ExcutePostAddEntryEmp(CreateUserUrl, Token);

            //获取离职人员的subjectid集合
            sublist = StaffManagementService.GetSubListByEmpNo(GetSubjectIDUrl,Token);

            if (sublist.Count > 0 && sublist != null)
            {
                //执行删除每天离职员工的方法
                ReturnMessage3 = StaffManagementService.ExcutePostDelLeaveEmp(CreateUserUrl, Token, sublist);
            }
        }
    }
}

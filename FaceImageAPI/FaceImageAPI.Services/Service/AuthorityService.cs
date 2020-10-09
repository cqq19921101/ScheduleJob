using Autofac.Extras.DynamicProxy;
using FaceImageAPI.Domain.AOP;
using FaceImageAPI.Repository.IRepository;
using FaceImageAPI.Services.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceImageAPI.Services.Service
{
    /// <summary>
    /// 权限管理服务
    /// </summary>
    //[Intercept(typeof(ExceptionLogInterceptor))]
    public class AuthorityService : IAuthorityService
    {
        private readonly IAuthorityRepository _AuthorityRepository;
        /// <summary>
        /// 构造注入
        /// </summary>
        /// <param name="studentRepository"></param>
        public AuthorityService(IAuthorityRepository AuthorityRepository)
        {
            _AuthorityRepository = AuthorityRepository;
        }

        /// <summary>
        /// 根据登录账号密码获取Token
        /// </summary>
        /// <param name="url"></param>
        /// <param name="LoginID"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public string GetToken(string url, string LoginID, string Password)
        {
            return _AuthorityRepository.GetToken(url, LoginID, Password);
        }
    }
}

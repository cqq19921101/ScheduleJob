using FaceImageAPI.Repository.IRepository;

namespace FaceImageAPI.Services
{
    /// <summary>
    /// 权限管理服务
    /// </summary>
    public class AuthorityService :IAuthorityService
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
        public string GetToken(string url,string LoginID, string Password)
        {
            return _AuthorityRepository.GetToken(url,LoginID,Password);
        }
    }
}

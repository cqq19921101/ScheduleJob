namespace FaceImageAPI.Services.IService
{
    /// <summary>
    /// 权限管理接口
    /// </summary>
    public interface IAuthorityService
    {
        /// <summary>
        /// 根据登录账号密码获取Token
        /// </summary>
        /// <param name="url">api address</param>
        /// <param name="LoginID">登录账号</param>
        /// <param name="Password">登录密码</param>
        /// <returns></returns>
        string GetToken(string url, string LoginID, string Password);
    }
}

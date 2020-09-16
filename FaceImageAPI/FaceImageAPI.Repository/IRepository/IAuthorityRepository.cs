namespace FaceImageAPI.Repository.IRepository
{
    /// <summary>
    /// 权限管理仓储接口
    /// </summary>
    public interface IAuthorityRepository
    {
        /// <summary>
        /// 根据登录账号密码获取Token
        /// </summary>
        /// <param name="url"></param>
        /// <param name="LoginID"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        string GetToken(string url, string LoginID, string Password);
    }
}

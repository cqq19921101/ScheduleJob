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
using System.Threading.Tasks;
using static FaceImageAPI.Entity.JsonEntity;

namespace FaceImageAPI.Repository.Repository
{
    /// <summary>
    /// 权限管理仓储接口实现
    /// </summary>
    public class AuthorityRepository : IAuthorityRepository
    {
        /// <summary>
        /// 根据登录账号密码获取Token
        /// </summary>
        /// <param name="url"></param>
        /// <param name="LoginID"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public string GetToken(string url, string LoginID, string Password)
        {
            string result = string.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            request.Timeout = 30 * 1000;//设置30s的超时
            request.ContentType = "application/json";
            request.UserAgent = "Koala Admin";
            request.Method = "POST";

            var temp = new
            {
                username = LoginID,
                password = Password,
                auth_token = true
            };

            var postData = JsonHelper.ObjectToString(temp);
            byte[] data = Encoding.UTF8.GetBytes(postData);
            request.ContentLength = data.Length;
            Stream postStream = request.GetRequestStream();
            postStream.Write(data, 0, data.Length);
            using (var res = request.GetResponse() as HttpWebResponse)
            {
                if (res.StatusCode == HttpStatusCode.OK)
                {
                    StreamReader reader = new StreamReader(res.GetResponseStream(), Encoding.UTF8);
                    result = reader.ReadToEnd();
                    reader.Close();
                }
            }
            postStream.Close();
            request.Abort();

            JsonEntity.Root da = JsonConvert.DeserializeObject<JsonEntity.Root>(result);
            return da.data.auth_token;
        }
    }
}

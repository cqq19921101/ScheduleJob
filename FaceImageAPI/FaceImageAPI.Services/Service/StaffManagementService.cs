using FaceImageAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FaceImageAPI.Services
{
    /// <summary>
    /// 员工管理服务
    /// </summary>
    public class StaffManagementService :IStaffManagementService
    {
        private readonly IStaffManagementRepository _StaffManagementRepository;

        /// <summary>
        /// 构造函数注入
        /// </summary>
        public StaffManagementService(IStaffManagementRepository StaffManagementRepository)
        {
            _StaffManagementRepository = StaffManagementRepository;
        }

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
        public string PostCreateUpLoadUser(string url, string Token, int timeOut, string FileName, string FilePath, Dictionary<string, string> strdic)
        {
            return _StaffManagementRepository.PostCreateUpLoadUser(url,Token,timeOut,FileName,FilePath,strdic);
        }

        /// <summary>
        /// 16进制转byte[]类型
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public byte[] ConvertHexStringToBytes(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException("参数长度不正确");
            }

            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return returnBytes;
        }

    }
}

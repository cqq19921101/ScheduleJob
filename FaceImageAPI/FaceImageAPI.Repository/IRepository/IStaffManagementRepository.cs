﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceImageAPI.Repository.IRepository
{
    /// <summary>
    /// 员工管理仓储接口
    /// </summary>
    public interface IStaffManagementRepository
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
        string PostCreateUpLoadUser(string url, string Token, int timeOut, string FileName, string FilePath, Dictionary<string, string> strdic);
    }
}

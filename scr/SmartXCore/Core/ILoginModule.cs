using System;
using System.Collections.Generic;
using System.Text;

namespace SmartXCore.Core
{
    public interface ILoginModule : IBaseModule
    {
        /// <summary>
        /// 获取UUID
        /// </summary>
        bool GetUuid();
        /// <summary>
        /// 获取二维码
        /// </summary>
        void GetQRCode();
        /// <summary>
        /// 登录
        /// </summary>
        bool Login();

        /// <summary>
        /// 保持在线并检查新消息
        /// </summary>
        void BeginSyncCheck();
    }
}

using SmartXCore.Model;
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
        bool GetQRCode();
        /// <summary>
        /// 等待扫码登录
        /// </summary>
        WatiForLoginResult WatiForLogin();

        /// <summary>
        /// 获取登录参数
        /// </summary>
        bool WebLogin();
        /// <summary>
        /// 微信初始化
        /// </summary>
        bool WebwxInit();
        /// <summary>
        /// 开启状态通知
        /// </summary>
        bool StatusNotify();

        /// <summary>
        /// 获取联系人
        /// </summary>
        bool GetContact();

        bool Login();


        /// <summary>
        /// 保持在线并检查新消息
        /// </summary>
        void BeginSyncCheck();

        /// <summary>
        /// 同步检测
        /// </summary>
        bool SyncCheck();
        /// <summary>
        /// 消息同步
        /// </summary>
        void WebwxSync();

        void Logout();

    }
}

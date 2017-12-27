using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SmartXCore.Event
{
    [Description("通知事件类型")]
    public enum NotifyEventType
    {
        /// <summary>
        /// 二维码已就绪
        /// </summary>
        [Description("二维码已就绪")]
        QRCodeReady,

        /// <summary>
        /// 二维码失效
        /// </summary>
        [Description("二维码失效")]
        QRCodeInvalid,

        /// <summary>
        /// 二维码扫描成功
        /// </summary>
        [Description("二维码扫描成功")]
        QRCodeScanCode,

        /// <summary>
        /// 二维码确认登录
        /// </summary>
        [Description("二维码确认登录")]
        QRCodeSuccess,

        /// <summary>
        /// 登录成功
        /// </summary>
        [Description("登录成功")]
        LoginSuccess,
        
        /// <summary>
        /// 重新登录成功
        /// </summary>
        [Description("重新登录成功")]
        ReloginSuccess,

        /// <summary>
        /// 消息
        /// </summary>
        [Description("消息")]
        Message,

        /// <summary>
        /// 红包
        /// </summary>
        [Description("红包")]
        RedEnvelope,

        /// <summary>
        /// 错误
        /// </summary>
        [Description("错误")]
        Error,

        /// <summary>
        /// 离线
        /// </summary>
        [Description("离线")]
        Offline,
    }
}

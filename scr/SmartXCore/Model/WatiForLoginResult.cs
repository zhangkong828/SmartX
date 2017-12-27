using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartXCore.Model
{
    public enum WatiForLoginResult
    {
        /// <summary>
        /// 初始状态
        /// </summary>
        [Description("初始状态")]
        Initial = 0,
        /// <summary>
        /// 确认登录
        /// </summary>
        [Description("确认登录")]
        Success = 200,
        /// <summary>
        /// 登陆超时
        /// </summary>
        [Description("登陆超时")]
        QRCodeInvalid = 408,
        /// <summary>
        /// 扫描成功
        /// </summary>
        [Description("扫描成功")]
        ScanCode = 201,

    }
}

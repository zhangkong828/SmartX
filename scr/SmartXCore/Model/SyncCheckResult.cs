using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartXCore.Model
{
    public enum SyncCheckResult
    {
        /// <summary>
        /// 什么都没有
        /// </summary>
        Nothing = 0,

        /// <summary>
        /// 新消息
        /// </summary>
        NewMsg = 2,

        /// <summary>
        /// 红包
        /// </summary>
        RedEnvelope = 6,

        /// <summary>
        /// 正在使用手机微信
        /// </summary>
        UsingPhone = 7,

    }
}

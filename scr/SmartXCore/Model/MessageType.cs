using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartXCore.Model
{
    public enum MessageType
    {
        Unknown = 0,

        /// <summary>
        /// 文字消息
        /// </summary>
        [Description("文字消息")]
        Text = 1,

        /// <summary>
        /// 图片消息
        /// </summary>
        [Description("图片消息")]
        Photo = 3,

        /// <summary>
        /// 语音消息
        /// </summary>
        [Description("语音消息")]
        Voice = 34,

        /// <summary>
        /// 好友确认
        /// </summary>
        [Description("好友确认")]
        FriendsConfirmed = 37,

        /// <summary>
        /// 共享名片
        /// </summary>
        [Description("共享名片")]
        BusinessCard = 42,

        /// <summary>
        /// 动画表情
        /// </summary>       
        [Description("动画表情")]
        Face = 47,

        /// <summary>
        /// 位置消息
        /// </summary>       
        [Description("位置消息")]
        Position = 48,

        /// <summary>
        /// 分享链接
        /// </summary>
        [Description("分享链接")]
        Link = 49,

        /// <summary>
        /// 微信初始化
        /// </summary>
        [Description("微信初始化")]
        WxInit = 51,

        /// <summary>
        /// 小视频
        /// </summary>
        [Description("小视频")]
        Video = 62,

        /// <summary>
        /// 系统消息
        /// </summary>
        [Description("系统消息")]
        System = 10000,

        /// <summary>
        /// 消息撤回
        /// </summary>
        [Description("消息撤回")]
        MsgWithdraw = 10002
    }
}

using SmartX;
using SmartX.Core;
using SmartX.Event;
using SmartX.Extensions;
using SmartX.Model;
using SmartX.Module;
using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

namespace SimpleConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            WebWeChatClient.Build(listener).Start();

            Console.ReadKey();
        }


        static NotifyEventListener listener = async (client, notifyEvent) =>
        {
            var logger = client.Logger;
            switch (notifyEvent.Type)
            {
                case NotifyEventType.QRCodeReady:
                    {
                        var bytes = notifyEvent.Target as byte[];
                        //直接打印 有点问题
                        using (var ms = new MemoryStream(bytes))
                        {
                            //获取微信原图像
                            var image = new Bitmap(Image.FromStream(ms));
                            //解析二维码
                            var content = QrCodeHelper.Decode(image);
                            Console.WriteLine(content);
                            //重新生成二维码
                            image = QrCodeHelper.Encode(content);
                            //打印
                            QrCodeHelper.ConsoleWriteImage(image);
                        }
                        ColorConsole(ConsoleColor.Gray, "请使用手机微信扫描二维码");
                        break;
                    }

                case NotifyEventType.QRCodeScanCode:
                    ColorConsole(ConsoleColor.Gray, "已扫描，等待登录...");
                    break;

                case NotifyEventType.QRCodeSuccess:
                    ColorConsole(ConsoleColor.Gray, "已确认登录");
                    ColorConsole(ConsoleColor.Gray, "开始获取联系人...");
                    break;

                case NotifyEventType.QRCodeInvalid:
                    ColorConsole(ConsoleColor.Red, "二维码已失效");
                    break;

                case NotifyEventType.LoginSuccess:
                    var store = client.GetModule<StoreModule>();
                    ColorConsole(ConsoleColor.Gray, $"获取好友列表成功，共{store.FriendCount}个");
                    ColorConsole(ConsoleColor.Gray, $"获取公众号|服务号列表成功，共{store.PublicUserCount}个");
                    break;

                case NotifyEventType.BeginSyncCheck:
                    ColorConsole(ConsoleColor.Gray, "开启同步检测...{0}", notifyEvent.Target as string);
                    break;

                case NotifyEventType.SyncCheckSuccess:
                    ColorConsole(ConsoleColor.Gray, "同步检测成功");
                    ColorConsole(ConsoleColor.Gray, "开始循环监听消息...");
                    break;

                case NotifyEventType.SyncCheckError:
                    ColorConsole(ConsoleColor.Red, "同步检测失败");
                    ColorConsole(ConsoleColor.Magenta, "请重新登录");
                    break;

                case NotifyEventType.Message:
                    {
                        //处理消息
                        var msg = (Message)notifyEvent.Target;
                        MessageHandle(client, msg);
                        break;
                    }

                case NotifyEventType.Offline:
                    ColorConsole(ConsoleColor.Red, "微信已离线");
                    break;

                case NotifyEventType.Error:
                    var error = notifyEvent.Target as string;
                    ColorConsole(ConsoleColor.Red, error);
                    break;

                default:
                    Console.WriteLine(notifyEvent.Type.GetFullDescription());
                    break;

            }
        };

        /// <summary>
        /// 处理消息
        /// </summary>
        static void MessageHandle(IContext client, Message msg)
        {
            var mySelf = client.GetModule<SessionModule>().User.UserName;
            if (msg.FromUserName == mySelf)
                return;//不处理自己发的消息
            var groupName = string.Empty;
            var fromName = string.Empty;
            var content = string.Empty;
            if (msg.IsGroup() && msg.MsgType != MessageType.System)
            {
                //群消息，获取群成员信息
                var match = Regex.Match(msg.Content, "(.+?):<br\\/>(.*)");
                fromName = match.Groups[1].Value;
                content = match.Groups[2].Value;
                var contactMember = client.GetModule<StoreModule>().ContactMemberDic;
                var groupId = msg.FromUserName;
                if (contactMember.ContainsKey(groupId))
                {
                    groupName = contactMember[groupId].ShowName;
                    fromName = contactMember[groupId].MemberList.FirstOrDefault(x => x.UserName == fromName)?.ShowName;
                }
                else
                {
                    if (client.GetModule<IContactModule>().GetGroupMember(groupId))
                    {
                        groupName = contactMember[groupId].ShowName;
                        fromName = contactMember[groupId].MemberList.FirstOrDefault(x => x.UserName == fromName)?.ShowName;
                    }
                    else
                        groupName = "群消息";
                }
            }
            else
            {
                fromName = msg.FromUser?.ShowName;
                content = msg.Content;
            }
            if (msg.MsgType != MessageType.Text && msg.MsgType != MessageType.System)
                content = "发送了" + msg.MsgType.GetDescription();
            var message = string.Format("{0}:{1}", fromName, content);
            if (!groupName.IsNullOrEmpty())
                message = string.Format("[{0}]{1}", groupName, message);
            //文字消息
            if (msg.MsgType == MessageType.Text)
                ColorConsole(ConsoleColor.DarkGreen, message);
            else
                ColorConsole(ConsoleColor.DarkYellow, message);

        }


        /// <summary>
        /// 彩色打印
        /// </summary>
        static void ColorConsole(ConsoleColor color, string str, params object[] arg)
        {
            var preForegroundColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + "> " + str, arg);
            Console.ForegroundColor = preForegroundColor;
        }

    }
}

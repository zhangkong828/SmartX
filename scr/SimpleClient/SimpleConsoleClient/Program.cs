using SmartXCore;
using SmartXCore.Core;
using SmartXCore.Event;
using SmartXCore.Extensions;
using SmartXCore.Model;
using SmartXCore.Module;
using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;

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
                        //打印二维码 
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
                        ColorConsole(ConsoleColor.DarkBlue, "请使用手机微信扫描二维码");
                        break;
                    }

                case NotifyEventType.QRCodeScanCode:
                    ColorConsole(ConsoleColor.DarkBlue, "已扫描，请点击确认登录");
                    break;

                case NotifyEventType.QRCodeSuccess:
                    ColorConsole(ConsoleColor.DarkBlue, "确认登录");
                    ColorConsole(ConsoleColor.DarkBlue, "开始获取联系人...");
                    break;

                case NotifyEventType.QRCodeInvalid:
                    ColorConsole(ConsoleColor.DarkBlue, "二维码已失效");
                    break;

                case NotifyEventType.LoginSuccess:
                    ColorConsole(ConsoleColor.DarkBlue, "登录成功");
                    var store = client.GetModule<StoreModule>();
                    ColorConsole(ConsoleColor.DarkBlue, $"应有{store.MemberCount}个联系人，读取到联系人{store.ContactMemberDic.Count}个");
                    ColorConsole(ConsoleColor.DarkBlue, $"共有{store.GroupCount}个群|{store.FriendCount}个好友|{store.SpecialUserCount}个微信官方账号|{store.PublicUserCount}公众号或服务号");
                    break;

                case NotifyEventType.Message:
                    {
                        //处理消息
                        var msg = (Message)notifyEvent.Target;
                        MessageHandle(client, msg);
                        break;
                    }

                case NotifyEventType.Offline:
                    ColorConsole(ConsoleColor.DarkBlue, "微信已离线");
                    break;

                case NotifyEventType.Error:
                    var error = notifyEvent.Target as string;
                    ColorConsole(ConsoleColor.Yellow, error);
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
            var store = client.GetModule<StoreModule>();
            var contactMember = store.ContactMemberDic;
            var userName = client.GetModule<SessionModule>().User.UserName;
            //文字消息
            if (msg.FromUserName != userName && msg.MsgType == MessageType.Text && !msg.Content.IsNullOrEmpty())
            {
                //是否是群消息
                var isGroup = msg.FromUserName.StartsWith("@@");
                var fromName = string.Empty;
                var content = string.Empty;
                if (isGroup)
                {
                    var regex = new Regex("@.+?<br/>(.*)");
                    var match = regex.Match(msg.Content);
                    fromName = "群消息";
                    content = match.Groups[1].Value;

                }
                else
                {
                    fromName = msg.FromUser?.ShowName;
                    content = msg.Content;
                }
                var message = string.Format("[{0}]:{1}", fromName, content);

                ColorConsole(ConsoleColor.Green, message);
            }
            //系统消息
            else if (msg.MsgType == MessageType.System)
            {
                if (msg.Content.Contains("红包"))
                {
                    ColorConsole(ConsoleColor.Red, "【收到红包】");
                }
                else
                {
                    ColorConsole(ConsoleColor.Red, msg.Content);
                }
            }
        }


        /// <summary>
        /// 彩色打印
        /// </summary>
        static void ColorConsole(ConsoleColor color, string str, params object[] arg)
        {
            var preForegroundColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(str, arg);
            Console.ForegroundColor = preForegroundColor;
        }

    }
}

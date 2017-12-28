using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartXCore.Core;
using SmartXCore.Extensions;
using SmartXCore.Model;

namespace SmartXCore.Module
{
    [Description("通讯录模块")]
    public class ContactModule : BaseModule, IContactModule
    {
        public ContactModule(IContext context) : base(context)
        {
        }

        public bool GetContact()
        {
            var url = string.Format(ApiUrls.GetContact, _session.BaseUrl, _session.PassTicket, _session.Skey, _timestamp);
            var obj = new { _session.BaseRequest };
            var response = _httpClient.PostJsonAsync(url, obj.ToJson()).Result;
            var json = response.RawText().ToJToken();
            if (json["BaseResponse"]["Ret"].ToString() == "0")
            {
                _store.MemberCount = json["MemberCount"].ToObject<int>();
                var list = json["MemberList"].ToObject<ContactMember[]>();
                _store.ContactMemberDic.ReplaceBy(list, m => m.UserName);

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool GetGroupMember(string groupId)
        {
            return GetGroupMember(new List<string>() { groupId });
        }


        public bool GetGroupMember(IEnumerable<string> groups)
        {
            var url = string.Format(ApiUrls.BatchGetContact, _session.BaseUrl, _session.PassTicket, _timestamp);
            var obj = new
            {
                _session.BaseRequest,
                Count = groups.Count(),
                List = groups.Select(group => new { UserName = group, EncryChatRoomId = "" })
            };
            var response = _httpClient.PostJsonAsync(url, obj.ToJson()).Result;
            var json = response.RawText().ToJToken();
            if (json["BaseResponse"]["Ret"].ToString() == "0")
            {
                var list = json["ContactList"].ToObject<List<ContactMember>>();
                foreach (var item in list)
                {
                    _store.ContactMemberDic[item.UserName] = item;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace SmartXCore.Core
{
    public interface IContactModule : IBaseModule
    {
        bool GetContact();

        bool GetGroupMember(string groupId);

        bool GetGroupMember(IEnumerable<string> groups);
    }
}

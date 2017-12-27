using SmartXCore.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartXCore.Core
{
    public interface IChatModule : IBaseModule
    {
        bool SendMsg(MessageSent msg);
    }
}

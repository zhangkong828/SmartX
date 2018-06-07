using SmartX.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartX.Core
{
    public interface IChatModule : IBaseModule
    {
        bool SendMsg(MessageSent msg);
    }
}

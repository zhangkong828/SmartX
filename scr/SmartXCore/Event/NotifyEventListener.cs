using SmartXCore.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartXCore.Event
{
    public delegate Task NotifyEventListener(IContext sender, NotifyEvent e);
}

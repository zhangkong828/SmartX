using SmartX.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartX.Event
{
    public delegate Task NotifyEventListener(IContext sender, NotifyEvent e);
}

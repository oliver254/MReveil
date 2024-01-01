using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monbsoft.MReveil.Messaging;

internal class ResetAlarmMessage : ValueChangedMessage<bool>
{
    public ResetAlarmMessage(bool value)
        : base(value)
    {        
    }
}

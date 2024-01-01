using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Monbsoft.MReveil.Messaging;

public class AlarmMessage : ValueChangedMessage<bool>
{
    public AlarmMessage(bool value) 
        : base(value)
    {
    }
}

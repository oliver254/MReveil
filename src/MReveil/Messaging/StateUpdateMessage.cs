using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Monbsoft.MReveil.Messaging;

public class StateUpdateMessage : ValueChangedMessage<TimeSpan>
{
    public StateUpdateMessage(TimeSpan value)
        : base(value)
    {        
    }
}

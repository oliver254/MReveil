using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Monbsoft.MReveil.Messaging;

public class DurationSetMessage : ValueChangedMessage<TimeSpan>
{
    public DurationSetMessage(TimeSpan value)
        : base(value)
    {
    }
}

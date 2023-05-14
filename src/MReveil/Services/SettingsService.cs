namespace Monbsoft.MReveil.Services;

public class SettingsService
{
    public int LongBreak 
    {
        get => Preferences.Get(nameof(LongBreak), 15);
        set => Preferences.Set(nameof(LongBreak),value);
    }
    public int ShortBreak
    {
        get => Preferences.Get(nameof(ShortBreak), 5);
        set => Preferences.Set(nameof(ShortBreak), value);
    }
    public int Sprint
    {
        get => Preferences.Get(nameof(Sprint), 25);
        set => Preferences.Set(nameof(Sprint), value);
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MReveil.Services;

public class ThemeService
{
    public ThemeService()
    {

    }

    public string Mode
    {
        get => Preferences.Get(nameof(Mode), string.Empty);
        set => Preferences.Set(nameof(Mode), value);
    }

    public void Initialize()
    {
        SetMode(Mode);
    }
    public void SetMode(string mode)
    {
        switch(mode)
        {
            case "Light":
                {
                    Application.Current.UserAppTheme = AppTheme.Light;
                    break;
                }
            case "Dark":
                {
                    Application.Current.UserAppTheme = AppTheme.Dark;
                    break;
                }
            default:
                {
                    Application.Current.UserAppTheme = AppTheme.Unspecified;
                    break;
                }
        }
        Mode = mode;
    }
}

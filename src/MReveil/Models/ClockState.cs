using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monbsoft.MReveil.Models;

public class ClockState : IState
{
    public TimeSpan Time
    {
        get;
        private set;
    }

    public void Refresh()
    {
        Time = DateTime.Now.TimeOfDay;
    }

}

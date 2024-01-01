using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monbsoft.MReveil.Models;

public interface IState
{
    TimeSpan Time { get; }

    void Refresh();
}

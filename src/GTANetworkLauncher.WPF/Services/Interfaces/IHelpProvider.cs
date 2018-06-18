using System.Collections.Generic;
using GrandTheftMultiplayer.Launcher.Models.Help;

namespace GrandTheftMultiplayer.Launcher.Services.Interfaces
{
    public interface IHelpProvider
    {
        IEnumerable<HelpItem> GetHelpItems();
    }
}
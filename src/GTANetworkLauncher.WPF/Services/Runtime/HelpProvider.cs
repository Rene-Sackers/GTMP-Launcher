using System.Collections.Generic;
using GalaSoft.MvvmLight.Messaging;
using GrandTheftMultiplayer.Launcher.Models.GameLauncherService;
using GrandTheftMultiplayer.Launcher.Models.Help;
using GrandTheftMultiplayer.Launcher.Models.Messaging.LauncherSplashScreen;
using GrandTheftMultiplayer.Launcher.Services.Interfaces;

namespace GrandTheftMultiplayer.Launcher.Services.Runtime
{
    public class HelpProvider : IHelpProvider
    {
        public IEnumerable<HelpItem> GetHelpItems()
        {
            return new List<HelpItem>
            {
                new HelpItem
                {
                    Title = "GTA V Activation Required",
                    ActionText = "Start Singleplayer",
                    Text = "Launch the game in singleplayer to verify a legit copy.",
                    Action = () => Messenger.Default.Send(new LaunchGameMessage(GameLaunchMode.Singleplayer))
                }
            };
        }
    }
}

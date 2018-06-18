using System;
using System.Windows;
using MahApps.Metro;

namespace GrandTheftMultiplayer.Launcher
{
    public partial class App
    {
	    protected override void OnStartup(StartupEventArgs e)
		{
		    ThemeManager.AddAccent("GtMpRed", new Uri("pack://application:,,,/Resources/Accents/GtMpRed.xaml"));
		    ThemeManager.AddAppTheme("GtMpRed", new Uri("pack://application:,,,/Resources/Themes/GtMpRed.xaml"));

            // now change app style to the custom accent and current theme
            ThemeManager.ChangeAppStyle(Current,
		        ThemeManager.GetAccent("GtMpRed"),
		        ThemeManager.GetAppTheme("GtMpRed"));

            base.OnStartup(e);
        }
	}
}

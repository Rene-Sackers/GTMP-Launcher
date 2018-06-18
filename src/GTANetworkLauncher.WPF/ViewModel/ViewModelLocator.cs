using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GrandTheftMultiplayer.Launcher.Controls.ViewModels;
using GrandTheftMultiplayer.Launcher.Services.Design;
using GrandTheftMultiplayer.Launcher.Services.Interfaces;
using GrandTheftMultiplayer.Launcher.Services.Runtime;
using Microsoft.Practices.ServiceLocation;

namespace GrandTheftMultiplayer.Launcher.ViewModel
{
    public class ViewModelLocator
    {
		public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

	    public LauncherSplashScreenViewModel LauncherSplashScreen => ServiceLocator.Current.GetInstance<LauncherSplashScreenViewModel>();

	    public UpdateSplashScreenViewModel UpdateSplashScreen => ServiceLocator.Current.GetInstance<UpdateSplashScreenViewModel>();

		public ServerBrowserViewModel ServerBrowser => SimpleIoc.Default.GetInstanceWithoutCaching<ServerBrowserViewModel>();

        public ViewModelLocator()
        {
			SimpleIoc.Default.Reset();

            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

			if (ViewModelBase.IsInDesignModeStatic)
            {
				RegisterSingleton<INotificationService, DesignNotificationService>();

				SimpleIoc.Default.Register<ISettingsProvider, DesignSettingsProvider>();
                SimpleIoc.Default.Register<INewsProvider, DesignNewsProvider>();
                SimpleIoc.Default.Register<IHelpProvider, HelpProvider>();
                SimpleIoc.Default.Register<IServerListProvider, DesignServerListProvider>();
                SimpleIoc.Default.Register<IServerQueryingService, DesignServerQueryingService>();
                SimpleIoc.Default.Register<IStatisticsProvider, DesignStatisticsProvider>();
                SimpleIoc.Default.Register<IServerStatusProvider, DesignServerStatusProvider>();
				SimpleIoc.Default.Register<ISplashScreenProgressProvider, DesignSplashScreenProgressProvider>();
				SimpleIoc.Default.Register<ILogger, DesignLogger>();
			}
            else
			{
				RegisterSingleton<INotificationService, NotificationService>();

                //SimpleIoc.Default.Register<IServerListIpsProvider, LocalServerListIpsService>();
                SimpleIoc.Default.Register<IServerListIpsProvider, ServerListIpsProvider>();

                SimpleIoc.Default.Register<ISettingsProvider, SettingsProvider>();
				SimpleIoc.Default.Register<INewsProvider, NewsProvider>();
			    SimpleIoc.Default.Register<IHelpProvider, HelpProvider>();
                SimpleIoc.Default.Register<IServerListProvider, ServerListProvider>();
                SimpleIoc.Default.Register<IServerQueryingService, ServerQueryingService>();
                SimpleIoc.Default.Register<IStatisticsProvider, StatisticsProvider>();
                SimpleIoc.Default.Register<IServerStatusProvider, ServerStatusProvider>();
				SimpleIoc.Default.Register<ISplashScreenProgressProvider, SplashScreenProgressProvider>();
				SimpleIoc.Default.Register<ILogger, Logger>();
			}

            SimpleIoc.Default.Register<IEnsureRegistryKeyService, EnsureRegistryKeyService>();
            SimpleIoc.Default.Register<ITroubleshootProvider, TroubleshootProvider>();
            SimpleIoc.Default.Register<IGameBackupService, GameBackupService>();
            SimpleIoc.Default.Register<IGameLauncherService, GameLauncherService>();
            SimpleIoc.Default.Register<ILowKeySuppressionService, LowKeySuppressionService>();
            SimpleIoc.Default.Register<ISocialClubCommandlineService, SocialClubCommandlineService>();
            SimpleIoc.Default.Register<ILaunchDependencyCheckService, LaunchDependencyCheckService>();
			SimpleIoc.Default.Register<ICefFilesUpdater, CefFilesUpdater>();
			SimpleIoc.Default.Register<IClientFilesUpdater, ClientFilesUpdater>();
			SimpleIoc.Default.Register<IGamePathProvider, GamePathProvider>();
			SimpleIoc.Default.Register<IGtaVersionProvider, GtaVersionProvider>();
			SimpleIoc.Default.Register<IGameInjectionService, GameInjectionService>();
			SimpleIoc.Default.Register<ISelfUpdateService, SelfUpdateService>();
			SimpleIoc.Default.Register<IServerHostProvider, ServerHostProvider>();
            SimpleIoc.Default.Register<IProtocolGameLauncherService, ProtocolGameLauncherService>();

            SimpleIoc.Default.Register<LauncherSplashScreenViewModel>(true);
	        SimpleIoc.Default.Register<UpdateSplashScreenViewModel>(true);
			SimpleIoc.Default.Register<ServerBrowserViewModel>();
	        SimpleIoc.Default.Register<MainViewModel>();
        }

	    private static TInstance RegisterSingleton<TInterface, TInstance>()
			where TInterface : class
			where TInstance : class, TInterface
		{
		    var instance = (TInstance)Activator.CreateInstance(typeof(TInstance));

			SimpleIoc.Default.Register<TInterface>(() => instance);

			return instance;
	    }
        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}
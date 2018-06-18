using System;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using GrandTheftMultiplayer.Launcher.Models.Messaging.MainWindow;
using GrandTheftMultiplayer.Launcher.Models.PromptWindow;
using GrandTheftMultiplayer.Launcher.ViewModel;

namespace GrandTheftMultiplayer.Launcher.Views
{
    public partial class MainWindow
    {
	    private readonly LauncherSplashScreen _launcherSplashScreen;
	    private readonly UpdateSplashScreen _updateSplashScreen;

	    public MainWindow()
		{
			_launcherSplashScreen = new LauncherSplashScreen();
			_updateSplashScreen = new UpdateSplashScreen();

		    Messenger.Default.Register<ShowEulaDialog>(this, true, ShowEulaDialog);

            InitializeComponent();
        }

        private void ShowEulaDialog(ShowEulaDialog message)
        {
            if (!IsLoaded) Loaded += (sender, args) => DoShowEulaDialog();
            else DoShowEulaDialog();
        }

        private async void DoShowEulaDialog()
        {
            var promtViewModel = new PromptWindowViewModel();

            var acceptButton = new PromptButton("Accept");
            var declineButton = new PromptButton("Decline");

            promtViewModel.Buttons.Add(acceptButton);
            promtViewModel.Buttons.Add(declineButton);

            IsEnabled = false;
            var clickedButton = await promtViewModel.Show(this);
            IsEnabled = true;

            Messenger.Default.Send(new ShowEulaDialogResult(clickedButton == acceptButton));
        }

        private void OnClosed(object sender, EventArgs e)
	    {
		    _launcherSplashScreen.Close();
		    _updateSplashScreen.Close();
	    }

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			var textBox = sender as TextBox;
			textBox?.ScrollToEnd();
		}

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_launcherSplashScreen != null)
                _launcherSplashScreen.Owner = this;
        }
    }
}

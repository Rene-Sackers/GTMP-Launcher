using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GrandTheftMultiplayer.Launcher.Helpers;
using ServerModel = GrandTheftMultiplayer.Launcher.Models.ServerApi.Server;

namespace GrandTheftMultiplayer.Launcher.Controls.Views
{
    public partial class ServerBrowser
    {
        public ServerBrowser()
        {
            InitializeComponent();
        }

        private void ServersListViewOnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Escape) return;

            var listView = sender as ListView;
            if (listView != null) listView.SelectedItem = null;
        }

        private void ServersListViewOnLoaded(object sender, RoutedEventArgs e)
        {
            GridViewSort.ApplySort(ServersListView.Items, nameof(ServerModel.PlayerCount), ServersListView, PlayerCountColumnHeader);
        }
    }
}

using System.Diagnostics;
using System.Windows.Navigation;

namespace GrandTheftMultiplayer.Launcher.Views
{
    public partial class PromptWindow
    {
        public PromptWindow()
        {
            InitializeComponent();
        }

        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}

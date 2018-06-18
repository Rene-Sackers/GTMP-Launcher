using GalaSoft.MvvmLight.Command;

namespace GrandTheftMultiplayer.Launcher.Models.Help
{
	public class HelpDisplayModel
	{
		public HelpItem HelpItem { get; }
        
		public RelayCommand TriggerActionCommand { get; set; }

        public HelpDisplayModel(HelpItem helpItem)
		{
		    HelpItem = helpItem;

		    TriggerActionCommand = new RelayCommand(() =>
		    {
		        HelpItem.Action.Invoke();
		    });
		}
	}
}
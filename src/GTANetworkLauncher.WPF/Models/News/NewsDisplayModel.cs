using System.ComponentModel;
using System.Runtime.CompilerServices;
using GalaSoft.MvvmLight.Command;
using GrandTheftMultiplayer.Launcher.Models.Forum;
using GrandTheftMultiplayer.Launcher.Properties;

namespace GrandTheftMultiplayer.Launcher.Models.News
{
	public class NewsDisplayModel : INotifyPropertyChanged
	{
		private bool _readMoreExpanded;

		public ForumPostItem ForumPostItem { get; }

		public bool ReadMoreExpanded
		{
			get => _readMoreExpanded;
		    set
			{
				if (value == _readMoreExpanded) return;
				_readMoreExpanded = value;
				OnPropertyChanged();
			}
		}

		public RelayCommand ToggleReadMoreCommand { get; set; }

		public NewsDisplayModel(ForumPostItem forumPostItem)
		{
			ForumPostItem = forumPostItem;
			ToggleReadMoreCommand = new RelayCommand(() => ReadMoreExpanded = !ReadMoreExpanded);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GrandTheftMultiplayer.Launcher.Models.PromptWindow;
using GrandTheftMultiplayer.Launcher.Views;

namespace GrandTheftMultiplayer.Launcher.ViewModel
{
    public class PromptWindowViewModel : ViewModelBase
    {
        private PromptWindow _promptWindow;

        public ObservableCollection<PromptButton> Buttons { get; set; } = new ObservableCollection<PromptButton>();

        private TaskCompletionSource<PromptButton> _completionSource;

        public PromptWindowViewModel()
        {
            Buttons.CollectionChanged += ButtonsOnCollectionChanged;
        }

        private void ButtonsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (notifyCollectionChangedEventArgs.NewItems?.Count > 0)
                foreach (var addedButton in notifyCollectionChangedEventArgs.NewItems.OfType<PromptButton>())
                addedButton.Clicked += PromptButtonClicked;

            if (notifyCollectionChangedEventArgs.OldItems?.Count > 0)
                foreach (var removedButton in notifyCollectionChangedEventArgs.OldItems.OfType<PromptButton>())
                removedButton.Clicked -= PromptButtonClicked;
        }

        private void PromptButtonClicked(PromptButton button)
        {
            _completionSource?.TrySetResult(button);
            Close();
        }

        public Task<PromptButton> Show(Window ownerWindow = null)
        {
            _completionSource?.TrySetResult(null);

            Close();

            _completionSource = new TaskCompletionSource<PromptButton>();
            _promptWindow = new PromptWindow
            {
                DataContext = this,
                Owner = ownerWindow
            };

            _promptWindow.Show();

            return _completionSource.Task;
        }
        
        public void Close()
        {
            if (_promptWindow?.IsLoaded == true)
                _promptWindow?.Close();
        }
    }
}
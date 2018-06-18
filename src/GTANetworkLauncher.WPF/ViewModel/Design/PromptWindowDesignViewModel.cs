using GrandTheftMultiplayer.Launcher.Models.PromptWindow;

namespace GrandTheftMultiplayer.Launcher.ViewModel.Design
{
    public class PromptWindowDesignViewModel : PromptWindowViewModel
    {
        public PromptWindowDesignViewModel()
        {
            Buttons.Add(new PromptButton("Accept"));
            Buttons.Add(new PromptButton("Decline"));
        }
    }
}
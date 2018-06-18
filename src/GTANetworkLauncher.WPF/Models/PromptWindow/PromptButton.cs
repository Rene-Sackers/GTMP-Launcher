using System;
using GalaSoft.MvvmLight.Command;

namespace GrandTheftMultiplayer.Launcher.Models.PromptWindow
{
    public class PromptButton
    {
        public string Title { get; }

        public RelayCommand ClickedCommand { get; }

        public delegate void ClickedHandler(PromptButton button);

        public event ClickedHandler Clicked;

        public PromptButton(string title)
        {
            Title = title;
            ClickedCommand = new RelayCommand(ButtonClicked);
        }

        private void ButtonClicked()
        {
            Clicked?.Invoke(this);
        }
    }
}
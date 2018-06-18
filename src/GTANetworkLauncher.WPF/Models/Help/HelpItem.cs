using System;

namespace GrandTheftMultiplayer.Launcher.Models.Help
{
    public class HelpItem
    {
        public string Title { get; set; }
        
        public string Text { get; set; }
        
        public string ActionText { get; set; }

        public Action Action { get; set; }
    }
}
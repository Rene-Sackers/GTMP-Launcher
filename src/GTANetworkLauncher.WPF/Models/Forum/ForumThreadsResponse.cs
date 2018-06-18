using System.Collections.Generic;
using Newtonsoft.Json;

namespace GrandTheftMultiplayer.Launcher.Models.Forum
{
    public class ForumThreadsResponse
    {
        [JsonProperty("threads")]
        public IList<ForumThreadItem> Threads { get; set; }
    }
}

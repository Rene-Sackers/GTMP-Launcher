using System;
using GrandTheftMultiplayer.Launcher.Helpers;
using Newtonsoft.Json;

namespace GrandTheftMultiplayer.Launcher.Models.Forum
{
    public class ForumThreadItem
    {
        [JsonProperty("thread_id")]
        public int Id { get; set; }

        [JsonProperty("post_date")]
        [JsonConverter(typeof(UnixTimestampToDatetimeConverter))]
        public DateTime Timestamp { get; set; }

        [JsonProperty("first_post_id")]
        public int FirstPostId { get; set; }
    }
}

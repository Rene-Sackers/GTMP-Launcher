using System;
using GrandTheftMultiplayer.Launcher.Helpers;
using Newtonsoft.Json;

namespace GrandTheftMultiplayer.Launcher.Models.Forum
{
    public class ForumPostItem
    {
		[JsonProperty("thread_id")]
		public int Id { get; set; }

        [JsonProperty("post_date")]
        [JsonConverter(typeof(UnixTimestampToDatetimeConverter))]
        public DateTime Timestamp { get; set; }

        public string Title { get; set; }

        [JsonProperty("username")]
        public string Author { get; set; }

        [JsonProperty("message")]
        public string Text { get; set; }

        [JsonProperty("absolute_url")]
        public string Url { get; set; }
    }
}
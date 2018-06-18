using System;
using GrandTheftMultiplayer.Launcher.Helpers;
using Newtonsoft.Json;

namespace GrandTheftMultiplayer.Launcher.Models.Status
{
    public class ServerStatus
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Link { get; set; }

        public ServerStatuses Status { get; set; }

        public int Order { get; set; }

        [JsonProperty("group_id")]
        public int GroupId { get; set; }

        public bool Enabled { get; set; }

        [JsonProperty("create_at")]
        [JsonConverter(typeof(MicrosoftDateTimeConverter))]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        [JsonConverter(typeof(MicrosoftDateTimeConverter))]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("deleted_at")]
        [JsonConverter(typeof(MicrosoftDateTimeConverter))]
        public DateTime? DeletedAt { get; set; }

        [JsonProperty("status_name")]
        public string StatusName { get; set; }
    }
}

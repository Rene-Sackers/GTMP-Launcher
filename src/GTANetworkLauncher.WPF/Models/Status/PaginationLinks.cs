using Newtonsoft.Json;

namespace GrandTheftMultiplayer.Launcher.Models.Status
{
    public class PaginationLinks
    {
        [JsonProperty("next_page")]
        public string NextPage { get; set; }

        [JsonProperty("previous_page")]
        public string PreviousPage { get; set; }
    }
}
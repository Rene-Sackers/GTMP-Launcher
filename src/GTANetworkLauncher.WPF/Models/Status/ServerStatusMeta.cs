using Newtonsoft.Json;

namespace GrandTheftMultiplayer.Launcher.Models.Status
{
    public class ServerStatusMeta
    {
        public Pagination Oagination { get; set; }
    }

    public class Pagination
    {
        public int Total { get; set; }

        public int Count { get; set; }

        [JsonProperty("per_page")]
        public int PerPage { get; set; }

        [JsonProperty("current_page")]
        public int CurrentPage { get; set; }

        [JsonProperty("total_pages")]
        public int TotalPages { get; set; }

        public PaginationLinks Links { get; set; }
    }
}
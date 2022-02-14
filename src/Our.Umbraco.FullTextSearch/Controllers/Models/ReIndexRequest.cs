using Newtonsoft.Json;

namespace Our.Umbraco.FullTextSearch.Controllers.Models
{
    public class ReIndexRequest
    {
        [JsonProperty("nodeIds")]
        public int[] NodeIds { get; set; }

        [JsonProperty("includeDescendants")]
        public bool IncludeDescendants { get; set; }
    }
}

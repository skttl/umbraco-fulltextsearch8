using Newtonsoft.Json;

namespace Our.Umbraco.FullTextSearch.Controllers.Models
{
    public class IndexStatus
    {
        [JsonProperty("totalIndexableNodes")]
        public long TotalIndexableNodes { get; set; }
        [JsonProperty("totalIndexedNodes")]
        public long TotalIndexedNodes { get; set; }
        [JsonProperty("incorrectIndexedNodes")]
        public long IncorrectIndexedNodes { get; set; }
        [JsonProperty("missingIndexedNodes")]
        public long MissingIndexedNodes { get; set; }
    }
}

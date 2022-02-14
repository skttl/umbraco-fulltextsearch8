using Newtonsoft.Json;
using System.Collections.Generic;

namespace Our.Umbraco.FullTextSearch.Controllers.Models
{
    public class IndexedNodeResult
    {
        [JsonProperty("items")]
        public List<IndexedNode> Items { get; set; }
        [JsonProperty("pageNumber")]
        public int PageNumber { get; set; }
        [JsonProperty("totalPages")]
        public long TotalPages { get; set; }

        public IndexedNodeResult()
        {
            Items = new List<IndexedNode>();
        }
    }
}

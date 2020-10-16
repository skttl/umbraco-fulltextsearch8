using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Our.Umbraco.FullTextSearch.Controllers.Models
{
    public class IndexedNode
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("icon")]
        public string Icon { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("allFields")]
        public IReadOnlyDictionary<string,string> AllFields { get; set; }
    }
}

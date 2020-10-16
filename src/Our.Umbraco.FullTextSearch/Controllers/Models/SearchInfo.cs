using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Our.Umbraco.FullTextSearch.Controllers.Models
{
    public class SearchInfo
    {
        [JsonProperty("defaultSettings")]
        public SearchSettings DefaultSettings { get; set; }
        [JsonProperty("fieldNames")]
        public List<string> FieldNames { get; set; }
        [JsonProperty("cultures")]
        public List<string> Cultures { get; set; }
    }
}

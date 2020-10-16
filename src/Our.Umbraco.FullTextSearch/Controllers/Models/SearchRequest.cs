using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Our.Umbraco.FullTextSearch.Controllers.Models
{
    public class SearchRequest
    {
        [JsonProperty("advancedSettings")]
        public SearchSettings AdvancedSettings { get; set; }
        [JsonProperty("searchTerms")]
        public string SearchTerms { get; set; }
        [JsonProperty("pageNumber")]
        public int PageNumber { get; set; }
    }
}

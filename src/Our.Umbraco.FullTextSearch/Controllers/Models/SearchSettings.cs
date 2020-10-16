using Newtonsoft.Json;
using Our.Umbraco.FullTextSearch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Our.Umbraco.FullTextSearch.Controllers.Models
{
    public class SearchSettings
    {
        [JsonProperty("searchType")]
        public string SearchType { get; set; }
        [JsonProperty("titleProperties")]
        public List<string> TitleProperties { get; set; }
        [JsonProperty("titleBoost")]
        public double TitleBoost { get; set; }
        [JsonProperty("bodyProperties")]
        public List<string> BodyProperties { get; set; }
        [JsonProperty("summaryProperties")]
        public List<string> SummaryProperties { get; set; }
        [JsonProperty("summaryLength")]
        public int SummaryLength { get; set; }
        [JsonProperty("rootNodeIds")]
        public int[] RootNodeIds { get; set; }
        [JsonProperty("culture")]
        public string Culture { get; set; }
        [JsonProperty("enableWildcards")]
        public bool EnableWildcards { get; set; }
        [JsonProperty("fuzzyness")]
        public double Fuzzyness { get; set; }
    }
}

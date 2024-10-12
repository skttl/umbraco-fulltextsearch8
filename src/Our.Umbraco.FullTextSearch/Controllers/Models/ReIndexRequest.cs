using Newtonsoft.Json;
using System;

namespace Our.Umbraco.FullTextSearch.Controllers.Models;

public class ReIndexRequest
{
    [JsonProperty("nodeKey")]
    public Guid? NodeKey { get; set; }

    [JsonProperty("includeDescendants")]
    public bool IncludeDescendants { get; set; }
}

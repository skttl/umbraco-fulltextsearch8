using Newtonsoft.Json;

namespace Our.Umbraco.FullTextSearch.Controllers.Models
{
    public class ReIndexResult
    {
        public ReIndexResult(bool success, string message = "")
        {
            Success = success;
            Message = message;
        }
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}

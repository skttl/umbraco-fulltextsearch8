using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Our.Umbraco.FullTextSearch.Options;

namespace Our.Umbraco.FullTextSearch.Controllers;

[ApiVersion("5.0")]
[ApiExplorerSettings(GroupName = "fulltextsearch")]
public class ConfigController : FullTextSearchControllerBase
{
    private readonly FullTextSearchOptions _options;

    public ConfigController(
        IOptions<FullTextSearchOptions> options
        )
    {
        _options = options.Value;
    }

    [HttpGet("config")]
    [ProducesResponseType(typeof(FullTextSearchOptions), 200)]
    public FullTextSearchOptions Get()
    {
        return _options;
    }
}
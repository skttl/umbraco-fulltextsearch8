using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Our.Umbraco.FullTextSearch.Options;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;

namespace Our.Umbraco.FullTextSearch.Controllers
{
    [PluginController("FullTextSearch")]
    public class ConfigController : UmbracoAuthorizedApiController
    {
        private readonly FullTextSearchOptions _options;

        public ConfigController(
            IOptions<FullTextSearchOptions> options
            )
        {
            _options = options.Value;
        }

        [HttpGet]
        public FullTextSearchOptions Get()
        {
            return _options;
        }
    }
}

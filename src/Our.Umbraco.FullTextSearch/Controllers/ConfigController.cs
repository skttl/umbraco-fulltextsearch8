using Our.Umbraco.FullTextSearch.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Umbraco.Core.Logging;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Our.Umbraco.FullTextSearch.Controllers
{
    [PluginController("FullTextSearch")]
    public class ConfigController : UmbracoAuthorizedApiController
    {
        private readonly IFullTextSearchConfig _fullTextConfig;
        private readonly ILogger _logger;

        public ConfigController(
            IFullTextSearchConfig fullTextSearchConfig,
            ILogger logger)
        {
            _fullTextConfig = fullTextSearchConfig;
            _logger = logger;
        }

        [HttpPost]
        public IFullTextSearchConfig Reload()
        {
            _fullTextConfig.LoadAndParseConfig();
            return _fullTextConfig;
        }

        [HttpGet]
        public IFullTextSearchConfig Get()
        {
            return _fullTextConfig;
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Web.Common.Authorization;
using Umbraco.Cms.Web.Common.Routing;

namespace Our.Umbraco.FullTextSearch.Controllers;

[ApiController]
[BackOfficeRoute("fulltextsearch/api/v{version:apiVersion}/fulltextsearch")]
[Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
[MapToApi("fulltextsearch")]
public class FullTextSearchControllerBase
{

}

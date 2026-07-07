using Umbraco.Cms.Api.Common.OpenApi;
using Umbraco.Cms.Api.Management.OpenApi;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Our.Umbraco.FullTextSearch;

public class Composer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.AddFullTextSearch();
        builder.AddBackOfficeOpenApiDocument(
            "fulltextsearch",
            document => document
                .WithTitle("Full Text Search Api")
                .WithBackOfficeAuthentication()
                .WithJsonOptions(Constants.JsonOptionsNames.BackOffice)
        );
    }
}

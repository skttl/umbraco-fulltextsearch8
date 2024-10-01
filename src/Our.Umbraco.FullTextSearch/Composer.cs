using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Our.Umbraco.FullTextSearch;

public class Composer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.AddFullTextSearch();
        builder.Services.ConfigureOptions<ConfigureSwaggerGenOptions>();
    }
}

internal class ConfigureSwaggerGenOptions : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        options.SwaggerDoc(
            "fulltextsearch",
            new OpenApiInfo
            {
                Title = "Full Text Search Api",
                Version = "Latest",
                Description = "API for working with Full Text Search"
            });
    }
}
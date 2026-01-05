using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Manifest;
using Umbraco.Cms.Infrastructure.Manifest;

namespace Our.Umbraco.FullTextSearch;

public class ManifestReaderComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddSingleton<IPackageManifestReader, ManifestReader>();
    }
}

public class ManifestReader(IFileVersionProvider fileVersionProvider) : IPackageManifestReader
{
    public Task<IEnumerable<PackageManifest>> ReadPackageManifestsAsync()
    {
        var script = $"/App_Plugins/FullTextSearch/assets.js";
        var versionedScript = fileVersionProvider.AddFileVersionToPath(
            new PathString(script),
            script
        );
        var version = GetVersion();

        List<PackageManifest> manifest =
        [
            new PackageManifest
            {
                Id = "Our.Umbraco.FullTextSearch.Backoffice",
                Name = "Our.Umbraco.FullTextSearch.Backoffice",
                AllowTelemetry = true,
                Version = version,
                Extensions =
                [
                    new JsonObject
                    {
                        ["name"] = "Full Text Search Backoffice Bundle",
                        ["alias"] = "Our.Umbraco.FullTextSearch.Backoffice.Bundle",
                        ["type"] = "bundle",
                        ["js"] = versionedScript,
                    },
                ],
            },
        ];

        return Task.FromResult(manifest.AsEnumerable());
    }

    private string GetVersion()
    {
        var assembly = typeof(ManifestReader).Assembly;
        return assembly.GetName()?.Version?.ToString() ?? Guid.NewGuid().ToString("N");
    }
}

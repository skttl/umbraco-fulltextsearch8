/* TODO: Replace with umbraco-package.json
using System.Collections.Generic;
using Umbraco.Cms.Core.Manifest;

namespace Our.Umbraco.FullTextSearch
{
    public class FullTextSearchManifestFilter : IManifestFilter
    {
        public void Filter(List<PackageManifest> manifests)
        {
            manifests.Add(new PackageManifest()
            {
                AllowPackageTelemetry = true,
                PackageName = "Full Text Search",
                Scripts = new[]
                {
                    "/App_Plugins/FullTextSearch/scripts/dashboard.controller.js",
                    "/App_Plugins/FullTextSearch/scripts/reindex.controller.js",
                    "/App_Plugins/FullTextSearch/scripts/search.controller.js",
                    "/App_Plugins/FullTextSearch/scripts/searchAdvancedSettings.controller.js",
                    "/App_Plugins/FullTextSearch/scripts/status.controller.js",
                    "/App_Plugins/FullTextSearch/scripts/statusnodes.controller.js",
                    "/App_Plugins/FullTextSearch/scripts/fulltextsearch.resource.js"
                },
                Stylesheets = new[]
                {
                    "/App_Plugins/FullTextSearch/css/fulltextsearch.css"
                }

            });
        }
    }
}
*/
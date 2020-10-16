angular.module("umbraco.resources").factory("Our.Umbraco.FullTextSearch.Resource",
    function ($http, umbRequestHelper, localizationService) {
        return {
            localizeKeys: function (objKeys) {
                var keys = Object.keys(objKeys);

                localizationService.localizeMany(keys).then(function (data) {
                    for (var i = 0; i < data.length; i++) {
                        if (data[i] !== "[" + keys[i] + "]") { // only update value, if a value is found
                            objKeys[keys[i]] = data[i];
                        }
                    }
                });

                return objKeys;
            },
            reindexNodes: function (nodeIds, includeDescendants) {
                if (!nodeIds) {
                    nodeIds = "*"
                }

                if (nodeIds != "*" && includeDescendants) {
                    nodeIds + "&includeDescendants=true";
                }

                var url = Umbraco.Sys.ServerVariables.umbracoSettings.umbracoPath + "/backoffice/FullTextSearch/Index/ReindexNodes?nodeIds=" + nodeIds;
                return umbRequestHelper.resourcePromise(
                    $http.post(url),
                    "Failed to reindex nodes"
                );
            },
            getIndexStatus: function () {
                var url = Umbraco.Sys.ServerVariables.umbracoSettings.umbracoPath + "/backoffice/FullTextSearch/Index/GetIndexStatus";
                return umbRequestHelper.resourcePromise(
                    $http.get(url),
                    "Failed getting index status"
                );
            },
            getNodes: function (endpoint, pageNumber) {
                var url = Umbraco.Sys.ServerVariables.umbracoSettings.umbracoPath + "/backoffice/FullTextSearch/Index/" + endpoint + "?pageNumber=" + (pageNumber ?? 1);
                return umbRequestHelper.resourcePromise(
                    $http.get(url),
                    "Failed getting node data"
                );
            },
            getSearchSettings: function () {
                var url = Umbraco.Sys.ServerVariables.umbracoSettings.umbracoPath + "/backoffice/FullTextSearch/Index/GetSearchSettings";
                return umbRequestHelper.resourcePromise(
                    $http.get(url),
                    "Failed getting search settings"
                );
            },
            getSearchResult: function (searchTerms, advancedSettings, pageNumber) {
                var url = Umbraco.Sys.ServerVariables.umbracoSettings.umbracoPath + "/backoffice/FullTextSearch/Index/GetSearchResult";
                return umbRequestHelper.resourcePromise(
                    $http.post(url, { searchTerms, advancedSettings, pageNumber }),
                    "Failed getting search result"
                );
            }
        };
    });
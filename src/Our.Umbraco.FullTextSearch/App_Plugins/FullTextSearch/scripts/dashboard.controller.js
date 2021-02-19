angular.module("umbraco")
    .controller("Our.Umbraco.FullTextSearch.Dashboard.Controller",
        [
            "Our.Umbraco.FullTextSearch.Resource",
            function (fullTextSearchResource) {

                var vm = this;

                /*
                 * Localize keys
                 */
                vm.dictionaryKeys = fullTextSearchResource.localizeKeys({
                    fullTextSearch_status: "Status",
                    fullTextSearch_reindex: "Reindex",
                    fullTextSearch_search: "Search",
                });

                vm.version = Umbraco.Sys.ServerVariables.FullTextSearch.Version;

                vm.content = {
                    navigation: [
                        {
                            "name": vm.dictionaryKeys.fullTextSearch_status,
                            "alias": "status",
                            "icon": "icon-temperatrure-alt",
                            "view": "/App_Plugins/FullTextSearch/views/status.html",
                            "active": true
                        },
                        {
                            "name": vm.dictionaryKeys.fullTextSearch_reindex,
                            "alias": "reindex",
                            "icon": "icon-inbox",
                            "view": "/App_Plugins/FullTextSearch/views/reindex.html",
                        },
                        {
                            "name": vm.dictionaryKeys.fullTextSearch_search,
                            "alias": "search",
                            "icon": "icon-search",
                            "view": "/App_Plugins/FullTextSearch/views/search.html",
                        }
                    ]
                };
            }
        ]);

angular.module("umbraco")
    .controller("Our.Umbraco.FullTextSearch.Dashboard.SearchController",
        [
            "Our.Umbraco.FullTextSearch.Resource",
            "editorService",
            function (fullTextSearchResource, editorService) {

                var vm = this;

                /*
                 * Localize keys
                 */
                vm.dictionaryKeys = fullTextSearchResource.localizeKeys({
                    fullTextSearch_enterKeywordsHere: "Enter keywords here",
                    fullTextSearch_advancedSettings: "Advanced settings"
                });

                vm.search = {
                    loading: false,
                    items: [],
                    currentPage: 1,
                    totalPages: 1,
                    inspect: function (item) {
                        var options = {
                            title: item.name,
                            view: "/App_Plugins/FullTextSearch/views/search/inspectNode.html",
                            size: "medium",
                            allFields: item.allFields,
                            close: function () {
                                editorService.close();
                            }
                        }
                        editorService.open(options);
                    },
                    getData: function (pageNumber) {
                        fullTextSearchResource.getSearchResult(vm.search.searchTermsCopy, vm.search.advancedSettings, pageNumber ?? 1).then(
                            function (data) {
                                vm.search.items = data.items;
                                vm.search.currentPage = data.pageNumber;
                                vm.search.totalPages = data.totalPages;
                                vm.search.loading = false;
                            }
                        );
                    },
                    performSearch: function () {
                        if (vm.search.searchTerms != "") {
                            vm.search.searchTermsCopy = vm.search.searchTerms;
                            vm.search.getData(1);
                        }
                    },
                    searchTerms: "",
                    defaultSettings: {},
                    advancedSettings: null,
                    options: {
                        fieldNames: {},
                        cultures: {}
                    },
                    showAdvancedSettings: function () {
                        if (vm.search.advancedSettings == null) {
                            vm.search.initAdvancedSettings();
                        }
                        var options = {
                            title: vm.dictionaryKeys.fullTextSearch_advancedSettings,
                            view: "/App_Plugins/FullTextSearch/views/search/advancedSettings.html",
                            size: "small",
                            advancedSettings: vm.search.advancedSettings,
                            options: vm.search.options,
                            submit: function (model) {
                                vm.search.advancedSettings = model.advancedSettings;
                                editorService.close();
                            },
                            close: function () {
                                editorService.close();
                            }
                        };
                        editorService.open(options);
                    },
                    toggleAdvancedSettings: function () {
                        if (vm.search.advancedSettings == null) {
                            vm.search.showAdvancedSettings();
                        }
                        else {
                            vm.search.resetAdvancedSettings();
                        }
                    },
                    initAdvancedSettings: function () {
                        vm.search.advancedSettings = JSON.parse(JSON.stringify(vm.search.defaultSettings))
                    },
                    resetAdvancedSettings: function () {
                        vm.search.advancedSettings = null;
                    }
                }

                fullTextSearchResource.getSearchSettings().then(function (data) {
                    vm.search.defaultSettings = data.defaultSettings;
                    data.cultures.map((culture, i) => {
                        vm.search.options.cultures[culture] = {
                            value: culture,
                            sortOrder: i
                        };
                    });
                });
            }
        ]);

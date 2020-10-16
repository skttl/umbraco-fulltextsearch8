angular.module("umbraco")
    .controller("Our.Umbraco.FullTextSearch.Dashboard.StatusController",
        [
            "$timeout",
            "Our.Umbraco.FullTextSearch.Resource",
            "listViewHelper",
            "editorService",
            function ($timeout, fullTextSearchResource, listViewHelper, editorService) {

                var vm = this;

                /*
                 * Localize keys
                 */
                vm.dictionaryKeys = fullTextSearchResource.localizeKeys({
                    fullTextSearch_includeDescendants: "Include descendants",
                    fullTextSearch_allNodes: "All nodes",
                    fullTextSearch_selectedNodes: "Selected nodes",
                    fullTextSearch_description: "Description",
                    fullTextSearch_indexedNodes: "Indexed nodes",
                    fullTextSearch_missingNodes: "Missing nodes",
                    fullTextSearch_incorrectIndexedNodes: "Incorrect indexed nodes"
                });

                /*
                 * Index status
                 */

                vm.indexStatus = {
                    buttonState: "init",
                    loading: true,
                    status: {},
                    load: function () {
                        vm.indexStatus.loading = true;
                        vm.indexStatus.buttonState = "busy";
                        fullTextSearchResource.getIndexStatus().then(
                            function (data) {
                                vm.indexStatus.status = data;
                                vm.indexStatus.loading = false;
                                vm.indexStatus.buttonState = "success";
                            }
                        );
                    }
                }
                vm.indexStatus.load();


                /*
                 * List view
                 */

                vm.listView = {
                    show: false,
                    items: [],
                    selection: [],
                    options: {
                        includeProperties: [
                            { alias: "description", header: vm.dictionaryKeys.fullTextSearch_description }
                        ],
                    },
                    methods: {
                        selectItem: function (selectedItem, $index, $event) {
                            listViewHelper.selectHandler(selectedItem, $index, vm.listView.items, vm.listView.selection, $event);
                        },
                        selectAll: function ($event) {
                            listViewHelper.selectAllItemsToggle(vm.listView.items, vm.listView.selection);
                        },
                        isSelectedAll: function () {
                            return listViewHelper.isSelectedAll(vm.listView.items, vm.listView.selection);
                        },
                        clickItem: function (item) {
                            var editor = {
                                id: item.id,
                                submit: function (model) {
                                    editorService.close();
                                },
                                close: function (model) {
                                    editorService.close();
                                },
                                allowSaveAndClose: true,
                                allowPublishAndClose: true
                            };
                            editorService.contentEditor(editor);
                        },
                        clearSelection: function () {
                            vm.listView.selection = [];
                        }
                    },

                    defaultButton: {
                        labelKey: "fullTextSearch_reindex",
                        handler: function () {
                            vm.listView.actionInProgress = true;
                            fullTextSearchResource.reindexNodes(vm.listView.selection.map(s => s.id).join(",")).then(
                                function (response) {
                                    $timeout(() => {
                                        vm.listView.getData(vm.listView.currentPage);
                                        vm.listView.methods.clearSelection();
                                        vm.indexStatus.load();
                                        vm.listView.actionInProgress = false;
                                    }, 500);
                                });
                        }
                    },
                    subButtons: [
                        {
                            labelKey: "fullTextSearch_reindexWithDescendants",
                            handler: function () {
                                vm.listView.actionInProgress = true;
                                fullTextSearchResource.reindexNodes(vm.listView.selection.map(s => s.id).join(","), true).then(
                                    function (response) {
                                        $timeout(() => {
                                            vm.listView.getData(vm.listView.currentPage);
                                            vm.listView.methods.clearSelection();
                                            vm.indexStatus.load();
                                            vm.listView.actionInProgress = false;
                                        }, 500);
                                    });
                            }
                        }
                    ],
                    getIndexedNodes: function () {
                        vm.listView.show = true;
                        vm.listView.headerKey = "indexedNodes";
                        vm.listView.getData(1);
                        vm.listView.openEditor();
                    },
                    getMissingNodes: function () {
                        vm.listView.show = true;
                        vm.listView.headerKey = "missingNodes";
                        vm.listView.getData(1);
                        vm.listView.openEditor();
                    },
                    getIncorrectIndexedNodes: function () {
                        vm.listView.show = true;
                        vm.listView.headerKey = "incorrectIndexedNodes";
                        vm.listView.getData(1);
                        vm.listView.openEditor();
                    },
                    openEditor: function () {
                        vm.listView.methods.clearSelection();
                        var options = {
                            title: vm.dictionaryKeys["fullTextSearch_" + vm.listView.headerKey],
                            view: "/App_Plugins/FullTextSearch/views/status/nodes.html",
                            listView: vm.listView,
                            submit: function (model) {
                                vm.listView.methods.clearSelection();
                                editorService.close();
                            },
                            close: function () {
                                vm.listView.methods.clearSelection();
                                editorService.close();
                            }
                        };
                        editorService.open(options);
                    },
                    getData: function (pageNumber) {
                        var endpoint = "";
                        if (vm.listView.headerKey == "missingNodes") {
                            endpoint = "GetMissingNodes";
                        }
                        else if (vm.listView.headerKey == "incorrectIndexedNodes") {
                            endpoint = "GetIncorrectIndexedNodes";
                        }
                        else if (vm.listView.headerKey == "indexedNodes") {
                            endpoint = "GetIndexedNodes";
                        }

                        vm.listView.loading = true;
                        fullTextSearchResource.getNodes(endpoint, pageNumber ?? 1).then(
                            function (data) {
                                vm.listView.items = data.Items;
                                vm.listView.currentPage = data.PageNumber;
                                vm.listView.totalPages = data.TotalPages;
                                vm.listView.loading = false;
                            }
                        );
                    }
                }
            }
        ]);

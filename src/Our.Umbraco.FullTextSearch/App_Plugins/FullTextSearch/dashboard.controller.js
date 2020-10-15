angular.module("umbraco")
    .controller("Our.Umbraco.FullTextSearch.Dashboard.Controller",
        function ($scope, $http, $timeout, listViewHelper, localizationService) {

            var vm = this;

            /*
             * Localize keys
             */

            vm.dictionaryKeys = {
                fullTextSearch_includeDescendants: "Include descendants",
                fullTextSearch_allNodes: "All nodes",
                fullTextSearch_selectedNodes: "Selected nodes",
                fullTextSearch_description: "Description",
            };

            var keys = Object.keys(vm.dictionaryKeys);

            localizationService.localizeMany(keys).then(function (data) {
                console.log(data);
                for (var i = 0; i < data.length; i++) {
                    vm.dictionaryKeys[keys[i]] = data[i];
                }
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
                    $http.get('/umbraco/backoffice/FullTextSearch/Index/GetIndexStatus').then(
                        function (response) {
                            vm.indexStatus.status = response.data;
                            vm.indexStatus.loading = false;
                            vm.indexStatus.buttonState = "success";
                        }
                    );
                }
            }
            vm.indexStatus.load();

            /*
             * Reindex
             */
            vm.reindexNodes = {
                includeDescendants: false,
                toggleIncludeDescendants: function () {
                    vm.reindexNodes.includeDescendants = !vm.reindexNodes.includeDescendants;
                },
                pickNodes: false,
                pickedNodes:
                {
                    label: 'Nodes',
                    hideLabel: true,
                    editor: "Umbraco.MultiNodeTreePicker",
                    view: 'contentPicker',
                    config: {
                        ignoreUserStartNodes: false,
                        maxNumber: 0,
                        minNumber: 0,
                        multiPicker: true,
                        showOpenButton: false,
                        startNode: null
                    },
                    value: ""
                },
                togglePickNodes: function () {
                    vm.reindexNodes.includeDescendants = false;
                    if (vm.reindexNodes.pickNodes) {
                        vm.reindexNodes.pickedNodes.value = "";
                        vm.reindexNodes.pickNodes = false;
                    }
                    else {
                        vm.reindexNodes.pickNodes = true;
                    }
                },
                buttonState: "init",
                reindex: function () {
                    vm.reindexNodes.buttonState = "busy";
                    var nodeIds = vm.reindexNodes.pickNodes ? vm.reindexNodes.pickedNodes.value : "*";

                    reindexNodes(nodeIds, vm.reindexNodes.includeDescendants).then(
                        function (response) {
                            vm.reindexNodes.buttonState = "success";
                        }
                    );
                }
            }


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
                        // Todo: editor service
                    },
                    clearSelection: function () {
                        vm.listView.selection = [];
                    }
                },

                defaultButton: {
                    labelKey: "fullTextSearch_reindex",
                    handler: function () {
                        vm.listView.actionInProgress = true;
                        rebuildCache(vm.listView.selection.map(s => s.id).join(",")).then(
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
                        label: "fullTextSearch_reindexWithDescendants",
                        handler: function () {
                            vm.listView.actionInProgress = true;
                            rebuildCache(vm.listView.selection.map(s => s.id).join(","), true).then(
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
                reload: function() {
                },
                getMissingNodes: function () {
                    vm.listView.show = true;
                    vm.listView.headerKey = "missingNodes";
                    vm.listView.getData(1);
                },
                getIncorrectIndexedNodes: function () {
                    vm.listView.show = true;
                    vm.listView.headerKey = "incorrectIndexedNodes";
                    vm.listView.getData(1);
                },
                getData: function (pageNumber) {
                    var endpoint = "";
                    if (vm.listView.headerKey == "missingModes") {
                        endpoint = "GetMissingNodes";
                    }
                    else if (vm.listView.headerKey == "incorrectIndexedNodes") {
                        endpoint = "GetIncorrectIndexedNodes";
                    }

                    vm.listView.loading = true;
                    $http.get("/umbraco/backoffice/FullTextSearch/Index/" + endpoint + "?pageNumber=" + (pageNumber ?? 1)).then(
                        function (response) {
                            vm.listView.items = response.data.Items;
                            vm.listView.currentPage = response.data.PageNumber;
                            vm.listView.totalPages = response.data.TotalPages;
                            vm.listView.loading = false;
                        }
                    );
                }
            }

            var reindexNodes = function (nodeIds, includeDescendants) {
                if (!nodeIds) {
                    nodeIds = "*"
                }

                if (nodeIds != "*" && includeDescendants) {
                    nodeIds + "&includeDescendants=true";
                }

                return $http.post('/umbraco/backoffice/FullTextSearch/Index/ReindexNodes?nodeIds=' + nodeIds);
            }
        });

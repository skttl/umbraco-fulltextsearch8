angular.module("umbraco")
    .controller("Our.Umbraco.FullTextSearch.Dashboard.ReindexController",
        [
            "Our.Umbraco.FullTextSearch.Resource",
            function (fullTextSearchResource) {

                var vm = this;

                /*
                 * Localize keys
                 */
                vm.dictionaryKeys = fullTextSearchResource.localizeKeys({
                    fullTextSearch_includeDescendants: "Include descendants",
                    fullTextSearch_allNodes: "All nodes",
                    fullTextSearch_selectedNodes: "Selected nodes",
                });

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
                        label: "Nodes",
                        hideLabel: true,
                        editor: "Umbraco.MultiNodeTreePicker",
                        view: "contentPicker",
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

                        fullTextSearchResource.reindexNodes(nodeIds, vm.reindexNodes.includeDescendants).then(
                            function (response) {
                                vm.reindexNodes.buttonState = "success";
                            }
                        );
                    }
                }
            }
        ]);

angular.module("umbraco")
    .controller("Our.Umbraco.FullTextSearch.Dashboard.Controller",
        function ($scope, $http) {

            var vm = this;

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
             * Cache handling
             */
            vm.rebuildCache = {
                includeDescendants: false,
                toggleIncludeDescendants: function () {
                    vm.rebuildCache.includeDescendants = !vm.rebuildCache.includeDescendants;
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
                    vm.rebuildCache.includeDescendants = false;
                    if (vm.rebuildCache.pickNodes) {
                        vm.rebuildCache.pickedNodes.value = "";
                        vm.rebuildCache.pickNodes = false;
                    }
                    else {
                        vm.rebuildCache.pickNodes = true;
                    }
                },
                buttonState: "init",
                rebuild: function () {
                    vm.rebuildCache.buttonState = "busy";
                    var nodeIds = vm.rebuildCache.pickNodes ? vm.rebuildCache.pickedNodes.value : "*";
                    if (vm.rebuildCache.includeDescendants && nodeIds != "*") {
                        nodeIds += "&includeDescendants=true"
                    }

                    $http.post('/umbraco/backoffice/FullTextSearch/Index/RebuildCache?nodeIds=' + nodeIds).then(
                        function (response) {
                            vm.rebuildCache.buttonState = "success";
                        }
                    );
                }
            }

        });

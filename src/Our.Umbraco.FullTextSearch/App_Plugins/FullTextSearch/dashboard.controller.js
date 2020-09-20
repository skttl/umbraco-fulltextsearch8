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
                buttonState: "init",
                rebuild: function () {
                    vm.rebuildCache.buttonState = "busy";

                    $http.post('/umbraco/backoffice/FullTextSearch/Index/RebuildCache?nodeIds=*').then(
                        function (response) {
                            vm.rebuildCache.buttonState = "success";
                        }
                    );
                }
            }

        });

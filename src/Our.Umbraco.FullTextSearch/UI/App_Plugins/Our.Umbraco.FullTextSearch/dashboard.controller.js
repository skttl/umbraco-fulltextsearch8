angular.module("umbraco")
    .controller("Our.Umbraco.FullTextSearch.Dashboard.Controller",
        function ($scope, $http) {

            var vm = this;

            vm.loadingCacheTask = 0;
            vm.loadingCacheTasks = false;
            vm.cacheTasks = [];

            vm.reIndexNodes = function () {
                $http.post('/umbraco/backoffice/fulltextsearch/index/reindexnode?nodeIds=*').then(
                    function (response) {
                        console.log(response);
                    }
                );
            };

            vm.getCacheTasks = function () {
                vm.loadingCacheTasks = true;
                $http.get('/umbraco/backoffice/fulltextsearch/Index/GetCacheTasks').then(
                    function (response) {
                        vm.cacheTasks = response.data;
                        vm.loadingCacheTasks = false;
                    }
                );
            };

            vm.restartCacheTask = function (taskId, nodeId) {
                vm.loadingCacheTask = taskId;
                $http.post('/umbraco/backoffice/fulltextsearch/index/restartcachetask?taskId=' + taskId + '&nodeId=' + nodeId).then(
                    function (response) {
                        vm.getCacheTasks();
                        vm.loadingCacheTask = 0;
                    }
                );
            };

            vm.deleteCacheTask = function (taskId) {
                vm.loadingCacheTask = taskId;
                $http.post('/umbraco/backoffice/fulltextsearch/index/deletecachetask?taskId=' + taskId).then(
                    function (response) {
                        vm.cacheTasks = vm.cacheTasks.filter(function (task) { return task.Id != taskId; });
                        vm.loadingCacheTask = 0;
                    }
                );
            };

            vm.getCacheTasks();
        });

angular.module("umbraco")
    .controller("Our.Umbraco.FullTextSearch.Dashboard.Controller",
        function ($scope, $http) {

            var vm = this;

            vm.reIndexNodes = function () {
                $http.post('/umbraco/backoffice/fulltextsearch/index/reindexnode?nodeIds=*').then(
                    function (response) {

                    }
                );
            };

        });

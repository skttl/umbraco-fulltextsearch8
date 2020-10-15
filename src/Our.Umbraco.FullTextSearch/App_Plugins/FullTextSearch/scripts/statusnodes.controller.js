angular.module("umbraco")
    .controller("Our.Umbraco.FullTextSearch.Dashboard.StatusNodesController",

        function ($scope) {

            var vm = this;

            vm.submit = submit;
            vm.close = close;

            function submit() {
                if ($scope.model.submit) {
                    $scope.model.submit($scope.model);
                }
            }

            function close() {
                if ($scope.model.close) {
                    $scope.model.close();
                }
            }
        }
    );

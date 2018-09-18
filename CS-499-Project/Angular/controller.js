(function () {
    'use strict';

    angular
        .module('app')
        .controller('controller', controller);

    controller.$inject = ['$scope'];

    function controller($scope) {
        $scope.title = 'controller';
        $scope.accounts = "something";
        $scope.customer = "something";
        $scope.account_types = "something";

        activate();

        function activate() { }
    }
})();

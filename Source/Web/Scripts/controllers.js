/// <reference path="_references.ts" />
angular.module('app.controllers', []).controller('HomeCtrl', function () {
}).controller('LoginCtrl', [
    '$scope',
    '$rootScope',
    '$http',
    '$state',
    '$location',
    function ($scope, $rootScope, $http, $state, $location) {
        //console.dir($location.$$search.auth);
        $scope.user = {};
        $scope.modelState = $location.$$search.auth == 'failed' ? { '': ['Failed to login via external authentication provider.'] } : {};

        $scope.login = function (user) {
            $http.post('/api/login', user).success(function (data, status) {
                if (status == 200) {
                    $rootScope.user = data;
                    $state.transitionTo('main.home');
                } else {
                    $scope.modelState = data.modelState;
                }
            }).error(function (data, status) {
                if (status == 400) {
                    // TODO: Set error message
                }
            });
        };

        $scope.externalLogin = function (provider) {
            var form = document.createElement('form');
            form.method = 'POST';
            form.action = '/api/externallogin/' + provider;
            document.body.appendChild(form);
            form.submit();
        };

        $scope.hasErrors = function () {
            for (var key in $scope.modelState) {
                if ($scope.modelState.hasOwnProperty(key)) {
                    return true;
                }
            }
            return false;
        };

        $scope.getErrors = function () {
            var key, errors = [];
            for (key in $scope.modelState) {
                if ($scope.modelState.hasOwnProperty(key) && key != '') {
                    errors = errors.concat($scope.modelState[key]);
                }
            }
            return errors;
        };
    }
]).controller('RegisterCtrl', [
    '$scope',
    '$rootScope',
    '$http',
    '$state',
    function ($scope, $rootScope, $http, $state) {
        $scope.user = {};
        $scope.modelState = {};

        $scope.register = function (user) {
            $http.post('/api/register', user).success(function (data, status) {
                if (status == 201) {
                    $rootScope.user = data;
                    $state.transitionTo('main.home');
                } else {
                    $scope.modelState = data.modelState;
                }
            }).error(function (data, status) {
                if (status == 400) {
                    $scope.modelState = data.modelState;
                } else {
                    $scope.modelState = { '': [data.message ? data.message : 'Internal server error.'] };
                }
            });
        };

        $scope.hasErrors = function () {
            for (var key in $scope.modelState) {
                if ($scope.modelState.hasOwnProperty(key)) {
                    return true;
                }
            }
            return false;
        };

        $scope.getErrors = function () {
            var key, errors = [];
            for (key in $scope.modelState) {
                if ($scope.modelState.hasOwnProperty(key) && key != '') {
                    errors = errors.concat($scope.modelState[key]);
                }
            }
            return errors;
        };
    }
]);
//# sourceMappingURL=controllers.js.map

/// <reference path="_references.ts" />

angular.module('app.controllers', [])

    .controller('HomeCtrl', () => {})

    .controller('LoginCtrl', ['$scope', '$rootScope', '$http', '$state', '$location', ($scope, $rootScope, $http: ng.IHttpService, $state, $location) => {

        console.dir($location.$$search.auth);

        $scope.user = {};
        $scope.modelState = $location.$$search.auth == 'failed' ? { '': ['Failed to login via external authentication provider.'] } : {};

        $scope.login = (user) => {
            $http.post('/api/login', user)
                .success((data, status: number) => {
                    if (status == 200) {
                        $rootScope.user = data;
                        $state.transitionTo('main.home');
                    } else {
                        $scope.modelState = data.modelState;
                    }
                })
                .error((data, status) => {
                    if (status == 400) {
                        $scope.modelState = data.modelState;
                    }
                });
        };

        $scope.externalLogin = (provider) => {
            var form = <HTMLFormElement>document.createElement('form');
            form.method = 'POST';
            form.action = '/api/auth/' + provider;
            document.body.appendChild(form);
            form.submit();
        };

        $scope.hasErrors = () => {
            for (var key in $scope.modelState) {
                if ($scope.modelState.hasOwnProperty(key)) {
                    return true;
                }
            }
            return false;
        };

        $scope.getErrors = () => {
            var key, errors = [];
            for (key in $scope.modelState) {
                if ($scope.modelState.hasOwnProperty(key) && key != '') {
                    errors = errors.concat($scope.modelState[key]);
                }
            }
            return errors;
        };
    }])

    .controller('RegisterCtrl', ['$scope', '$rootScope', '$http', '$state', ($scope, $rootScope, $http: ng.IHttpService, $state) => {

        $scope.user = {};
        $scope.modelState = {};

        $scope.register = (user) => {
            $http.post('/api/register', user).success((data, status: number) =>
            {
                if (status == 201) {
                    $rootScope.user = data;
                    $state.transitionTo('main.home');
                } else {
                    $scope.modelState = data.modelState;
                }
            }).error((data, status: number) => {
                if (status == 400) {
                    $scope.modelState = data.modelState;
                } else {
                    $scope.modelState = { '': [data.message ? data.message : 'Internal server error.'] };
                }
            });
        };

        $scope.hasErrors = () => {
            for (var key in $scope.modelState) {
                if ($scope.modelState.hasOwnProperty(key)) {
                    return true;
                }
            }
            return false;
        };

        $scope.getErrors = () => {
            var key, errors = [];
            for (key in $scope.modelState) {
                if ($scope.modelState.hasOwnProperty(key)  && key != '') {
                    errors = errors.concat($scope.modelState[key]);
                }
            }
            return errors;
        };
    }]);
https:
angular.module('http-auth-interceptor', []).provider('authService', function () {
    var buffer = [];
    this.pushToBuffer = function (config, deferred) {
        buffer.push({
            config: config,
            deferred: deferred
        });
    };
    this.$get = [
        '$rootScope', 
        '$injector', 
        function ($rootScope, $injector) {
            var $http;
            function retry(config, deferred) {
                $http = $http || $injector.get('$http');
                $http(config).then(function (response) {
                    deferred.resolve(response);
                });
            }
            function retryAll() {
                for(var i = 0; i < buffer.length; ++i) {
                    retry(buffer[i].config, buffer[i].deferred);
                }
                buffer = [];
            }
            return {
                loginConfirmed: function () {
                    $rootScope.$broadcast('event:auth-loginConfirmed');
                    retryAll();
                }
            };
        }
    ];
}).config([
    '$httpProvider', 
    'authServiceProvider', 
    function ($httpProvider, authServiceProvider) {
        var interceptor = [
            '$rootScope', 
            '$q', 
            function ($rootScope, $q) {
                function success(response) {
                    return response;
                }
                function error(response) {
                    if(response.status === 401) {
                        var deferred = $q.defer();
                        authServiceProvider.pushToBuffer(response.config, deferred);
                        $rootScope.$broadcast('event:auth-loginRequired');
                        return deferred.promise;
                    }
                    return $q.reject(response);
                }
                return function (promise) {
                    return promise.then(success, error);
                };
            }
        ];
        $httpProvider.responseInterceptors.push(interceptor);
    }
])
var HttpAndRegularPromiseTests;
(function (HttpAndRegularPromiseTests) {
    var someController = function ($scope, $http, $q) {
        $http.get("http://somewhere/some/resource").success(function (data) {
            $scope.person = data;
        });
        $http.get("http://somewhere/some/resource").then(function (response) {
            $scope.person = response.data;
        });
        $http.get("http://somewhere/some/resource").then(function (response) {
            $scope.person = response.data;
        });
        var aPromise = $q.when({
            firstName: "Jack",
            lastName: "Sparrow"
        });
        aPromise.then(function (person) {
            $scope.person = person;
        });
        var bPromise = $q.when(42);
        bPromise.then(function (answer) {
            $scope.theAnswer = answer;
        });
        var cPromise = $q.when([
            "a", 
            "b", 
            "c"
        ]);
        cPromise.then(function (letters) {
            $scope.letters = letters;
        });
    };
})(HttpAndRegularPromiseTests || (HttpAndRegularPromiseTests = {}));
var My;
(function (My) {
    })(My || (My = {}));
var mod = angular.module('tests', []);
mod.controller('name', function ($scope) {
});
mod.controller('name', [
    '$scope', 
    function ($scope) {
    }
]);
mod.controller(My.Namespace);
mod.directive('name', function ($scope) {
});
mod.directive('name', [
    '$scope', 
    function ($scope) {
    }
]);
mod.directive(My.Namespace);
mod.factory('name', function ($scope) {
});
mod.factory('name', [
    '$scope', 
    function ($scope) {
    }
]);
mod.factory(My.Namespace);
mod.filter('name', function ($scope) {
});
mod.filter('name', [
    '$scope', 
    function ($scope) {
    }
]);
mod.filter(My.Namespace);
mod.provider('name', function ($scope) {
});
mod.provider('name', [
    '$scope', 
    function ($scope) {
    }
]);
mod.provider(My.Namespace);
mod.service('name', function ($scope) {
});
mod.service('name', [
    '$scope', 
    function ($scope) {
    }
]);
mod.service(My.Namespace);
mod.constant('name', 23);
mod.constant('name', "23");
mod.constant(My.Namespace);
mod.value('name', 23);
mod.value('name', "23");
mod.value(My.Namespace);

angular.module('app', [
    'ui.state', 
    'app.filters', 
    'app.services', 
    'app.directives', 
    'app.controllers'
]).config([
    '$routeProvider', 
    '$locationProvider', 
    '$stateProvider', 
    function ($routeProvider, $locationProvider, $stateProvider) {
        $locationProvider.html5Mode(true);
        $stateProvider.state('main', {
            templateUrl: '/views/layout'
        }).state('shell', {
            template: '<div class="container"><ui-view><ui-view></div>'
        }).state('main.home', {
            url: '/',
            templateUrl: '/views/home'
        }).state('main.about', {
            url: '/about',
            title: 'About Us',
            templateUrl: '/views/about'
        }).state('main.contacts', {
            url: '/contacts',
            title: 'Contacts',
            templateUrl: '/views/contacts'
        }).state('shell.login', {
            url: '/login',
            title: 'Sign In',
            templateUrl: '/views/login',
            controller: 'LoginCtrl'
        }).state('shell.register', {
            url: '/register',
            title: 'Register',
            templateUrl: '/views/register',
            controller: 'RegisterCtrl'
        });
    }]).run([
    '$rootScope', 
    function ($rootScope) {
        $rootScope.$on('$stateChangeSuccess', function (event, current, previous) {
            $rootScope.title = current.title;
        });
    }]);

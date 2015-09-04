'use strict';
/*exported dashboardModule*/

dashboardModule.controller('DashboardController',
['$window',
function ($window) {
    var vm = this;

    vm.myEPAs = [];

    vm.initialize = function () {
    };

    vm.initialize();
}]);
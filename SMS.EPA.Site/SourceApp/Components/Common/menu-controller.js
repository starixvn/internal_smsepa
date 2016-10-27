(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('commonModule').controller('MenuController', menuController);
    menuController.$inject = ['CommonModel'];

    function menuController(CommonModel) {
        var vm = this;

        vm.menus = [];

        CommonModel.getMenusByEmployeeId().then(function () {
            vm.menus = CommonModel.menus;
        });
    }
})(angular);
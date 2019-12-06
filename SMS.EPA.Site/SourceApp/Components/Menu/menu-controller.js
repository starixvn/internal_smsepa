(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('commonModule').controller('MenuController', menuController);
    menuController.$inject = ['CommonModel', 'utilitiesFactory'];

    function menuController(CommonModel, utilitiesFactory) {
        var vm = this;

        vm.menus = [];
        vm.openLink = utilitiesFactory.openLink;

        CommonModel.getMenusByEmployeeId().then(function () {
            vm.menus = CommonModel.menus;
        });
    }
})(angular);
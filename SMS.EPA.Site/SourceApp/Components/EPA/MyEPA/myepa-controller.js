'use strict';
/*global epaModule*/

epaModule.controller('MyEPAController',
['CommonModel',
function (CommonModel) {
    var vm = this;

    vm.statusList = CommonModel.statusList;

    vm.filter = {
        status: ''
    };

    vm.initialize = function () {
        CommonModel.getAllStatus().then(function () {
            vm.statusList = CommonModel.statusList;
        });
    };

    vm.initialize();
}]);
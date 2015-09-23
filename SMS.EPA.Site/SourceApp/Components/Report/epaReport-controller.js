'use strict';
/*global reportModule*/

reportModule.controller('EPAReportController',
['CommonModel',
function (CommonModel) {
    var vm = this;

    vm.treeDepartment = CommonModel.treeDepartment;
    vm.statusList = CommonModel.statusList;

    vm.initialize = function () {
        CommonModel.getDepartment().then(function () {
            vm.treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);
        });

        CommonModel.getAllStatus().then(function () {
            vm.statusList = CommonModel.statusList;
        });
    };

    vm.initialize();
}]);
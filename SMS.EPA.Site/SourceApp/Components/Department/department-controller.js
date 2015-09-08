'use strict';
/*global departmentModule*/

departmentModule.controller('DepartmentController',
['CommonModel', 'utilitiesFactory',
function (CommonModel, utilitiesFactory) {
    var vm = this;

    vm.departments = CommonModel.departments;
    vm.treeDepartment = CommonModel.treeDepartment;

    var resetCurrentData = function () {
        vm.viewMode = false;
        vm.deptSelected = {};
    };

    vm.viewDetails = function (dept) {
        vm.deptSelected = dept;

        if (dept.Manager && dept.Manager.EmployeeId) {
            vm.manager = vm.deptSelected.Manager.NickName + " (" + vm.deptSelected.Manager.FullName + ")";
        }
        else {
            vm.manager = null;
        }

        vm.viewMode = true;
    };

    vm.toggleCollapseExpand = function () {
        if (!vm.hasTreeEvent) {
            $('.tree li.parent_li > span').on('click', function (e) {
                var children = $(this).parent('li.parent_li').find(' > ul > li');
                if (children.is(":visible")) {
                    children.hide('fast');
                    $(this).attr('title', 'Expand this branch').find(' > i').addClass('glyphicon-plus-sign').removeClass('glyphicon-minus-sign');
                } else {
                    children.show('fast');
                    $(this).attr('title', 'Collapse this branch').find(' > i').addClass('glyphicon-minus-sign').removeClass('glyphicon-plus-sign');
                }

                e.stopPropagation();
            });

            vm.hasTreeEvent = true;
        }
    };

    vm.cancel = function () {
        resetCurrentData();
    };

    vm.initialize = function () {
        utilitiesFactory.getDateFormat(vm);
        resetCurrentData();

        CommonModel.getDepartment().then(function () {
            vm.departments = CommonModel.departments;
            vm.treeDepartment = CommonModel.treeDepartment;
        });
    };

    vm.initialize();
}]);
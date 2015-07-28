'use strict';
/*global departmentModule*/

departmentModule.controller('DepartmentController',
['DepartmentModel', 'EmployeeModel', 'CommonModel', 'utilitiesFactory',
function (DepartmentModel, EmployeeModel, CommonModel, utilitiesFactory) {
    var vm = this;

    vm.departments = CommonModel.departments;
    vm.treeDepartment = CommonModel.treeDepartment;

    vm.deptSelected = {};

    vm.hasTreeEvent = false;
    vm.createMode = false;
    vm.editMode = false;
    vm.isDisabled = false;

    utilitiesFactory.getDateFormat(vm);

    /// Search employee auto complete.
    vm.getEmployee = function () {
        return EmployeeModel.getAllEmployee(vm.searchKey);
    };

    vm.unSelectEmployee = function () {
        if (vm.searchKey.trim() === '') {
            if (vm.deptSelected.Manager) {
                vm.deptSelected.Manager = null;
            }
        }
    };

    vm.viewDetails = function (dept) {
        vm.deptSelected = angular.copy(dept);
        vm.searchKey = vm.deptSelected.Manager.NickName + " (" + vm.deptSelected.Manager.FullName + ")";

        vm.createMode = false;
        vm.editMode = true;
        vm.isDisabled = true;
    };

    vm.toggleEditMode = function () {
        vm.isDisabled = false;
    };

    vm.treeEvent = function () {
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

    vm.updateDepartment = function (dept) {
        if (DepartmentModel.validateDepartment(dept)) {
            DepartmentModel.updateDepartment(dept);
        }
    };

    vm.menuOptions = function (dept) {
        return [
            ['Add Child', function () {
                vm.deptSelected = angular.copy(dept);
                vm.createMode = true;
                vm.editMode = false;
            }],
            ['Delete', function () {
            }]
        ];
    };

    vm.initialize = function () {
        CommonModel.getDepartment().then(function () {
            vm.departments = CommonModel.departments;
            vm.treeDepartment = CommonModel.treeDepartment;

            vm.hasTreeEvent = false;
        });
    };

    vm.initialize();
}]);
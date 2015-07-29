'use strict';
/*global departmentModule*/

departmentModule.controller('DepartmentController',
['DepartmentModel', 'EmployeeModel', 'CommonModel', 'utilitiesFactory',
function (DepartmentModel, EmployeeModel, CommonModel, utilitiesFactory) {
    var vm = this;

    vm.departments = CommonModel.departments;
    vm.treeDepartment = CommonModel.treeDepartment;

    var findExclusiveDepartment = function () {
        angular.forEach(vm.departments, function (dept) {
            if (dept.DepartmentId < 0) {
                vm.exclusiveDepartment.push(dept.DepartmentId);
            }
        });
    };

    var resetCurrentData = function () {
        vm.hasTreeEvent = false;
        vm.createMode = false;
        vm.editMode = false;
        vm.isDisabled = false;
        vm.newDepartment = {};
        vm.deptSelected = {};
    };

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
        vm.exclusiveDepartment = [];
        findExclusiveDepartment();
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

    vm.hideChildNode = function (dept) {
        var index = vm.exclusiveDepartment.indexOf(dept.DepartmentId);
        if (index !== -1) {
            return true;
        }

        return false;
    };

    vm.menuOptions = function (dept) {
        return [
            ['Add Child', function () {
                vm.newDepartment = {
                    Code: '',
                    Name: '',
                    ParentId: dept.DepartmentId,
                    Manager: {},
                    IsActive: true
                };
                vm.createMode = true;
                vm.editMode = false;
                vm.searchKey = null;
            }, function () {
                if (dept.DepartmentId < 0) {
                    return false;
                }

                return true;
            }],
            ['Delete', function () {
                DepartmentModel.openPopupDeleteDepartment(vm, dept);
            }, function () {
                if (dept.DepartmentId < 0 || dept.DepartmentId === 1) {
                    return false;
                }

                return true;
            }]
        ];
    };

    vm.createDepartment = function (dept) {
        if (DepartmentModel.validateDepartment(dept)) {
            DepartmentModel.createDepartment(vm, dept);
        }
    };

    vm.updateDepartment = function (dept) {
        if (DepartmentModel.validateDepartment(dept)) {
            DepartmentModel.updateDepartment(vm, dept);
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
'use strict';
/*global departmentModule*/

departmentModule.factory('DepartmentModel', ['departmentFactory', 'errorFactory',
function (departmentFactory, errorFactory) {
    var departmentModel = {};

    departmentModel.departments = [];
    departmentModel.treeDepartment = {};

    departmentModel.getDepartment = function (parentId, isActive) {
        return departmentFactory.getDepartment(parentId, isActive)
        .success(function (data) {
            departmentModel.treeDepartment = data.TreeDepartment;
            departmentModel.departments = data.Departments;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    return departmentModel;
}]);
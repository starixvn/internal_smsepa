(function (angular) {
    'use strict';

    angular.module('departmentModule').factory('DepartmentModel', departmentModel);
    departmentModel.$inject = ['departmentFactory', 'errorFactory'];

    function departmentModel(departmentFactory, errorFactory) {
        var departments = [],
            treeDepartment = {};

        var model = {
            departments: departments,
            getDepartment: getDepartment,
            treeDepartment: treeDepartment
        };

        return model;

        function getDepartment(parentId, isActive) {
            return departmentFactory.getDepartment(parentId, isActive).success(function (data) {
                model.treeDepartment = data.TreeDepartment;
                model.departments = data.Departments;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }
    }
})(angular);
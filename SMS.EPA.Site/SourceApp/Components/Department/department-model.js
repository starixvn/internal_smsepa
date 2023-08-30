(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('departmentModule').factory('DepartmentModel', departmentModel);
    departmentModel.$inject = ['departmentFactory', 'errorFactory'];

    function departmentModel(departmentFactory, errorFactory) {
        let model = {
            departments: [],
            getDepartment: getDepartment
        };

        return model;

        function getDepartment() {
            return departmentFactory.getDepartment().success(function (data) {
                model.departments = data;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }
    }
})(angular);
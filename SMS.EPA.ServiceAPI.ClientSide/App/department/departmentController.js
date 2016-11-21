(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('app').controller('DepartmentController', departmentController);

    departmentController.$inject = ['departmentService'];

    function departmentController(departmentService) {
        var vm = this;

        vm.departmentModel = [{
            DepartmentId: 0,
            Code: '',
            Name: '',
            ParentId: 0,
            ManagerId: 0,
            DepartmentLevel: 0,
            IsDeleted: false,
            ModifiedDate: Date.now()
        }];
        vm.result = {
            code: 0,
            message: ''
        };
        vm.insert = insert;
        vm.title = "Hello Department!";

        function insert() {
            departmentService.insert(vm.departmentModel).success(function (response, status) {
                vm.result.code = status;
                vm.result.message = response.Message;
            }).error(function (response, status) {
                vm.result = {
                    code: status,
                    message: response.Message
                };
            });
        }
    }
})(angular);
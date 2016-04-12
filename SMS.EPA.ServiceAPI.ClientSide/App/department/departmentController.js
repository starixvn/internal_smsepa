(function () {
    'use strict';

    angular
        .module('app')
        .controller('departmentController', DepartmentController)

    DepartmentController.$inject = ['departmentService']

    function DepartmentController(departmentService) {
        var vm = this;
        vm.departmentModel = {
            Id: 0,
            Name: ''
        };
        vm.result = {
            code: 0,
            message: ''
        };
        vm.insert = insert;
        vm.title = "Hello Department!";

        function insert() {
            departmentService.insert(vm.departmentModel)
            .success(function (response, status) {
                vm.result.code = status;
            })
            .error(function (response, status) {
                vm.result = {
                    code: status,
                    message: response.Message
                };
            })
        }
    }
})();

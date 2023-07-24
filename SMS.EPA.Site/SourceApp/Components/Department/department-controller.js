(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('departmentModule').controller('DepartmentController', departmentController);
    departmentController.$inject = ['DepartmentModel', 'utilitiesFactory', 'lockScreenService'];

    function departmentController(DepartmentModel, utilitiesFactory, lockScreenService) {
        var vm = this;

        vm.departments = [];
        vm.treeDepartment = {};

        vm.cancel = cancel;
        vm.toggleCollapseExpand = toggleCollapseExpand;

        initialize();

        function cancel() {
            resetCurrentData();
        }

        function initialize() {
            lockScreenService.start();
            utilitiesFactory.getDateFormat(vm);
            resetCurrentData();

            DepartmentModel.getDepartment().then(function () {
                vm.departments = DepartmentModel.departments;
                vm.treeDepartment = DepartmentModel.treeDepartment;
                lockScreenService.stop();
            });
        }

        function resetCurrentData() {
            vm.viewMode = false;
            vm.deptSelected = {};
        }

        function toggleCollapseExpand() {
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
        }
    }
})(angular);
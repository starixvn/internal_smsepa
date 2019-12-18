(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('targetTemplateModule').controller('TargetTemplateController', targetTemplateController);
    targetTemplateController.$inject = ['$scope', '$location', 'TargetTemplateModel', 'CommonModel', 'utilitiesFactory', 'lockScreenService', 'baseUrl'];

    function targetTemplateController($scope, $location, TargetTemplateModel, CommonModel, utilitiesFactory, lockScreenService, baseUrl) {
        var vm = this;

        vm.targetTypes = [];
        vm.targetTemplates = [];
        vm.permission = null;
        vm.policyEnum = utilitiesFactory.policy;
        vm.departments = CommonModel.departments;
        vm.treeDepartment = CommonModel.treeDepartment;
        vm.filter = {
            departmentId: null,
            isGroup: null,
            targetTypeId: '',
            isActive: 'true'
        };
        vm.currentPage = 1;
        vm.pageSize = 20;
        vm.updateIsGroup = updateIsGroup;

        utilitiesFactory.numberEntryList(vm);
        utilitiesFactory.maxLengthDescription(vm);

        vm.readMore = function (item) {
            utilitiesFactory.readMore(item);
        };

        vm.readLess = function (item) {
            utilitiesFactory.readLess(item);
        };

        function getTargetTemplate(targetTypeId, isActive, departmentId, isGroup) {
            lockScreenService.start();
            TargetTemplateModel.getTargetTemplate(targetTypeId, isActive, departmentId, isGroup).then(function () {
                vm.targetTemplates = TargetTemplateModel.targetTemplates;
                utilitiesFactory.searchOnPage(vm, vm.targetTemplates);
                lockScreenService.stop();
            });
        }

        vm.query = function (filter) {
            var departmentId = null;
            vm.searchKey = '';
            vm.currentPage = 1;

            if (filter.departmentId) {
                departmentId = filter.departmentId < 0 ? -filter.departmentId : filter.departmentId;
            }

            getTargetTemplate(filter.targetTypeId, filter.isActive, departmentId, filter.isGroup);
        };

        vm.getIndex = function (index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        };

        vm.create = function () {
            TargetTemplateModel.openPopupCreateTargetTemplate(vm).then(function () {
                vm.modalInstance = null;
                getTargetTemplate(vm.filter.targetTypeId, vm.filter.isActive, vm.filter.departmentId, vm.filter.isGroup);
            });
        };

        vm.confirmActive = function (targetTemplate) {
            TargetTemplateModel.openPopupUpdateStatusTargetTemplate(targetTemplate);
        };

        vm.update = function (targetTemplate, viewKPIs) {
            if (!viewKPIs) {
                $location.path(baseUrl + 'TargetTemplate/Update/' + targetTemplate.TargetTemplateId).search({ viewInfo: true });
            }
            else {
                $location.path(baseUrl + 'TargetTemplate/Update/' + targetTemplate.TargetTemplateId).search({ viewKPIs: true });
            }
        };

        vm.deleteTargetTemplate = function (targetTemplate) {
            TargetTemplateModel.openPopupDeleteTargetTemplate(vm, targetTemplate);
        };

        vm.detail = function (targetTemplate, event) {
            var url = baseUrl + 'TargetTemplate/Detail/' + targetTemplate.TargetTemplateId;
            utilitiesFactory.handleOpenLink(event, url);
        };

        vm.exportTargetKPIs = function (targetTemplateId) {
            vm.exportCriteria = {
                targetTemplateId: targetTemplateId
            };

            $scope.$broadcast('exportTargetTemplateManagement');
        }

        function initialize() {
            CommonModel.getDepartment(null, true).then(function () {
                vm.departments = CommonModel.departments;
                vm.treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);
            });

            TargetTemplateModel.getPermissionOnTargetTemplate().then(function () {
                vm.permission = TargetTemplateModel.permission;
            });

            CommonModel.getTargetType().then(function () {
                vm.targetTypes = CommonModel.targetTypes;
            });

            getTargetTemplate(vm.filter.targetTypeId, vm.filter.isActive, vm.filter.departmentId, vm.filter.isGroup);
        }

        function updateIsGroup(departmentId) {
            if (departmentId) {
                vm.filter.isGroup = departmentId < 0 ? true : false;
            } else {
                vm.filter.isGroup = null;
            }
        }

        initialize();
    }
})(angular);
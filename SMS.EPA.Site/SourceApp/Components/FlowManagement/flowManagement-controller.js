'use strict';
/*global flowMngModule*/

flowMngModule.controller("FlowTemplateController",
['FlowTemplateModel', 'CommonModel', 'utilitiesFactory', 'enumerationsFactory', 'baseUrl',
function (FlowTemplateModel, CommonModel, utilitiesFactory, enumerationsFactory, baseUrl) {
    var vm = this;

    vm.targetTypes = CommonModel.targetTypes;
    vm.flowTemplates = FlowTemplateModel.flowTemplates;
    vm.permission = FlowTemplateModel.permission;

    vm.policyEnum = enumerationsFactory.policy;

    vm.filter = {
        targetTypeId: '',
        isActive: 'true'
    };

    utilitiesFactory.numberEntryList(vm);
    vm.pageSize = 15;
    vm.currentPage = 1;

    var getFlowTemplate = function (targetTypeId, isActive) {
        FlowTemplateModel.getFlowTemplate(targetTypeId, isActive).then(function () {
            vm.flowTemplates = FlowTemplateModel.flowTemplates;
            utilitiesFactory.searchOnPage(vm, vm.flowTemplates);
        });
    };

    vm.createFlowTemplate = function () {
        vm.createFlowTemplateUrl = baseUrl + 'Flow/Create';
    };

    vm.updateFlowTemplate = function (flowTemplate) {
        vm.updateFlowTemplateUrl = baseUrl + 'Flow/Update/' + flowTemplate.FlowTemplateId;
    };

    vm.updateStatusFlowGroup = function (flowTemplate) {
        FlowTemplateModel.openPopupUpdateStatusFlowTemplate(flowTemplate);
    };

    vm.deleteFlowTemplate = function (flowTemplate) {
        FlowTemplateModel.openPopupDeleteFlowTemplate(vm, flowTemplate);
    };

    vm.query = function (filter) {
        vm.searchKey = '';
        vm.currentPage = 1;
        getFlowTemplate(filter.targetTypeId, filter.isActive);
    };

    vm.initialize = function () {
        FlowTemplateModel.getUserPermissionOnFlowManagement().then(function () {
            vm.permission = FlowTemplateModel.permission;
        });

        CommonModel.getTargetType().then(function () {
            vm.targetTypes = CommonModel.targetTypes;
        });

        getFlowTemplate(vm.filter.targetTypeId, vm.filter.isActive);
    };

    vm.initialize();

    /// Event read more / less text.
    utilitiesFactory.maxLengthDescription(vm);

    vm.readMore = function (item) {
        utilitiesFactory.readMore(item);
    };

    vm.readLess = function (item) {
        utilitiesFactory.readLess(item);
    };

    /// Get index of row in table.
    vm.getIndex = function (index, currentPage, pageSize) {
        return CommonModel.getIndex(index, currentPage, pageSize);
    };
}]);

flowMngModule.controller("CreateFlowTemplateController",
['$location', '$window', 'FlowTemplateModel', 'CommonModel', 'enumerationsFactory', 'baseUrl', 'historyFactory',
function ($location, $window, FlowTemplateModel, CommonModel, enumerationsFactory, baseUrl, historyFactory) {
    var vm = this;

    vm.targetTypes = CommonModel.targetTypes;
    vm.permission = FlowTemplateModel.permission;

    vm.policyEnum = enumerationsFactory.policy;

    vm.flowTemplate = {
        Name: '',
        TargetTypeId: '',
        IsActive: true,
        Description: ''
    };

    vm.initialize = function () {
        FlowTemplateModel.getUserPermissionOnFlowManagement().then(function () {
            vm.permission = FlowTemplateModel.permission;

            if (vm.permission !== vm.policyEnum.Host) {
                $location.path(baseUrl + 'Home/AccessDenied');
            }
        });

        CommonModel.getTargetType(true).then(function () {
            vm.targetTypes = CommonModel.targetTypes;
        });
    };

    vm.initialize();

    vm.save = function (flowTemplate) {
        if (FlowTemplateModel.validateFlowTemplate(flowTemplate)) {
            FlowTemplateModel.createFlowTemplate(flowTemplate);
        }
    };

    vm.cancel = function () {
        historyFactory.back();
    };
}]);

flowMngModule.controller("UpdateFlowTemplateController",
['$window', '$routeParams', 'FlowTemplateModel', 'CommonModel', 'enumerationsFactory', 'cacheService',
function ($window, $routeParams, FlowTemplateModel, CommonModel, enumerationsFactory, cacheService) {
    var vm = this;

    vm.flowTemplateId = parseInt($routeParams.id);
    vm.departments = CommonModel.departments;
    vm.treeDepartment = CommonModel.treeDepartment;
    vm.remindDay = CommonModel.remindDay;
    vm.permission = FlowTemplateModel.permission;

    vm.policyEnum = enumerationsFactory.policy;
    vm.targetTypeEnum = enumerationsFactory.targetType;

    vm.tabDepartmentSelected = function () {
        cacheService.setProperty(1);
    };

    vm.tabPersonalSelected = function () {
        cacheService.setProperty(2);
    };

    vm.tabResultSettingSelected = function () {
        cacheService.setProperty(3);
    };

    vm.initialize = function () {
        FlowTemplateModel.getUserPermissionOnFlowManagement().then(function () {
            vm.permission = FlowTemplateModel.permission;
        });

        CommonModel.getDepartment().then(function () {
            vm.departments = CommonModel.departments;
            vm.treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);
        });

        FlowTemplateModel.getFlowTemplateById(vm.flowTemplateId).then(function () {
            vm.flowTemplate = FlowTemplateModel.flowTemplate;
            vm.flowTemplate.TargetTypeId = FlowTemplateModel.flowTemplate.TargetType.TargetTypeId;
        });
    };

    vm.initialize();
}]);

flowMngModule.controller("GeneralInformationController",
['$window', '$scope', '$routeParams', 'FlowTemplateModel', 'CommonModel', 'historyFactory',
function ($window, $scope, $routeParams, FlowTemplateModel, CommonModel, historyFactory) {
    var vm = this;

    vm.flowTemplateId = parseInt($routeParams.id);
    vm.targetTypes = CommonModel.targetTypes;

    $scope.$watch(function () { return $scope.updateCtrl.flowTemplate; },
        function (newVal) {
            if (newVal) {
                vm.flowTemplate = $scope.updateCtrl.flowTemplate;

                CommonModel.getTargetType(true).then(function () {
                    vm.targetTypes = CommonModel.targetTypes;

                    var index = vm.targetTypes.map(function (e) { return e.TargetTypeId; }).indexOf(vm.flowTemplate.TargetTypeId);
                    if (index === -1) {
                        vm.targetTypes.push(vm.flowTemplate.TargetType);
                    }
                });
            }
        });

    vm.save = function (flowTemplate) {
        if (FlowTemplateModel.validateFlowTemplate(flowTemplate)) {
            FlowTemplateModel.updateFlowTemplate(flowTemplate);
        }
    };

    vm.cancel = function () {
        historyFactory.back();
    };
}]);
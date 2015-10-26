'use strict';
/*global flowMngModule*/

flowMngModule.controller("FlowMngController",
['FlowMngModel', 'CommonModel', 'utilitiesFactory', 'baseUrl',
function (FlowMngModel, CommonModel, utilitiesFactory, baseUrl) {
    var vm = this;

    vm.targetTypes = CommonModel.targetTypes;
    vm.flowTemplates = FlowMngModel.flowTemplates;
    vm.permission = FlowMngModel.permission;

    vm.filter = {
        targetTypeId: '',
        isActive: 'true'
    };

    utilitiesFactory.numberEntryList(vm);
    vm.pageSize = 15;
    vm.currentPage = 1;

    var getFlowTemplate = function (targetTypeId, isActive) {
        FlowMngModel.getFlowTemplate(targetTypeId, isActive).then(function () {
            vm.flowTemplates = FlowMngModel.flowTemplates;
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
        FlowMngModel.openPopupUpdateStatusFlowTemplate(flowTemplate);
    };

    vm.deleteFlowTemplate = function (flowTemplate) {
        FlowMngModel.openPopupDeleteFlowTemplate(vm, flowTemplate);
    };

    vm.query = function (filter) {
        vm.searchKey = '';
        vm.currentPage = 1;
        getFlowTemplate(filter.targetTypeId, filter.isActive);
    };

    vm.initialize = function () {
        FlowMngModel.getUserPermissionOnFlowManagement().then(function () {
            vm.permission = FlowMngModel.permission;
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
['$location', '$window', 'FlowMngModel', 'CommonModel', 'baseUrl',
function ($location, $window, FlowMngModel, CommonModel, baseUrl) {
    var vm = this;

    vm.targetTypes = CommonModel.targetTypes;
    vm.permission = FlowMngModel.permission;

    vm.flowTemplate = {
        Name: '',
        TargetTypeId: '',
        IsActive: true,
        Description: ''
    };

    vm.initialize = function () {
        FlowMngModel.getUserPermissionOnFlowManagement().then(function () {
            vm.permission = FlowMngModel.permission;

            if (vm.permission !== 2) {
                $location.path(baseUrl + 'Home/AccessDenied');
            }
        });

        CommonModel.getTargetType(true).then(function () {
            vm.targetTypes = CommonModel.targetTypes;
        });
    };

    vm.initialize();

    vm.save = function (flowTemplate) {
        if (FlowMngModel.validateFlowTemplate(flowTemplate)) {
            FlowMngModel.createFlowTemplate(flowTemplate);
        }
    };

    vm.cancel = function () {
        $window.history.back();
    };
}]);

flowMngModule.controller("UpdateFlowTemplateController",
['$window', 'FlowMngModel', 'CommonModel', 'cacheService',
function ($window, FlowMngModel, CommonModel, cacheService) {
    var vm = this;

    vm.departments = CommonModel.departments;
    vm.treeDepartment = CommonModel.treeDepartment;
    vm.remindDay = FlowMngModel.remindDay;
    vm.permission = FlowMngModel.permission;

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
        FlowMngModel.getUserPermissionOnFlowManagement().then(function () {
            vm.permission = FlowMngModel.permission;
        });

        CommonModel.getDepartment().then(function () {
            vm.departments = CommonModel.departments;
            vm.treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);
        });
    };

    vm.initialize();
}]);

flowMngModule.controller("GeneralInformationController",
['$window', '$routeParams', 'FlowMngModel', 'CommonModel',
function ($window, $routeParams, FlowMngModel, CommonModel) {
    var vm = this;

    vm.flowTemplateId = parseInt($routeParams.id);
    vm.targetTypes = CommonModel.targetTypes;

    vm.initialize = function () {
        CommonModel.getTargetType(true).then(function () {
            vm.targetTypes = CommonModel.targetTypes;
        });

        FlowMngModel.getFlowTemplateById(vm.flowTemplateId).then(function () {
            vm.flowTemplate = FlowMngModel.flowTemplate;
            vm.flowTemplate.TargetTypeId = FlowMngModel.flowTemplate.TargetType.TargetTypeId;

            var index = vm.targetTypes.map(function (e) { return e.TargetTypeId; }).indexOf(vm.flowTemplate.TargetTypeId);
            if (index === -1) {
                vm.targetTypes.push(vm.flowTemplate.TargetType);
            }
        });
    };

    vm.initialize();

    vm.save = function (flowTemplate) {
        if (FlowMngModel.validateFlowTemplate(flowTemplate)) {
            FlowMngModel.updateFlowTemplate(flowTemplate);
        }
    };

    vm.cancel = function () {
        $window.history.back();
    };
}]);
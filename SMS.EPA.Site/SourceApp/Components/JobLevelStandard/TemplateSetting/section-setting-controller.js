(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('jobLevelStandardModule').controller('SectionTemplateSettingController', controller);
    controller.$inject = ['$scope', 'baseUrl', 'JLSTemplateSettingModel', 'CommonModel', 'utilitiesFactory', 'lockScreenService'];

    function controller($scope, baseUrl, TemplateSettingModel, CommonModel, utilitiesFactory, lockScreenService) {
        let vm = this;

        vm.currentPage = 1;
        vm.filter = {};
        vm.objectType = {
            section: 3
        };
        vm.isFirstLoad = true;
        vm.pageSize = 20;
        vm.selectedAll = false;
        vm.templateSetting = {
            templateId: null,
            sectionIds: ''
        };
        vm.templateSettings = [];
        vm.templates = [];
        vm.assignTargetTemplate = assignTargetTemplate;
        vm.checkAll = checkAll;
        vm.deleteTemplateSetting = deleteTemplateSetting;
        vm.getIndex = getIndex;
        vm.query = query;
        vm.syncSelected = syncSelected;
        vm.exportReport = exportReport;
        vm.orgCharts = [];

        utilitiesFactory.numberEntryList(vm);

        $scope.$watch(function () { return $scope.targetCtrl.dataTab === 1; }, function (newVal) {
            if (newVal && vm.isFirstLoad) {
                initialize();
            }
        });

        function assignTargetTemplate() {
            vm.templateSetting.sectionIds = getSelectedSections(vm.templateSettings);

            TemplateSettingModel.assignTemplate(vm.templateSetting, vm.objectType.section).then(function () {
                vm.query();
                vm.selectedAll = false;
            });
        }

        function checkAll(selectedAll) {
            angular.forEach(vm.filteredItems, function (targetSection) {
                targetSection.Selected = selectedAll;
            });

            angular.forEach(vm.templateSettings, function (targetSection) {
                targetSection.Selected = selectedAll;
            });
        }

        function deleteTemplateSetting(employee) {
            TemplateSettingModel.openPopupDelete(vm, employee);
        }

        function getIndex(index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        }

        function getSelectedSections(arr) {
            let sectionIds = '';
            angular.forEach(arr, function (targetSection) {
                if (targetSection.Selected) {
                    sectionIds += sectionIds ? ',' + targetSection.ObjectId : targetSection.ObjectId;
                }
            });

            return sectionIds;
        }

        function getTemplateSetting(objectIds, showAll) {
            lockScreenService.start();
            TemplateSettingModel.getTemplateSetting(objectIds, vm.objectType.section, showAll)
                .then(function () {
                    vm.templateSettings = TemplateSettingModel.templateSettings;
                    utilitiesFactory.searchOnPage(vm, vm.templateSettings);
                    vm.isFirstLoad = false;
                    lockScreenService.stop();
                });
        }

        function initFilter() {
            vm.filter = {
                objectIds: null,
                showAll: false
            };
        }

        function initialize() {
            initFilter();

            CommonModel.getOrgChartJLSTemplateSetting().then(function () {
                vm.orgCharts = CommonModel.orgChart;
            });

            TemplateSettingModel.getTemplates().then(function () {
                vm.templates = TemplateSettingModel.templates;

                if (vm.templates && vm.templates.length > 0) {
                    vm.templateSetting.templateId = vm.templates[0].TemplateId;
                }
            });

            getTemplateSetting(vm.filter.objectIds, vm.filter.showAll);
        }

        function query() {
            vm.searchKey = '';
            vm.currentPage = 1;

            getTemplateSetting(vm.filter.objectIds, vm.filter.showAll);
        }

        function syncSelected(id, selected) {
            angular.forEach(vm.templateSettings, function (targetSection) {
                if (targetSection.ObjectId === id) {
                    angular.forEach(vm.filteredItems, function (filterItem) {
                        if (targetSection.ObjectId === filterItem.ObjectId) {
                            filterItem.Selected = selected;
                            targetSection.Selected = selected;
                        }
                    });
                }
            });
        }

        function exportReport() {
            vm.exportCriteria = {
                objectIds: vm.filter.objectIds,
                objectTypeId: vm.objectType.section,
                showAll: vm.filter.showAll,
            }

            const url = baseUrl + 'ExportJLSTemplateSetting/ExportSectionTemplateSetting';
            const data = $.param(vm.exportCriteria);

            const downloadUrl = url + '?' + data;
            let iframe = $('body iframe');

            if (iframe.length === 0) {
                iframe = $('<iframe width="0" height="0" class="display-none"></iframe>');
                iframe.appendTo('body');
            }

            iframe.attr('src', downloadUrl);
        }

        vm.onSelectSection = function (objectIds) {
            vm.filter.objectIds = objectIds;
        };
    }
})(angular);
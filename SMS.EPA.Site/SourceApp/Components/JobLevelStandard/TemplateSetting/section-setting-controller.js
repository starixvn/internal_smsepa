(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('jobLevelStandardModule').controller('SectionTemplateSettingController', controller);
    controller.$inject = ['$scope', 'baseUrl', 'JLSTemplateSettingModel',
        'CommonModel', 'utilitiesFactory', 'lockScreenService'];

    function controller($scope, baseUrl, JLSTemplateSettingModel, CommonModel, utilitiesFactory, lockScreenService) {
        var vm = this;

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
        vm.treeDepartment = CommonModel.treeDepartment;
        vm.assignTargetTemplate = assignTargetTemplate;
        vm.checkAll = checkAll;
        vm.deleteTemplateSetting = deleteTemplateSetting;
        vm.getIndex = getIndex;
        vm.query = query;
        vm.syncSelected = syncSelected;
        vm.exportReport = exportReport;
        vm.showDeleteBtn = showDeleteBtn;

        utilitiesFactory.numberEntryList(vm);

        $scope.$watch(function () { return $scope.targetCtrl.dataTab === 2; }, function (newVal) {
            if (newVal && vm.isFirstLoad) {
                initialize();
            }
        });

        function showDeleteBtn(template) {
            return template !== null && template !== "";
        }

        function assignTargetTemplate() {
            vm.templateSetting.sectionIds = getSelectedEmployees(vm.templateSettings);

            JLSTemplateSettingModel.assignTemplateToSection(vm.templateSetting, vm.objectType.section).then(function () {
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
            JLSTemplateSettingModel.openPopupDelete(vm, employee);
        }

        function getIndex(index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        }

        function getSelectedEmployees(arr) {
            let sectionIds = '';
            angular.forEach(arr, function (targetSection) {
                if (targetSection.Selected) {
                    sectionIds += sectionIds ? ',' + targetSection.ObjectId : targetSection.ObjectId;
                }
            });

            return sectionIds;
        }

        function getTemplateSetting(objectId, showAll) {
            lockScreenService.start();
            JLSTemplateSettingModel.getTemplateSetting(objectId, vm.objectType.section, showAll)
                .then(function () {
                    vm.templateSettings = JLSTemplateSettingModel.templateSettings;
                    utilitiesFactory.searchOnPage(vm, vm.templateSettings);
                    vm.isFirstLoad = false;
                    lockScreenService.stop();
                });
        }

        function initFilter() {
            vm.filter = {
                departmentId: null,
                showAll: false
            };
        }

        function initialize() {
            initFilter();

            CommonModel.getDepartment(null, true).then(function () {
                vm.treeDepartment = CommonModel.generateTreeSection(CommonModel.treeDepartment.Childrens[0].Childrens);
            });

            JLSTemplateSettingModel.getTemplates().then(function () {
                vm.templates = JLSTemplateSettingModel.templates;

                if (vm.templates && vm.templates.length > 0) {
                    vm.templateSetting.templateId = vm.templates[0].TemplateId;
                }
            });

            getTemplateSetting(vm.filter.departmentId, vm.filter.showAll);
        }

        function query() {
            vm.searchKey = '';
            vm.currentPage = 1;

            getTemplateSetting(vm.filter.departmentId, vm.filter.showAll);
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
                objectId: vm.filter.departmentId,
                objectTypeId: vm.objectType.section,
                showAll: vm.filter.showAll,
            }

            var url = baseUrl + "ExportJLSSectionTemplateSetting/ExportJLSSectionTemplateSetting";
            var data = $.param(vm.exportCriteria);

            var downloadUrl = url + "?" + data;
            var iframe = $('body iframe');

            if (iframe.length === 0) {
                iframe = $('<iframe width="0" height="0" class="display-none"></iframe>');
                iframe.appendTo('body');
            }

            iframe.attr("src", downloadUrl);
        }
    }
})(angular);
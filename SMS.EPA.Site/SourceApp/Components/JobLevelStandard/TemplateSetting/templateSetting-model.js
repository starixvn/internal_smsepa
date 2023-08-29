(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('jobLevelStandardModule').factory('JLSTemplateSettingModel', model);
    model.$inject = ['$uibModal', 'JLSTemplateSettingFactory', 'notificationFactory', 'utilitiesFactory', 'errorFactory'];

    function model($uibModal, TemplateSettingFactory, notificationFactory, utilitiesFactory, errorFactory) {
        let model = {
            permission: 0,
            templateSettings: [],
            templates: [],
            getTemplateSetting: getTemplateSetting,
            getTemplates: getTemplates,
            getUserPermission: getUserPermission,
            openPopupDelete: openPopupDelete,
            assignTemplate: assignTemplate
        };

        return model;

        function getTemplates() {
            return TemplateSettingFactory.getTemplate()
                .success(function (templates) {
                    model.templates = templates;
                }).error(function (response, status) {
                    errorFactory.errorHandle(status);
                });
        }

        function getTemplateSetting(objectIds, typeId, showAll) {
            return TemplateSettingFactory.getTemplateSetting(objectIds, typeId, showAll)
                .success(function (templateSettings) {
                    model.templateSettings = templateSettings;
                }).error(function (response, status) {
                    errorFactory.errorHandle(status);
                });
        }

        function getUserPermission() {
            return TemplateSettingFactory.getUserPermission().success(function (permission) {
                model.permission = permission;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function assignTemplate(templateSetting, objectTypeId) {
            if (templateSetting.sectionIds) {
                const params = {
                    objectIds: templateSetting.sectionIds,
                    objectTypeId: objectTypeId,
                    templateId: templateSetting.templateId
                }

                return TemplateSettingFactory.assignTemplate(params).success(function (response) {
                    if (response.IsSuccessful) {
                        notificationFactory.success('UpdatedSuccessfully');
                    } else {
                        notificationFactory.error('', response.Message);
                    }
                }).error(function (response, status) {
                    errorFactory.errorHandle(status);
                });
            } else {
                notificationFactory.error('SelectSectionBeforeAssignWarning');
            }
        }

        function openPopupDelete(vm, employee) {
            var modalInstance = $uibModal.open({
                templateUrl: '/SourceApp/Shared/Templates/ModalConfirmContent.html',
                controller: 'ModalConfirmController as modalConfirmCtrl',
                backdrop: 'static',
                resolve: {
                    name: function () {
                        return employee.Template;
                    },
                    action: function () {
                        return 'delete';
                    }
                },
                windowClass: 'modal-confirm-center'
            });

            modalInstance.result.then(function () {
                return TemplateSettingFactory.deleteTemplateSetting(employee.ObjectId, employee.ObjectTypeId)
                    .success(function (response) {
                        if (response.IsSuccessful) {
                            let index = vm.templateSettings.indexOf(employee);
                            vm.templateSettings.splice(index, 1);
                            utilitiesFactory.searchOnPage(vm, vm.templateSettings);
                            notificationFactory.success('DeletedSuccessfully');
                        } else {
                            notificationFactory.error('', response.Message);
                        }
                    }).error(function (response, status) {
                        errorFactory.errorHandle(status);
                    });
            })['finally'](function () {
                modalInstance = '';
            });
        }
    }
})(angular);
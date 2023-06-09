(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('jobLevelStandardModule').factory('JobLevelStandardTemplateModel', templateModel);
    templateModel.$inject = ['$uibModal', 'jobLevelStandardTemplateFactory', 'errorFactory', 'notificationFactory', 'utilitiesFactory'];

    function templateModel($uibModal, templateFactory, errorFactory, notificationFactory, utilitiesFactory) {
        let templates = [],
            template = {},
            kpis = [],
            permission = 0;

        let model = {
            createTemplate: createTemplate,
            kpis: kpis,
            openPopupCreateTemplate: openPopupCreateTemplate,
            openPopupDeleteTemplate: openPopupDeleteTemplate,
            openPopupUpdateStatusTemplate: openPopupUpdateStatusTemplate,
            permission: permission,
            template: template,
            templates: templates,
            getTemplate: getTemplate,
            getTemplateById: getTemplateById,
            getPermissionOnTemplate: getPermissionOnTemplate,
            getKPI: getKPI,
            checkSelectedAllKPI: checkSelectedAllKPI,
            updateTemplateKPI: updateTemplateKPI,
            updateTemplateBasicInfo: updateTemplateBasicInfo,
            validateTemplate: validateTemplate
        };

        return model;

        function createTemplate(template, modalInstance) {
            return templateFactory.createTemplate(template).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success('CreatedSuccessfully');
                    modalInstance.close();
                } else {
                    notificationFactory.error('', response.Message);
                }
            }).error(function () {
                errorFactory.errorHandle(status);
            });
        }

        function updateTemplateKPI(targetTemplate) {
            return templateFactory.updateTemplateKPI(targetTemplate).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success('UpdatedSuccessfully');
                } else {
                    notificationFactory.error('', response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function checkSelectedAllKPI(vm, kpis) {
            vm.selectedAll = true;

            for (let i = 0; i < kpis.length; i++) {
                if (!kpis[i].IsSelected) {
                    vm.selectedAll = false;
                    break;
                }
            }
        }

        function getKPI(id, isSelectedAll) {
            return templateFactory.getKPI(id, isSelectedAll)
                .success(function (data) {
                    model.kpis = data;
                }).error(function (response, status) {
                    errorFactory.errorHandle(status);
                });
        }

        function getPermissionOnTemplate() {
            return templateFactory.getPermissionOnTemplate().success(function (permission) {
                model.permission = permission;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getTemplate(isActive) {
            return templateFactory.getTemplate(isActive).success(function (templates) {
                model.templates = templates;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getTemplateById(id) {
            return templateFactory.getTemplateById(id).success(function (template) {
                model.template = template;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function openPopupCreateTemplate(vm) {
            vm.modalInstance = $uibModal.open({
                templateUrl: '/SourceApp/Components/JobLevelStandard/TargetTemplate/Templates/create.html',
                controller: 'CreateJLSTemplateController as createJLSTemplateCtrl',
                backdrop: 'static',
                windowClass: 'modal-confirm-center'
            });

            return vm.modalInstance.result;
        }

        function openPopupDeleteTemplate(vm, template) {
            let modalInstance = $uibModal.open({
                templateUrl: '/SourceApp/Shared/Templates/ModalConfirmContent.html',
                controller: 'ModalConfirmController as modalConfirmCtrl',
                backdrop: 'static',
                resolve: {
                    name: function () {
                        return template.Name;
                    },
                    action: function () {
                        return 'delete';
                    }
                },
                windowClass: 'modal-confirm-center'
            });

            modalInstance.result.then(function () {
                templateFactory.deleteTemplate(template.TemplateId).success(function (response) {
                    if (response.IsSuccessful) {
                        var index = vm.templates.indexOf(template);
                        vm.templates.splice(index, 1);
                        utilitiesFactory.searchOnPage(vm, vm.templates);
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

        function openPopupUpdateStatusTemplate(template) {
            template.IsActive = !template.IsActive;

            let action = template.IsActive ? 'activate' : 'deactivate',
                newTemplate = {
                    TemplateId: template.TemplateId,
                    IsActive: template.IsActive
                };

            let modalInstance = $uibModal.open({
                templateUrl: '/SourceApp/Shared/Templates/ModalConfirmContent.html',
                controller: 'ModalConfirmController as modalConfirmCtrl',
                backdrop: 'static',
                resolve: {
                    name: function () {
                        return template.Name;
                    },
                    action: function () {
                        return action;
                    }
                },
                windowClass: 'modal-confirm-center'
            });

            let keyMessage = '';
            modalInstance.result.then(function () {
                templateFactory.updateStatusTemplate(newTemplate).success(function (response) {
                    if (response.IsSuccessful) {
                        keyMessage = action === 'activate' ? 'ActivationSuccess' : 'DeactivationSuccess';
                        notificationFactory.success(keyMessage);
                    } else {
                        keyMessage = action === 'activate' ? 'ActivationFail' : 'DeactivationFail';
                        template.IsActive = !newTemplate.IsActive;
                        notificationFactory.error(keyMessage);
                    }
                }).error(function (response, status) {
                    if (status === 405) {
                        errorFactory.errorHandle(status);
                    } else {
                        keyMessage = action === 'activate' ? 'ActivationFail' : 'DeactivationFail';
                        template.IsActive = !newTemplate.IsActive;
                        notificationFactory.error(keyMessage);
                    }
                });
            },
                function () {
                    template.IsActive = !newTemplate.IsActive;
                })['finally'](function () {
                    modalInstance = '';
                });
        }

        function updateTemplateBasicInfo(template) {
            return templateFactory.updateTemplateBasicInfo(template).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success('UpdatedSuccessfully');
                } else {
                    notificationFactory.error('', response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function validateTemplate(template) {
            let isValid = true;

            if (!template) {
                isValid = false;
            } else if (!template.Name) {
                notificationFactory.error('NameRequired');
                isValid = false;
            }

            return isValid;
        }
    }
})(angular);
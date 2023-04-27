(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('jobLevelStandardModule').factory('JobLevelStandardTemplateModel', templateModel);
    templateModel.$inject = [
        '$uibModal', 'jobLevelStandardTemplateFactory', 'errorFactory', 'notificationFactory', 'utilitiesFactory'
    ];

    function templateModel($uibModal, jlsTemplateFactory, errorFactory, notificationFactory, utilitiesFactory) {
        var templates = [],
            template = {},
            kpiList = [],
            permission = 0

        var model = {
            createTemplate: createTemplate,
            kpiList: kpiList,
            openPopupCreateTemplate: openPopupCreateTemplate,
            openPopupDeleteTemplate: openPopupDeleteTemplate,
            openPopupUpdateStatusTemplate: openPopupUpdateStatusTemplate,
            permission: permission,
            template: template,
            templates: templates,
            getTemplate: getTemplate,
            getTemplateById: getTemplateById,
            getPermissionOnTemplate: getPermissionOnTemplate,
            getKPIListSelected: getKPIListSelected,
            checkSelectedAllKPI: checkSelectedAllKPI,
            updateTemplateKPI: updateTemplateKPI,
            updateTemplateBasicInfo: updateTemplateBasicInfo,
            validateTemplate: validateTemplate
        };

        return model;

        function updateTemplateKPI(targetTemplate) {
            return jlsTemplateFactory.updateTemplateKPI(targetTemplate).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success("UpdatedSuccessfully");
                } else {
                    notificationFactory.error("", response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function checkSelectedAllKPI(vm, kpiList) {
            vm.SelectedAll = true;

            for (let i = 0; i < kpiList.length; i++) {
                if (!kpiList[i].IsSelected) {
                    vm.SelectedAll = false;
                    break;
                }
            }
        }

        function getKPIListSelected(id, isSelected) {
            return jlsTemplateFactory.getKPIListSelected(id, isSelected).success(
                function (data) {
                    model.kpiList = data;
                }).error(function (response, status) {
                    errorFactory.errorHandle(status);
                });
        }

        function createTemplate(template, modalInstance) {
            return jlsTemplateFactory.createTemplate(template).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success("CreatedSuccessfully");
                    modalInstance.close();
                } else {
                    notificationFactory.error("", response.Message);
                }
            }).error(function () {
                errorFactory.errorHandle(status);
            });
        }

        function getPermissionOnTemplate() {
            return jlsTemplateFactory.getPermissionOnTemplate().success(function (permission) {
                model.permission = permission;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getTemplate(isActive) {
            return jlsTemplateFactory.getTemplate(isActive).success(function (templates) {
                model.templates = templates;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getTemplateById(id) {
            return jlsTemplateFactory.getTemplateById(id).success(function (template) {
                model.template = template;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function openPopupCreateTemplate(vm) {
            vm.modalInstance = $uibModal.open({
                templateUrl: '/SourceApp/Components/JobLevelStandard/TargetTemplate/Templates/create.html',
                controller: "CreateJLSTemplateController as createJLSTemplateCtrl",
                backdrop: 'static',
                windowClass: "modal-confirm-center"
            });

            return vm.modalInstance.result;
        }

        function openPopupDeleteTemplate(vm, template) {
            var modalInstance = $uibModal.open({
                templateUrl: '/SourceApp/Shared/Templates/ModalConfirmContent.html',
                controller: "ModalConfirmController as modalConfirmCtrl",
                backdrop: 'static',
                resolve: {
                    name: function () {
                        return template.Name;
                    },
                    action: function () {
                        return 'delete';
                    }
                },
                windowClass: "modal-confirm-center"
            });

            modalInstance.result.then(function () {
                jlsTemplateFactory.deleteTemplate(template.TemplateId).success(function (response) {
                    if (response.IsSuccessful) {
                        var index = vm.templates.indexOf(template);
                        vm.templates.splice(index, 1);
                        utilitiesFactory.searchOnPage(vm, vm.templates);
                        notificationFactory.success("DeletedSuccessfully");
                    } else {
                        notificationFactory.error("", response.Message);
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

            var action = template.IsActive ? "activate" : "deactivate",
                newTemplate = {
                    TemplateId: template.TemplateId,
                    IsActive: template.IsActive
                };

            var modalInstance = $uibModal.open({
                templateUrl: '/SourceApp/Shared/Templates/ModalConfirmContent.html',
                controller: "ModalConfirmController as modalConfirmCtrl",
                backdrop: 'static',
                resolve: {
                    name: function () {
                        return template.Name;
                    },
                    action: function () {
                        return action;
                    }
                },
                windowClass: "modal-confirm-center"
            });

            var keyMessage = '';

            modalInstance.result.then(function () {
                jlsTemplateFactory.updateStatusTemplate(newTemplate).success(function (response) {
                    if (response.IsSuccessful) {
                        keyMessage = action === "activate" ? "ActivationSuccess" : "DeactivationSuccess";
                        notificationFactory.success(keyMessage);
                    } else {
                        keyMessage = action === "activate" ? "ActivationFail" : "DeactivationFail";
                        template.IsActive = !newTemplate.IsActive;
                        notificationFactory.error(keyMessage);
                    }
                }).error(function (response, status) {
                    if (status === 405) {
                        errorFactory.errorHandle(status);
                    } else {
                        keyMessage = action === "activate" ? "ActivationFail" : "DeactivationFail";
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
            return jlsTemplateFactory.updateTemplateBasicInfo(template).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success("UpdatedSuccessfully");
                } else {
                    notificationFactory.error("", response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function validateTemplate(template) {
            var isValid = true;

            if (!template) {
                isValid = false;
            } else if (!template.Name) {
                notificationFactory.error("NameRequired");
                isValid = false;
            }

            return isValid;
        }
    }
})(angular);
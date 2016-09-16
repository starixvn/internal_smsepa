(function (angular) {
    'use strict';

    angular.module('kpiGroupModule').factory('KPIGroupModel', kpiGroupModel);
    kpiGroupModel.$inject = ['$modal', 'kpiGroupFactory', 'utilitiesFactory', 'notificationFactory', 'errorFactory'];

    function kpiGroupModel($modal, kpiGroupFactory, utilitiesFactory, notificationFactory, errorFactory) {
        var kpiGroups = [],
            kpiGroup = {},
            permission = 0;

        var model = {
            createKPIGroup: createKPIGroup,
            getKPIGroup: getKPIGroup,
            getKPIGroupById: getKPIGroupById,
            getUserPermissionOnKPIGroup: getUserPermissionOnKPIGroup,
            kpiGroup: kpiGroup,
            kpiGroups: kpiGroups,
            openPopupCreateKPIGroup: openPopupCreateKPIGroup,
            openPopupDeleteKPIGroup: openPopupDeleteKPIGroup,
            openPopupUpdateKPIGroup: openPopupUpdateKPIGroup,
            openPopupUpdateStatusKPIGroup: openPopupUpdateStatusKPIGroup,
            permission: permission,
            updateKPIGroup: updateKPIGroup,
            validateKPIGroup: validateKPIGroup
        };

        return model;

        function createKPIGroup(kpiGroup, modalInstance) {
            return kpiGroupFactory.createKPIGroup(kpiGroup).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success('CreatedSuccessfully');
                    modalInstance.close();
                } else {
                    notificationFactory.error('', response.Message);
                }
            })
            .error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getKPIGroup(isActive) {
            return kpiGroupFactory.getKPIGroup(isActive).success(function (kpiGroups) {
                model.kpiGroups = kpiGroups;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getKPIGroupById(kpiGroupId) {
            return kpiGroupFactory.getKPIGroupById(kpiGroupId).success(function (kpiGroup) {
                model.kpiGroup = kpiGroup;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getUserPermissionOnKPIGroup() {
            return kpiGroupFactory.getUserPermissionOnKPIGroup().success(function (permission) {
                model.permission = parseInt(permission);
            })
            .error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function openPopupCreateKPIGroup(vm) {
            vm.modalInstance = $modal.open({
                templateUrl: 'SourceApp/Components/KPIGroup/Templates/create.html',
                controller: 'CreateKPIGroupController as createKPIGroupCtrl',
                backdrop: 'static',
                windowClass: 'modal-confirm-center'
            });

            return vm.modalInstance.result;
        }

        function openPopupDeleteKPIGroup(vm, kpiGroup) {
            var modalInstance = $modal.open({
                templateUrl: 'SourceApp/Shared/Templates/ModalConfirmContent.html',
                controller: 'ModalConfirmController as modalConfirmCtrl',
                backdrop: 'static',
                resolve: {
                    name: function () {
                        return kpiGroup.Name;
                    },
                    action: function () {
                        return 'delete';
                    }
                },
                windowClass: 'modal-confirm-center'
            });

            modalInstance.result.then(function () {
                kpiGroupFactory.deleteKPIGroup(kpiGroup.KPIGroupId).success(function (response) {
                    if (response.IsSuccessful) {
                        var index = vm.kpiGroups.indexOf(kpiGroup);
                        vm.kpiGroups.splice(index, 1);
                        utilitiesFactory.searchOnPage(vm, vm.kpiGroups);

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

        function openPopupUpdateKPIGroup(vm, kpiGroup) {
            vm.modalInstance = $modal.open({
                templateUrl: 'SourceApp/Components/KPIGroup/Templates/edit.html',
                controller: 'UpdateKPIGroupController as updateKPIGroupCtrl',
                backdrop: 'static',
                resolve: {
                    kpiGroup: function () {
                        return angular.copy(kpiGroup);
                    }
                },
                windowClass: 'modal-confirm-center'
            });

            return vm.modalInstance.result;
        }

        function openPopupUpdateStatusKPIGroup(kpiGroup) {
            kpiGroup.IsActive = !kpiGroup.IsActive;

            var action = kpiGroup.IsActive ? 'activate' : 'deactivate',
                newKPIGroup = {
                    KPIGroupId: kpiGroup.KPIGroupId,
                    IsActive: kpiGroup.IsActive
                };

            var modalInstance = $modal.open({
                templateUrl: 'SourceApp/Shared/Templates/ModalConfirmContent.html',
                controller: 'ModalConfirmController as modalConfirmCtrl',
                backdrop: 'static',
                resolve: {
                    name: function () {
                        return kpiGroup.Name;
                    },
                    action: function () {
                        return action;
                    }
                },
                windowClass: 'modal-confirm-center'
            });

            var keyMessage = '';

            modalInstance.result.then(function () {
                kpiGroupFactory.updateStatusKPIGroup(newKPIGroup).success(function (response) {
                    if (response.IsSuccessful) {
                        keyMessage = action === 'activate' ? 'ActivationSuccess' : 'DeactivationSuccess';
                        notificationFactory.success(keyMessage);
                    } else {
                        kpiGroup.IsActive = !newKPIGroup.IsActive;
                        notificationFactory.error('', response.Message);
                    }
                }).error(function (response, status) {
                    if (status === 405) {
                        errorFactory.errorHandle(status);
                    } else {
                        keyMessage = action === 'activate' ? 'ActivationFail' : 'DeactivationFail';
                        kpiGroup.IsActive = !newKPIGroup.IsActive;
                        notificationFactory.error(keyMessage);
                    }
                });
            }, function () {
                kpiGroup.IsActive = !newKPIGroup.IsActive;
            })['finally'](function () {
                modalInstance = '';
            });
        }

        function updateKPIGroup(kpiGroup, modalInstance) {
            return kpiGroupFactory.updateKPIGroup(kpiGroup).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success('UpdatedSuccessfully');
                    modalInstance.close(kpiGroup);
                } else {
                    notificationFactory.error('', response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function validateKPIGroup(kpiGroup) {
            var isValid = true;

            if (!kpiGroup) {
                isValid = false;
            } else if (!kpiGroup.Name) {
                notificationFactory.error('NameRequired');
                isValid = false;
            }

            return isValid;
        }
    }
})(angular);
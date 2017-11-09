(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('kpiModule').factory('KPIModel', kpiModel);
    kpiModel.$inject = ['$uibModal', 'kpiFactory', 'utilitiesFactory', 'notificationFactory', 'errorFactory'];

    function kpiModel($uibModal, kpiFactory, utilitiesFactory, notificationFactory, errorFactory) {
        var kpis = [],
            permission = 0;

        var model = {
            createKPI: createKPI,
            getKPI: getKPI,
            getUserPermissionOnKPI: getUserPermissionOnKPI,
            kpis: kpis,
            openPopupCreateKPI: openPopupCreateKPI,
            openPopupDeleteKPI: openPopupDeleteKPI,
            openPopupUpdateKPI: openPopupUpdateKPI,
            openPopupUpdateRequiredEvaluateKPI: openPopupUpdateRequiredEvaluateKPI,
            openPopupUpdateStatusKPI: openPopupUpdateStatusKPI,
            permission: permission,
            updateKPI: updateKPI,
            validateKPI: validateKPI
        };

        return model;

        function createKPI(vm, isCreateAndContinued, kpiGroup, modalInstance) {
            return kpiFactory.createKPI(vm.kpi).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success('CreatedSuccessfully');

                    if (kpiGroup) {
                        kpiGroup.NumberKPI += 1;
                    }

                    if (isCreateAndContinued) {
                        vm.kpi = {
                            Name: '',
                            KPIGroupIds: '',
                            HasComment: true,
                            IsRequiredComment: true,
                            IsRequiredEvaluate: true,
                            Description: ''
                        };
                    } else {
                        modalInstance.close();
                    }
                } else {
                    notificationFactory.error('', response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getKPI(kpiGroupId, isActive, isRequiredEvaluate) {
            return kpiFactory.getKPI(kpiGroupId, isActive, isRequiredEvaluate).success(function (kpis) {
                model.kpis = kpis;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getUserPermissionOnKPI() {
            return kpiFactory.getUserPermissionOnKPI().success(function (permission) {
                model.permission = parseInt(permission);
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function openPopupCreateKPI(vm, kpiGroup) {
            vm.modalInstance = $uibModal.open({
                templateUrl: 'SourceApp/Components/KPI/Templates/create.html',
                controller: 'CreateKPIController as createKPICtrl',
                backdrop: 'static',
                resolve: {
                    kpiGroup: function () {
                        return kpiGroup;
                    }
                },
                windowClass: 'modal-confirm-center'
            });

            return vm.modalInstance.result;
        }

        function openPopupDeleteKPI(vm, kpi) {
            var modalInstance = $uibModal.open({
                templateUrl: 'SourceApp/Shared/Templates/ModalConfirmContent.html',
                controller: 'ModalConfirmController as modalConfirmCtrl',
                backdrop: 'static',
                resolve: {
                    name: function () {
                        return kpi.Name;
                    },
                    action: function () {
                        return 'delete';
                    }
                },
                windowClass: 'modal-confirm-center'
            });

            modalInstance.result.then(function () {
                kpiFactory.deleteKPI(kpi.KPIId).success(function (response) {
                    if (response.IsSuccessful) {
                        var index = vm.kpis.indexOf(kpi);
                        vm.kpis.splice(index, 1);
                        utilitiesFactory.searchOnPage(vm, vm.kpis);

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

        function openPopupUpdateKPI(vm, kpi) {
            vm.modalInstance = $uibModal.open({
                templateUrl: 'SourceApp/Components/KPI/Templates/edit.html',
                controller: 'UpdateKPIController as updateKPICtrl',
                backdrop: 'static',
                resolve: {
                    kpi: function () {
                        return angular.copy(kpi);
                    }
                },
                windowClass: 'modal-confirm-center'
            });

            return vm.modalInstance.result;
        }

        function openPopupUpdateRequiredEvaluateKPI(kpi) {
            kpi.IsRequiredEvaluate = !kpi.IsRequiredEvaluate;

            var action = kpi.IsRequiredEvaluate ? 'required' : 'not required',
                newKPI = {
                    KPIId: kpi.KPIId,
                    IsRequiredEvaluate: kpi.IsRequiredEvaluate
                };

            var modalInstance = $uibModal.open({
                templateUrl: 'SourceApp/Components/KPI/Templates/modalConfirmRequired.html',
                controller: 'ModalConfirmController as modalConfirmCtrl',
                backdrop: 'static',
                resolve: {
                    name: function () {
                        return kpi.Name;
                    },
                    action: function () {
                        return action;
                    }
                },
                windowClass: 'modal-confirm-center'
            });

            var keyMessage = '';

            modalInstance.result.then(function () {
                kpiFactory.updateRequiredEvaluateKPI(newKPI).success(function (response) {
                    if (response.IsSuccessful) {
                        keyMessage = action === 'required' ? 'ActivationSuccess' : 'DeactivationSuccess';
                        notificationFactory.success(keyMessage);
                    } else {
                        kpi.IsRequiredEvaluate = !newKPI.IsRequiredEvaluate;
                        notificationFactory.error('', response.Message);
                    }
                }).error(function (response, status) {
                    if (status === 405) {
                        errorFactory.errorHandle(status);
                    } else {
                        keyMessage = action === 'required' ? 'ActivationFail' : 'DeactivationFail';
                        kpi.IsRequiredEvaluate = !newKPI.IsRequiredEvaluate;
                        notificationFactory.error(keyMessage);
                    }
                });
            }, function () {
                kpi.IsRequiredEvaluate = !newKPI.IsRequiredEvaluate;
            })['finally'](function () {
                modalInstance = '';
            });
        }

        function openPopupUpdateStatusKPI(kpi) {
            kpi.IsActive = !kpi.IsActive;

            var action = kpi.IsActive ? 'activate' : 'deactivate',
                newKPI = {
                    KPIId: kpi.KPIId,
                    IsActive: kpi.IsActive
                };

            var modalInstance = $uibModal.open({
                templateUrl: 'SourceApp/Shared/Templates/ModalConfirmContent.html',
                controller: 'ModalConfirmController as modalConfirmCtrl',
                backdrop: 'static',
                resolve: {
                    name: function () {
                        return kpi.Name;
                    },
                    action: function () {
                        return action;
                    }
                },
                windowClass: 'modal-confirm-center'
            });

            var keyMessage = '';

            modalInstance.result.then(function () {
                kpiFactory.updateStatusKPI(newKPI).success(function (response) {
                    if (response.IsSuccessful) {
                        keyMessage = action === 'activate' ? 'ActivationSuccess' : 'DeactivationSuccess';
                        notificationFactory.success(keyMessage);
                    } else {
                        kpi.IsActive = !newKPI.IsActive;
                        notificationFactory.error('', response.Message);
                    }
                }).error(function (response, status) {
                    if (status === 405) {
                        errorFactory.errorHandle(status);
                    } else {
                        keyMessage = action === 'activate' ? 'ActivationFail' : 'DeactivationFail';
                        kpi.IsActive = !newKPI.IsActive;
                        notificationFactory.error(keyMessage);
                    }
                });
            }, function () {
                kpi.IsActive = !newKPI.IsActive;
            })['finally'](function () {
                modalInstance = '';
            });
        }

        function updateKPI(kpi, modalInstance) {
            return kpiFactory.updateKPI(kpi).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success('UpdatedSuccessfully');
                    modalInstance.close(kpi);
                } else {
                    notificationFactory.error('', response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function validateKPI(kpi, currentChoseKPIGroups) {
            var isValid = true;

            if (!kpi) {
                isValid = false;
            } else if (!kpi.Name) {
                notificationFactory.error('TitleRequire');
                isValid = false;
            } else if (currentChoseKPIGroups.length === 0) {
                notificationFactory.error('GroupRequire');
                isValid = false;
            }

            return isValid;
        }
    }
})(angular);
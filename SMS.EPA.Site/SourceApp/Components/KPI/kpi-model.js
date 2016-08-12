'use strict';

(function (angular) {
    angular.module('kpiModule').factory('KPIModel', KPIModel);
    KPIModel.$inject = ['$modal', 'kpiFactory', 'utilitiesFactory', 'notificationFactory', 'errorFactory'];

    function KPIModel($modal, kpiFactory, utilitiesFactory, notificationFactory, errorFactory) {
        var kpiModel = {};

        kpiModel.kpis = [];
        kpiModel.permission = 0;

        function createKPI(vm, isCreateAndContinued, kpiGroup, modalInstance) {
            return kpiFactory.createKPI(vm.kpi)
            .success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success("CreatedSuccessfully");

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
                    notificationFactory.error("", response.Message);
                }
            })
            .error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getKPI(kpiGroupId, isActive, isRequiredEvaluate) {
            return kpiFactory.getKPI(kpiGroupId, isActive, isRequiredEvaluate)
            .success(function (kpis) {
                kpiModel.kpis = kpis;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getUserPermissionOnKPI() {
            return kpiFactory.getUserPermissionOnKPI()
            .success(function (permission) {
                kpiModel.permission = parseInt(permission);
            })
            .error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function openPopupCreateKPI(vm, kpiGroup) {
            vm.modalInstance = $modal.open({
                templateUrl: 'SourceApp/Components/KPI/Templates/create.html',
                controller: "CreateKPIController as createKPICtrl",
                backdrop: 'static',
                resolve: {
                    kpiGroup: function () {
                        return kpiGroup;
                    }
                },
                windowClass: "modal-confirm-center"
            });

            return vm.modalInstance.result;
        }

        function openPopupDeleteKPI(vm, kpi) {
            var modalInstance = $modal.open({
                templateUrl: 'SourceApp/Shared/Templates/ModalConfirmContent.html',
                controller: "ModalConfirmController as modalConfirmCtrl",
                backdrop: 'static',
                resolve: {
                    name: function () {
                        return kpi.Name;
                    },
                    action: function () {
                        return 'delete';
                    }
                },
                windowClass: "modal-confirm-center"
            });

            modalInstance.result.then(function () {
                kpiFactory.deleteKPI(kpi.KPIId).success(function (response) {
                    if (response.IsSuccessful) {
                        var index = vm.kpis.indexOf(kpi);
                        vm.kpis.splice(index, 1);
                        utilitiesFactory.searchOnPage(vm, vm.kpis);

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

        function openPopupUpdateKPI(vm, kpi) {
            vm.modalInstance = $modal.open({
                templateUrl: 'SourceApp/Components/KPI/Templates/edit.html',
                controller: "UpdateKPIController as updateKPICtrl",
                backdrop: 'static',
                resolve: {
                    kpi: function () {
                        return angular.copy(kpi);
                    }
                },
                windowClass: "modal-confirm-center"
            });

            return vm.modalInstance.result;
        }

        function openPopupUpdateRequiredEvaluateKPI(kpi) {
            kpi.IsRequiredEvaluate = !kpi.IsRequiredEvaluate;

            var action = kpi.IsRequiredEvaluate ? "required" : "not required",
                newKPI = {
                    KPIId: kpi.KPIId,
                    IsRequiredEvaluate: kpi.IsRequiredEvaluate
                };

            var modalInstance = $modal.open({
                templateUrl: 'SourceApp/Components/KPI/Templates/modalConfirmRequired.html',
                controller: "ModalConfirmController as modalConfirmCtrl",
                backdrop: 'static',
                resolve: {
                    name: function () {
                        return kpi.Name;
                    },
                    action: function () {
                        return action;
                    }
                },
                windowClass: "modal-confirm-center"
            });

            var keyMessage = '';

            modalInstance.result.then(function () {
                kpiFactory.updateRequiredEvaluateKPI(newKPI)
                .success(function (response) {
                    if (response.IsSuccessful) {
                        keyMessage = action === "required" ? "ActivationSuccess" : "DeactivationSuccess";
                        notificationFactory.success(keyMessage);
                    } else {
                        kpi.IsRequiredEvaluate = !newKPI.IsRequiredEvaluate;
                        notificationFactory.error("", response.Message);
                    }
                })
                .error(function (response, status) {
                    if (status === 405) {
                        errorFactory.errorHandle(status);
                    } else {
                        keyMessage = action === "required" ? "ActivationFail" : "DeactivationFail";
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

            var action = kpi.IsActive ? "activate" : "deactivate",
                newKPI = {
                    KPIId: kpi.KPIId,
                    IsActive: kpi.IsActive
                };

            var modalInstance = $modal.open({
                templateUrl: 'SourceApp/Shared/Templates/ModalConfirmContent.html',
                controller: "ModalConfirmController as modalConfirmCtrl",
                backdrop: 'static',
                resolve: {
                    name: function () {
                        return kpi.Name;
                    },
                    action: function () {
                        return action;
                    }
                },
                windowClass: "modal-confirm-center"
            });

            var keyMessage = '';

            modalInstance.result.then(function () {
                kpiFactory.updateStatusKPI(newKPI)
                .success(function (response) {
                    if (response.IsSuccessful) {
                        keyMessage = action === "activate" ? "ActivationSuccess" : "DeactivationSuccess";
                        notificationFactory.success(keyMessage);
                    } else {
                        kpi.IsActive = !newKPI.IsActive;
                        notificationFactory.error("", response.Message);
                    }
                })
                .error(function (response, status) {
                    if (status === 405) {
                        errorFactory.errorHandle(status);
                    } else {
                        keyMessage = action === "activate" ? "ActivationFail" : "DeactivationFail";
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
            return kpiFactory.updateKPI(kpi)
            .success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success("UpdatedSuccessfully");
                    modalInstance.close(kpi);
                } else {
                    notificationFactory.error("", response.Message);
                }
            })
            .error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function validateKPI(kpi, currentChoseKPIGroups) {
            var isValid = true;

            if (!kpi) {
                isValid = false;
            } else {
                if (!kpi.Name) {
                    notificationFactory.error("TitleRequire");
                    isValid = false;
                } else if (currentChoseKPIGroups.length === 0) {
                    notificationFactory.error("GroupRequire");
                    isValid = false;
                }
            }

            return isValid;
        }

        kpiModel.createKPI = createKPI;
        kpiModel.getKPI = getKPI;
        kpiModel.getUserPermissionOnKPI = getUserPermissionOnKPI;
        kpiModel.openPopupCreateKPI = openPopupCreateKPI;
        kpiModel.openPopupDeleteKPI = openPopupDeleteKPI;
        kpiModel.openPopupUpdateKPI = openPopupUpdateKPI;
        kpiModel.openPopupUpdateRequiredEvaluateKPI = openPopupUpdateRequiredEvaluateKPI;
        kpiModel.openPopupUpdateStatusKPI = openPopupUpdateStatusKPI;
        kpiModel.updateKPI = updateKPI;
        kpiModel.validateKPI = validateKPI;

        return kpiModel;
    }
})(angular);
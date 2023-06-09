(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('jobLevelStandardModule').factory('JobLevelStandardKPIModel', kpiModel);
    kpiModel.$inject = ['$uibModal', 'JobLevelStandardKPIFactory', 'utilitiesFactory', 'notificationFactory', 'errorFactory'];

    function kpiModel($uibModal, JobLevelStandardKPIFactory, utilitiesFactory, notificationFactory, errorFactory) {
        let kpis = [],
            permission = 0;

        let model = {
            createKPI: createKPI,
            getJobLevelStandKPI: getJobLevelStandKPI,
            getUserPermissionOnKPI: getUserPermissionOnKPI,
            kpis: kpis,
            openPopupCreateKPI: openPopupCreateKPI,
            openPopupDeleteKPI: openPopupDeleteKPI,
            openPopupUpdateKPI: openPopupUpdateKPI,
            openPopupUpdateStatusKPI: openPopupUpdateStatusKPI,
            permission: permission,
            updateKPI: updateKPI,
        };

        return model;

        function createKPI(vm, isCreateAndContinued, modalInstance) {
            return JobLevelStandardKPIFactory.createKPI(vm.kpi).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success('CreatedSuccessfully');
                    if (isCreateAndContinued) {
                        vm.kpi = {
                            Name: ''
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

        function getJobLevelStandKPI(isActive) {
            return JobLevelStandardKPIFactory.getJobLevelStandKPI(isActive).success(function (kpis) {
                model.kpis = kpis;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getUserPermissionOnKPI() {
            return JobLevelStandardKPIFactory.getUserPermissionOnKPI().success(function (permission) {
                model.permission = parseInt(permission);
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function openPopupCreateKPI(vm) {
            vm.modalInstance = $uibModal.open({
                templateUrl: 'SourceApp/Components/JobLevelStandard/KPI/Templates/create.html',
                controller: 'CreateJobLevelStandardKPIController as createKPICtrl',
                backdrop: 'static',
                resolve: {
                },
                windowClass: 'modal-confirm-center'
            });

            return vm.modalInstance.result;
        }

        function openPopupDeleteKPI(vm, kpi) {
            let modalInstance = $uibModal.open({
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
                JobLevelStandardKPIFactory.deleteKPI(kpi.KPIId).success(function (response) {
                    if (response.IsSuccessful) {
                        const index = vm.kpis.indexOf(kpi);
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
                templateUrl: 'SourceApp/Components/JobLevelStandard/KPI/Templates/edit.html',
                controller: 'UpdateJobLevelStandardKPIController as updateKPICtrl',
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

        function openPopupUpdateStatusKPI(kpi) {
            kpi.IsActive = !kpi.IsActive;

            const action = kpi.IsActive ? 'activate' : 'deactivate',
                newKPI = {
                    KPIId: kpi.KPIId,
                    IsActive: kpi.IsActive
                };

            let modalInstance = $uibModal.open({
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

            let keyMessage = '';

            modalInstance.result.then(function () {
                JobLevelStandardKPIFactory.updateStatusKPI(newKPI).success(function (response) {
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
            return JobLevelStandardKPIFactory.updateKPI(kpi).success(function (response) {
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
    }
})(angular);
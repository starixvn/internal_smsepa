(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('epaModule').factory('UpdateFinalScoreModel', updateFinalScoreModel);
    updateFinalScoreModel.$inject = ['$uibModal', 'updateFinalScoreFactory', 'notificationFactory', 'errorFactory'];

    function updateFinalScoreModel($uibModal, updateFinalScoreFactory, notificationFactory, errorFactory) {
        let model = {};

        model.periods = [];
        model.employees = [];
        model.epaInfos = [];

        model.getPeriodToUpdateFinalScore = function () {
            return updateFinalScoreFactory.getPeriodToUpdateFinalScore().success(function (periods) {
                model.periods = periods;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.getEmployeeToUpdateFinalScore = function (periodId, teamIds, isLocal, query) {
            return updateFinalScoreFactory.getEmployeeToUpdateFinalScore(periodId, teamIds, isLocal, query).success(function (employees) {
                model.employees = employees;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.getEPAToUpdateFinalScore = function (periodId, teamIds, isLocal, employeeId) {
            return updateFinalScoreFactory.getEPAToUpdateFinalScore(periodId, teamIds, isLocal, employeeId).success(function (epaInfos) {
                model.epaInfos = epaInfos;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.updateFinalScore = function (vm, employeeIds) {
            let modalInstance = $uibModal.open({
                templateUrl: '/SourceApp/Shared/Templates/ModalConfirmContent.html',
                controller: 'ModalConfirmController as modalConfirmCtrl',
                backdrop: 'static',
                resolve: {
                    name: function () {
                        return '';
                    },
                    action: function () {
                        return 'update final score for selected employee(s)';
                    }
                },
                windowClass: 'modal-confirm-center'
            });

            modalInstance.result.then(function () {
                return updateFinalScoreFactory.updateFinalScore({ periodId: vm.filter.periodId, employeeIds }).success(function (response) {
                    if (response.IsSuccessful) {
                        vm.dataSaved = true;

                        const employeeId = vm.filter.employee ? vm.filter.employee.EmployeeId : null;
                        updateFinalScoreFactory.getEPAToUpdateFinalScore(vm.filter.periodId, vm.filter.teamIds, vm.filter.isLocal, employeeId)
                            .success(function (epaInfos) {
                                angular.forEach(vm.epaInfos, function (epaInfo) {
                                    if (epaInfo.Selected) {
                                        angular.forEach(epaInfos, function (newEpaInfo) {
                                            if (newEpaInfo.EPAId === epaInfo.EPAId) {
                                                epaInfo.NewFinalScore = newEpaInfo.FinalScore;
                                            }
                                        });
                                    }
                                });
                            }).error(function (response, status) {
                                errorFactory.errorHandle(status);
                            });

                        notificationFactory.success('UpdatedSuccessfully');
                    } else {
                        notificationFactory.error('', response.Message);
                    }
                }).error(function (response, status) {
                    errorFactory.errorHandle(status);
                });
            })['finally'](function () {
                modalInstance = '';
            });
        };

        return model;
    }
})(angular);
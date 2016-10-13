'use strict';
/*global epaModule*/

epaModule.factory('UpdateFinalScoreModel',
['$modal', 'updateFinalScoreFactory', 'notificationFactory', 'errorFactory',
function ($modal, updateFinalScoreFactory, notificationFactory, errorFactory) {
    var model = this;

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

    model.getEmployeeToUpdateFinalScore = function (periodId, groupId, isLocal, query) {
        return updateFinalScoreFactory.getEmployeeToUpdateFinalScore(periodId, groupId, isLocal, query).success(function (employees) {
            model.employees = employees;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    model.getEPAToUpdateFinalScore = function (periodId, groupId, isLocal, employeeId) {
        return updateFinalScoreFactory.getEPAToUpdateFinalScore(periodId, groupId, isLocal, employeeId).success(function (epaInfos) {
            model.epaInfos = epaInfos;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    model.updateFinalScore = function (vm, selectedEPAs) {
        var modalInstance = $modal.open({
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
            return updateFinalScoreFactory.updateFinalScore(selectedEPAs).success(function (response) {
                if (response.IsSuccessful) {
                    vm.dataSaved = true;
                    notificationFactory.success('UpdatedSuccessfully');

                    var employeeId = vm.filter.employee ? vm.filter.employee.EmployeeId : null;
                    updateFinalScoreFactory.getEPAToUpdateFinalScore(vm.filter.periodId, vm.filter.groupId, vm.filter.isLocal, employeeId)
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
}]);
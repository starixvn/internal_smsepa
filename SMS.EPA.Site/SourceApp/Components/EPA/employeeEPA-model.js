'use strict';
/*global epaModule*/

epaModule.factory('EmployeeEPAModel',
['$modal', 'employeeEPAFactory', 'utilitiesFactory', 'notificationFactory', 'errorFactory',
function ($modal, employeeEPAFactory, utilitiesFactory, notificationFactory, errorFactory) {
    var employeeEPAModel = this;

    employeeEPAModel.epaInfo = {};
    employeeEPAModel.periods = [];
    employeeEPAModel.employees = [];
    employeeEPAModel.epaInfos = [];

    employeeEPAModel.getEPAInfoById = function (epaId) {
        return employeeEPAFactory.getEPAInfoById(epaId).success(function (epaInfo) {
            employeeEPAModel.epaInfo = epaInfo;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    employeeEPAModel.getNotificationInputScore = function (minScore, maxScore, message) {
        return utilitiesFactory.formatString(message, minScore, maxScore);
    };

    employeeEPAModel.validateScoreInput = function (weight, minScore, maxScore, message) {
        var isValid = true;

        if (weight > maxScore || weight < minScore) {
            isValid = false;

            var msg = utilitiesFactory.formatString(message, minScore, maxScore);
            notificationFactory.error('', msg);
        }

        return isValid;
    };

    employeeEPAModel.getClassProgress = function (statusEndProcess) {
        if (statusEndProcess === 0) {
            return 'in-process';
        }
        else if (statusEndProcess === 1) {
            return 'end-process';
        }
        else {
            return 'open';
        }
    };

    employeeEPAModel.convertScoreToGrade = function (finalScore, standardGrades) {
        var score = utilitiesFactory.parseFloat(finalScore);
        var grade;

        angular.forEach(standardGrades, function (standardGrade) {
            if (score >= standardGrade.LeftScore && score <= standardGrade.RightScore) {
                grade = standardGrade.Code;
            }
        });

        return grade;
    };

    employeeEPAModel.scoreSpread = function (kpiGroup) {
        var averageWeight;

        if (!kpiGroup.HasKPIOptional && kpiGroup.IsSpreadKPI) {
            averageWeight = kpiGroup.Weight / kpiGroup.KPIs.length;
            averageWeight = utilitiesFactory.splitNumber(averageWeight, 4);

            angular.forEach(kpiGroup.KPIs, function (kpi) {
                kpi.Weight = averageWeight;
            });
        }
    };

    employeeEPAModel.checkScore = function (score, minScore, maxScore) {
        var result = score;

        if (score > maxScore) {
            result = maxScore;
        } else if (score < minScore) {
            result = minScore;
        }

        return result;
    };

    employeeEPAModel.getPeriodToUpdateFinalScore = function () {
        return employeeEPAFactory.getPeriodToUpdateFinalScore().success(function (periods) {
            employeeEPAModel.periods = periods;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    employeeEPAModel.getEmployeeToUpdateFinalScore = function (periodId, groupId, isLocal, query) {
        return employeeEPAFactory.getEmployeeToUpdateFinalScore(periodId, groupId, isLocal, query).success(function (employees) {
            employeeEPAModel.employees = employees;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    employeeEPAModel.getEPAToUpdateFinalScore = function (periodId, groupId, isLocal, employeeId) {
        return employeeEPAFactory.getEPAToUpdateFinalScore(periodId, groupId, isLocal, employeeId).success(function (epaInfos) {
            employeeEPAModel.epaInfos = epaInfos;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    employeeEPAModel.updateFinalScore = function (vm, selectedEPAs) {
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
            return employeeEPAFactory.updateFinalScore(selectedEPAs).success(function (response) {
                if (response.IsSuccessful) {
                    vm.dataSaved = true;
                    notificationFactory.success('UpdatedSuccessfully');

                    var employeeId = vm.filter.employee ? vm.filter.employee.EmployeeId : null;
                    employeeEPAFactory.getEPAToUpdateFinalScore(vm.filter.periodId, vm.filter.groupId, vm.filter.isLocal, employeeId)
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

    return employeeEPAModel;
}]);
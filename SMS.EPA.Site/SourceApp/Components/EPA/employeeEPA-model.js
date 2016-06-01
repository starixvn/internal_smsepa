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
        return employeeEPAFactory.getEPAInfoById(epaId)
        .success(function (epaInfo) {
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

    employeeEPAModel.openPopupViewComment = function (vm, comment) {
        vm.modalInstance = $modal.open({
            templateUrl: 'SourceApp/Components/EPA/Templates/modalViewComment.html',
            controller: "ModalViewCommentController as modalViewCommentCtrl",
            backdrop: 'static',
            resolve: {
                comment: function () {
                    return comment;
                }
            },
            windowClass: "modal-confirm-center"
        });

        return vm.modalInstance.result;
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
        return employeeEPAFactory.getPeriodToUpdateFinalScore()
        .success(function (periods) {
            employeeEPAModel.periods = periods;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    employeeEPAModel.getEmployeeToUpdateFinalScore = function (periodId, groupId, query) {
        return employeeEPAFactory.getEmployeeToUpdateFinalScore(periodId, groupId, query)
        .success(function (employees) {
            employeeEPAModel.employees = employees;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    employeeEPAModel.getEPAToUpdateFinalScore = function (periodId, groupId, employeeId) {
        return employeeEPAFactory.getEPAToUpdateFinalScore(periodId, groupId, employeeId)
        .success(function (epaInfos) {
            employeeEPAModel.epaInfos = epaInfos;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    employeeEPAModel.updateFinalScore = function (vm) {
        var criteria = {
            PeriodId: vm.filter.periodId,
            GroupId: vm.filter.groupId,
            EmployeeId: vm.filter.employee ? vm.filter.employee.EmployeeId : null
        };

        return employeeEPAFactory.updateFinalScore(criteria)
        .success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("UpdatedSuccessfully");

                employeeEPAFactory.getEPAToUpdateFinalScore(criteria.PeriodId, criteria.GroupId, criteria.EmployeeId)
                .success(function (epaInfos) {
                        angular.forEach(vm.epaInfos, function(epaInfo) {
                            angular.forEach(epaInfos, function (newEpaInfo) {
                                if (newEpaInfo.EPAId === epaInfo.EPAId) {
                                    epaInfo.NewFinalScore = newEpaInfo.FinalScore;
                                }
                            });
                        });
                    }).error(function (response, status) {
                    errorFactory.errorHandle(status);
                });
            } else {
                notificationFactory.error("", response.Message);
            }
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    return employeeEPAModel;
}]);
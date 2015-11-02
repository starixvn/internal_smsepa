'use strict';
/*global epaModule*/

epaModule.factory('EmployeeEPAModel',
['$modal', 'employeeEPAFactory', 'utilitiesFactory', 'notificationFactory', 'errorFactory',
function ($modal, employeeEPAFactory, utilitiesFactory, notificationFactory, errorFactory) {
    var employeeEPAModel = this;

    employeeEPAModel.epaInfo = {};

    employeeEPAModel.getEPAInfoById = function (epaId) {
        return employeeEPAFactory.getEPAInfoById(epaId)
        .success(function (epaInfo) {
            employeeEPAModel.epaInfo = epaInfo;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    employeeEPAModel.getNotificationInputScore = function (minScore, maxScore, message) {
        return utilitiesFactory.formatString(message, utilitiesFactory.parseFloat(minScore), utilitiesFactory.parseFloat(maxScore));
    };

    employeeEPAModel.validateScoreInput = function (weight, minScore, maxScore, message) {
        var isValid = true;

        if (utilitiesFactory.parseFloat(weight) > utilitiesFactory.parseFloat(maxScore)
            || utilitiesFactory.parseFloat(weight) < utilitiesFactory.parseFloat(minScore)) {
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
        var weight, averageWeight;

        if (!kpiGroup.HasKPIOptional && kpiGroup.IsSpreadKPI) {
            averageWeight = utilitiesFactory.parseFloat(kpiGroup.Weight / kpiGroup.KPIs.length);

            angular.forEach(kpiGroup.KPIs, function (kpi) {
                kpi.Weight = averageWeight;
            });

            if ((averageWeight * kpiGroup.KPIs.length) > kpiGroup.Weight) {
                weight = averageWeight - ((averageWeight * kpiGroup.KPIs.length) - kpiGroup.Weight);
                kpiGroup.KPIs[kpiGroup.KPIs.length - 1].Weight = utilitiesFactory.parseFloat(weight);
            } else if ((averageWeight * kpiGroup.KPIs.length) < kpiGroup.Weight) {
                weight = averageWeight + (kpiGroup.Weight - (averageWeight * kpiGroup.KPIs.length));
                kpiGroup.KPIs[kpiGroup.KPIs.length - 1].Weight = utilitiesFactory.parseFloat(weight);
            }
        }
    };

    employeeEPAModel.checkScore = function (score, minScore, maxScore) {
        var result = score;

        if (score > maxScore) {
            result = maxScore;
        } else if(score < minScore) {
            result = minScore;
        }

        return result;
    };

    return employeeEPAModel;
}]);
'use strict';
/*global epaModule*/

epaModule.factory('SelfEvalutorModel',
['$modal', 'EmployeeEPAModel', 'employeeEPAFactory', 'utilitiesFactory', 'notificationFactory', 'errorFactory',
function ($modal, EmployeeEPAModel, employeeEPAFactory, utilitiesFactory, notificationFactory, errorFactory) {
    var selfEvalutorModel = this;

    selfEvalutorModel.myEPAs = [];
    selfEvalutorModel.groups = [];
    selfEvalutorModel.members = [];
    selfEvalutorModel.memberEPAs = [];
    selfEvalutorModel.epaInfo = {};
    selfEvalutorModel.permission = 0;

    // common
    selfEvalutorModel.calculateScore = function (vm) {
        var totalScore = 0;

        angular.forEach(vm.epaInfo.KPIGroups, function (kpiGroup) {
            if (kpiGroup.HasKPIOptional) {
                selfEvalutorModel.scoreSpreadForKPIGroupHasKPIOptional(kpiGroup, vm.maxStar);
            }

            angular.forEach(kpiGroup.KPIs, function (kpi) {
                var score = utilitiesFactory.splitNumber(kpi.CurrentEvaluation.Score, 4),
                    kpiWeight = utilitiesFactory.splitNumber(kpi.Weight, 2),
                    scoreRating = utilitiesFactory.splitNumber(kpi.CurrentEvaluation.Score, 2);

                score = score ? parseFloat(score) : 0;

                if ((score > 0 && scoreRating === kpiWeight) || (kpi.CurrentEvaluation.Rate === vm.maxStar)) {
                    kpi.CurrentEvaluation.IsMaxScore = true;
                    totalScore += kpi.Weight;
                } else {
                    kpi.CurrentEvaluation.IsMaxScore = false;
                    totalScore += score;
                }
            });
        });

        vm.epaInfo.Process.CurrentProcess.TotalScore = utilitiesFactory.parseFloat(totalScore);
        var maxTotalScore = vm.epaInfo.Process.CurrentProcess.MaxTotalScore;
        vm.epaInfo.Process.CurrentProcess.TotalScore = EmployeeEPAModel.checkScore(vm.epaInfo.Process.CurrentProcess.TotalScore, 0, maxTotalScore);

        vm.finalScore = utilitiesFactory.parseFloat(totalScore + vm.epaInfo.DisciplineScore);
        vm.finalScore = EmployeeEPAModel.checkScore(vm.finalScore, 0, vm.epaInfo.Process.CurrentProcess.MaxPerformanceScore);

        vm.epaInfo.Grade = EmployeeEPAModel.convertScoreToGrade(vm.finalScore, vm.standardGrades);
    };

    selfEvalutorModel.scoreSpreadForKPIGroupHasKPIOptional = function (kpiGroup, maxStar) {
        var score, averageWeight, totalStarEvaluated = 0,
            numberKPIOptional = kpiGroup.NumberKPIOptional;
        var kpis = [];

        angular.forEach(kpiGroup.KPIs, function (kpi) {
            if (kpi.CurrentEvaluation.Rate > 0) {
                kpis.push(kpi);
                totalStarEvaluated += kpi.CurrentEvaluation.Rate;
            }

            if (!kpi.IsRequiredEvaluate && kpi.CurrentEvaluation.Rate > 0) {
                numberKPIOptional -= 1;
            }
        });

        var totalKPIMustEvaluate = kpiGroup.KPIs.length - numberKPIOptional;
        averageWeight = kpiGroup.Weight / totalKPIMustEvaluate;
        averageWeight = utilitiesFactory.splitNumber(averageWeight, 4);

        angular.forEach(kpis, function (kpi) {
            kpi.Weight = averageWeight;
            score = (averageWeight * kpi.CurrentEvaluation.Rate) / maxStar;
            kpi.CurrentEvaluation.Score = utilitiesFactory.splitNumber(score, 4);
        });
    };

    selfEvalutorModel.openPopupNotificationInvalidComment = function (vm, message) {
        vm.modalInstance = $modal.open({
            templateUrl: 'SourceApp/Components/EPA/Templates/modalNotification.html',
            controller: "ModalNotificationController as modalNotificationCtrl",
            backdrop: 'static',
            resolve: {
                message: function () {
                    return message;
                }
            },
            windowClass: "modal-confirm-center"
        });

        return vm.modalInstance.result;
    };

    selfEvalutorModel.validateEPA = function (epaInfo) {
        var isValid = true;

        angular.forEach(epaInfo.KPIGroups, function (kpiGroup) {
            angular.forEach(kpiGroup.KPIs, function (kpi) {
                var score = utilitiesFactory.parseFloat(kpi.CurrentEvaluation.Score);

                if (kpi.IsRequiredEvaluate && (score <= 0)) {
                    isValid = false;
                    kpi.CurrentEvaluation.EvaluateInvalid = true;
                } else {
                    kpi.CurrentEvaluation.EvaluateInvalid = false;
                }

                if (kpi.HasComment && (!kpi.CurrentEvaluation.Comment || kpi.CurrentEvaluation.Comment.length === 0)
                    && (kpi.IsRequiredComment || (epaInfo.RequiredCommentOnMaxScore && !kpi.IsRequiredComment && kpi.CurrentEvaluation.IsMaxScore))) {
                    isValid = false;
                    kpi.CurrentEvaluation.CommentInvalid = true;
                    kpi.ExpandedComment = true;
                } else {
                    kpi.CurrentEvaluation.CommentInvalid = false;
                }
            });
        });

        return isValid;
    };

    // self
    selfEvalutorModel.getUserPermissionOnEmployee = function () {
        return employeeEPAFactory.getUserPermissionOnEmployee()
        .success(function (permission) {
            selfEvalutorModel.permission = parseInt(permission);
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    selfEvalutorModel.getEPAByEmployee = function () {
        return employeeEPAFactory.getEPAByEmployee()
        .success(function (myEPAs) {
            selfEvalutorModel.myEPAs = myEPAs;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    selfEvalutorModel.getEPAInfoByEmployee = function (epaId) {
        return employeeEPAFactory.getEPAInfoByEmployee(epaId)
        .success(function (epaInfo) {
            selfEvalutorModel.epaInfo = epaInfo;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    selfEvalutorModel.updateEPAScoreByEmployee = function (vm) {
        var criteria = {
            EPAId: vm.epaInfo.EPAId,
            EmployeeId: vm.epaInfo.Employee.EmployeeId,
            EPAProcess: vm.epaInfo.Process.CurrentProcess,
            KPIGroups: vm.epaInfo.KPIGroups
        };

        return employeeEPAFactory.updateEPAScoreByEmployee(criteria)
        .success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("UpdatedSuccessfully");
                vm.dataSaved = true;
            }
            else {
                notificationFactory.error("", response.Message);
            }
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    // evaluator
    selfEvalutorModel.getUserPermissionOnEvaluator = function () {
        return employeeEPAFactory.getUserPermissionOnEvaluator()
        .success(function (permission) {
            selfEvalutorModel.permission = parseInt(permission);
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    selfEvalutorModel.getGroupByEvaluator = function () {
        return employeeEPAFactory.getGroupByEvaluator()
        .success(function (groups) {
            selfEvalutorModel.groups = groups;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    selfEvalutorModel.getMemberByEvaluator = function (groupId, isLocal, isMonthly) {
        return employeeEPAFactory.getMemberByEvaluator(groupId, isLocal, isMonthly)
        .success(function (members) {
            selfEvalutorModel.members = members;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    selfEvalutorModel.getEPAByEvaluator = function (groupId, isLocal, isMonthly, employeeIds) {
        return employeeEPAFactory.getEPAByEvaluator(groupId, isLocal, isMonthly, employeeIds)
        .success(function (memberEPAs) {
            selfEvalutorModel.memberEPAs = memberEPAs;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    selfEvalutorModel.getEPAInfoByEvaluator = function (epaId, evaluatorId) {
        return employeeEPAFactory.getEPAInfoByEvaluator(epaId, evaluatorId)
        .success(function (epaInfo) {
            selfEvalutorModel.epaInfo = epaInfo;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    selfEvalutorModel.updateEPAScoreByEvaluator = function (vm) {
        var criteria = {
            EPAId: vm.epaInfo.EPAId,
            EmployeeId: vm.epaInfo.Employee.EmployeeId,
            EPAProcess: vm.epaInfo.Process.CurrentProcess,
            KPIGroups: vm.epaInfo.KPIGroups
        };

        return employeeEPAFactory.updateEPAScoreByEvaluator(criteria)
        .success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("UpdatedSuccessfully");
                vm.dataSaved = true;
            }
            else {
                notificationFactory.error("", response.Message);
            }
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    return selfEvalutorModel;
}]);
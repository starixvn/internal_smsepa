'use strict';
/*global epaModule*/

epaModule.factory('SelfEvalutorModel',
['$modal', 'EmployeeEPAModel', 'employeeEPAFactory', 'utilitiesFactory', 'notificationFactory', 'errorFactory',
function ($modal, EmployeeEPAModel, employeeEPAFactory, utilitiesFactory, notificationFactory, errorFactory) {
    var selfEvalutorModel = this;

    selfEvalutorModel.myEPAs = [];
    selfEvalutorModel.members = [];
    selfEvalutorModel.memberEPAs = [];
    selfEvalutorModel.epaInfo = {};

    // common
    selfEvalutorModel.calculateScore = function (vm) {
        var totalScore = 0;

        angular.forEach(vm.epaInfo.KPIGroups, function (kpiGroup) {
            angular.forEach(kpiGroup.KPIs, function (kpi) {
                var score = utilitiesFactory.parseFloat(kpi.CurrentEvaluation.Score);
                totalScore += score;

                if (score > 0 && score === kpi.Weight) {
                    kpi.CurrentEvaluation.IsMaxScore = true;
                } else {
                    kpi.CurrentEvaluation.IsMaxScore = false;
                }
            });
        });

        if (vm.epaInfo.DisciplineScore < 0 && vm.epaInfo.HasDisciplineScore) {
            vm.epaInfo.DisciplineScore = 0;
        }

        vm.epaInfo.CurrentProcess.TotalScore = totalScore;
        vm.finalScore = totalScore + vm.epaInfo.DisciplineScore;

        if (vm.finalScore > vm.epaInfo.MaxFinalScore) {
            vm.finalScore = vm.epaInfo.MaxFinalScore;
        } else if (vm.finalScore < 0) {
            vm.finalScore = 0;
        }

        vm.epaInfo.Grade = EmployeeEPAModel.convertScoreToGrade(vm.finalScore, vm.standardGrades);
    };

    selfEvalutorModel.scoreSpreadForKPIGroupHasKPIOptional = function (kpiGroup, maxStar) {
        var score, averageWeight, totalKPIMustEvaluate, totalStarMustEvaluate, totalStarEvaluated = 0,
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

        totalKPIMustEvaluate = kpiGroup.KPIs.length - numberKPIOptional;
        totalStarMustEvaluate = totalKPIMustEvaluate * maxStar;
        averageWeight = utilitiesFactory.parseFloat(kpiGroup.Weight / totalKPIMustEvaluate);

        angular.forEach(kpis, function (kpi) {
            kpi.CurrentEvaluation.Score = utilitiesFactory.parseFloat(averageWeight * (kpi.CurrentEvaluation.Rate / maxStar));
        });

        if (((averageWeight * totalKPIMustEvaluate) > kpiGroup.Weight) && totalStarEvaluated === totalStarMustEvaluate) {
            score = utilitiesFactory.parseFloat(averageWeight - ((averageWeight * totalKPIMustEvaluate) - kpiGroup.Weight));
            kpis[kpis.length - 1].CurrentEvaluation.Score = score;
        } else if ((averageWeight * totalKPIMustEvaluate) < kpiGroup.Weight && totalStarEvaluated === totalStarMustEvaluate) {
            score = utilitiesFactory.parseFloat(averageWeight + (kpiGroup.Weight - (averageWeight * totalKPIMustEvaluate)));
            kpis[kpis.length - 1].CurrentEvaluation.Score = score;
        }
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
        return employeeEPAFactory.updateEPAScoreByEmployee(vm.epaInfo)
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
    selfEvalutorModel.getMemberByEvaluator = function (groupId) {
        return employeeEPAFactory.getMemberByEvaluator(groupId)
        .success(function (members) {
            selfEvalutorModel.members = members;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    selfEvalutorModel.getEPAByEvaluator = function (groupId, employeeIds) {
        return employeeEPAFactory.getEPAByEvaluator(groupId, employeeIds)
        .success(function (memberEPAs) {
            selfEvalutorModel.memberEPAs = memberEPAs;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    selfEvalutorModel.getEPAInfoByEvaluator = function (epaId) {
        return employeeEPAFactory.getEPAInfoByEvaluator(epaId)
        .success(function (epaInfo) {
            selfEvalutorModel.epaInfo = epaInfo;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    selfEvalutorModel.updateEPAScoreByEvaluator = function (vm) {
        return employeeEPAFactory.updateEPAScoreByEvaluator(vm.epaInfo)
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
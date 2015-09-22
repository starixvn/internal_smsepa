'use strict';
/*global epaModule*/

epaModule.controller('EmployeeEPAController',
['EmployeeEPAModel',
function (EmployeeEPAModel) {
    var vm = this;

    vm.myEPAs = EmployeeEPAModel.myEPAs;

    vm.viewEPADetail = function (epa) {
        vm.viewEPADetailUrl = '/SelfEvaluation/Detail?epaId=' + epa.EPAId;
    };

    vm.viewEPAResult = function (epa) {
        vm.viewEPAResultUrl = '/EPA/Detail/' + epa.EPAId;
    };

    vm.initialize = function () {
        EmployeeEPAModel.getEPAByEmployee().then(function () {
            vm.myEPAs = EmployeeEPAModel.myEPAs;
        });
    };

    vm.initialize();
}]);

epaModule.controller('EmployeeEPAInfoController',
['$window', '$translate', '$routeParams', 'EmployeeEPAModel', 'notificationFactory',
function ($window, $translate, $routeParams, EmployeeEPAModel, notificationFactory) {
    var vm = this;

    vm.epaId = parseInt($routeParams.epaId);
    vm.employeeId = parseInt($routeParams.employeeId);

    vm.maxStar = 5;
    vm.dataSaved = false;
    vm.isReadonly = false;
    vm.evaluatedDate = null;

    vm.epaInfo = EmployeeEPAModel.epaInfo;

    var calculateScore = function (epaInfo) {
        var totalScore = 0;

        angular.forEach(epaInfo.KPIGroups, function (kpiGroup) {
            angular.forEach(kpiGroup.KPIs, function (kpi) {
                if (kpi.EvaluateBySelf.Score && parseFloat(kpi.EvaluateBySelf.Score) > 0) {
                    totalScore += parseFloat(kpi.EvaluateBySelf.Score);
                }
            });
        });

        epaInfo.ProcessBySelf.TotalScore = totalScore.toFixed(2);

        if (epaInfo.HasAttendanceScore) {
            vm.finalScore = totalScore + epaInfo.AttendanceScore;
        }
        else {
            vm.finalScore = totalScore;
        }
    };

    var checkStatus = function (epaInfo) {
        if (epaInfo.ProcessBySelf.IsEndProcess) {
            vm.isReadonly = true;
            vm.hoveringOver = null;
            vm.hoveringLeave = null;
        }
        else {
            vm.hoveringOver = function (value, kpi) {
                kpi.EvaluateBySelf.Percent = (100 * value) / vm.maxStar;
            };

            vm.hoveringLeave = function (kpiGroup, kpi) {
                kpi.EvaluateBySelf.Percent = (100 * kpi.EvaluateBySelf.Rate) / vm.maxStar;

                if (kpi.EvaluateBySelf.Percent > 0) {
                    if (kpiGroup.TotalKPIOptional === 0) {
                        var score = kpi.Weight * (kpi.EvaluateBySelf.Percent / 100);

                        kpi.EvaluateBySelf.Score = (score % 1) === 0 ? score : score.toFixed(2);
                    }
                    else {
                        var weight, totalKPIOptional = angular.copy(kpiGroup.TotalKPIOptional);

                        angular.forEach(kpiGroup.KPIs, function (item) {
                            if (!item.IsRequiredEvaluate && item.EvaluateBySelf.Rate > 0) {
                                totalKPIOptional -= 1;
                            }
                        });

                        weight = kpiGroup.Weight / (kpiGroup.KPIs.length - totalKPIOptional);

                        angular.forEach(kpiGroup.KPIs, function (item) {
                            if (item.EvaluateBySelf.Rate > 0) {
                                var score = weight * (item.EvaluateBySelf.Percent / 100);

                                item.EvaluateBySelf.Score = (score % 1) === 0 ? score : score.toFixed(2);
                            }
                        });
                    }

                    calculateScore(vm.epaInfo);
                }
            };
        }
    };

    var validateEPA = function (epaInfo) {
        var isValid = true;

        angular.forEach(epaInfo.KPIGroups, function (kpiGroup) {
            angular.forEach(kpiGroup.KPIs, function (kpi) {
                if (kpi.IsRequiredEvaluate
                    && (!kpi.EvaluateBySelf.Score || parseFloat(kpi.EvaluateBySelf.Score) <= 0)) {
                    isValid = false;
                    kpi.EvaluateBySelf.EvaluateValid = true;
                }
                else {
                    kpi.EvaluateBySelf.EvaluateValid = false;
                }

                if (kpi.HasComment && kpi.IsRequiredComment && (!kpi.EvaluateBySelf.Comment || kpi.EvaluateBySelf.Comment.length === 0)) {
                    isValid = false;
                    kpi.EvaluateBySelf.CommentValid = true;
                }
                else {
                    kpi.EvaluateBySelf.CommentValid = false;
                }
            });
        });

        if (!isValid) {
            notificationFactory.error("EvaluateEPAInvalid");
        }

        return isValid;
    };

    vm.getClassProgress = function (statusEndProcess) {
        return EmployeeEPAModel.getClassProgress(statusEndProcess);
    };

    vm.cancelRating = function (kpiGroup, kpi) {
        kpi.EvaluateBySelf.Rate = 0;
        kpi.EvaluateBySelf.Percent = 0;
        kpi.EvaluateBySelf.Score = null;

        if (kpiGroup.TotalKPIOptional > 0) {
            var weight, totalKPIOptional = angular.copy(kpiGroup.TotalKPIOptional);

            angular.forEach(kpiGroup.KPIs, function (item) {
                if (!item.IsRequiredEvaluate && item.EvaluateBySelf.Rate > 0) {
                    totalKPIOptional -= 1;
                }
            });

            weight = kpiGroup.Weight / (kpiGroup.KPIs.length - totalKPIOptional);

            angular.forEach(kpiGroup.KPIs, function (item) {
                if (item.EvaluateBySelf.Rate > 0) {
                    var score = weight * (item.EvaluateBySelf.Percent / 100);

                    item.EvaluateBySelf.Score = (score % 1) === 0 ? score : score.toFixed(2);
                }
            });
        }

        calculateScore(vm.epaInfo);
    };

    vm.getNotificationInputScore = function (kpi) {
        return EmployeeEPAModel.getNotificationInputScore(kpi.Weight, vm.scoreInputValidMessage);
    };

    vm.inputScore = function (kpi) {
        kpi.EvaluateBySelf.Rate = 0;
        kpi.EvaluateBySelf.Percent = null;

        if (!EmployeeEPAModel.validateScoreInput(kpi.EvaluateBySelf.Score, kpi.Weight, vm.scoreInputValidMessage)) {
            kpi.EvaluateBySelf.Score = null;
        }
        else {
            calculateScore(vm.epaInfo);
        }
    };

    vm.save = function (epaInfo) {
        if (validateEPA(epaInfo)) {
            EmployeeEPAModel.updateEPAScoreByEmployee(vm);
        }
    };

    vm.cancel = function () {
        $window.history.back();
    };

    vm.initialize = function () {
        $translate('ScoreInputValid').then(function (message) {
            vm.scoreInputValidMessage = message;
        });

        EmployeeEPAModel.getEPAInfoByEmployee(vm.epaId, vm.employeeId).then(function () {
            var data = EmployeeEPAModel.epaInfo;
            var totalScoreOfSelf = data.ProcessBySelf.TotalScore ? parseFloat(data.ProcessBySelf.TotalScore) : 0;

            if (data.HasAttendanceScore) {
                vm.finalScore = totalScoreOfSelf !== 0 ? (totalScoreOfSelf + data.AttendanceScore) : data.AttendanceScore;
            }
            else {
                vm.finalScore = totalScoreOfSelf;
            }
            vm.evaluatedDate = data.ProcessBySelf.EvaluateDate ? data.ProcessBySelf.EvaluateDate : null;
            vm.epaInfo = data;
            checkStatus(vm.epaInfo);

            angular.forEach(vm.epaInfo.KPIGroups, function (kpiGroup) {
                if (!EmployeeEPAModel.checkKPIOptional(kpiGroup) && kpiGroup.IsSpreadKPI) {
                    var weight = kpiGroup.Weight / kpiGroup.KPIs.length;

                    angular.forEach(kpiGroup.KPIs, function (kpi) {
                        if ((weight % 1) === 0) {
                            kpi.Weight = weight;
                        }
                        else {
                            kpi.Weight = weight.toFixed(2);
                        }
                    });
                }
            });
        });
    };

    vm.initialize();
}]);
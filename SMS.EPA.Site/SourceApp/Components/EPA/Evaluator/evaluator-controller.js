'use strict';
/*global epaModule*/

epaModule.controller('EvaluatorController',
['EmployeeEPAModel', 'CommonModel', 'utilitiesFactory',
function (EmployeeEPAModel, CommonModel, utilitiesFactory) {
    var vm = this;

    vm.memberEPAs = EmployeeEPAModel.memberEPAs;
    vm.members = EmployeeEPAModel.members;

    vm.filter = {
        groupId: '',
        employeeIds: ''
    };

    vm.getMember = function (groupId) {
        EmployeeEPAModel.getMemberByEvaluator(groupId).then(function () {
            vm.members = EmployeeEPAModel.members;
        });
    };

    var getEPAByEvaluator = function (groupId, employeeIds) {
        EmployeeEPAModel.getEPAByEvaluator(groupId, employeeIds).then(function () {
            vm.memberEPAs = EmployeeEPAModel.memberEPAs;
        });
    };

    vm.query = function (filter) {
        filter.employeeIds = utilitiesFactory.getSelectedData(vm.members, 'EmployeeId');
        getEPAByEvaluator(filter.groupId, filter.employeeIds);
    };

    vm.viewEPADetail = function (memberEPA) {
        vm.viewEPADetailUrl = '/Evaluator/Detail?epaId=' + memberEPA.EPAId;
    };

    vm.viewEPAResult = function (memberEPA) {
        vm.viewEPAResultUrl = '/EPA/Detail/' + memberEPA.EPAId;
    };

    vm.initialize = function () {
        CommonModel.getAllGroup().then(function () {
            vm.groupOptions = CommonModel.groups;
        });

        vm.getMember(vm.filter.groupId);

        vm.query(vm.filter);
    };

    vm.initialize();
}]);

epaModule.controller('MemberEPAInfoOfEvaluatorController',
['$window', '$translate', '$routeParams', 'EmployeeEPAModel', 'CommonModel', 'notificationFactory',
function ($window, $translate, $routeParams, EmployeeEPAModel, CommonModel, notificationFactory) {
    var vm = this;

    vm.epaId = parseInt($routeParams.epaId);

    vm.maxStar = 5;
    vm.dataSaved = false;
    vm.isReadonly = false;
    vm.evaluatedDate = null;

    var calculateScore = function (epaInfo) {
        var totalScore = 0;

        angular.forEach(epaInfo.KPIGroups, function (kpiGroup) {
            angular.forEach(kpiGroup.KPIs, function (kpi) {
                if (kpi.EvaluateByEvaluators[0].Score && parseFloat(kpi.EvaluateByEvaluators[0].Score) > 0) {
                    totalScore += parseFloat(kpi.EvaluateByEvaluators[0].Score);
                }
            });
        });

        epaInfo.ProcessByEvaluators[0].TotalScore = totalScore.toFixed(2);

        if (epaInfo.HasAttendanceScore) {
            vm.finalScore = totalScore + epaInfo.AttendanceScore;
        }
        else {
            vm.finalScore = totalScore;
        }
    };

    var checkStatus = function (epaInfo) {
        if (epaInfo.ProcessByEvaluators[0].IsEndProcess) {
            vm.isReadonly = true;
            vm.hoveringOver = null;
            vm.hoveringLeave = null;
        }
        else {
            vm.hoveringOver = function (value, kpi) {
                kpi.EvaluateByEvaluators[0].Percent = (100 * value) / vm.maxStar;
            };

            vm.hoveringLeave = function (kpiGroup, kpi) {
                kpi.EvaluateByEvaluators[0].Percent = (100 * kpi.EvaluateByEvaluators[0].Rate) / vm.maxStar;

                if (kpi.EvaluateByEvaluators[0].Percent > 0) {
                    if (kpiGroup.TotalKPIOptional === 0) {
                        var score = kpi.Weight * (kpi.EvaluateByEvaluators[0].Percent / 100);

                        kpi.EvaluateByEvaluators[0].Score = (score % 1) === 0 ? score : score.toFixed(2);
                    }
                    else {
                        var weight, totalKPIOptional = angular.copy(kpiGroup.TotalKPIOptional);

                        angular.forEach(kpiGroup.KPIs, function (item) {
                            if (!item.IsRequiredEvaluate && item.EvaluateByEvaluators[0].Rate > 0) {
                                totalKPIOptional -= 1;
                            }
                        });

                        weight = kpiGroup.Weight / (kpiGroup.KPIs.length - totalKPIOptional);

                        angular.forEach(kpiGroup.KPIs, function (item) {
                            if (item.EvaluateByEvaluators[0].Rate > 0) {
                                var score = weight * (item.EvaluateByEvaluators[0].Percent / 100);

                                item.EvaluateByEvaluators[0].Score = (score % 1) === 0 ? score : score.toFixed(2);
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
                    && (!kpi.EvaluateByEvaluators[0].Score || parseFloat(kpi.EvaluateByEvaluators[0].Score) <= 0)) {
                    isValid = false;
                    kpi.EvaluateByEvaluators[0].EvaluateValid = true;
                }
                else {
                    kpi.EvaluateByEvaluators[0].EvaluateValid = false;
                }

                if (kpi.HasComment
                    && kpi.IsRequiredComment
                    && (!kpi.EvaluateByEvaluators[0].Comment || kpi.EvaluateByEvaluators[0].Comment.length === 0)) {
                    isValid = false;
                    kpi.EvaluateByEvaluators[0].CommentValid = true;
                }
                else {
                    kpi.EvaluateByEvaluators[0].CommentValid = false;
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

    vm.getNotificationInputScore = function (kpi) {
        return EmployeeEPAModel.getNotificationInputScore(kpi.Weight, vm.scoreInputValidMessage);
    };

    vm.inputScore = function (kpi) {
        kpi.EvaluateByEvaluators[0].Rate = 0;
        kpi.EvaluateByEvaluators[0].Percent = null;

        if (!EmployeeEPAModel.validateScoreInput(kpi.EvaluateByEvaluators[0].Score, kpi.Weight, vm.scoreInputValidMessage)) {
            kpi.EvaluateByEvaluators[0].Score = null;
        }
        else {
            calculateScore(vm.epaInfo);
        }
    };

    vm.cancelRating = function (kpiGroup, kpi) {
        kpi.EvaluateByEvaluators[0].Rate = 0;
        kpi.EvaluateByEvaluators[0].Percent = 0;
        kpi.EvaluateByEvaluators[0].Score = null;

        if (kpiGroup.TotalKPIOptional > 0) {
            var weight, totalKPIOptional = angular.copy(kpiGroup.TotalKPIOptional);

            angular.forEach(kpiGroup.KPIs, function (item) {
                if (!item.IsRequiredEvaluate && item.EvaluateByEvaluators[0].Rate > 0) {
                    totalKPIOptional -= 1;
                }
            });

            weight = kpiGroup.Weight / (kpiGroup.KPIs.length - totalKPIOptional);

            angular.forEach(kpiGroup.KPIs, function (item) {
                if (item.EvaluateByEvaluators[0].Rate > 0) {
                    var score = weight * (item.EvaluateByEvaluators[0].Percent / 100);

                    item.EvaluateByEvaluators[0].Score = (score % 1) === 0 ? score : score.toFixed(2);
                }
            });
        }

        calculateScore(vm.epaInfo);
    };

    vm.cancel = function () {
        $window.history.back();
    };

    vm.save = function (epaInfo) {
        if (validateEPA(epaInfo)) {
            EmployeeEPAModel.updateEPAScoreByEvaluator(vm);
        }
    };

    vm.initialize = function () {
        $translate('ScoreInputValid').then(function (message) {
            vm.scoreInputValidMessage = message;
        });

        EmployeeEPAModel.getEPAInfoByEvaluator(vm.epaId, vm.employeeId, vm.evaluatorId).then(function () {
            var data = EmployeeEPAModel.epaInfo;
            var totalScoreOfEvaluator = data.ProcessByEvaluators[0].TotalScore ? parseFloat(data.ProcessByEvaluators[0].TotalScore) : 0;

            vm.evaluatedDate = data.ProcessByEvaluators[0].EvaluateDate ? data.ProcessByEvaluators[0].EvaluateDate : null;

            if (data.HasAttendanceScore) {
                vm.finalScore = totalScoreOfEvaluator !== 0 ? (totalScoreOfEvaluator + data.AttendanceScore) : data.AttendanceScore;
            }
            else {
                vm.finalScore = totalScoreOfEvaluator;
            }
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
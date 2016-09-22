'use strict';
/*global epaModule*/

epaModule.controller('EvaluatorController',
['EmployeeEPAModel', 'SelfEvalutorModel', 'CommonModel', 'utilitiesFactory', 'historyFactory', 'lockScreenService', 'baseUrl',
function (EmployeeEPAModel, SelfEvalutorModel, CommonModel, utilitiesFactory, historyFactory, lockScreenService, baseUrl) {
    var vm = this;

    vm.memberEPAs = [];
    vm.members = [];
    vm.standardGrades = CommonModel.standardGrades;
    vm.targetTypeEnum = utilitiesFactory.targetType;
    vm.sortBySelfScore = { ascending: false, descending: false };
    vm.filter = {};
    vm.monthlyFilterSelected = false;

    function initFilter() {
        var filter = historyFactory.getState();
        return filter ? filter : {
            groupId: '',
            isLocal: '',
            isMonthly: false,
            employeeIds: ''
        };
    }

    function sortBySelfScore(epaInfo1, epaInfo2) {
        var tempa = epaInfo1.Process.Self.TotalScore ? parseFloat(epaInfo1.Process.Self.TotalScore) : 0,
            tempb = epaInfo2.Process.Self.TotalScore ? parseFloat(epaInfo2.Process.Self.TotalScore) : 0;

        if (epaInfo1.HasMonthlyEPA && epaInfo1.Period.TargetTypeId === vm.targetTypeEnum.SemiAnnual) {
            tempa = epaInfo1.MonthlyEPA.AverageSelfScoreConverted;
        }

        if (epaInfo2.HasMonthlyEPA && epaInfo2.Period.TargetTypeId === vm.targetTypeEnum.SemiAnnual) {
            tempb = epaInfo2.MonthlyEPA.AverageSelfScoreConverted;
        }

        return tempa - tempb;
    }

    vm.changeSorting = function () {
        if (!vm.sortBySelfScore.ascending) {
            vm.sortBySelfScore.ascending = true;
            vm.sortBySelfScore.descending = false;
            vm.memberEPAs.sort(sortBySelfScore);
        } else {
            vm.sortBySelfScore.ascending = false;
            vm.sortBySelfScore.descending = true;
            vm.memberEPAs.reverse();
        }
    };

    vm.getMember = function () {
        SelfEvalutorModel.getMemberByEvaluator(vm.filter.groupId, vm.filter.isLocal, vm.filter.isMonthly).then(function () {
            vm.members = SelfEvalutorModel.members;
            utilitiesFactory.setSelectedItem(vm.members, "EmployeeId", vm.filter.employeeIds);
        });
    };

    vm.selectedCls = function () {
        if (vm.sortBySelfScore.ascending) {
            return 'sorting-asc';
        } else if (vm.sortBySelfScore.descending) {
            return 'sorting-desc';
        }
    };

    function getEPAByEvaluator(groupId, isLocal, isMonthly, employeeIds) {
        lockScreenService.start();
        vm.sortBySelfScore = { ascending: false, descending: false };

        SelfEvalutorModel.getEPAByEvaluator(groupId, isLocal, isMonthly, employeeIds).then(function () {
            vm.memberEPAs = SelfEvalutorModel.memberEPAs;

            angular.forEach(vm.memberEPAs, function (memberEPA) {
                if (memberEPA.HasMonthlyEPA && memberEPA.Period.TargetTypeId === vm.targetTypeEnum.SemiAnnual) {
                    memberEPA.Process.Self.TotalScore = memberEPA.MonthlyEPA.AverageSelfScoreConverted;
                    memberEPA.FinalScore = utilitiesFactory.parseFloat(memberEPA.MonthlyEPA.AverageEvaluatorScoreConverted) + memberEPA.DisciplineScore;
                } else {
                    memberEPA.Process.Self.TotalScore = utilitiesFactory.parseFloat(memberEPA.Process.Self.TotalScore);
                    memberEPA.FinalScore = utilitiesFactory.parseFloat(memberEPA.Process.CurrentProcess.TotalScore) + memberEPA.DisciplineScore;
                }
                memberEPA.FinalScore = EmployeeEPAModel.checkScore(memberEPA.FinalScore, 0, memberEPA.Process.CurrentProcess.MaxPerformanceScore);

                if (memberEPA.LastFinalScore.length > 0) {
                    memberEPA.LastFinalScore = utilitiesFactory.parseFloat(memberEPA.LastFinalScore);
                } else {
                    memberEPA.LastFinalScore = null;
                }

                memberEPA.Grade = EmployeeEPAModel.convertScoreToGrade(memberEPA.FinalScore, vm.standardGrades);
            });

            if (isMonthly) {
                vm.monthlyFilterSelected = true;
            } else {
                vm.monthlyFilterSelected = false;
            }

            lockScreenService.stop();
        });

        historyFactory.saveState(vm.filter);
    }

    vm.query = function (filter) {
        filter.employeeIds = utilitiesFactory.getSelectedData(vm.members, 'EmployeeId');
        getEPAByEvaluator(filter.groupId, filter.isLocal, filter.isMonthly, filter.employeeIds);
    };

    vm.viewEPADetail = function (memberEPA) {
        vm.viewEPADetailUrl = baseUrl + 'Evaluator/Detail/' + memberEPA.EPAId;
    };

    vm.viewEPAResult = function (memberEPA) {
        if (memberEPA.HasMonthlyEPA && memberEPA.Period.TargetTypeId === vm.targetTypeEnum.SemiAnnual) {
            vm.viewEPAResultUrl = baseUrl + 'EPA/SemiDetail/' + memberEPA.EPAId;
        } else {
            vm.viewEPAResultUrl = baseUrl + 'EPA/Detail/' + memberEPA.EPAId;
        }
    };

    vm.viewOtherViolationDetail = function (memberEPA) {
        CommonModel.openPopupViewOtherViolationDetail(memberEPA.EPAId);
    };

    vm.viewEPAHistory = function (employeeId) {
        vm.viewEPAHistoryUrl = baseUrl + 'EPAHistory/Index?empId=' + employeeId;
    };

    function initialize() {
        vm.filter = initFilter();
        SelfEvalutorModel.getGroupByEvaluator().then(function () {
            vm.groupOptions = SelfEvalutorModel.groups;
        });

        vm.getMember();

        CommonModel.getStandardGrade().then(function () {
            vm.standardGrades = CommonModel.standardGrades;
            getEPAByEvaluator(vm.filter.groupId, vm.filter.isLocal, vm.filter.isMonthly, vm.filter.employeeIds);
        });
    }

    initialize();
}]);

epaModule.controller('MemberEPAInfoOfEvaluatorController',
['$timeout', '$translate', '$routeParams', 'EmployeeEPAModel', 'SelfEvalutorModel', 'CommonModel', 'utilitiesFactory', 'historyFactory', 'baseUrl',
function ($timeout, $translate, $routeParams, EmployeeEPAModel, SelfEvalutorModel, CommonModel, utilitiesFactory, historyFactory, baseUrl) {
    var vm = this;

    vm.permission = 0;
    vm.epaId = parseInt($routeParams.id);
    vm.evaluatorId = parseInt($routeParams.evaluatorId);
    vm.maxStar = utilitiesFactory.maxStar;
    vm.dataSaved = false;
    vm.evaluatedDate = null;
    vm.standardGrades = CommonModel.standardGrades;
    vm.policyEnum = utilitiesFactory.policy;
    vm.targetTypeEnum = utilitiesFactory.targetType;
    vm.statusEndProcessEnum = utilitiesFactory.statusEndProcess;
    vm.ratingTypeEnum = utilitiesFactory.ratingType;
    vm.hasRatingByStar = false;
    vm.showRatingScoreByStar = true;

    vm.viewBySelf = function () {
        return !vm.evaluatorId;
    };

    vm.viewBackButton = function () {
        return !$routeParams.newTab;
    };

    vm.initCollapseExpandCommentByEmployee = function (epaInfo, kpi) {
        if (epaInfo.Process.Self.IsEndProcess && (kpi.EvaluationBySelf.Comment && kpi.EvaluationBySelf.Comment.length > 0)) {
            kpi.ExpandedCommentByEmployee = true;
        }
        else {
            kpi.ExpandedCommentByEmployee = false;
        }
    };

    vm.getClassProgress = function (statusEndProcess) {
        return EmployeeEPAModel.getClassProgress(statusEndProcess);
    };

    vm.getNotificationInputScore = function (kpi) {
        var weight = utilitiesFactory.splitNumber(kpi.Weight, 2);
        return EmployeeEPAModel.getNotificationInputScore(0, weight, vm.scoreInputValidMessage);
    };

    vm.inputScore = function (kpi) {
        var weight = utilitiesFactory.splitNumber(kpi.Weight, 2),
            score = kpi.CurrentEvaluation.Score ? parseFloat(kpi.CurrentEvaluation.Score) : 0;

        score = utilitiesFactory.splitNumber(score, 2);
        kpi.CurrentEvaluation.Rate = 0;

        if (!EmployeeEPAModel.validateScoreInput(score, 0, weight, vm.scoreInputValidMessage)) {
            kpi.CurrentEvaluation.Score = null;
        }

        SelfEvalutorModel.calculateScore(vm, false);
    };

    vm.cancelRating = function (kpiGroup, kpi) {
        kpi.CurrentEvaluation.Rate = 0;
        kpi.CurrentEvaluation.Score = null;
        $("#star_" + kpi.KPIId).rating('update', kpi.CurrentEvaluation.Rate);

        if (kpiGroup.HasKPIOptional) {
            SelfEvalutorModel.scoreSpreadForKPIGroupHasKPIOptional(kpiGroup, vm.maxStar);
        }

        SelfEvalutorModel.calculateScore(vm, false);
    };

    vm.cancel = function () {
        historyFactory.back();
    };

    vm.save = function (epaInfo) {
        if (SelfEvalutorModel.validateEPA(epaInfo)) {
            SelfEvalutorModel.updateEPAScoreByEvaluator(vm);
        }
        else {
            SelfEvalutorModel.openPopupNotificationInvalidComment(vm, vm.evaluateEPAInvalidMessage);
        }
    };

    vm.generateRatingStar = function () {
        $timeout(function () {
            $(".rating-loading").rating();
        }, 1);
    };

    function checkStatus(epaInfo) {
        if (!epaInfo.IsAllowedToEdit) {
            vm.hoveringLeave = null;
        }
        else {
            vm.hoveringLeave = function (kpiGroup, kpi) {
                if (kpi.CurrentEvaluation.Rate > 0) {
                    if (kpiGroup.NumberKPIOptional === 0) {
                        var score = (kpi.Weight * kpi.CurrentEvaluation.Rate) / vm.maxStar;
                        score = utilitiesFactory.splitNumber(score, 4);
                        kpi.CurrentEvaluation.Score = score;
                    }
                    else {
                        SelfEvalutorModel.scoreSpreadForKPIGroupHasKPIOptional(kpiGroup, vm.maxStar);
                    }

                    SelfEvalutorModel.calculateScore(vm, false);
                }
            };
        }
    }

    function getEPAInfoByEvaluator() {
        SelfEvalutorModel.getEPAInfoByEvaluator(vm.epaId, vm.evaluatorId).then(function () {
            vm.epaInfo = SelfEvalutorModel.epaInfo;
            checkStatus(vm.epaInfo);

            if (vm.epaInfo.RatingType.RatingTypeId === vm.ratingTypeEnum.Star
                    || vm.epaInfo.RatingType.RatingTypeId === vm.ratingTypeEnum.HalfStar
                    || (vm.epaInfo.RatingType.RatingTypeId === vm.ratingTypeEnum.Score
                        && $.grep(vm.epaInfo.KPIGroups, function (e) { return e.NumberKPIOptional > 0; }).length > 0)) {
                vm.hasRatingByStar = true;
            }

            vm.epaInfo.Employee.Avatar = baseUrl + vm.epaInfo.Employee.Avatar;
            vm.epaInfo.Process.Self.TotalScore = utilitiesFactory.parseFloat(vm.epaInfo.Process.Self.TotalScore);
            vm.epaInfo.Process.CurrentProcess.TotalScore = utilitiesFactory.parseFloat(vm.epaInfo.Process.CurrentProcess.TotalScore);

            vm.finalScoreOfSelf = vm.epaInfo.Process.Self.TotalScore + vm.epaInfo.DisciplineScore;
            vm.finalScoreOfSelf = EmployeeEPAModel.checkScore(vm.finalScoreOfSelf, 0, vm.epaInfo.Process.Self.MaxPerformanceScore);
            vm.finalScore = vm.epaInfo.Process.CurrentProcess.TotalScore + vm.epaInfo.DisciplineScore;
            vm.finalScore = EmployeeEPAModel.checkScore(vm.finalScore, 0, vm.epaInfo.Process.CurrentProcess.MaxPerformanceScore);

            if (vm.epaInfo.LastFinalScore.length > 0) {
                vm.epaInfo.LastFinalScore = utilitiesFactory.parseFloat(vm.epaInfo.LastFinalScore);
            } else {
                vm.epaInfo.LastFinalScore = null;
            }

            vm.gradeOfSelf = EmployeeEPAModel.convertScoreToGrade(vm.finalScoreOfSelf, vm.standardGrades);
            vm.epaInfo.Grade = EmployeeEPAModel.convertScoreToGrade(vm.finalScore, vm.standardGrades);
            vm.evaluatedDate = vm.epaInfo.Process.CurrentProcess.EvaluateDate ? vm.epaInfo.Process.CurrentProcess.EvaluateDate : null;

            SelfEvalutorModel.getUserPermissionOnEvaluator().then(function () {
                vm.permission = SelfEvalutorModel.permission;

                if (vm.permission !== vm.policyEnum.Host) {
                    vm.epaInfo.IsAllowedToEdit = false;
                    vm.hoveringLeave = null;
                }
            });
        });
    }

    function initialize() {
        $translate('ScoreInputValid').then(function (message) {
            vm.scoreInputValidMessage = message;
        });

        $translate('EvaluateEPAInvalid').then(function (message) {
            vm.evaluateEPAInvalidMessage = message;
        });

        CommonModel.getStandardGrade().then(function () {
            vm.standardGrades = CommonModel.standardGrades;

            getEPAInfoByEvaluator();
        });
    }

    initialize();
}]);
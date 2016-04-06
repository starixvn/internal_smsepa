'use strict';
/*global epaModule*/

epaModule.controller('EvaluatorController',
['EmployeeEPAModel', 'SelfEvalutorModel', 'CommonModel', 'utilitiesFactory', 'enumerationsFactory', 'historyFactory', 'baseUrl',
function (EmployeeEPAModel, SelfEvalutorModel, CommonModel, utilitiesFactory, enumerationsFactory, historyFactory, baseUrl) {
    var vm = this;

    vm.memberEPAs = [];
    vm.members = [];
    vm.standardGrades = CommonModel.standardGrades;
    vm.targetTypeEnum = enumerationsFactory.targetType;

    vm.filter = {};

    function initFilter() {
        var filter = historyFactory.getState();
        return filter ? filter : {
            groupId: '',
            isLocal: '',
            isMonthly: false,
            employeeIds: ''
        };
    }

    vm.getMember = function () {
        SelfEvalutorModel.getMemberByEvaluator(vm.filter.groupId, vm.filter.isLocal, vm.filter.isMonthly).then(function () {
            vm.members = SelfEvalutorModel.members;
            utilitiesFactory.setSelectedItem(vm.members, "EmployeeId", vm.filter.employeeIds);
        });
    };

    function getEPAByEvaluator(groupId, isLocal, isMonthly, employeeIds) {
        SelfEvalutorModel.getEPAByEvaluator(groupId, isLocal, isMonthly, employeeIds).then(function () {
            vm.memberEPAs = SelfEvalutorModel.memberEPAs;

            angular.forEach(vm.memberEPAs, function (memberEPA) {
                memberEPA.Process.Self.TotalScore = utilitiesFactory.parseFloat(memberEPA.Process.Self.TotalScore);
                memberEPA.FinalScore = utilitiesFactory.parseFloat(memberEPA.Process.CurrentProcess.TotalScore) + memberEPA.DisciplineScore;
                memberEPA.FinalScore = EmployeeEPAModel.checkScore(memberEPA.FinalScore, 0, memberEPA.Process.CurrentProcess.MaxPerformanceScore);

                if (memberEPA.LastFinalScore.length > 0) {
                    memberEPA.LastFinalScore = utilitiesFactory.parseFloat(memberEPA.LastFinalScore);
                } else {
                    memberEPA.LastFinalScore = null;
                }

                memberEPA.Grade = EmployeeEPAModel.convertScoreToGrade(memberEPA.FinalScore, vm.standardGrades);
            });
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
['$window', '$timeout', '$translate', '$routeParams', 'EmployeeEPAModel', 'SelfEvalutorModel', 'CommonModel',
    'utilitiesFactory', 'enumerationsFactory', 'baseUrl', 'historyFactory',
function ($window, $timeout, $translate, $routeParams, EmployeeEPAModel, SelfEvalutorModel, CommonModel,
    utilitiesFactory, enumerationsFactory, baseUrl, historyFactory) {
    var vm = this;

    vm.permission = 0;
    vm.epaId = parseInt($routeParams.id);
    vm.evaluatorId = parseInt($routeParams.evaluatorId);

    vm.maxStar = utilitiesFactory.maxStar;
    vm.dataSaved = false;
    vm.evaluatedDate = null;

    vm.standardGrades = CommonModel.standardGrades;
    vm.policyEnum = enumerationsFactory.policy;
    vm.targetTypeEnum = enumerationsFactory.targetType;
    vm.statusEndProcessEnum = enumerationsFactory.statusEndProcess;
    vm.ratingTypeEnum = enumerationsFactory.ratingType;

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
                        var score = kpi.Weight * (kpi.CurrentEvaluation.Rate / vm.maxStar);
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
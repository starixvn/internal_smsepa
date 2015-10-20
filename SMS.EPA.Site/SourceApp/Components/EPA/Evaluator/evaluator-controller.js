'use strict';
/*global epaModule*/

epaModule.controller('EvaluatorController',
['EmployeeEPAModel', 'CommonModel', 'utilitiesFactory',
function (EmployeeEPAModel, CommonModel, utilitiesFactory) {
    var vm = this;

    vm.memberEPAs = EmployeeEPAModel.memberEPAs;
    vm.members = EmployeeEPAModel.members;
    vm.standardGrades = CommonModel.standardGrades;

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

            angular.forEach(vm.memberEPAs, function (memberEPA) {
                var totalScore = memberEPA.CurrentProcess.TotalScore ? parseFloat(memberEPA.CurrentProcess.TotalScore) : null,
                    disciplineScore = memberEPA.MaxDisciplineScore - memberEPA.DisciplineScore;

                if (disciplineScore < 0) {
                    disciplineScore = 0;
                }

                memberEPA.ProcessBySelf.TotalScore = memberEPA.ProcessBySelf.TotalScore ? parseFloat(memberEPA.ProcessBySelf.TotalScore) : 0;
                memberEPA.DisciplineScore = disciplineScore;

                if (memberEPA.HasDisciplineScore) {
                    memberEPA.FinalScore = totalScore + memberEPA.DisciplineScore;
                    memberEPA.ProcessBySelf.TotalScore += memberEPA.DisciplineScore;
                }
                else {
                    memberEPA.FinalScore = totalScore;
                }

                memberEPA.Grade = EmployeeEPAModel.convertScoreToGrade(memberEPA.FinalScore, memberEPA.MaxFinalScore, vm.standardGrades);
            });
        });
    };

    vm.query = function (filter) {
        filter.employeeIds = utilitiesFactory.getSelectedData(vm.members, 'EmployeeId');
        getEPAByEvaluator(filter.groupId, filter.employeeIds);
    };

    vm.viewEPADetail = function (memberEPA) {
        vm.viewEPADetailUrl = '/Evaluator/Detail/' + memberEPA.EPAId;
    };

    vm.viewEPAResult = function (memberEPA) {
        vm.viewEPAResultUrl = '/EPA/Detail/' + memberEPA.EPAId;
    };

    vm.initialize = function () {
        CommonModel.getAllGroup().then(function () {
            vm.groupOptions = CommonModel.groups;
        });

        vm.getMember(vm.filter.groupId);

        CommonModel.getStandardGrade().then(function () {
            vm.standardGrades = CommonModel.standardGrades;

            vm.query(vm.filter);
        });
    };

    vm.initialize();
}]);

epaModule.controller('MemberEPAInfoOfEvaluatorController',
['$window', '$translate', '$routeParams', 'EmployeeEPAModel', 'SelfEvalutorModel', 'CommonModel', 'utilitiesFactory',
function ($window, $translate, $routeParams, EmployeeEPAModel, SelfEvalutorModel, CommonModel, utilitiesFactory) {
    var vm = this;

    vm.epaId = parseInt($routeParams.id);

    vm.maxStar = 5;
    vm.dataSaved = false;
    vm.isReadonly = false;
    vm.evaluatedDate = null;

    vm.standardGrades = CommonModel.standardGrades;

    var checkStatus = function (epaInfo) {
        if (epaInfo.CurrentProcess.IsEndProcess) {
            vm.isReadonly = true;
            vm.hoveringLeave = null;
        }
        else {
            vm.hoveringLeave = function (kpiGroup, kpi) {
                if (kpi.CurrentEvaluation.Rate > 0) {
                    if (kpiGroup.NumberKPIOptional === 0) {
                        var score = utilitiesFactory.parseFloat(kpi.Weight * (kpi.CurrentEvaluation.Rate / vm.maxStar));
                        kpi.CurrentEvaluation.Score = score;
                    }
                    else {
                        SelfEvalutorModel.scoreSpreadForKPIGroupHasKPIOptional(kpiGroup, vm.maxStar);
                    }

                    SelfEvalutorModel.calculateScore(vm);
                }
            };
        }
    };

    vm.initCollapseExpandCommentByEmployee = function (epaInfo, kpi) {
        if (epaInfo.ProcessBySelf.IsEndProcess && (kpi.EvaluationBySelf.Comment && kpi.EvaluationBySelf.Comment.length > 0)) {
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
        return EmployeeEPAModel.getNotificationInputScore(kpi.Weight, vm.scoreInputValidMessage);
    };

    vm.inputScore = function (kpi) {
        kpi.CurrentEvaluation.Rate = 0;

        if (!EmployeeEPAModel.validateScoreInput(kpi.CurrentEvaluation.Score, kpi.Weight, vm.scoreInputValidMessage)) {
            kpi.CurrentEvaluation.Score = null;
        }
        else {
            SelfEvalutorModel.calculateScore(vm);
        }
    };

    vm.cancelRating = function (kpiGroup, kpi) {
        kpi.CurrentEvaluation.Rate = 0;
        kpi.CurrentEvaluation.Score = null;

        if (kpiGroup.HasKPIOptional) {
            SelfEvalutorModel.scoreSpreadForKPIGroupHasKPIOptional(kpiGroup, vm.maxStar);
        }

        SelfEvalutorModel.calculateScore(vm);
    };

    vm.cancel = function () {
        $window.history.back();
    };

    vm.save = function (epaInfo) {
        if (SelfEvalutorModel.validateEPA(epaInfo)) {
            EmployeeEPAModel.updateEPAScoreByEvaluator(vm);
        }
        else {
            EmployeeEPAModel.openPopupNotificationComment(vm, vm.evaluateEPAInvalidMessage);
        }
    };

    var getEPAInfoByEvaluator = function () {
        EmployeeEPAModel.getEPAInfoByEvaluator(vm.epaId).then(function () {
            vm.epaInfo = EmployeeEPAModel.epaInfo;

            var disciplineScore = vm.epaInfo.MaxDisciplineScore - vm.epaInfo.DisciplineScore;
            vm.epaInfo.CurrentProcess.TotalScore = vm.epaInfo.CurrentProcess.TotalScore ? parseFloat(vm.epaInfo.CurrentProcess.TotalScore) : 0;
            vm.epaInfo.ProcessBySelf.TotalScore = vm.epaInfo.ProcessBySelf.TotalScore ? parseFloat(vm.epaInfo.ProcessBySelf.TotalScore) : 0;

            if (disciplineScore < 0) {
                disciplineScore = 0;
            }

            vm.epaInfo.DisciplineScore = disciplineScore;
            vm.finalScore = vm.epaInfo.CurrentProcess.TotalScore;
            vm.finalScoreOfSelf = vm.epaInfo.ProcessBySelf.TotalScore;

            if (vm.epaInfo.HasDisciplineScore) {
                vm.finalScore += vm.epaInfo.DisciplineScore;
                vm.finalScoreOfSelf += vm.epaInfo.DisciplineScore;
            }

            vm.gradeOfSelf = EmployeeEPAModel.convertScoreToGrade(vm.finalScoreOfSelf, vm.epaInfo.MaxFinalScore, vm.standardGrades)

            vm.epaInfo.Grade = EmployeeEPAModel.convertScoreToGrade(vm.finalScore, vm.epaInfo.MaxFinalScore, vm.standardGrades);
            vm.evaluatedDate = vm.epaInfo.CurrentProcess.EvaluateDate ? vm.epaInfo.CurrentProcess.EvaluateDate : null;
            checkStatus(vm.epaInfo);

            angular.forEach(vm.epaInfo.KPIGroups, function (kpiGroup) {
                EmployeeEPAModel.scoreSpread(kpiGroup);
            });
        });
    };

    vm.initialize = function () {
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
    };

    vm.initialize();
}]);
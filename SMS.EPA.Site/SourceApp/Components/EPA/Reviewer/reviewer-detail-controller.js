(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('epaModule').controller('ReviewerDetailController', reviewerDetailController);
    reviewerDetailController.$inject = ['$timeout', '$translate', '$routeParams', 'EvaluationModel', 'ReviewerEPAModel', 'ReviewerManagementModel',
        'CommonModel', 'utilitiesFactory', 'historyFactory', 'lockScreenService', 'baseUrl'];

    function reviewerDetailController($timeout, $translate, $routeParams, EvaluationModel, ReviewerEPAModel, ReviewerManagementModel, CommonModel,
        utilitiesFactory, historyFactory, lockScreenService, baseUrl) {
        var vm = this;

        vm.permission = 0;
        vm.epaId = parseInt($routeParams.id);
        vm.reviewerId = $routeParams.reviewerId;
        vm.jobGrades = [];
        vm.standardGrades = [];
        vm.policyEnum = utilitiesFactory.policy;
        vm.targetTypeEnum = utilitiesFactory.targetType;
        vm.ratingTypeEnum = utilitiesFactory.ratingType;
        vm.dataSaved = false;
        vm.evaluatedDate = null;
        vm.finalScore = null;
        vm.hasRatingByStar = false;
        vm.showRatingScoreByStar = true;

        vm.probationStatusTypes = {
            Failed: 0,
            Passed: 1,
            Extended: 2
        };

        vm.range = function (n) {
            return new Array(n);
        };

        vm.getNotificationInputScore = function (processByReviewer) {
            if (processByReviewer) {
                return EvaluationModel.getNotificationInputScore(0, processByReviewer.MaxTotalScore, vm.scoreInputValidMessage);
            }

            return '';
        };

        vm.inputScore = function (processByReviewer) {
            if (!EvaluationModel.validateScoreInput(processByReviewer.TotalScore, 0, processByReviewer.MaxTotalScore, vm.scoreInputValidMessage)) {
                processByReviewer.TotalScore = null;
            }

            ReviewerManagementModel.calculatePerformanceScore(vm, true);
        };

        // Event read more / less text.
        utilitiesFactory.maxLengthDescription(vm);

        vm.readMore = function (item) {
            utilitiesFactory.readMore(item);
        };

        vm.readLess = function (item) {
            utilitiesFactory.readLess(item);
        };

        vm.save = function (epaInfo) {
            var totalcore = epaInfo.Process.CurrentProcess.TotalScore;

            if (ReviewerManagementModel.validateEvaluation(totalcore) && ReviewerEPAModel.validateRecommendation(epaInfo)) {
                var model = new Array(epaInfo);

                ReviewerEPAModel.adjustScoreByReviewer(vm, model);
            }
        };

        vm.cancel = function () {
            historyFactory.back();
        };

        vm.generateRatingStar = function () {
            $timeout(function () { $(".rating-loading").rating(); }, 1);
        };

        function getEPAInfoByReviewer() {
            ReviewerEPAModel.getEPAInfoByReviewer(vm.epaId, vm.reviewerId).then(function () {
                vm.epaInfo = ReviewerEPAModel.epaInfo;
                vm.epaInfo.Rowspan = vm.epaInfo.Process.Evaluators.length;
                vm.epaInfo.Employee.Avatar = baseUrl + vm.epaInfo.Employee.Avatar;

                if (vm.epaInfo.RatingType.RatingTypeId === vm.ratingTypeEnum.Star
                        || vm.epaInfo.RatingType.RatingTypeId === vm.ratingTypeEnum.HalfStar
                        || (vm.epaInfo.RatingType.RatingTypeId === vm.ratingTypeEnum.Score
                            && $.grep(vm.epaInfo.KPIGroups, function (e) { return e.NumberKPIOptional > 0; }).length > 0)) {
                    vm.hasRatingByStar = true;
                }

                if (vm.epaInfo.IsSemiHasMonthly && !vm.epaInfo.Process.CurrentProcess.TotalScore) {
                    vm.epaInfo.Process.CurrentProcess.TotalScore = vm.epaInfo.MonthlyEPA.AverageReviewerScoreConverted;
                } else if (!vm.epaInfo.Process.CurrentProcess.TotalScore && vm.epaInfo.Process.Evaluators[0].IsEndProcess) {
                    vm.epaInfo.Process.CurrentProcess.TotalScore = vm.epaInfo.Process.AverageScoreByEvaluators;
                }

                // self
                vm.epaInfo.Process.Self.TotalScore = utilitiesFactory.parseFloat(vm.epaInfo.Process.Self.TotalScore);

                // evaluator
                angular.forEach(vm.epaInfo.Process.Evaluators, function (evaluator) {
                    evaluator.TotalScore = utilitiesFactory.parseFloat(evaluator.TotalScore);
                });
                vm.epaInfo.Process.AverageScoreByEvaluators = vm.epaInfo.Process.AverageScoreByEvaluators;

                // reviewer
                vm.epaInfo.Process.CurrentProcess.TotalScore = utilitiesFactory.parseFloat(vm.epaInfo.Process.CurrentProcess.TotalScore);
                vm.finalScore = vm.epaInfo.Process.CurrentProcess.TotalScore + vm.epaInfo.DisciplineScore;
                vm.finalScore = EvaluationModel.checkScore(vm.finalScore, 0, vm.epaInfo.Process.CurrentProcess.MaxPerformanceScore);

                vm.epaInfo.Grade = EvaluationModel.convertScoreToGrade(
                    vm.epaInfo.Process.CurrentProcess.TotalScore,
                    vm.epaInfo.DisciplineScore,
                    vm.epaInfo.HasDisciplineScore,
                    vm.epaInfo.Process.CurrentProcess.MaxPerformanceScore,
                    vm.standardGrades);
                vm.evaluatedDate = vm.epaInfo.Process.CurrentProcess.EvaluateDate ? vm.epaInfo.Process.CurrentProcess.EvaluateDate : null;

                if (vm.epaInfo.Recommendation && vm.epaInfo.Recommendation.JobGrade && vm.epaInfo.Recommendation.JobGrade.JobGradeId) {
                    var index = vm.jobGrades.map(function (e) { return e.JobGradeId; }).indexOf(vm.epaInfo.Recommendation.JobGrade.JobGradeId);

                    if (index === -1) {
                        vm.jobGrades.push(vm.epaInfo.Recommendation.JobGrade);
                    }
                }

                ReviewerEPAModel.getUserPermissionOnReviewer().then(function () {
                    vm.permission = ReviewerEPAModel.permission;

                    if (vm.permission !== vm.policyEnum.Host) {
                        vm.epaInfo.IsAllowedToEdit = false;
                    }
                });

                lockScreenService.stop();
            });
        }

        function initialize() {
            lockScreenService.start();

            $translate('ScoreInputValid').then(function (message) {
                vm.scoreInputValidMessage = message;
            });

            CommonModel.getJobGrade(true).then(function () {
                vm.jobGrades = CommonModel.jobGrades;
            });

            CommonModel.getStandardGrade().then(function () {
                vm.standardGrades = CommonModel.standardGrades;

                getEPAInfoByReviewer();
            });
        }

        initialize();
    }
})(angular);
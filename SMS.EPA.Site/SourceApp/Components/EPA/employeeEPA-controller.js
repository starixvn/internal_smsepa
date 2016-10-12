'use strict';
/*global epaModule*/

epaModule.controller('ViewEPADetailController',
['$timeout', '$routeParams', 'EmployeeEPAModel', 'CommonModel', 'utilitiesFactory', 'historyFactory', 'baseUrl',
function ($timeout, $routeParams, EmployeeEPAModel, CommonModel, utilitiesFactory, historyFactory, baseUrl) {
    var vm = this;

    vm.epaId = parseInt($routeParams.id);
    vm.allowedExportPdf = $routeParams.exportPdf;
    vm.jobGrades = [];
    vm.standardGrades = [];
    vm.ratingTypeEnum = utilitiesFactory.ratingType;
    vm.statusEndProcessEnum = utilitiesFactory.statusEndProcess;
    vm.isViewDetail = true;
    vm.hasRatingByStar = false;
    vm.showRatingScoreByStar = true;

    vm.generateRatingStar = function () {
        $timeout(function () {
            $(".rating-loading").rating();
        }, 1);
    };

    // Event read more / less text.
    utilitiesFactory.maxLengthDescription(vm);

    vm.readMore = function (item) {
        utilitiesFactory.readMore(item);
    };

    vm.readLess = function (item) {
        utilitiesFactory.readLess(item);
    };

    vm.range = function (n) {
        return new Array(n);
    };

    vm.back = function () {
        historyFactory.back();
    };

    vm.exportPdf = function () {
        var url = baseUrl + 'ExportPDF/ExportPdf?epaId=' + vm.epaId;
        var iframe = $('body iframe');

        if (iframe.length === 0) {
            iframe = $('<iframe width="0" height="0" class="display-none"></iframe>');
            iframe.appendTo('body');
        }

        iframe.attr('src', url);
    };

    function getEPAInfoById() {
        EmployeeEPAModel.getEPAInfoById(vm.epaId).then(function () {
            vm.epaInfo = EmployeeEPAModel.epaInfo;
            vm.epaInfo.Employee.Avatar = baseUrl + vm.epaInfo.Employee.Avatar;

            if (vm.epaInfo.RatingType.RatingTypeId === vm.ratingTypeEnum.Star
                    || vm.epaInfo.RatingType.RatingTypeId === vm.ratingTypeEnum.HalfStar
                    || (vm.epaInfo.RatingType.RatingTypeId === vm.ratingTypeEnum.Score
                        && $.grep(vm.epaInfo.KPIGroups, function (e) { return e.NumberKPIOptional > 0; }).length > 0)) {
                vm.hasRatingByStar = true;
            }

            // self
            vm.epaInfo.Process.Self.TotalScore = utilitiesFactory.parseFloat(vm.epaInfo.Process.Self.TotalScore);

            // evaluator
            angular.forEach(vm.epaInfo.Process.Evaluators, function (evaluator) {
                evaluator.TotalScore = utilitiesFactory.parseFloat(evaluator.TotalScore);
            });

            // reviewer
            angular.forEach(vm.epaInfo.Process.Reviewers, function (reviewer) {
                reviewer.TotalScore = utilitiesFactory.parseFloat(reviewer.TotalScore);
            });

            // management
            angular.forEach(vm.epaInfo.Process.Managements, function (management) {
                management.TotalScore = utilitiesFactory.parseFloat(management.TotalScore);
            });

            if (vm.epaInfo.Period.IsEndPeriod && vm.epaInfo.FinalScore.length > 0) {
                vm.epaInfo.FinalScore = utilitiesFactory.parseFloat(vm.epaInfo.FinalScore);
                vm.epaInfo.Grade = EmployeeEPAModel.convertScoreToGrade(vm.epaInfo.FinalScore, vm.standardGrades);
            } else {
                vm.epaInfo.FinalScore = null;
            }

            if (vm.epaInfo.Recommendation && vm.epaInfo.Recommendation.JobGrade && vm.epaInfo.Recommendation.JobGrade.JobGradeId) {
                var index = vm.jobGrades.map(function (e) { return e.JobGradeId; }).indexOf(vm.epaInfo.Recommendation.JobGrade.JobGradeId);

                if (index === -1) {
                    vm.jobGrades.push(vm.epaInfo.Recommendation.JobGrade);
                }
            }
        });
    }

    function initialize() {
        CommonModel.getJobGrade(true).then(function () {
            vm.jobGrades = CommonModel.jobGrades;
        });

        CommonModel.getStandardGrade().then(function () {
            vm.standardGrades = CommonModel.standardGrades;

            getEPAInfoById();
        });
    }

    initialize();
}]);

epaModule.controller('ModalAdjustScoreController',
['$modalInstance', 'EmployeeEPAModel', 'ReviewerManagementModel', 'utilitiesFactory', 'notificationFactory', 'selectedEmployees', 'isReviewer',
function ($modalInstance, EmployeeEPAModel, ReviewerManagementModel, utilitiesFactory, notificationFactory, selectedEmployees, isReviewer) {
    var vm = this;

    vm.adjust = {
        Score: '',
        Comment: ''
    };

    vm.selectedEmployees = selectedEmployees;
    vm.isReviewer = isReviewer;

    vm.ok = function () {
        if (!vm.adjust.Score) {
            notificationFactory.error("ScoreRequire");
        } else {
            if (vm.isReviewer) {
                ReviewerManagementModel.calculateAdjustScoreMultiple(vm.selectedEmployees, vm.adjust, true);
                ReviewerManagementModel.adjustScoreByReviewer(vm, vm.selectedEmployees);
            } else {
                ReviewerManagementModel.calculateAdjustScoreMultiple(vm.selectedEmployees, vm.adjust, false);
                ReviewerManagementModel.adjustScoreByManagement(vm, vm.selectedEmployees);
            }

            $modalInstance.close(vm.adjust);
        }
    };

    vm.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}]);

epaModule.controller('ModalUpdateCommentController',
['$modalInstance', 'EmployeeEPAModel', 'ReviewerManagementModel', 'utilitiesFactory', 'epaInfo', 'comment', 'isEdit', 'isReviewer',
function ($modalInstance, EmployeeEPAModel, ReviewerManagementModel, utilitiesFactory, epaInfo, comment, isEdit, isReviewer) {
    var vm = this;

    vm.comment = comment;
    vm.isEdit = isEdit;
    vm.isReviewer = isReviewer;
    vm.epaInfo = epaInfo;

    vm.ok = function () {
        var model = [], scoreAdjusted = utilitiesFactory.parseFloat(vm.epaInfo.Process.CurrentProcess.AdjustScoreOriginal),
            averageScore = vm.epaInfo.Process.AverageScoreByEvaluators,
            totalScore;

        if (!vm.isReviewer && vm.epaInfo.Process.Reviewers.length > 0) {
            averageScore = vm.epaInfo.Process.AverageScoreByReviewers;
        }

        if (vm.epaInfo.HasDisciplineScore) {
            averageScore -= vm.epaInfo.DisciplineScore;
        }

        totalScore = averageScore + scoreAdjusted;
        vm.epaInfo.Process.CurrentProcess.TotalScore = totalScore;
        vm.epaInfo.Process.CurrentProcess.AdjustScore = scoreAdjusted;
        vm.epaInfo.Process.CurrentProcess.Comment = vm.comment;
        model.push(vm.epaInfo);

        if (vm.isReviewer) {
            ReviewerManagementModel.adjustScoreByReviewer(vm, model);
        }
        else {
            ReviewerManagementModel.adjustScoreByManagement(vm, model);
        }

        $modalInstance.close(vm.comment);
    };

    vm.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}]);

epaModule.controller('ModalNotificationController',
['$modalInstance', 'message',
function ($modalInstance, message) {
    var vm = this;

    vm.message = message;

    vm.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}]);

epaModule.controller("ModalRemindController",
['$modalInstance',
 function ($modalInstance) {
     var vm = this;

     vm.ok = function () {
         $modalInstance.close();
     };

     vm.cancel = function () {
         $modalInstance.dismiss("cancel");
     };
 }]);
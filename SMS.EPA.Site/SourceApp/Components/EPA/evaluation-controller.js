'use strict';
/*global epaModule*/

epaModule.controller('EPADetailController',
['$routeParams', 'EvaluationModel', 'CommonModel', 'utilitiesFactory', 'historyFactory', 'lockScreenService', 'baseUrl',
function ($routeParams, EvaluationModel, CommonModel, utilitiesFactory, historyFactory, lockScreenService, baseUrl) {
    var vm = this;

    vm.epaId = parseInt($routeParams.id);
    vm.allowedExportPdf = $routeParams.exportPdf;
    vm.jobGrades = [];
    vm.standardGrades = [];
    vm.isViewDetail = true;

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
        EvaluationModel.getEPAInfoById(vm.epaId).then(function () {
            vm.epaInfo = EvaluationModel.epaInfo;
            vm.epaInfo.Employee.Avatar = baseUrl + vm.epaInfo.Employee.Avatar;

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
                vm.epaInfo.Grade = EvaluationModel.convertScoreToGrade(vm.epaInfo.FinalScore, vm.standardGrades);
            } else {
                vm.epaInfo.FinalScore = null;
            }

            if (vm.epaInfo.Recommendation && vm.epaInfo.Recommendation.JobGrade && vm.epaInfo.Recommendation.JobGrade.JobGradeId) {
                var index = vm.jobGrades.map(function (e) { return e.JobGradeId; }).indexOf(vm.epaInfo.Recommendation.JobGrade.JobGradeId);

                if (index === -1) {
                    vm.jobGrades.push(vm.epaInfo.Recommendation.JobGrade);
                }
            }

            lockScreenService.stop();
        });
    }

    function initialize() {
        lockScreenService.start();

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
['$modalInstance', 'EvaluationModel', 'ReviewerEPAModel', 'ManagementEPAModel', 'ReviewerManagementModel',
    'utilitiesFactory', 'notificationFactory', 'selectedEmployees', 'isReviewer',
function ($modalInstance, EvaluationModel, ReviewerEPAModel, ManagementEPAModel, ReviewerManagementModel,
    utilitiesFactory, notificationFactory, selectedEmployees, isReviewer) {
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
                ReviewerEPAModel.adjustScoreByReviewer(vm, vm.selectedEmployees);
            } else {
                ReviewerManagementModel.calculateAdjustScoreMultiple(vm.selectedEmployees, vm.adjust, false);
                ManagementEPAModel.adjustScoreByManagement(vm, vm.selectedEmployees);
            }

            $modalInstance.close(vm.adjust);
        }
    };

    vm.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}]);

epaModule.controller('ModalUpdateCommentController',
['$modalInstance', 'EvaluationModel', 'ReviewerEPAModel', 'ManagementEPAModel', 'ReviewerManagementModel',
    'utilitiesFactory', 'epaInfo', 'comment', 'isEdit', 'isReviewer',
function ($modalInstance, EvaluationModel, ReviewerEPAModel, ManagementEPAModel, ReviewerManagementModel,
    utilitiesFactory, epaInfo, comment, isEdit, isReviewer) {
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
            ReviewerEPAModel.adjustScoreByReviewer(vm, model);
        } else {
            ManagementEPAModel.adjustScoreByManagement(vm, model);
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
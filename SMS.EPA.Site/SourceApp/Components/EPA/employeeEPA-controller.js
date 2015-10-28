'use strict';
/*global epaModule*/

epaModule.controller('ViewEPADetailController',
['$window', '$translate', '$routeParams', 'EmployeeEPAModel', 'CommonModel', 'utilitiesFactory',
function ($window, $translate, $routeParams, EmployeeEPAModel, CommonModel, utilitiesFactory) {
    var vm = this;

    vm.epaId = parseInt($routeParams.id);

    vm.isViewDetail = true;
    vm.isReadonly = true;
    vm.finalScore = null;

    vm.standardGrades = CommonModel.standardGrades;

    var calculateFinalScore = function (epaInfo) {
        var finalScore = 0;

        if (epaInfo.ProcessByManagements.length > 0) {
            finalScore = epaInfo.AverageScoreByManagements;
        } else if (epaInfo.ProcessByReviewers.length > 0) {
            finalScore = epaInfo.AverageScoreByReviewers;
        } else {
            finalScore = epaInfo.AverageScoreByEvaluators;
        }

        return finalScore;
    };

    // Event read more / less text.
    utilitiesFactory.maxLengthDescription(vm);

    vm.readMore = function (item) {
        utilitiesFactory.readMore(item);
    };

    vm.readLess = function (item) {
        utilitiesFactory.readLess(item);
    };

    vm.openPopupViewComment = function (comment) {
        EmployeeEPAModel.openPopupViewComment(vm, comment);
    };

    vm.getClassProgress = function (statusEndProcess) {
        return EmployeeEPAModel.getClassProgress(statusEndProcess);
    };

    vm.range = function (n) {
        return new Array(n);
    };

    vm.back = function () {
        $window.history.back();
    };

    var getEPAInfoById = function () {
        EmployeeEPAModel.getEPAInfoById(vm.epaId).then(function () {
            vm.epaInfo = EmployeeEPAModel.epaInfo;
            var totalScore;

            if (vm.epaInfo.HasDisciplineScore && vm.epaInfo.DisciplineScore < 0) {
                vm.epaInfo.DisciplineScore = 0;
            }

            // self
            totalScore = utilitiesFactory.parseFloat(vm.epaInfo.ProcessBySelf.TotalScore);
            vm.epaInfo.ProcessBySelf.TotalScore = totalScore + vm.epaInfo.DisciplineScore;

            // evaluator
            angular.forEach(vm.epaInfo.ProcessByEvaluators, function (evaluator) {
                totalScore = utilitiesFactory.parseFloat(evaluator.TotalScore);
                evaluator.TotalScore = totalScore + vm.epaInfo.DisciplineScore;
            });
            vm.epaInfo.AverageScoreByEvaluators += vm.epaInfo.DisciplineScore;

            // reviewer
            angular.forEach(vm.epaInfo.ProcessByReviewers, function (reviewer) {
                totalScore = utilitiesFactory.parseFloat(reviewer.TotalScore);
                reviewer.TotalScore = totalScore + vm.epaInfo.DisciplineScore;
            });
            vm.epaInfo.AverageScoreByReviewers += vm.epaInfo.DisciplineScore;

            // management
            angular.forEach(vm.epaInfo.ProcessByManagements, function (management) {
                totalScore = utilitiesFactory.parseFloat(management.TotalScore);
                management.TotalScore = totalScore + vm.epaInfo.DisciplineScore;
            });
            vm.epaInfo.AverageScoreByManagements += vm.epaInfo.DisciplineScore;

            vm.finalScore = calculateFinalScore(vm.epaInfo) + vm.epaInfo.DisciplineScore;

            if (vm.finalScore > vm.epaInfo.MaxFinalScore) {
                vm.finalScore = vm.epaInfo.MaxFinalScore;
            } else if (vm.finalScore < 0) {
                vm.finalScore = 0;
            }

            vm.epaInfo.Grade = EmployeeEPAModel.convertScoreToGrade(vm.finalScore, vm.standardGrades);

            angular.forEach(vm.epaInfo.KPIGroups, function (kpiGroup) {
                EmployeeEPAModel.scoreSpread(kpiGroup);
            });
        });
    };

    vm.initialize = function () {
        CommonModel.getStandardGrade().then(function () {
            vm.standardGrades = CommonModel.standardGrades;

            getEPAInfoById();
        });
    };

    vm.initialize();
}]);

epaModule.controller('ModalViewCommentController',
['$modalInstance', 'comment',
function ($modalInstance, comment) {
    var vm = this;

    vm.comment = comment;

    vm.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
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
        }
        else {
            if (vm.isReviewer) {
                ReviewerManagementModel.calculateAdjustScoreMultiple(vm.selectedEmployees, vm.adjust, true);
                ReviewerManagementModel.adjustScoreByReviewer(vm, vm.selectedEmployees);
            }
            else {
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
['$modalInstance', 'EmployeeEPAModel', 'ReviewerManagementModel', 'utilitiesFactory', 'memberEPA', 'comment', 'isEdit', 'isReviewer',
function ($modalInstance, EmployeeEPAModel, ReviewerManagementModel, utilitiesFactory, memberEPA, comment, isEdit, isReviewer) {
    var vm = this;

    vm.comment = comment;
    vm.isEdit = isEdit;
    vm.isReviewer = isReviewer;
    vm.memberEPA = memberEPA;

    vm.ok = function () {
        var model = [], scoreAdjusted = utilitiesFactory.parseFloat(vm.memberEPA.CurrentProcess.AdjustScoreOriginal),
            averageScore = vm.memberEPA.AverageScoreByEvaluators,
            totalScore;

        if (!vm.isReviewer && vm.memberEPA.ProcessByReviewers.length > 0) {
            averageScore = vm.memberEPA.AverageScoreByReviewers;
        }

        if (vm.memberEPA.HasDisciplineScore) {
            averageScore -= vm.memberEPA.DisciplineScore;
        }

        totalScore = averageScore + scoreAdjusted;
        vm.memberEPA.CurrentProcess.TotalScore = totalScore;
        vm.memberEPA.CurrentProcess.AdjustScore = scoreAdjusted;
        vm.memberEPA.CurrentProcess.Comment = vm.comment;
        model.push(vm.memberEPA);

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

appModule.controller("ModalRemindController",
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
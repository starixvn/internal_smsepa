'use strict';
/*global epaModule*/

epaModule.controller('ModalAdjustScoreController',
    [
        '$modalInstance',
        'EvaluationModel',
        'ReviewerEPAModel',
        'ManagementEPAModel',
        'ReviewerManagementModel',
        'utilitiesFactory',
        'notificationFactory',
        'selectedEmployees',
        'isReviewer',
        function(
            $modalInstance,
            EvaluationModel,
            ReviewerEPAModel,
            ManagementEPAModel,
            ReviewerManagementModel,
            utilitiesFactory,
            notificationFactory,
            selectedEmployees,
            isReviewer) {
            var vm = this;

            vm.adjust = {
                Score: '',
                Comment: ''
            };

            vm.selectedEmployees = selectedEmployees;
            vm.isReviewer = isReviewer;

            vm.ok = function() {
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

            vm.cancel = function() {
                $modalInstance.dismiss('cancel');
            };
        }
    ]);

epaModule.controller('ModalUpdateCommentController',
    [
        '$modalInstance',
        'EvaluationModel',
        'ReviewerEPAModel',
        'ManagementEPAModel',
        'ReviewerManagementModel',
        'utilitiesFactory',
        'epaInfo',
        'comment',
        'isEdit',
        'isReviewer',
        function($modalInstance,
            EvaluationModel,
            ReviewerEPAModel,
            ManagementEPAModel,
            ReviewerManagementModel,
            utilitiesFactory,
            epaInfo,
            comment,
            isEdit,
            isReviewer) {
            var vm = this;

            vm.comment = comment;
            vm.isEdit = isEdit;
            vm.isReviewer = isReviewer;
            vm.epaInfo = epaInfo;

            vm.ok = function() {
                var model = [];
                var scoreAdjusted = utilitiesFactory.parseFloat(vm.epaInfo.Process.CurrentProcess.AdjustScoreOriginal);
                var averageScore = vm.epaInfo.Process.AverageScoreByEvaluators;

                if (!vm.isReviewer && vm.epaInfo.Process.Reviewers.length > 0) {
                    averageScore = vm.epaInfo.Process.AverageScoreByReviewers;
                }

                var totalScore = averageScore + scoreAdjusted;
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

            vm.cancel = function() {
                $modalInstance.dismiss('cancel');
            };
        }
    ]);

epaModule.controller('ModalNotificationController',
    [
        '$modalInstance', 'message',
        function($modalInstance, message) {
            var vm = this;

            vm.message = message;

            vm.cancel = function() {
                $modalInstance.dismiss('cancel');
            };
        }
    ]);

epaModule.controller("ModalRemindController",
    [
        '$modalInstance',
        function($modalInstance) {
            var vm = this;

            vm.ok = function() {
                $modalInstance.close();
            };

            vm.cancel = function() {
                $modalInstance.dismiss("cancel");
            };
        }
    ]);
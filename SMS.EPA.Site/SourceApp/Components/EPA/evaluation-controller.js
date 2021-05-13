(function (angular) {
    'use strict';

    angular.module('epaModule').controller('ModalAdjustScoreController', modalAdjustScoreController);
    modalAdjustScoreController.$inject = ['$uibModalInstance', 'ReviewerEPAModel', 'ManagementEPAModel',
        'ReviewerManagementModel', 'notificationFactory', 'selectedEmployees', 'isReviewer'];

    function modalAdjustScoreController($uibModalInstance, ReviewerEPAModel, ManagementEPAModel,
        ReviewerManagementModel, notificationFactory, selectedEmployees, isReviewer) {
        let vm = this;

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

                $uibModalInstance.close(vm.adjust);
            }
        };

        vm.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };
    }
})(angular);

(function (angular) {
    'use strict';

    angular.module('epaModule').controller('ModalUpdateCommentController', modalUpdateCommentController);
    modalUpdateCommentController.$inject = ['$uibModalInstance', 'ReviewerEPAModel', 'ManagementEPAModel', 'utilitiesFactory',
        'epaInfo', 'comment', 'isEdit', 'isReviewer'];

    function modalUpdateCommentController($uibModalInstance, ReviewerEPAModel, ManagementEPAModel, utilitiesFactory,
        epaInfo, comment, isEdit, isReviewer) {
        let vm = this;

        vm.comment = comment;
        vm.isEdit = isEdit;
        vm.isReviewer = isReviewer;
        vm.epaInfo = epaInfo;

        vm.ok = function () {
            const model = [];
            const scoreAdjusted = utilitiesFactory.parseFloat(vm.epaInfo.Process.CurrentProcess.AdjustScoreOriginal);
            let averageScore = vm.epaInfo.Process.AverageScoreByEvaluators;

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

            $uibModalInstance.close(vm.comment);
        };

        vm.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };
    }
})(angular);

(function (angular) {
    'use strict';

    angular.module('epaModule').controller('ModalNotificationController', modalNotificationController);
    modalNotificationController.$inject = ['$uibModalInstance', 'message'];

    function modalNotificationController($uibModalInstance, message) {
        let vm = this;

        vm.message = message;

        vm.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };
    }
})(angular);

(function (angular) {
    'use strict';

    angular.module('epaModule').controller('ModalRemindController', modalRemindController);
    modalRemindController.$inject = ['$uibModalInstance'];

    function modalRemindController($uibModalInstance) {
        let vm = this;

        vm.ok = function () {
            $uibModalInstance.close();
        };

        vm.cancel = function () {
            $uibModalInstance.dismiss("cancel");
        };
    }
})(angular);
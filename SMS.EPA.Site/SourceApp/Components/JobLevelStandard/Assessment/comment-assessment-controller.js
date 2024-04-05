(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('jobLevelStandardModule').controller('CommentAssessmentController', commentController);
    commentController.$inject = ['$uibModalInstance', 'AssessmentModel','comment'];

    function commentController($uibModalInstance, AssessmentModel, comment) {
        let vm = this;

        vm.comment = comment;

        vm.save = function () {
            AssessmentModel.comment = vm.comment;
            $uibModalInstance.close();
        };

        vm.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };
    }
})(angular);
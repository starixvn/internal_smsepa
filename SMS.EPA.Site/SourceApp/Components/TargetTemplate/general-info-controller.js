(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('targetTemplateModule').controller('TargetTemplateGeneralInfoController', generalInfoController);
    generalInfoController.$inject = ['$scope', 'TargetTemplateModel', 'CommonModel'];

    function generalInfoController($scope, TargetTemplateModel, CommonModel) {
        var vm = this;

        vm.scoreRatingTypeOptions = [];
        vm.targetTypes = [];
        vm.ratingTypes = [];

        vm.save = function (targetTemplate) {
            if (TargetTemplateModel.validateTargetTemplate(targetTemplate)) {
                TargetTemplateModel.updateTargetTemplateBasicInfo(targetTemplate, $scope.updateCtrl.originalScoreTypeId).then(function () {
                    if (targetTemplate.ScoreRatingTypeId !== $scope.updateCtrl.originalScoreTypeId) {
                        $scope.updateCtrl.getTargetTemplateById(targetTemplate.TargetTemplateId);
                    }
                });
            }
        };

        function initialize() {
            CommonModel.getTargetType(true).then(function () {
                vm.targetTypes = CommonModel.targetTypes;
            });

            TargetTemplateModel.getScoreRatingType(null, true).then(function () {
                vm.scoreRatingTypeOptions = TargetTemplateModel.scoreRatingTypes;
            });

            TargetTemplateModel.getRatingType().then(function () {
                vm.ratingTypes = TargetTemplateModel.ratingTypes;
            });
        }

        initialize();
    }
})(angular);
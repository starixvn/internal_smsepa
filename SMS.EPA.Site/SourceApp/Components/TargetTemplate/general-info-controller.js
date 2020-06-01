(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('targetTemplateModule').controller('TargetTemplateGeneralInfoController', generalInfoController);
    generalInfoController.$inject = ['$scope', 'TargetTemplateModel', 'CommonModel', 'utilitiesFactory'];

    function generalInfoController($scope, TargetTemplateModel, CommonModel, utilitiesFactory) {
        var vm = this;

        vm.scoreRatingTypeOptions = [];
        vm.targetTypes = [];
        vm.ratingTypes = [];
        vm.targetTypeEnum = utilitiesFactory.targetType;

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

        vm.handleChangeTargetType = function () {
            if ($scope.updateCtrl.targetTemplate.TargetTypeId === vm.targetTypeEnum.Feedback360) {
                $scope.updateCtrl.targetTemplate.RatingTypeId = utilitiesFactory.ratingType.Star;
                $scope.updateCtrl.targetTemplate.ScoreRatingTypeId = utilitiesFactory.scoreRatingType.Full100;
            }
        };

        initialize();
    }
})(angular);
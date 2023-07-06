(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('targetTemplateModule').controller('TargetTemplateGeneralInfoController', generalInfoController);
    generalInfoController.$inject = ['$scope', 'TargetTemplateModel', 'CommonModel'];

    function generalInfoController($scope, TargetTemplateModel, CommonModel) {
        var vm = this;

        vm.templateId = $scope.updateCtrl.targetTemplateId;
        vm.scoreRatingTypeOptions = [];
        vm.targetTypes = [];
        vm.ratingTypes = [];

        vm.save = function (targetTemplate) {
            var newScoreRatingType = vm.scoreRatingTypeOptions.find(element => element.ScoreRatingTypeId === targetTemplate.ScoreRatingTypeId);
            var totalWeightAfterEdit = newScoreRatingType ? newScoreRatingType.SelfEvaluated : null;

            TargetTemplateModel.getKPIOfTargetTemplate(vm.templateId, true).then(function () {
                var totalWeight = 0;
                TargetTemplateModel.kpiList.forEach(element => totalWeight += element.Weight);
                if (totalWeight !== totalWeightAfterEdit) {
                    TargetTemplateModel.openPopupConfirmEditTargetTemplate(targetTemplate, $scope);
                } else if (TargetTemplateModel.validateTargetTemplate(targetTemplate)) {
                    TargetTemplateModel.updateTargetTemplateBasicInfo(targetTemplate, $scope.updateCtrl.originalScoreTypeId).then(function () {
                        if (targetTemplate.ScoreRatingTypeId !== $scope.updateCtrl.originalScoreTypeId) {
                            $scope.updateCtrl.getTargetTemplateById(targetTemplate.TargetTemplateId);
                        }
                    });
                }
            });
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
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
        vm.totalWeight = 0;

        vm.save = function (targetTemplate) {
            var scoreRatingTypeId = $scope.updateCtrl.targetTemplate.ScoreRatingTypeId;
            if (TargetTemplateModel.validateTargetTemplate(targetTemplate, vm, scoreRatingTypeId)) {
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

            TargetTemplateModel.getKPIOfTargetTemplate(vm.templateId, false).then(function () {
                TargetTemplateModel.kpiList.forEach(element => vm.totalWeight += element.Weight);
            });
        }

        initialize();
    }
})(angular);
(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('targetTemplateModule').controller('CreateTargetTemplateController', createTargetTemplateController);
    createTargetTemplateController.$inject = ['$uibModalInstance', 'TargetTemplateModel', 'CommonModel', 'utilitiesFactory'];

    function createTargetTemplateController($uibModalInstance, TargetTemplateModel, CommonModel, utilitiesFactory) {
        var vm = this;

        vm.permission = 0;
        vm.ratingTypes = [];
        vm.scoreRatingTypeOptions = [];
        vm.targetTemplate = {
            Name: '',
            TargetTypeId: '',
            ScoreRatingTypeId: '',
            RatingTypeId: '',
            IsActive: true,
            Description: ''
        };
        vm.targetTypes = [];
        vm.targetTypeEnum = utilitiesFactory.targetType;

        vm.cancel = cancel;
        vm.save = save;

        initialize();

        function cancel() {
            $uibModalInstance.dismiss("cancel");
        }

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

        function save(targetTemplate) {
            if (TargetTemplateModel.validateTargetTemplate(targetTemplate)) {
                TargetTemplateModel.createTargetTemplate(targetTemplate, $uibModalInstance);
            }
        }

        vm.handleChangeTargetType = function () {
            if (vm.targetTemplate.TargetTypeId === vm.targetTypeEnum.Feedback360) {
                vm.targetTemplate.RatingTypeId = utilitiesFactory.ratingType.Star;
                vm.targetTemplate.ScoreRatingTypeId = utilitiesFactory.scoreRatingType.Full100;
            }
        };
    }
})(angular);
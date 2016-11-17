(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('targetTemplateModule').controller('CreateTargetTemplateController', createTargetTemplateController);
    createTargetTemplateController.$inject = ['$modalInstance', 'TargetTemplateModel', 'CommonModel'];

    function createTargetTemplateController($modalInstance, TargetTemplateModel, CommonModel) {
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

        vm.cancel = cancel;
        vm.save = save;

        initialize();

        function cancel() {
            $modalInstance.dismiss("cancel");
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
                TargetTemplateModel.createTargetTemplate(targetTemplate, $modalInstance);
            }
        }
    }
})(angular);
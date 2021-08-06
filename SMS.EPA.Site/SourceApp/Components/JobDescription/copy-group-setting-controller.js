(function (angular) {
    'use strict';

    angular.module('jobDescriptionModule').controller('CopyGroupSettingController', copyGroupSettingController);
    copyGroupSettingController.$inject = ['$uibModalInstance', 'notificationFactory', 'CommonModel', 'JobDescriptionModel', 'group'];

    function copyGroupSettingController($uibModalInstance, notificationFactory, CommonModel, JobDescriptionModel, group) {
        let vm = this;

        vm.group = group;
        vm.jobLevels = [];
        vm.groupHasSettings = [];
        vm.cancel = cancel;
        vm.save = save;
        vm.jobLevelId = '';
        vm.fromGroupId = '';

        initialize();

        function initialize() {
            CommonModel.getJobLevel(false).then(function () {
                vm.jobLevels = CommonModel.jobLevels;
            });

            JobDescriptionModel.getGroupHasSetting().then(
                function () {
                    vm.groupHasSettings = JobDescriptionModel.groupHasSettings.filter(item => item.GroupId !== vm.group.GroupId);
                });
        }

        function validate() {
            let isValid = true;
            if (vm.fromGroupId === '') {
                isValid = false;
                notificationFactory.error('', 'Please select section you want to copy setting.');
            }

            return isValid;
        }

        function cancel() {
            $uibModalInstance.dismiss('cancel');
        }

        function save() {
            if (validate()) {
                const setting = { jobLevelId: vm.jobLevelId, fromGroupId: vm.fromGroupId, toGroupId: vm.group.GroupId };
                JobDescriptionModel.copyGroupSetting(setting, $uibModalInstance);
            }
        }
    }
})(angular);
(function (angular) {
    'use strict';

    angular.module('dashboardModule').controller('AnnouncementController', announcementController);
    announcementController.$inject = ['$uibModalInstance', 'announcements'];

    function announcementController($uibModalInstance, announcements) {
        let vm = this;

        vm.announcements = announcements;

        vm.ok = function () {
            $uibModalInstance.close();
        };

        vm.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };
    }
})(angular);
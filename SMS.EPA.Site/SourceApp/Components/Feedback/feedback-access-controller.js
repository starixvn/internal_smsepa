(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('feedbackModule').controller('FeedbackController', feedbackController);
    feedbackController.$inject = ['CommonModel', 'errorFactory', 'utilitiesFactory', 'feedbackFactory', 'lockScreenService', 'baseUrl'];

    function feedbackController(CommonModel, errorFactory, utilitiesFactory, feedbackFactory, lockScreenService, baseUrl) {
        var vm = this;

        vm.periodOptions = [];
        vm.filter = { periodId: 0 };
        vm.currentPage = 1;
        vm.pageSize = 20;
        vm.feedbacks = [];

        function getFeedback(periodId) {
            lockScreenService.start();
            feedbackFactory.getFeedback(periodId).success(function (feedbacks) {
                vm.feedbacks = feedbacks;
                utilitiesFactory.searchOnPage(vm, vm.feedbacks);
                lockScreenService.stop();
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function initialize() {
            utilitiesFactory.numberEntryList(vm);

            CommonModel.getPeriod(utilitiesFactory.targetType.SemiAnnual, true).then(function () {
                vm.periodOptions = CommonModel.periods.sort(function (a, b) {
                    return new Date(b.StartDate) - new Date(a.StartDate);
                });

                if (vm.periodOptions.length > 0 && !vm.filter.periodId) {
                    vm.filter.periodId = vm.periodOptions[0].PeriodId;
                }

                getFeedback(vm.filter.periodId);
            });
        }

        vm.getIndex = function (index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        };

        vm.detail = function (feedbackId, event) {
            utilitiesFactory.handleOpenLink(event, 'feedback/detail/' + feedbackId);
        };

        initialize();
    }
})(angular);
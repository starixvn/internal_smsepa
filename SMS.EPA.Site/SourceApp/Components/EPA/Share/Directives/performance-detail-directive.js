(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('epaModule').directive('epaPerformanceDetail', performanceDetail);
    performanceDetail.$inject = ['$timeout', 'utilitiesFactory'];

    function performanceDetail($timeout, utilitiesFactory) {
        return {
            restrict: 'E',
            scope: {
                epaInfo: '='
            },
            controller: function () {
                var vm = this;
                vm.hasRatingByStar = false;
                vm.ratingTypes = utilitiesFactory.ratingType;
                vm.showRatingScoreByStar = true;
                utilitiesFactory.maxLengthDescription(vm);

                if (vm.epaInfo) {
                    vm.rowspan = vm.epaInfo.Process.Evaluators.lenght;
                    var hasKPIOptional = $.grep(vm.epaInfo.KPIGroups, function (e) { return e.NumberKPIOptional > 0; }).length > 0;

                    if (vm.epaInfo.RatingType.RatingTypeId === vm.ratingTypes.Star
                        || vm.epaInfo.RatingType.RatingTypeId === vm.ratingTypes.HalfStar
                        || (vm.epaInfo.RatingType.RatingTypeId === vm.ratingTypes.Score && hasKPIOptional)) {
                        vm.hasRatingByStar = true;
                    }
                }

                vm.generateRatingStar = function () {
                    $timeout(function () { $(".rating-loading").rating(); }, 1);
                };

                vm.readMore = function (item) {
                    utilitiesFactory.readMore(item);
                };

                vm.readLess = function (item) {
                    utilitiesFactory.readLess(item);
                };

                vm.checkIsRatingByStar = function (kpiGroup) {
                    return vm.epaInfo.RatingType.RatingTypeId === vm.ratingTypes.Star
                       || vm.epaInfo.RatingType.RatingTypeId === vm.ratingTypes.HalfStar
                       || (vm.epaInfo.RatingType.RatingTypeId === vm.ratingTypes.Score && kpiGroup.NumberKPIOptional > 0);
                };
            },
            controllerAs: 'performanceDetailCtrl',
            bindToController: true,
            templateUrl: '/SourceApp/Components/EPA/Share/Directives/Templates/performanceDetails.tmpl.html'
        };
    }
})(angular);
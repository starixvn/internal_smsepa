(function (angular) {
    'use strict';

    angular.module('epaModule').controller('EPAByKPIController', epaByKPIController);
    epaByKPIController.$inject = ['$window', '$timeout', '$translate', 'EPAByKPIModel', 'EmployeeEPAModel',
        'CommonModel', 'utilitiesFactory', 'historyFactory', 'baseUrl'];

    function epaByKPIController($window, $timeout, $translate, EPAByKPIModel, EmployeeEPAModel,
        CommonModel, utilitiesFactory, historyFactory, baseUrl) {
        var vm = this;
        vm.periods = [];
        vm.targetTemplates = [];
        vm.kpis = [];
        vm.memberEPAs = [];
        vm.maxStar = utilitiesFactory.maxStar;
        vm.currentPage = 1;
        vm.pageSize = 20;
        vm.filter = {
            periodId: '',
            targetTemplateId: '',
            kpiId: '',
            evaluatorId: ''
        };
        vm.evaluators = [];
        vm.RatingType = utilitiesFactory.ratingType;
        vm.hasRatingByStar = false;
        vm.hasRatingByStar = false;
        vm.showRatingScoreByStar = true;

        function getCSSSelected(kpiId) {
            var cssClass = '';

            if (kpiId === vm.filter.kpiId) {
                cssClass = 'alert-danger';
            }

            return cssClass;
        }

        function toggleCollapseExpand() {
            if (!vm.hasTreeEvent) {
                $('.tree li.parent_li > span').on('click', function (e) {
                    var children = $(this).parent('li.parent_li').find(' > ul > li');
                    if (children.is(":visible")) {
                        children.hide('fast');
                        $(this).attr('title', 'Expand this branch').find(' > i').addClass('glyphicon-plus-sign').removeClass('glyphicon-minus-sign');
                    } else {
                        children.show('fast');
                        $(this).attr('title', 'Collapse this branch').find(' > i').addClass('glyphicon-minus-sign').removeClass('glyphicon-plus-sign');
                    }

                    e.stopPropagation();
                });

                vm.hasTreeEvent = true;
            }
        }

        function getTargetTemplateByPeriod(periodId) {
            EPAByKPIModel.getTargetTemplateToView(periodId).then(function () {
                vm.targetTemplates = EPAByKPIModel.targetTemplates;
            });
        }

        function getEvaluatorsByTargetTemplate(periodId, targetTemplateId) {
            EPAByKPIModel.getEvaluatorsByTargetTemplate(periodId, targetTemplateId).then(function () {
                vm.evaluators = EPAByKPIModel.evaluators;
            });
        }

        function getKPIByToView(filter) {
            vm.kpiSelected = null;
            vm.filter.kpiId = '';
            vm.memberEPAs = [];

            EPAByKPIModel.getKPIToView(filter.periodId, filter.targetTemplateId, filter.evaluatorId).then(function () {
                vm.kpis = EPAByKPIModel.kpis;
            });
        }

        function KPIChange(kpiId) {
            if (kpiId !== vm.filter.kpiId) {
                vm.filter.kpiId = kpiId;
                var index = vm.kpis.map(function (e) { return e.KPIId; }).indexOf(vm.filter.kpiId);
                if (index !== -1) {
                    vm.kpiSelected = vm.kpis[index];
                }

                query();
            }
        }

        function query() {
            EPAByKPIModel.getEmployeeEPAToView(vm.filter.periodId, vm.filter.targetTemplateId, vm.filter.kpiId, vm.filter.evaluatorId).then(function () {
                vm.memberEPAs = EPAByKPIModel.memberEPAs;
                utilitiesFactory.searchOnPage(vm, vm.memberEPAs);

                angular.forEach(vm.memberEPAs, function (memberEPA) {
                    if (memberEPA.RatingType.RatingTypeId === vm.RatingType.Star
                        || memberEPA.RatingType.RatingTypeId === vm.RatingType.HalfStar
                        || (memberEPA.RatingType.RatingTypeId === vm.RatingType.Score && memberEPA.KPIGroup.NumberKPIOptional > 0)) {
                        vm.hasRatingByStar = true;
                    }

                    if (memberEPA.KPIGroup.HasKPIOptional) {
                        var totalStarEvaluated = 0,
                            numberKPIOptional = memberEPA.KPIGroup.NumberKPIOptional,
                            totalScoreOriginal = utilitiesFactory.parseFloat(memberEPA.TotalScoreOriginal);

                        angular.forEach(memberEPA.KPIGroup.KPIs, function (kpi) {
                            if (kpi.Rate > 0) {
                                totalStarEvaluated += kpi.Rate;
                            }

                            if (!kpi.IsRequiredEvaluate && kpi.Rate > 0) {
                                numberKPIOptional -= 1;
                            }
                        });

                        var totalKPIMustEvaluate = memberEPA.KPIGroup.KPIs.length - numberKPIOptional,
                            totalStarMustEvaluate = totalKPIMustEvaluate * vm.maxStar,
                            totalScoreEvaluated = (memberEPA.KPIGroup.Weight * totalStarEvaluated) / totalStarMustEvaluate;

                        totalScoreEvaluated = utilitiesFactory.parseFloat(totalScoreEvaluated);
                        memberEPA.TotalScoreOriginal = utilitiesFactory.parseFloat(totalScoreOriginal - totalScoreEvaluated);
                    }

                    EmployeeEPAModel.scoreSpread(memberEPA.KPIGroup);
                });
            });
        }

        function viewDetail(memberEPA) {
            var url = baseUrl + 'Evaluator/Detail/' + memberEPA.EPAId + '?evaluatorId=' + vm.filter.evaluatorId + '&newTab=' + true;
            $window.open(url, '_blank');
        }

        function back() {
            historyFactory.back();
        }

        function generateRatingStar() {
            $timeout(function () {
                $(".rating-loading").rating();
            }, 1);
        }

        function initialize() {
            EPAByKPIModel.getPeriodToView().then(function () {
                vm.periods = EPAByKPIModel.periods;
            });
        }

        vm.toggleCollapseExpand = toggleCollapseExpand;
        vm.getCSSSelected = getCSSSelected;
        vm.getTargetTemplateByPeriod = getTargetTemplateByPeriod;
        vm.getEvaluatorsByTargetTemplate = getEvaluatorsByTargetTemplate;
        vm.getKPIByToView = getKPIByToView;
        vm.KPIChange = KPIChange;
        vm.viewDetail = viewDetail;
        vm.back = back;
        vm.generateRatingStar = generateRatingStar;
        vm.initialize = initialize;

        vm.initialize();
    }
})(angular);
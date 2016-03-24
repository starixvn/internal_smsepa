'use strict';
/*global epaModule*/

epaModule.controller('EPAByKPIController',
['$window', '$timeout', '$translate', 'EPAByKPIModel', 'EmployeeEPAModel', 'CommonModel', 'utilitiesFactory', 'enumerationsFactory', 'baseUrl',
function ($window, $timeout, $translate, EPAByKPIModel, EmployeeEPAModel, CommonModel, utilitiesFactory, enumerationsFactory, baseUrl) {
    var vm = this;
    vm.kpis = [];
    vm.memberEPAs = [];
    vm.maxStar = 5;
    vm.currentPage = 1;
    vm.pageSize = 20;
    vm.filter = {
        targetTemplateId: '',
        kpiId: '',
        evaluatorId: ''
    };
    vm.evaluators = [];
    vm.RatingType = enumerationsFactory.ratingType;
    vm.toggleCollapseExpand = toggleCollapseExpand;
    vm.getCSSSelected = getCSSSelected;
    vm.getEvaluatorsByTargetTemplate = getEvaluatorsByTargetTemplate;
    vm.getKPIByTargetTemplate = getKPIByTargetTemplate;
    vm.KPIChange = KPIChange;
    vm.viewDetail = viewDetail;
    vm.back = back;
    vm.generateRatingStar = generateRatingStar;
    vm.initialize = initialize;

    vm.initialize();

    function getCSSSelected(kpiId) {
        var cssClass = '';

        if (kpiId === vm.filter.kpiId) {
            cssClass = 'alert-danger';
        }

        return cssClass;
    };

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
    };

    function getEvaluatorsByTargetTemplate() {
        EPAByKPIModel.getEvaluatorsByTargetTemplate(vm.filter.targetTemplateId).then(function () {
            vm.evaluators = EPAByKPIModel.evaluators;
        });
    };

    function getKPIByTargetTemplate() {
        vm.kpiSelected = null;
        vm.filter.kpiId = '';
        vm.memberEPAs = [];

        EPAByKPIModel.getKPIToView(vm.filter.targetTemplateId, vm.filter.evaluatorId).then(function () {
            vm.kpis = EPAByKPIModel.kpis;
        });
    };

    function KPIChange(kpiId) {
        vm.filter.kpiId = kpiId;
        query();
    };

    function query() {
        EPAByKPIModel.getEmployeeEPAToView(vm.filter.targetTemplateId, vm.filter.kpiId, vm.filter.evaluatorId).then(function () {
            vm.memberEPAs = EPAByKPIModel.memberEPAs;
            utilitiesFactory.searchOnPage(vm, vm.memberEPAs);
            angular.forEach(vm.memberEPAs, function (memberEPA) {
                if (memberEPA.KPIGroup.HasKPIOptional) {
                    var totalScoreEvaluated = 0, totalKPIMustEvaluate, totalStarMustEvaluate, totalStarEvaluated = 0,
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

                    totalKPIMustEvaluate = memberEPA.KPIGroup.KPIs.length - numberKPIOptional;
                    totalStarMustEvaluate = totalKPIMustEvaluate * vm.maxStar;

                    totalScoreEvaluated = utilitiesFactory.parseFloat(memberEPA.KPIGroup.Weight * (totalStarEvaluated / totalStarMustEvaluate));
                    memberEPA.TotalScoreOriginal = utilitiesFactory.parseFloat(totalScoreOriginal - totalScoreEvaluated);
                }

                EmployeeEPAModel.scoreSpread(memberEPA.KPIGroup);
            });
        });

    };

    function viewDetail(memberEPA) {
        var url = baseUrl + 'EPA/Detail/' + memberEPA.EPAId;
        $window.open(url, '_blank');
    };

    function back() {
        $window.history.back();
    };

    function generateRatingStar() {
        $timeout(function () {
            $(".rating-loading").rating();
        }, 1);
    };

    function initialize() {
        EPAByKPIModel.getTargetTemplateToView().then(function () {
            vm.targetTemplates = EPAByKPIModel.targetTemplates;
        });
    };

}]);

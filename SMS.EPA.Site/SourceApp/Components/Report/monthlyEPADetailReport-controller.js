'use strict';
/*global reportModule*/

reportModule.controller('MonthlyEPADetailReportController',
['$scope', '$timeout', 'ReportModel', 'CommonModel', 'utilitiesFactory', 'usSpinnerService', 'baseUrl',
function ($scope, $timeout, ReportModel, CommonModel, utilitiesFactory, usSpinnerService, baseUrl) {
    var vm = this;

    vm.treeDepartment = CommonModel.treeDepartment;
    vm.periods = CommonModel.periods;
    vm.epaDetail = ReportModel.epaDetail;
    vm.scoredBy = [
        { ColumnId: 6, PhaseId: 2, Name: 'Self' },
        { ColumnId: 7, PhaseId: 3, Name: 'Evaluator' },
        { ColumnId: 8, PhaseId: 4, Name: 'Reviewer' },
    ];

    vm.hideColumns = [];
    vm.showColumns = [6, 7, 8];
    vm.showColumnsOriginal = [6, 7, 8];

    vm.pageOption = utilitiesFactory.pageOption;

    vm.filter = {
        periodId: '',
        departmentId: '',
        isLocal: '',
        phaseIds: '',
        pageIndex: utilitiesFactory.pageOption.pageIndex,
        itemsPerPage: utilitiesFactory.pageOption.itemsPerPage
    };

    var getShowHideColumns = function () {
        vm.hideColumns = [], vm.showColumns = [];

        angular.forEach(vm.scoredBy, function (item) {
            if (item.ticked) {
                vm.showColumns.push(item.ColumnId);
            }
            else {
                vm.hideColumns.push(item.ColumnId);
            }
        });

        if (vm.hideColumns.length === vm.showColumnsOriginal.length) {
            vm.hideColumns = [];
            vm.showColumns = angular.copy(vm.showColumnsOriginal);
        }
    };

    var getMonthlyEPADetailReport = function (filter) {
        usSpinnerService.spin("spinner-1");

        ReportModel.getMonthlyEPADetailReport(
            filter.periodId,
            filter.departmentId,
            filter.isLocal,
            filter.phaseIds,
            filter.pageIndex,
            filter.itemsPerPage)
        .then(function () {
            vm.epaDetail = ReportModel.epaDetail;
            usSpinnerService.stop('spinner-1');
        });
    };

    vm.pageChanging = function (pageIndex, itemsPerPage) {
        vm.filter.pageIndex = pageIndex;
        vm.filter.itemsPerPage = itemsPerPage;

        getMonthlyEPADetailReport(vm.filter);
    };

    vm.query = function (filter) {
        getShowHideColumns();
        filter.pageIndex = utilitiesFactory.pageOption.pageIndex;
        filter.phaseIds = utilitiesFactory.getSelectedData(vm.scoredBy, 'PhaseId');

        getMonthlyEPADetailReport(filter);
    };

    vm.exportPdf = function (epaId, hasMonthlyEPA) {
        ReportModel.requestExportEPA(epaId)
        .then(function () {
            $timeout(function () {
                var url = baseUrl + 'ExportPDF/DownloadEPADetail?epaId=' + epaId + '&fileName=' + ReportModel.pdfFileName;
                var iframe = $('body iframe');
                if (iframe.length === 0) {
                    iframe = $('<iframe width="0" height="0" class="display-none"></iframe>');
                    iframe.appendTo('body');
                }
                iframe.attr('src', url);
            }, 4000);
        });

    };

    vm.exportReport = function () {
        vm.exportCriteria = {
            periodId: vm.filter.periodId,
            departmentId: vm.filter.departmentId,
            isLocal: vm.filter.isLocal,
            phaseIds: vm.filter.phaseIds
        };

        $scope.$broadcast('exportMonthlyEpaDetailReport');
    };

    vm.initialize = function () {
        CommonModel.getDepartment().then(function () {
            vm.treeDepartment = CommonModel.generateTreeDepartment(CommonModel.treeDepartment.Childrens);
        });

        ReportModel.getAllMonthlyPeriod().then(function () {
            vm.periods = ReportModel.periods;

            if (vm.periods.length > 0) {
                vm.filter.periodId = vm.periods[0].PeriodId;
            }

            getMonthlyEPADetailReport(vm.filter);
        });
    };

    vm.initialize();
}]);
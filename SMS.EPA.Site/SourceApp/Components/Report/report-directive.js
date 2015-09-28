'use strict';
/*global reportModule*/

reportModule.directive('gridEpaSummaryReport', ['$timeout', 'utilitiesFactory', function ($timeout, utilitiesFactory) {
    var generateGroupHeaders = function (periodSelecteds) {
        var groupHeaders = [];
        if (periodSelecteds && periodSelecteds.length > 0) {
            groupHeaders = [
                {
                    Columns: [
                        { HeaderText: "#", Name: "No", MergeRowNumber: 2 },
                        { HeaderText: "Code", Name: "Code", MergeRowNumber: 2 },
                        { HeaderText: "Full Name", Name: "FullName", MergeRowNumber: 2 },
                        { HeaderText: "Nick Name", Name: "NickName", MergeRowNumber: 2 },
                        { HeaderText: "Section", Name: "Section", MergeRowNumber: 2 },
                        { HeaderText: "Job Title", Name: "JobTitle", MergeRowNumber: 2 },
                        { HeaderText: "Final Score", Name: "1", MergeColNumber: periodSelecteds.length }
                    ]
                }
            ];
        }
        else {
            groupHeaders = [
                {
                    Columns: [
                        { HeaderText: "#", Name: "No", MergeRowNumber: 2 },
                        { HeaderText: "Code", Name: "Code", MergeRowNumber: 2 },
                        { HeaderText: "Full Name", Name: "FullName", MergeRowNumber: 2 },
                        { HeaderText: "Nick Name", Name: "NickName", MergeRowNumber: 2 },
                        { HeaderText: "Section", Name: "Section", MergeRowNumber: 2 },
                        { HeaderText: "Job Title", Name: "JobTitle", MergeRowNumber: 2 }
                    ]
                }
            ];
        }

        return groupHeaders;
    };

    var generateColumnHeaders = function (periodSelecteds) {
        var columnHeaders = [
            { HeaderText: "#", Width: 40, Name: "No", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Code", Width: 70, Name: "Code", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Full Name", Width: 180, Name: "FullName", HeaderAlign: "center", CellAlign: "left" },
            { HeaderText: "Nick Name", Width: 100, Name: "NickName", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Section", Width: 100, Name: "Section", HeaderAlign: "center", CellAlign: "left" },
            { HeaderText: "Job Title", Width: 168, Name: "JobTitle", HeaderAlign: "center", CellAlign: "left" }
        ];

        if (periodSelecteds && periodSelecteds.length > 0) {
            for (var i = 0; i < periodSelecteds.length; i++) {
                var periodHeader = { HeaderText: periodSelecteds[i].Name, Width: 120, Name: "" + (i + 1), HeaderAlign: "center", CellAlign: "center" };

                columnHeaders.push(periodHeader);
            }
        }

        return columnHeaders;
    };

    var generateBodyRows = function (gridData) {
        var rows = [];

        if (gridData && gridData.EPAEmployees) {
            angular.forEach(gridData.EPAEmployees, function (epaEmployee, i) {
                var currentRow = {
                    Columns: [
                        { Name: "No", Value: i + 1 },
                        { Name: "Code", Value: epaEmployee.Code },
                        { Name: "FullName", Value: epaEmployee.FullName },
                        { Name: "NickName", Value: epaEmployee.NickName },
                        { Name: "Section", Value: epaEmployee.Group.GroupName },
                        { Name: "JobTitle", Value: epaEmployee.JobTitle.JobTitleName }
                    ]
                };

                angular.forEach(gridData.PeriodSelecteds, function (periodSelected, j) {
                    var periodRow,
                        index = epaEmployee.Periods.map(function (e) { return e.PeriodId; }).indexOf(periodSelected.PeriodId);

                    if (index !== -1) {
                        var finalScore = epaEmployee.Periods[index].FinalScore;
                        periodRow = { Name: "" + (j + 1), Value: finalScore };
                    }
                    else {
                        periodRow = { Name: "" + (j + 1), Value: "-" };
                    }

                    currentRow.Columns.push(periodRow);
                });

                rows.push(currentRow);
            });
        }

        return rows;
    };

    return {
        restrict: "A",
        scope: {
            gridData: "=",
            pageOption: '=',
            onPageChanging: '=',
            gridExpandHeight: '@'
        },
        link: function ($scope, iElement) {
            var gridOptions = {
                customizeHeader: true,
                gridExpandHeight: $scope.gridExpandHeight,
                showPager: true,
                customPager: true,
                numberOfFrozenColumn: 6,
                autoFixFrozenRowHeight: true
            };

            $scope.$watch('gridData', function (gridData) {
                $timeout(function () {
                    var columnHeaders = generateColumnHeaders(gridData.PeriodSelecteds),
                        groupHeaderRows = generateGroupHeaders(gridData.PeriodSelecteds);

                    var bodyRows = generateBodyRows(gridData);

                    gridOptions.groupHeaderRows = groupHeaderRows;
                    gridOptions.columns = columnHeaders;
                    gridOptions.bodyRows = bodyRows;

                    gridOptions.pagerOption = {
                        startPerPage: utilitiesFactory.pageOption.itemsPerPage,
                        totalItem: gridData ? gridData.TotalItem : 0,
                        itemsPerPageArray: utilitiesFactory.pageOption.itemsPerPageArray,
                        itemsPerPage: $scope.pageOption.itemsPerPage,
                        currentPage: $scope.pageOption.pageIndex,
                        pagerSelectCallBack: function (pageIndex, itemsPerPage) {
                            $scope.onPageChanging(pageIndex, itemsPerPage);
                        }
                    };

                    $(iElement).fanexGrid(gridOptions);
                });
            });
        }
    };
}]);

reportModule.directive('exportEpaSummaryReport', ['baseUrl', function (baseUrl) {
    return {
        link: function (scope) {
            scope.$on('exportEpaSummaryReport', function () {
                var data = $.param(scope.epaSummaryReportCtrl.exportCriteria),
                    url = baseUrl + 'ExportReport/ExportEPASummaryReport',
                    downloadUrl = url + "?" + data,
                    iframe = $('body iframe');

                if (iframe.length === 0) {
                    iframe = $('<iframe width="0" height="0" class="display-none"></iframe>');
                    iframe.appendTo('body');
                }

                iframe.attr('src', downloadUrl);
            });
        }
    };
}]);
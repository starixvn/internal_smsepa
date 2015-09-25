'use strict';
/*global reportModule*/

reportModule.directive('gridEpaSummaryReport', ['$timeout', 'utilitiesFactory', function ($timeout, utilitiesFactory) {
    var generateGroupHeaders = function (periodSelecteds) {
        var groupHeaders = [
            {
                Columns: [
                    { HeaderText: "#", Name: "No", MergeRowNumber: 2 },
                    { HeaderText: "Code", Name: "Code", MergeRowNumber: 2 },
                    { HeaderText: "Full Name", Name: "FullName", MergeRowNumber: 2 },
                    { HeaderText: "Nick Name", Name: "NickName", MergeRowNumber: 2 },
                    { HeaderText: "Section", Name: "Section", MergeRowNumber: 2 },
                    { HeaderText: "Job Title", Name: "JobTitle", MergeRowNumber: 2 },
                    { HeaderText: "Final Score", Name: periodSelecteds[0].Name, MergeColNumber: periodSelecteds.length }
                ]
            }
        ];

        return groupHeaders;
    };

    var generateColumnHeaders = function (periodSelecteds) {
        var columnHeaders = [
            { HeaderText: "#", Width: 40, Name: "No", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Code", Width: 80, Name: "Code", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Full Name", Width: 200, Name: "FullName", HeaderAlign: "center", CellAlign: "left" },
            { HeaderText: "Nick Name", Width: 100, Name: "NickName", HeaderAlign: "center", CellAlign: "left" },
            { HeaderText: "Section", Width: 100, Name: "Section", HeaderAlign: "center", CellAlign: "left" },
            { HeaderText: "Job Title", Width: 100, Name: "JobTitle", HeaderAlign: "center", CellAlign: "left" },
            { HeaderText: "Final Score 1", Width: 100, Name: "FinalScore1", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Final Score 2", Width: 100, Name: "FinalScore2", HeaderAlign: "center", CellAlign: "center" }
        ];

        for (var i = 0; i < periodSelecteds.length; i++) {
            var periodHeader = { HeaderText: periodSelecteds[i].Name, Width: 100, Name: "" + (i + 1), HeaderAlign: "center", CellAlign: "center" };

            columnHeaders.push(periodHeader);
        }

        return columnHeaders;
    };

    var generateBodyRows = function (gridData) {
        var rows = [];

        if (gridData) {
            angular.forEach(gridData.Systems, function (system) {
                var currentRow = {
                    Columns: [
                    { Name: "FunctionName", Value: system.Name, MergeColNumber: 4 }
                    ],
                    SubRows: []
                };

                rows.push(currentRow);

                angular.forEach(system.FunctionGroups, function (functionGroup) {
                    var functionGroupRow = {
                        Columns: [
                            { Name: "FunctionName", Value: functionGroup.Name, MergeColNumber: 4 }
                        ],
                        SubRows: []
                    };

                    currentRow.SubRows.push(functionGroupRow);

                    angular.forEach(functionGroup.Functions, function (func) {
                        var functionRow = {
                            Columns: [
                                { Name: "No", Value: func.Index },
                                { Name: "FunctionName", Value: func.Name },
                                { Name: "Description", Value: func.Description }
                            ]
                        };

                        functionGroupRow.SubRows.push(functionRow);
                    });
                });
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
            $scope.$watch('gridData', function (gridData) {
                $timeout(function () {
                    var gridOptions = {
                        customizeHeader: true,
                        gridExpandHeight: $scope.gridExpandHeight,
                        showPager: true,
                        customPager: true,
                        numberOfFrozenColumn: 6
                    };

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
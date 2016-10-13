(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('reportModule').directive('gridEpaSummaryReport', gridEpaSummaryReport);
    gridEpaSummaryReport.$inject = ['$timeout', 'utilitiesFactory'];

    function gridEpaSummaryReport($timeout, utilitiesFactory) {
        var status = {
            NotAvailable: 'N/A',
            Pending: 'Pending'
        };

        function generateGroupHeaders(periodSelecteds) {
            var groupHeaders = [
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

            if (periodSelecteds && periodSelecteds.length > 0) {
                angular.forEach(periodSelecteds, function (period) {
                    var column = { HeaderText: period.Name, Name: "FinalScore" + period.PeriodId, MergeColNumber: 2, CssClass: 'column-truncate' };
                    groupHeaders[0].Columns.push(column);
                });
            }

            return groupHeaders;
        }

        function generateColumnHeaders(scope, periodSelecteds) {
            var sortCode = scope.sortColumnName === 'Code' ? scope.sortDirection : '',
                sortSection = scope.sortColumnName === 'Section' ? scope.sortDirection : '';

            var columnHeaders = [
                { HeaderText: "#", Width: 40, Name: "No", HeaderAlign: "center", CellAlign: "center" },
                { HeaderText: "Code", Width: 70, Name: "Code", HeaderAlign: "center", CellAlign: "center", Sort: true, SortDirection: sortCode, SortType: 'string' },
                { HeaderText: "Full Name", Width: 180, Name: "FullName", HeaderAlign: "center", CellAlign: "left", CanResize: true },
                { HeaderText: "Nick Name", Width: 120, Name: "NickName", HeaderAlign: "center", CellAlign: "center" },
                { HeaderText: "Section", Width: 120, Name: "Section", HeaderAlign: "center", CellAlign: "left", Sort: true, SortDirection: sortSection, SortType: 'string' },
                { HeaderText: "Job Title", Width: 225, Name: "JobTitle", HeaderAlign: "center", CellAlign: "left", CanResize: true }
            ];

            if (periodSelecteds && periodSelecteds.length > 0) {
                angular.forEach(periodSelecteds, function (period) {
                    var colFinalScore = { HeaderText: "Final Score", Width: 100, Name: "FinalScore" + period.PeriodId, HeaderAlign: "center", CellAlign: "center" },
                        colGrade = { HeaderText: "Grade", Width: 80, Name: "Grade" + period.PeriodId, HeaderAlign: "center", CellAlign: "center" };
                    columnHeaders.push(colFinalScore);
                    columnHeaders.push(colGrade);
                });
            }

            return columnHeaders;
        }

        function generateBodyRows(gridData, pageOption) {
            var rows = [];
            var rowIndex = pageOption.itemsPerPage * pageOption.pageIndex;

            if (gridData && gridData.EPAInfos) {
                angular.forEach(gridData.EPAInfos, function (epaInfo, i) {
                    var currentRow = {
                        Columns: [
                            { Name: "No", Value: rowIndex + i + 1 },
                            { Name: "Code", Value: epaInfo.Code },
                            { Name: "FullName", Value: epaInfo.FullName },
                            { Name: "NickName", Value: epaInfo.NickName },
                            { Name: "Section", Value: epaInfo.Group.GroupName },
                            { Name: "JobTitle", Value: epaInfo.JobTitle.JobTitleName }
                        ]
                    };

                    angular.forEach(gridData.PeriodSelecteds, function (periodSelected) {
                        var colFinalScore, colGrade,
                            index = epaInfo.Periods.map(function (e) { return e.PeriodId; }).indexOf(periodSelected.PeriodId);

                        if (index !== -1) {
                            var finalScore = '',
                                grade = '';

                            if (epaInfo.Periods[index].IsEndPeriod) {
                                if (epaInfo.Periods[index].FinalScore) {
                                    finalScore = utilitiesFactory.parseFloat(epaInfo.Periods[index].FinalScore);
                                    grade = epaInfo.Periods[index].Grade;
                                } else {
                                    finalScore = status.NotAvailable;
                                    grade = status.NotAvailable;
                                }
                            } else {
                                finalScore = status.Pending;
                                grade = status.Pending;
                            }

                            colFinalScore = { Name: "FinalScore" + periodSelected.PeriodId, Value: finalScore };
                            colGrade = { Name: "Grade" + periodSelected.PeriodId, Value: grade };
                        } else {
                            colFinalScore = { Name: "FinalScore" + periodSelected.PeriodId, Value: "-" };
                            colGrade = { Name: "Grade" + periodSelected.PeriodId, Value: "-" };
                        }

                        currentRow.Columns.push(colFinalScore);
                        currentRow.Columns.push(colGrade);
                    });

                    rows.push(currentRow);
                });
            }

            return rows;
        }

        return {
            restrict: "A",
            scope: {
                gridData: "=",
                pageOption: '=',
                onPageChanging: '=',
                onSortChanging: '=',
                exportReport: '=',
                sortColumnName: '=',
                sortDirection: '=',
                gridExpandHeight: '@'
            },
            link: function ($scope, iElement) {
                var gridOptions = {
                    customizeHeader: true,
                    gridExpandHeight: $scope.gridExpandHeight,
                    showPager: true,
                    customPager: true,
                    autoFixFrozenRowHeight: true,
                    autoResizeColumn: true,
                    showTitle: true,
                    titleInfo: {
                        text: "EPA Summary Report",
                        showExcelButton: true,
                        exportExcelFunction: function () {
                            $scope.exportReport();
                        }
                    },
                    sortOption: {
                        serverSortFunction: function (sortColumnName, sortDirection) {
                            $scope.onSortChanging(sortColumnName, sortDirection);
                        }
                    }
                };

                $scope.$watch('gridData', function (gridData) {
                    $timeout(function () {
                        if (gridData.PeriodSelecteds && gridData.EPAInfos) {
                            var columnHeaders = generateColumnHeaders($scope, gridData.PeriodSelecteds),
                                groupHeaderRows = generateGroupHeaders(gridData.PeriodSelecteds);

                            var bodyRows = generateBodyRows(gridData, $scope.pageOption);

                            gridOptions.groupHeaderRows = groupHeaderRows;
                            gridOptions.columns = columnHeaders;
                            gridOptions.bodyRows = bodyRows;

                            if (gridData.PeriodSelecteds.length > 0) {
                                gridOptions.numberOfFrozenColumn = 6;
                            }

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

                            $('.column-truncate').on('mouseover', function () {
                                var periodname = $(this).text();

                                $(this).attr('title', periodname);
                            });
                        }
                    });
                });
            }
        };
    }
})(angular);
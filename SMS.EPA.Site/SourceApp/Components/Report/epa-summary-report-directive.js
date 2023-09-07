(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('reportModule').directive('gridEpaSummaryReport', gridEpaSummaryReport);
    gridEpaSummaryReport.$inject = ['$timeout', 'utilitiesFactory'];

    function gridEpaSummaryReport($timeout, utilitiesFactory) {
        const status = {
            NotAvailable: 'N/A'
        };

        function generateGroupHeaders(periodSelecteds) {
            const groupHeaders = [
                {
                    Columns: [
                        { HeaderText: '#', Name: 'No', MergeRowNumber: 2 },
                        { HeaderText: 'Code', Name: 'Code', MergeRowNumber: 2 },
                        { HeaderText: 'Nickname', Name: 'NickName', MergeRowNumber: 2 },
                        { HeaderText: 'Full Name', Name: 'FullName', MergeRowNumber: 2 },
                        { HeaderText: 'Department', Name: 'Department', MergeRowNumber: 2 },
                        { HeaderText: 'Section', Name: 'Section', MergeRowNumber: 2 },
                        { HeaderText: 'Team', Name: 'Team', MergeRowNumber: 2 },
                        { HeaderText: 'Job Title', Name: 'JobTitle', MergeRowNumber: 2 }
                    ]
                }
            ];

            if (periodSelecteds && periodSelecteds.length > 0) {
                angular.forEach(periodSelecteds, function (period) {
                    const column = {
                        HeaderText: period.Name,
                        Name: 'FinalScore' + period.PeriodId,
                        MergeColNumber: 2,
                        CssClass: 'column-truncate'
                    };
                    groupHeaders[0].Columns.push(column);
                });
            }

            return groupHeaders;
        }

        function generateColumnHeaders(scope, periodSelecteds) {
            const sortCode = scope.sortColumnName === 'Code' ? scope.sortDirection : '',
                sortTeam = scope.sortColumnName === 'Team' ? scope.sortDirection : '';

            const columnHeaders = [
                { HeaderText: '#', Width: 40, Name: 'No', HeaderAlign: 'center', CellAlign: 'center' },
                {
                    HeaderText: 'Code', Width: 80, Name: 'Code',
                    HeaderAlign: 'center', CellAlign: 'center', AllowTruncate: true,
                    Sort: true, SortDirection: sortCode, SortType: 'string'
                },
                { HeaderText: 'Nickname', Width: 120, Name: 'NickName', HeaderAlign: 'center', CellAlign: 'left' },
                {
                    HeaderText: 'Full Name', Width: 180, Name: 'FullName',
                    HeaderAlign: 'center', CellAlign: 'left', AllowTruncate: true
                },
                { HeaderText: 'Department', Width: 180, Name: 'Department', HeaderAlign: 'center', CellAlign: 'left' },
                { HeaderText: 'Section', Width: 180, Name: 'Section', HeaderAlign: 'center', CellAlign: 'left' },
                {
                    HeaderText: 'Team', Width: 180, Name: 'Team',
                    HeaderAlign: 'center', CellAlign: 'left',
                    Sort: true, SortDirection: sortTeam, SortType: 'string'
                },
                { HeaderText: 'Job Title', Width: 150, Name: 'JobTitle', HeaderAlign: 'center', CellAlign: 'left' }
            ];

            if (periodSelecteds && periodSelecteds.length > 0) {
                angular.forEach(periodSelecteds, function (period) {
                    const colFinalScore = {
                        HeaderText: 'Final Score',
                        Width: 100,
                        Name: 'FinalScore' + period.PeriodId,
                        HeaderAlign: 'center',
                        CellAlign: 'center'
                    },
                        colGrade = {
                            HeaderText: 'Grade',
                            Width: 80,
                            Name: 'Grade' + period.PeriodId,
                            HeaderAlign: 'center',
                            CellAlign: 'center'
                        };
                    columnHeaders.push(colFinalScore);
                    columnHeaders.push(colGrade);
                });
            }

            return columnHeaders;
        }

        function generateBodyRows(gridData, pageOption) {
            const rows = [];
            const rowIndex = pageOption.itemsPerPage * pageOption.pageIndex;

            if (gridData && gridData.EPAInfos) {
                angular.forEach(gridData.EPAInfos, function (epaInfo, i) {
                    const currentRow = {
                        Columns: [
                            { Name: 'No', Value: rowIndex + i + 1 },
                            { Name: 'Code', Value: epaInfo.Code },
                            { Name: 'NickName', Value: epaInfo.NickName },
                            { Name: 'FullName', Value: epaInfo.FullName },
                            { Name: 'Department', Value: epaInfo.Department },
                            { Name: 'Section', Value: epaInfo.Section },
                            { Name: 'Team', Value: epaInfo.Group !== null ? epaInfo.Group.GroupName : '' },
                            { Name: 'JobTitle', Value: epaInfo.JobTitle.JobTitleName }
                        ]
                    };

                    angular.forEach(gridData.PeriodSelecteds, function (periodSelected) {
                        const index = epaInfo.Periods.map(function (e) { return e.PeriodId; })
                            .indexOf(periodSelected.PeriodId);

                        let finalScore,
                            grade,
                            cssClass = '';

                        if (index !== -1) {
                            // Calculate Final Score and Grade
                            if (epaInfo.Periods[index].FinalScore.length > 0) {
                                finalScore = utilitiesFactory.parseFloat(epaInfo.Periods[index].FinalScore);
                                grade = epaInfo.Periods[index].Grade;
                            } else {
                                finalScore = status.NotAvailable;
                                grade = status.NotAvailable;
                            }

                            if (!epaInfo.Periods[index].IsClosed) {
                                cssClass = 'red';
                            }
                        } else {
                            finalScore = '-';
                            grade = '-';
                        }

                        // Set data for colFinalScore and colGrade
                        const colFinalScore = { Name: 'FinalScore' + periodSelected.PeriodId, Value: finalScore, CssClass: cssClass };
                        const colGrade = { Name: 'Grade' + periodSelected.PeriodId, Value: grade, CssClass: cssClass };

                        // Display to user
                        currentRow.Columns.push(colFinalScore);
                        currentRow.Columns.push(colGrade);
                    });

                    rows.push(currentRow);
                });
            }

            return rows;
        }

        return {
            restrict: 'A',
            scope: {
                gridData: '=',
                pageOption: '=',
                onSortChanging: '=',
                exportReport: '=',
                sortColumnName: '=',
                sortDirection: '=',
                gridExpandHeight: '@'
            },
            link: function ($scope, iElement) {
                const gridOptions = {
                    customizeHeader: true,
                    gridExpandHeight: $scope.gridExpandHeight,
                    showPager: true,
                    autoFixFrozenRowHeight: true,
                    autoResizeColumn: true,
                    showTitle: true,
                    titleInfo: {
                        text: 'EPA Summary Report',
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
                            gridOptions.groupHeaderRows = generateGroupHeaders(gridData.PeriodSelecteds);
                            gridOptions.columns = generateColumnHeaders($scope, gridData.PeriodSelecteds);
                            gridOptions.bodyRows = generateBodyRows(gridData, $scope.pageOption);

                            if (gridData.PeriodSelecteds.length > 0) {
                                gridOptions.numberOfFrozenColumn = 6;
                            }

                            gridOptions.pagerOption = {
                                startPerPage: utilitiesFactory.pageOption.itemsPerPage,
                                totalItem: gridData ? gridData.EPAInfos.length : 0,
                                itemsPerPageArray: utilitiesFactory.pageOption.itemsPerPageArray,
                                itemsPerPage: $scope.pageOption.itemsPerPage,
                                currentPage: $scope.pageOption.pageIndex
                            };

                            $(iElement).fanexGrid(gridOptions);

                            $('.column-truncate').on('mouseover', function () {
                                const periodname = $(this).text();

                                $(this).attr('title', periodname);
                            });
                        }
                    });
                });
            }
        };
    }
})(angular);
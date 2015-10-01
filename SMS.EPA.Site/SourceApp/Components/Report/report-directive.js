'use strict';
/*global reportModule*/

reportModule.directive('gridEpaSummaryReport', ['$timeout', 'utilitiesFactory', function ($timeout, utilitiesFactory) {
    var generateGroupHeaders = function (periodSelecteds) {
        var groupHeaders = [
            {
                Columns: [
                    { HeaderText: "Hiden Column", Name: "HidenColumn", MergeColNumber: 1 },
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
            var column = { HeaderText: "Final Score", Name: "1", MergeColNumber: periodSelecteds.length };

            groupHeaders[0].Columns.push(column);
        }

        return groupHeaders;
    };

    var generateColumnHeaders = function (periodSelecteds) {
        var columnHeaders = [
            { HeaderText: "Hiden Column", Width: 40, Name: "HidenColumn", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "#", Width: 40, Name: "No", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Code", Width: 70, Name: "Code", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Full Name", Width: 180, Name: "FullName", HeaderAlign: "center", CellAlign: "left" },
            { HeaderText: "Nick Name", Width: 100, Name: "NickName", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Section", Width: 160, Name: "Section", HeaderAlign: "center", CellAlign: "left" },
            { HeaderText: "Job Title", Width: 190, Name: "JobTitle", HeaderAlign: "center", CellAlign: "left" }
        ];

        if (periodSelecteds && periodSelecteds.length > 0) {
            for (var i = 0; i < periodSelecteds.length; i++) {
                var periodHeader = { HeaderText: periodSelecteds[i].Name, Width: 100, Name: "" + (i + 1), HeaderAlign: "center", CellAlign: "center" };

                columnHeaders.push(periodHeader);
            }
        }

        return columnHeaders;
    };

    var generateBodyRows = function (gridData, pageOption) {
        var rows = [];
        var rowIndex = pageOption.itemsPerPage * pageOption.pageIndex;

        if (gridData && gridData.EPAEmployees) {
            angular.forEach(gridData.EPAEmployees, function (epaEmployee, i) {
                var currentRow = {
                    Columns: [
                        { Name: "HidenColumn", Value: i + 1 },
                        { Name: "No", Value: rowIndex + i + 1 },
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
                numberOfFrozenColumn: 7,
                autoFixFrozenRowHeight: true
            };

            $scope.$watch('gridData', function (gridData) {
                $timeout(function () {
                    if (gridData.PeriodSelecteds && gridData.EPAEmployees) {
                        var columnHeaders = generateColumnHeaders(gridData.PeriodSelecteds),
                            groupHeaderRows = generateGroupHeaders(gridData.PeriodSelecteds);

                        var bodyRows = generateBodyRows(gridData, $scope.pageOption);

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
                        $(iElement).fanexGrid("hideColumns", gridOptions, [0]);
                    }
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

reportModule.directive('gridEpaDetailReport', ['$timeout', 'utilitiesFactory', function ($timeout, utilitiesFactory) {
    var generateColumnHeaders = function () {
        var columnHeaders = [
            { HeaderText: "#", Width: 40, Name: "No", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Code", Width: 60, Name: "Code", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Full Name", Width: 160, Name: "FullName", HeaderAlign: "center", CellAlign: "left" },
            { HeaderText: "Nick Name", Width: 100, Name: "NickName", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Section", Width: 150, Name: "Section", HeaderAlign: "center", CellAlign: "left" },
            { HeaderText: "Job Title", Width: 150, Name: "JobTitle", HeaderAlign: "center", CellAlign: "left" },
            { HeaderText: "Target Type", Width: 150, Name: "TargetType", HeaderAlign: "center", CellAlign: "left" },
            { HeaderText: "Self Score", Width: 80, Name: "SelfScore", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Evaluator Score", Width: 80, Name: "EvaluatorScore", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Reviewer Score", Width: 80, Name: "ReviewerScore", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Management Score", Width: 80, Name: "ManagementScore", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Discipline Score", Width: 80, Name: "DisciplineScore", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Final Score", Width: 80, Name: "FinalScore", HeaderAlign: "center", CellAlign: "center" }
        ];

        return columnHeaders;
    };

    var generateBodyRows = function (gridData, pageOption) {
        var rows = [];
        var rowIndex = pageOption.itemsPerPage * pageOption.pageIndex;

        if (gridData.MemberEPAs) {
            angular.forEach(gridData.MemberEPAs, function (memberEPA, index) {
                var disciplineScore = '';
                if (memberEPA.HasDisciplineScore) {
                    disciplineScore = memberEPA.DisciplineScore;
                }

                var currentRow = {
                    Columns: [
                        { Name: "No", Value: rowIndex + index + 1 },
                        { Name: "Code", Value: memberEPA.Employee.Code },
                        { Name: "FullName", Value: memberEPA.Employee.FullName },
                        { Name: "NickName", Value: memberEPA.Employee.NickName },
                        { Name: "Section", Value: memberEPA.Employee.Group.GroupName },
                        { Name: "JobTitle", Value: memberEPA.Employee.JobTitle.JobTitleName },
                        { Name: "TargetType", Value: '' },
                        { Name: "SelfScore", Value: memberEPA.ProcessBySelf.TotalScore },
                        { Name: "EvaluatorScore", Value: memberEPA.AverageScoreByEvaluators },
                        { Name: "ReviewerScore", Value: memberEPA.AverageScoreByReviewers },
                        { Name: "ManagementScore", Value: memberEPA.AverageScoreByManagements },
                        { Name: "DisciplineScore", Value: disciplineScore },
                        { Name: "FinalScore", Value: memberEPA.FinalScore }
                    ]
                };

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
            hideColumns: '=',
            gridExpandHeight: '@'
        },
        link: function ($scope, iElement) {
            var columHeaders = generateColumnHeaders();

            var gridOptions = {
                columns: columHeaders,
                gridExpandHeight: $scope.gridExpandHeight,
                showPager: true,
                customPager: true,
                numberOfFrozenColumn: 6,
                autoFixFrozenRowHeight: true,
                showColumnsShowHide: false,
                columnsNeedHide: []
            };

            $scope.$watch('gridData', function (gridData) {
                $timeout(function () {
                    if (gridData.MemberEPAs) {
                        var bodyRows = generateBodyRows(gridData, $scope.pageOption);

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
                        $(iElement).fanexGrid("hideColumns", gridOptions, $scope.hideColumns);
                    }
                });
            });


        }
    };
}]);
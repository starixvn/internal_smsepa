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
                    { HeaderText: "Job Title", Name: "JobTitle", MergeRowNumber: 2 }
                ]
            }
        ];

        if (periodSelecteds && periodSelecteds.length > 0) {
            angular.forEach(periodSelecteds, function (period) {
                var column = { HeaderText: period.Name, Name: "FinalScore" + period.PeriodId, MergeColNumber: 2 };
                groupHeaders[0].Columns.push(column);
            });
        }

        return groupHeaders;
    };

    var generateColumnHeaders = function (periodSelecteds) {
        var columnHeaders = [
            { HeaderText: "#", Width: 40, Name: "No", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Code", Width: 70, Name: "Code", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Full Name", Width: 180, Name: "FullName", HeaderAlign: "center", CellAlign: "left" },
            { HeaderText: "Nick Name", Width: 100, Name: "NickName", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Section", Width: 160, Name: "Section", HeaderAlign: "center", CellAlign: "left" },
            { HeaderText: "Job Title", Width: 225, Name: "JobTitle", HeaderAlign: "center", CellAlign: "left" }
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
    };

    var generateBodyRows = function (gridData, pageOption) {
        var rows = [];
        var rowIndex = pageOption.itemsPerPage * pageOption.pageIndex;

        if (gridData && gridData.EPAEmployees) {
            angular.forEach(gridData.EPAEmployees, function (epaEmployee, i) {
                var currentRow = {
                    Columns: [
                        { Name: "No", Value: rowIndex + i + 1 },
                        { Name: "Code", Value: epaEmployee.Code },
                        { Name: "FullName", Value: epaEmployee.FullName },
                        { Name: "NickName", Value: epaEmployee.NickName },
                        { Name: "Section", Value: epaEmployee.Group.GroupName },
                        { Name: "JobTitle", Value: epaEmployee.JobTitle.JobTitleName }
                    ]
                };

                angular.forEach(gridData.PeriodSelecteds, function (periodSelected, j) {
                    var colFinalScore, colGrade,
                        index = epaEmployee.Periods.map(function (e) { return e.PeriodId; }).indexOf(periodSelected.PeriodId);

                    if (index !== -1) {
                        var finalScore, grade, disciplineScore;

                        if (epaEmployee.Periods[index].Status.StatusId === 1) {
                            finalScore = utilitiesFactory.parseFloat(epaEmployee.Periods[index].FinalScore);
                            grade = epaEmployee.Periods[index].Grade;

                            if (epaEmployee.Periods[index].HasDisciplineScore) {
                                disciplineScore = epaEmployee.Periods[index].MaxDisciplineScore - epaEmployee.Periods[index].DisciplineScore;

                                if (disciplineScore < 0) {
                                    disciplineScore = 0;
                                }

                                finalScore += disciplineScore;
                            }
                        }

                        colFinalScore = { Name: "FinalScore" + periodSelected.PeriodId, Value: finalScore };
                        colGrade = { Name: "Grade" + periodSelected.PeriodId, Value: grade };
                    }
                    else {
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
    };

    return {
        restrict: "A",
        scope: {
            gridData: "=",
            pageOption: '=',
            onPageChanging: '=',
            exportReport: '=',
            gridExpandHeight: '@'
        },
        link: function ($scope, iElement) {
            var gridOptions = {
                customizeHeader: true,
                gridExpandHeight: $scope.gridExpandHeight,
                showPager: true,
                customPager: true,
                numberOfFrozenColumn: 6,
                enableHiddenColumns: false,
                autoFixFrozenRowHeight: true,
                showTitle: true,
                titleInfo: {
                    text: "EPA Summary Report",
                    showExcelButton: true,
                    exportExcelFunction: function () {
                        $scope.exportReport();
                    }
                }
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
            { HeaderText: "Job Title", Width: 180, Name: "JobTitle", HeaderAlign: "center", CellAlign: "left" },
            { HeaderText: "Target Type", Width: 120, Name: "TargetType", HeaderAlign: "center", CellAlign: "left" },
            { HeaderText: "Self Score", Width: 80, Name: "SelfScore", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Evaluator Score", Width: 80, Name: "EvaluatorScore", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Reviewer Score", Width: 80, Name: "ReviewerScore", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Management Score", Width: 80, Name: "ManagementScore", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Discipline Score", Width: 80, Name: "DisciplineScore", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Final Score", Width: 80, Name: "FinalScore", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Grade", Width: 80, Name: "Grade", HeaderAlign: "center", CellAlign: "center" }
        ];

        return columnHeaders;
    };

    var calculateScore = function (memberEPA) {
        var result = {
            SelfScore: '',
            EvaluatorScore: '',
            ReviewerScore: '',
            ManagementScore: '',
            DisciplineScore: '',
            FinalScore: '',
            Grade: ''
        };

        if (memberEPA.HasDisciplineScore) {
            result.DisciplineScore = memberEPA.MaxDisciplineScore - memberEPA.DisciplineScore;

            if (result.DisciplineScore < 0) {
                result.DisciplineScore = 0;
            }
        }

        if (memberEPA.EPAId > 0 && memberEPA.ProcessBySelf.IsEndProcess) {
            result.SelfScore = utilitiesFactory.parseFloat(memberEPA.ProcessBySelf.TotalScore);
            result.SelfScore += result.DisciplineScore;
        }

        if (memberEPA.EPAId > 0 && memberEPA.ProcessByEvaluators[0].IsEndProcess) {
            result.EvaluatorScore = memberEPA.AverageScoreByEvaluators;
            result.EvaluatorScore += result.DisciplineScore;
        }

        if (memberEPA.EPAId > 0 && memberEPA.ProcessByReviewers.length > 0 && memberEPA.ProcessByReviewers[0].IsEndProcess) {
            result.ReviewerScore = memberEPA.AverageScoreByReviewers;
            result.ReviewerScore += result.DisciplineScore;
        }

        if (memberEPA.EPAId > 0 && memberEPA.ProcessByManagements.length > 0 && memberEPA.ProcessByManagements[0].IsEndProcess) {
            result.ManagementScore = memberEPA.AverageScoreByManagements;
            result.ManagementScore += result.DisciplineScore;
        }

        if (memberEPA.Status.StatusId === 1) {
            result.FinalScore = utilitiesFactory.parseFloat(memberEPA.FinalScore);
            result.FinalScore += result.DisciplineScore;
            result.Grade = memberEPA.Grade;
        }

        return result;
    };

    var generateBodyRows = function (gridData, pageOption) {
        var rows = [];
        var rowIndex = pageOption.itemsPerPage * pageOption.pageIndex;

        if (gridData.MemberEPAs) {
            angular.forEach(gridData.MemberEPAs, function (memberEPA, index) {
                var score = calculateScore(memberEPA);

                var currentRow = {
                    Columns: [
                        { Name: "No", Value: rowIndex + index + 1 },
                        { Name: "Code", Value: memberEPA.Employee.Code },
                        { Name: "FullName", Value: memberEPA.Employee.FullName },
                        { Name: "NickName", Value: memberEPA.Employee.NickName },
                        { Name: "Section", Value: memberEPA.Employee.Group.GroupName },
                        { Name: "JobTitle", Value: memberEPA.Employee.JobTitle.JobTitleName },
                        { Name: "TargetType", Value: memberEPA.Period.TargetTypeName },
                        { Name: "SelfScore", Value: score.SelfScore },
                        { Name: "EvaluatorScore", Value: score.EvaluatorScore },
                        { Name: "ReviewerScore", Value: score.ReviewerScore },
                        { Name: "ManagementScore", Value: score.ManagementScore },
                        { Name: "DisciplineScore", Value: score.DisciplineScore },
                        { Name: "FinalScore", Value: score.FinalScore },
                        { Name: "Grade", Value: score.Grade }
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
            exportReport: '=',
            gridExpandHeight: '@'
        },
        link: function ($scope, iElement) {
            var columHeaders = generateColumnHeaders();

            var gridOptions = {
                columns: columHeaders,
                gridExpandHeight: $scope.gridExpandHeight,
                showPager: true,
                customPager: true,
                numberOfFrozenColumn: 4,
                autoFixFrozenRowHeight: true,
                columnsNeedHide: [],
                enableHiddenColumns: false,
                showTitle: true,
                titleInfo: {
                    text: "EPA Detail Report",
                    showExcelButton: true,
                    exportExcelFunction: function () {
                        $scope.exportReport();
                    }
                }
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

reportModule.directive('exportEpaDetailReport', ['baseUrl', function (baseUrl) {
    return {
        link: function (scope) {
            scope.$on('exportEpaDetailReport', function () {
                var data = $.param(scope.epaDetailReportCtrl.exportCriteria),
                    url = baseUrl + 'ExportReport/ExportEPADetailReport',
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
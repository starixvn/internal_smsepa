'use strict';
/*global reportModule*/

reportModule.directive('gridEpaSummaryReport', ['$timeout', 'utilitiesFactory',
function ($timeout, utilitiesFactory) {
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
            { HeaderText: "Full Name", Width: 180, Name: "FullName", HeaderAlign: "center", CellAlign: "left" },
            { HeaderText: "Nick Name", Width: 100, Name: "NickName", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Section", Width: 160, Name: "Section", HeaderAlign: "center", CellAlign: "left", Sort: true, SortDirection: sortSection, SortType: 'string' },
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
}]);

reportModule.directive('gridEpaDetailReport', ['$timeout', 'utilitiesFactory',
function ($timeout, utilitiesFactory) {
    var status = {
        NotAvailable: 'N/A',
        Pending: 'Pending'
    };

    function generateColumnHeaders(scope) {
        var sortCode = scope.sortColumnName === 'Code' ? scope.sortDirection : '',
            sortSection = scope.sortColumnName === 'Section' ? scope.sortDirection : '';

        var columnHeaders = [
            { HeaderText: "#", Width: 40, Name: "No", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Code", Width: 60, Name: "Code", HeaderAlign: "center", CellAlign: "center", Sort: true, SortDirection: sortCode, SortType: 'string' },
            { HeaderText: "Full Name", Width: 160, Name: "FullName", HeaderAlign: "center", CellAlign: "left" },
            { HeaderText: "Nick Name", Width: 100, Name: "NickName", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Section", Width: 150, Name: "Section", HeaderAlign: "center", CellAlign: "left", Sort: true, SortDirection: sortSection, SortType: 'string' },
            { HeaderText: "Job Title", Width: 180, Name: "JobTitle", HeaderAlign: "center", CellAlign: "left" },
            { HeaderText: "Target Type", Width: 120, Name: "TargetType", HeaderAlign: "center", CellAlign: "left" },
            { HeaderText: "Self Score", Width: 80, Name: "SelfScore", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Evaluator Score", Width: 80, Name: "EvaluatorScore", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Reviewer Score", Width: 80, Name: "ReviewerScore", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Management Score", Width: 80, Name: "ManagementScore", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Discipline Score", Width: 80, Name: "DisciplineScore", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Final Score", Width: 80, Name: "FinalScore", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Grade", Width: 80, Name: "Grade", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Export PDF", Width: 80, Name: "ExportPdf", HeaderAlign: "center", CellAlign: "center" }
        ];

        return columnHeaders;
    }

    function calculateScore(epaInfo) {
        var result = {
            SelfScore: '',
            EvaluatorScore: '',
            ReviewerScore: '',
            ManagementScore: '',
            DisciplineScore: '',
            FinalScore: '',
            Grade: ''
        };

        if (epaInfo.HasDisciplineScore) {
            result.DisciplineScore = epaInfo.DisciplineScore;
        } else {
            if (epaInfo.DisciplineScore !== 0) {
                result.DisciplineScore = epaInfo.DisciplineScore;
            } else {
                result.DisciplineScore = '-';
            }
        }

        if (epaInfo.EPAId > 0 && epaInfo.Process.Self.StatusEndProcess !== utilitiesFactory.processStatusType.Open) {
            if (epaInfo.HasMonthlyEPA && epaInfo.Period.TargetTypeId === utilitiesFactory.targetType.SemiAnnual) {
                result.SelfScore = epaInfo.MonthlyEPA.AverageSelfScoreConverted;
            } else {
                result.SelfScore = utilitiesFactory.parseFloat(epaInfo.Process.Self.TotalScore);
            }
        }

        if (epaInfo.EPAId > 0 && epaInfo.Process.Evaluators[0].StatusEndProcess !== utilitiesFactory.processStatusType.Open) {
            if (epaInfo.HasMonthlyEPA && epaInfo.Period.TargetTypeId === utilitiesFactory.targetType.SemiAnnual) {
                result.EvaluatorScore = epaInfo.MonthlyEPA.AverageEvaluatorScoreConverted;
            } else {
                result.EvaluatorScore = epaInfo.Process.AverageScoreByEvaluators;
            }
        }

        if (epaInfo.EPAId > 0 && epaInfo.Process.Reviewers[0].StatusEndProcess !== utilitiesFactory.processStatusType.Open) {
            result.ReviewerScore = epaInfo.Process.AverageScoreByReviewers;
        }

        if (epaInfo.EPAId > 0
            && epaInfo.Process.Managements.length > 0
            && epaInfo.Process.Managements[0].StatusEndProcess !== utilitiesFactory.processStatusType.Open) {
            result.ManagementScore = epaInfo.Process.AverageScoreByManagements;
        }

        if (epaInfo.Period.IsEndPeriod) {
            if (epaInfo.FinalScore) {
                result.FinalScore = utilitiesFactory.parseFloat(epaInfo.FinalScore);
                result.Grade = epaInfo.Grade;
            } else {
                result.FinalScore = status.NotAvailable;
                result.Grade = status.NotAvailable;
            }
        } else {
            result.FinalScore = status.Pending;
            result.Grade = status.Pending;
        }

        return result;
    }

    function generateBodyRows(gridData, pageOption) {
        var rows = [];
        var rowIndex = pageOption.itemsPerPage * pageOption.pageIndex;

        if (gridData.MemberEPAs) {
            angular.forEach(gridData.MemberEPAs, function (memberEPA, index) {
                var score = calculateScore(memberEPA),
                    cssSelfScore = memberEPA.EPAId > 0 && !memberEPA.HasMonthlyEPA && !memberEPA.Process.Self.IsEndProcess ? 'red' : '',
                    cssEvaluatorScore = memberEPA.EPAId > 0 && !memberEPA.HasMonthlyEPA && !memberEPA.Process.Evaluators[0].IsEndProcess ? 'red' : '',
                    cssReviewerScore = memberEPA.EPAId > 0 && !memberEPA.Process.Reviewers[0].IsEndProcess ? 'red' : '',
                    cssManagementScore = '',
                    cssDisciplineScore = memberEPA.EPAId > 0 && memberEPA.DisciplineScore < 0 ? 'red' : '',
                    pdfIcon = '<a class="icon-pdf exportPDF" data-epaid="' + memberEPA.EPAId + '" data-hasmonthlyepa="' + memberEPA.HasMonthlyEPA + '"></input>';

                if (memberEPA.EPAId > 0
                    && memberEPA.Process.Managements
                    && memberEPA.Process.Managements.length > 0
                    && !memberEPA.Process.Managements[0].IsEndProcess) {
                    cssManagementScore = 'red';
                }

                var currentRow = {
                    Columns: [
                        { Name: "No", Value: rowIndex + index + 1 },
                        { Name: "Code", Value: memberEPA.Employee.Code },
                        { Name: "FullName", Value: memberEPA.Employee.FullName },
                        { Name: "NickName", Value: memberEPA.Employee.NickName },
                        { Name: "Section", Value: memberEPA.Employee.GroupName },
                        { Name: "JobTitle", Value: memberEPA.Employee.JobTitleName },
                        { Name: "TargetType", Value: memberEPA.Period.TargetTypeName },
                        { Name: "SelfScore", Value: "<span class='" + cssSelfScore + "'>" + score.SelfScore + "</span>" },
                        { Name: "EvaluatorScore", Value: "<span class='" + cssEvaluatorScore + "'>" + score.EvaluatorScore + "</span>" },
                        { Name: "ReviewerScore", Value: "<span class='" + cssReviewerScore + "'>" + score.ReviewerScore + "</span>" },
                        { Name: "ManagementScore", Value: "<span class='" + cssManagementScore + "'>" + score.ManagementScore + "</span>" },
                        { Name: "DisciplineScore", Value: "<span class='" + cssDisciplineScore + "'>" + score.DisciplineScore + "</span>" },
                        { Name: "FinalScore", Value: score.FinalScore },
                        { Name: "Grade", Value: score.Grade },
                        { Name: "ExportPdf", Value: memberEPA.EPAId > 0 && memberEPA.Status.StatusId === 1 && memberEPA.Period.IsEndPeriod ? pdfIcon : "" }
                    ]
                };

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
            hideColumns: '=',
            exportReport: '=',
            exportPdf: '=',
            sortColumnName: '=',
            sortDirection: '=',
            gridExpandHeight: '@'
        },
        link: function ($scope, iElement) {
            var gridOptions = {
                gridExpandHeight: $scope.gridExpandHeight,
                showPager: true,
                customPager: true,
                numberOfFrozenColumn: 7,
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
                },
                sortOption: {
                    serverSortFunction: function (sortColumnName, sortDirection) {
                        $scope.onSortChanging(sortColumnName, sortDirection);
                    }
                }
            };

            $scope.$watch('gridData', function (gridData) {
                $timeout(function () {
                    if (gridData.MemberEPAs) {
                        var columHeaders = generateColumnHeaders($scope),
                            bodyRows = generateBodyRows(gridData, $scope.pageOption);

                        gridOptions.columns = columHeaders;
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
                        $('a.exportPDF').on('click', function (event) {
                            event.preventDefault();

                            var epaId = parseInt($(this).data('epaid'));
                            var hasMonthlyEPA = $(this).data('hasmonthlyepa');
                            $scope.exportPdf(epaId, hasMonthlyEPA);
                        });
                    }
                });
            });
        }
    };
}]);

reportModule.directive('gridMonthlyEpaDetailReport', ['$timeout', 'utilitiesFactory',
function ($timeout, utilitiesFactory) {
    var status = {
        NotAvailable: 'N/A',
        Pending: 'Pending'
    };

    function generateColumnHeaders(scope) {
        var sortCode = scope.sortColumnName === 'Code' ? scope.sortDirection : '',
            sortSection = scope.sortColumnName === 'Section' ? scope.sortDirection : '';

        var columnHeaders = [
            { HeaderText: "#", Width: 40, Name: "No", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Code", Width: 60, Name: "Code", HeaderAlign: "center", CellAlign: "center", Sort: true, SortDirection: sortCode, SortType: 'string' },
            { HeaderText: "Full Name", Width: 160, Name: "FullName", HeaderAlign: "center", CellAlign: "left" },
            { HeaderText: "Nick Name", Width: 100, Name: "NickName", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Section", Width: 150, Name: "Section", HeaderAlign: "center", CellAlign: "left", Sort: true, SortDirection: sortSection, SortType: 'string' },
            { HeaderText: "Job Title", Width: 180, Name: "JobTitle", HeaderAlign: "center", CellAlign: "left" },
            { HeaderText: "Self Score", Width: 80, Name: "SelfScore", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Evaluator Score", Width: 80, Name: "EvaluatorScore", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Reviewer Score", Width: 80, Name: "ReviewerScore", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Final Score", Width: 80, Name: "FinalScore", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Grade", Width: 80, Name: "Grade", HeaderAlign: "center", CellAlign: "center" },
            //{ HeaderText: "Export PDF", Width: 80, Name: "ExportPdf", HeaderAlign: "center", CellAlign: "center" }
        ];

        return columnHeaders;
    }

    function calculateScore(epaInfo) {
        var result = {
            SelfScore: '',
            EvaluatorScore: '',
            ReviewerScore: '',
            FinalScore: '',
            Grade: ''
        };

        if (epaInfo.EPAId > 0 && epaInfo.Process.Self.StatusEndProcess !== utilitiesFactory.processStatusType.Open) {
            result.SelfScore = utilitiesFactory.parseFloat(epaInfo.Process.Self.TotalScore);
        }

        if (epaInfo.EPAId > 0 && epaInfo.Process.Evaluators[0].StatusEndProcess !== utilitiesFactory.processStatusType.Open) {
            result.EvaluatorScore = epaInfo.Process.AverageScoreByEvaluators;
        }

        if (epaInfo.EPAId > 0 && epaInfo.Process.Reviewers[0].StatusEndProcess !== utilitiesFactory.processStatusType.Open) {
            result.ReviewerScore = epaInfo.Process.AverageScoreByReviewers;
        }

        if (epaInfo.Period.IsEndPeriod) {
            if (epaInfo.FinalScore) {
                result.FinalScore = utilitiesFactory.parseFloat(epaInfo.FinalScore);
                result.Grade = epaInfo.Grade;
            } else {
                result.FinalScore = status.NotAvailable;
                result.Grade = status.NotAvailable;
            }
        } else {
            result.FinalScore = status.Pending;
            result.Grade = status.Pending;
        }

        return result;
    }

    function generateBodyRows(gridData, pageOption) {
        var rows = [];
        var rowIndex = pageOption.itemsPerPage * pageOption.pageIndex;

        if (gridData.MemberEPAs) {
            angular.forEach(gridData.MemberEPAs, function (memberEPA, index) {
                var score = calculateScore(memberEPA),
                //pdfIcon = '<a class="icon-pdf exportPDF" data-epaid="' + memberEPA.EPAId + '"></input>',
                cssSelfScore = memberEPA.EPAId > 0 && memberEPA.Process.Self.IsEndProcess ? '' : 'red',
                cssEvaluatorScore = memberEPA.EPAId > 0 && memberEPA.Process.Evaluators[0].IsEndProcess ? '' : 'red',
                cssReviewerScore = memberEPA.EPAId > 0 && memberEPA.Process.Reviewers[0].IsEndProcess ? '' : 'red';

                var currentRow = {
                    Columns: [
                        { Name: "No", Value: rowIndex + index + 1 },
                        { Name: "Code", Value: memberEPA.Employee.Code },
                        { Name: "FullName", Value: memberEPA.Employee.FullName },
                        { Name: "NickName", Value: memberEPA.Employee.NickName },
                        { Name: "Section", Value: memberEPA.Employee.GroupName },
                        { Name: "JobTitle", Value: memberEPA.Employee.JobTitleName },
                        { Name: "SelfScore", Value: "<span class='" + cssSelfScore + "'>" + score.SelfScore + "</span>" },
                        { Name: "EvaluatorScore", Value: "<span class='" + cssEvaluatorScore + "'>" + score.EvaluatorScore + "</span>" },
                        { Name: "ReviewerScore", Value: "<span class='" + cssReviewerScore + "'>" + score.ReviewerScore + "</span>" },
                        { Name: "FinalScore", Value: score.FinalScore },
                        { Name: "Grade", Value: score.Grade },
                        //{ Name: "ExportPdf", Value: memberEPA.EPAId > 0 && memberEPA.Status.StatusId == 1 ? pdfIcon : "" }
                    ]
                };

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
            hideColumns: '=',
            exportReport: '=',
            exportPdf: '=',
            sortColumnName: '=',
            sortDirection: '=',
            gridExpandHeight: '@'
        },
        link: function ($scope, iElement) {
            var gridOptions = {
                gridExpandHeight: $scope.gridExpandHeight,
                showPager: true,
                customPager: true,
                numberOfFrozenColumn: 6,
                autoFixFrozenRowHeight: true,
                columnsNeedHide: [],
                enableHiddenColumns: false,
                showTitle: true,
                titleInfo: {
                    text: "Monthly EPA Detail Report",
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
                    if (gridData.MemberEPAs) {
                        var columHeaders = generateColumnHeaders($scope),
                            bodyRows = generateBodyRows(gridData, $scope.pageOption);

                        gridOptions.columns = columHeaders;
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

                        //$('a.exportPDF').on('click', function (event) {
                        //    event.preventDefault();

                        //    var epaId = parseInt($(this).data('epaid'));
                        //    $scope.exportPdf(epaId, false);
                        //});
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

reportModule.directive('exportMonthlyEpaSummaryReport', ['baseUrl', function (baseUrl) {
    return {
        link: function (scope) {
            scope.$on('exportMonthlyEpaSummaryReport', function () {
                var data = $.param(scope.monthlySummaryReportCtrl.exportCriteria),
                    url = baseUrl + 'ExportReport/ExportMonthlyEPASummaryReport',
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

reportModule.directive('exportMonthlyEpaDetailReport', ['baseUrl', function (baseUrl) {
    return {
        link: function (scope) {
            scope.$on('exportMonthlyEpaDetailReport', function () {
                var data = $.param(scope.monthlyDetailReportCtrl.exportCriteria),
                    url = baseUrl + 'ExportReport/ExportMonthlyEPADetailReport',
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
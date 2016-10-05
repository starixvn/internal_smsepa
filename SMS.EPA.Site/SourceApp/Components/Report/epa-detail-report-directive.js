(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('reportModule').directive('gridEpaDetailReport', gridEpaDetailReport);
    gridEpaDetailReport.$inject = ['$timeout', 'utilitiesFactory'];

    function gridEpaDetailReport($timeout, utilitiesFactory) {
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
                { HeaderText: "Full Name", Width: 160, Name: "FullName", HeaderAlign: "center", CellAlign: "left", CanResize: true },
                { HeaderText: "Nick Name", Width: 100, Name: "NickName", HeaderAlign: "center", CellAlign: "center", CanResize: true },
                { HeaderText: "Section", Width: 150, Name: "Section", HeaderAlign: "center", CellAlign: "left", Sort: true, SortDirection: sortSection, SortType: 'string', CanResize: true },
                { HeaderText: "Job Title", Width: 180, Name: "JobTitle", HeaderAlign: "center", CellAlign: "left", CanResize: true },
                { HeaderText: "Group Date", Width: 80, Name: "GroupDate", HeaderAlign: "center", CellAlign: "center", FormatDateTime: "DD MMM, YYYY" },
                { HeaderText: "Target Type", Width: 120, Name: "TargetType", HeaderAlign: "center", CellAlign: "left", CanResize: true },
                { HeaderText: "Self Score", Width: 80, Name: "SelfScore", HeaderAlign: "center", CellAlign: "center", CanResize: true },
                { HeaderText: "Evaluator Score", Width: 80, Name: "EvaluatorScore", HeaderAlign: "center", CellAlign: "center", CanResize: true },
                { HeaderText: "Reviewer Score", Width: 80, Name: "ReviewerScore", HeaderAlign: "center", CellAlign: "center", CanResize: true },
                { HeaderText: "Management Score", Width: 80, Name: "ManagementScore", HeaderAlign: "center", CellAlign: "center", CanResize: true },
                { HeaderText: "Discipline Score", Width: 80, Name: "DisciplineScore", HeaderAlign: "center", CellAlign: "center", CanResize: true },
                { HeaderText: "Final Score", Width: 80, Name: "FinalScore", HeaderAlign: "center", CellAlign: "center", CanResize: true },
                { HeaderText: "Grade", Width: 80, Name: "Grade", HeaderAlign: "center", CellAlign: "center", CanResize: true },
                { HeaderText: "Export PDF", Width: 80, Name: "ExportPdf", HeaderAlign: "center", CellAlign: "center" },
                { HeaderText: "View Detail", Width: 80, Name: "ViewDetail", HeaderAlign: "center", CellAlign: "center" }
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
                        pdfIcon = '<a class="icon-pdf exportPDF" data-epaid="' + memberEPA.EPAId + '"></a>',
                        viewIcon = '<a class="icon-view viewEPADetail" data-epaid="' + memberEPA.EPAId + '"></a>';

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
                            { Name: "GroupDate", Value: memberEPA.Employee.JoinedDate },
                            { Name: "TargetType", Value: memberEPA.Period.TargetTypeName },
                            { Name: "SelfScore", Value: "<span class='" + cssSelfScore + "'>" + score.SelfScore + "</span>" },
                            { Name: "EvaluatorScore", Value: "<span class='" + cssEvaluatorScore + "'>" + score.EvaluatorScore + "</span>" },
                            { Name: "ReviewerScore", Value: "<span class='" + cssReviewerScore + "'>" + score.ReviewerScore + "</span>" },
                            { Name: "ManagementScore", Value: "<span class='" + cssManagementScore + "'>" + score.ManagementScore + "</span>" },
                            { Name: "DisciplineScore", Value: "<span class='" + cssDisciplineScore + "'>" + score.DisciplineScore + "</span>" },
                            { Name: "FinalScore", Value: score.FinalScore },
                            { Name: "Grade", Value: score.Grade },
                            { Name: "ExportPdf", Value: memberEPA.EPAId > 0 && memberEPA.Status.StatusId === 1 && memberEPA.Period.IsEndPeriod ? pdfIcon : "" },
                            { Name: "ViewDetail", Value: memberEPA.EPAId > 0 && memberEPA.Status.StatusId === 1 && memberEPA.Period.IsEndPeriod ? viewIcon : "" }
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
                viewEpaDetail: '=',
                sortColumnName: '=',
                sortDirection: '=',
                gridExpandHeight: '@'
            },
            link: function ($scope, iElement) {
                var gridOptions = {
                    gridExpandHeight: $scope.gridExpandHeight,
                    showPager: true,
                    customPager: true,
                    numberOfFrozenColumn: 8,
                    autoFixFrozenRowHeight: true,
                    autoResizeColumn: true,
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
                                $scope.exportPdf(epaId);
                            });

                            $('a.viewEPADetail').on('click', function (event) {
                                event.preventDefault();

                                var epaId = parseInt($(this).data('epaid'));
                                $scope.viewEpaDetail(epaId);
                            });
                        }
                    });
                });
            }
        };
    }
})(angular);
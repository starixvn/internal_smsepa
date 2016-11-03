(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('reportModule').directive('gridMonthlyEpaDetailReport', gridMonthlyEpaDetailReport);
    gridMonthlyEpaDetailReport.$inject = ['$window', '$timeout', 'utilitiesFactory', 'baseUrl'];

    function gridMonthlyEpaDetailReport($window, $timeout, utilitiesFactory, baseUrl) {
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
                { HeaderText: "Nick Name", Width: 120, Name: "NickName", HeaderAlign: "center", CellAlign: "center" },
                { HeaderText: "Section", Width: 150, Name: "Section", HeaderAlign: "center", CellAlign: "left", Sort: true, SortDirection: sortSection, SortType: 'string' },
                { HeaderText: "Job Title", Width: 180, Name: "JobTitle", HeaderAlign: "center", CellAlign: "left", CanResize: true },
                { HeaderText: "Group Date", Width: 80, Name: "GroupDate", HeaderAlign: "center", CellAlign: "center", FormatDateTime: "DD MMM, YYYY" },
                { HeaderText: "Self Score", Width: 80, Name: "SelfScore", HeaderAlign: "center", CellAlign: "center" },
                { HeaderText: "Evaluator Score", Width: 80, Name: "EvaluatorScore", HeaderAlign: "center", CellAlign: "center" },
                { HeaderText: "Reviewer Score", Width: 80, Name: "ReviewerScore", HeaderAlign: "center", CellAlign: "center" },
                { HeaderText: "Final Score", Width: 80, Name: "FinalScore", HeaderAlign: "center", CellAlign: "center" },
                { HeaderText: "Grade", Width: 80, Name: "Grade", HeaderAlign: "center", CellAlign: "center" },
                { HeaderText: "View Detail", Width: 80, Name: "ViewDetail", HeaderAlign: "center", CellAlign: "center" }
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
                    viewIcon = '<a class="icon-view viewEPADetail" data-epaid="' + memberEPA.EPAId + '"></a>',
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
                            { Name: "GroupDate", Value: memberEPA.Employee.JoinedDate },
                            { Name: "SelfScore", Value: "<span class='" + cssSelfScore + "'>" + score.SelfScore + "</span>" },
                            { Name: "EvaluatorScore", Value: "<span class='" + cssEvaluatorScore + "'>" + score.EvaluatorScore + "</span>" },
                            { Name: "ReviewerScore", Value: "<span class='" + cssReviewerScore + "'>" + score.ReviewerScore + "</span>" },
                            { Name: "FinalScore", Value: score.FinalScore },
                            { Name: "Grade", Value: score.Grade },
                            { Name: "ViewDetail", Value: memberEPA.EPAId > 0 ? viewIcon : '' }
                        ]
                    };

                    rows.push(currentRow);
                });
            }

            return rows;
        }

        function registerViewEPADetailEvent() {
            $('.freegrid').off('click', '.table-cell a.viewEPADetail');

            $('.freegrid').on('click', '.table-cell a.viewEPADetail', function (event) {
                event.preventDefault();

                var epaId = parseInt($(this).data('epaid'));
                $window.location.href = baseUrl + 'EPA/Detail/' + epaId;
            });
        }

        return {
            restrict: "A",
            scope: {
                gridData: "=",
                pageOption: '=',
                onSortChanging: '=',
                hideColumns: '=',
                exportReport: '=',
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
                    autoResizeColumn: true,
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
                                totalItem: gridData ? gridData.MemberEPAs.length : 0,
                                itemsPerPageArray: utilitiesFactory.pageOption.itemsPerPageArray,
                                itemsPerPage: $scope.pageOption.itemsPerPage,
                                currentPage: $scope.pageOption.pageIndex
                            };

                            $(iElement).fanexGrid(gridOptions);
                            $(iElement).fanexGrid("hideColumns", gridOptions, $scope.hideColumns);

                            registerViewEPADetailEvent();
                        }
                    });
                });
            }
        };
    }
})(angular);
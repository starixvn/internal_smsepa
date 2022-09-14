(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('reportModule').directive('gridMonthlyEpaDetailReport', gridMonthlyEpaDetailReport);
    gridMonthlyEpaDetailReport.$inject = ['$timeout', 'utilitiesFactory', 'baseUrl'];

    function gridMonthlyEpaDetailReport($timeout, utilitiesFactory, baseUrl) {
        var status = {
            NotAvailable: 'N/A',
            Pending: 'Pending'
        };

        function generateColumnHeaders(scope) {
            var sortCode = scope.sortColumnName === 'Code' ? scope.sortDirection : '',
                sortTeam = scope.sortColumnName === 'Team' ? scope.sortDirection : '';

            var columnHeaders = [
                { HeaderText: '#', Width: 40, Name: 'No', HeaderAlign: 'center', CellAlign: 'center' },
                {
                    HeaderText: 'Code', Width: 60, Name: 'Code', HeaderAlign: 'center', CellAlign: 'center', AllowTruncate: true,
                    Sort: true, SortDirection: sortCode, SortType: 'string'
                },
                { HeaderText: 'Nickname', Width: 120, Name: 'NickName', HeaderAlign: 'center', CellAlign: 'center', AllowTruncate: true },
                { HeaderText: 'Full Name', Width: 160, Name: 'FullName', HeaderAlign: 'center', CellAlign: 'left' },
                { HeaderText: 'Section', Width: 150, Name: 'Section', HeaderAlign: 'center', CellAlign: 'left' },
                {
                    HeaderText: 'Team', Width: 150, Name: 'Team', HeaderAlign: 'center', CellAlign: 'left',
                    Sort: true, SortDirection: sortTeam, SortType: 'string'
                },
                { HeaderText: 'Job Title', Width: 120, Name: 'JobTitle', HeaderAlign: 'center', CellAlign: 'left' },
                { HeaderText: 'Group Date', Width: 80, Name: 'GroupDate', HeaderAlign: 'center', CellAlign: 'center', FormatDateTime: 'DD MMM, YYYY' },
                { HeaderText: 'Target Template', Width: 180, Name: 'TargetTemplate', HeaderAlign: 'center', CellAlign: 'left' },
                { HeaderText: 'Self Score', Width: 80, Name: 'SelfScore', HeaderAlign: 'center', CellAlign: 'center' },
                { HeaderText: 'Evaluator Score', Width: 80, Name: 'EvaluatorScore', HeaderAlign: 'center', CellAlign: 'center' },
                { HeaderText: 'Reviewer Score', Width: 80, Name: 'ReviewerScore', HeaderAlign: 'center', CellAlign: 'center' },
                { HeaderText: 'Final Score', Width: 80, Name: 'FinalScore', HeaderAlign: 'center', CellAlign: 'center' },
                { HeaderText: 'Grade', Width: 80, Name: 'Grade', HeaderAlign: 'center', CellAlign: 'center' },
                { HeaderText: 'View Detail', Width: 80, Name: 'ViewDetail', HeaderAlign: 'center', CellAlign: 'center' }
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

            if (epaInfo.EPAId > 0) {
                switch (epaInfo.Process.Self.StatusId) {
                    case utilitiesFactory.processStatus.InProgress:
                        result.SelfScore = epaInfo.Process.Self.TotalScore.length > 0
                            ? utilitiesFactory.parseFloat(epaInfo.Process.Self.TotalScore) : 0;
                        break;
                    case utilitiesFactory.processStatus.Closed:
                        result.SelfScore = epaInfo.Process.Self.TotalScore.length > 0
                            ? utilitiesFactory.parseFloat(epaInfo.Process.Self.TotalScore) : status.NotAvailable;
                        break;
                }

                switch (epaInfo.Process.Evaluators[0].StatusId) {
                    case utilitiesFactory.processStatus.InProgress:
                        result.EvaluatorScore = epaInfo.Process.AverageScoreByEvaluators !== null
                            ? epaInfo.Process.AverageScoreByEvaluators : 0;
                        break;
                    case utilitiesFactory.processStatus.Closed:
                        result.EvaluatorScore = epaInfo.Process.AverageScoreByEvaluators !== null
                            ? epaInfo.Process.AverageScoreByEvaluators : status.NotAvailable;
                        break;
                }

                switch (epaInfo.Process.Reviewers[0].StatusId) {
                    case utilitiesFactory.processStatus.InProgress:
                        result.ReviewerScore = epaInfo.Process.AverageScoreByReviewers !== null
                            ? epaInfo.Process.AverageScoreByReviewers : 0;
                        break;
                    case utilitiesFactory.processStatus.Closed:
                        result.ReviewerScore = epaInfo.Process.AverageScoreByReviewers !== null ?
                            epaInfo.Process.AverageScoreByReviewers : status.NotAvailable;
                        break;
                }
            }

            if (epaInfo.StatusId === utilitiesFactory.epaStatus.Closed) {
                if (epaInfo.FinalScore.length > 0) {
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

            if (gridData.EpaInfos) {
                angular.forEach(gridData.EpaInfos, function (epaInfo, index) {
                    var score = calculateScore(epaInfo),
                    viewIcon = '<a class="icon-view viewEPADetail" data-epaid="' + epaInfo.EPAId + '"></a>',
                    cssSelfScore = epaInfo.EPAId > 0 && epaInfo.Process.Self.IsEndProcess ? '' : 'red',
                    cssEvaluatorScore = epaInfo.EPAId > 0 && epaInfo.Process.Evaluators[0].IsEndProcess ? '' : 'red',
                    cssReviewerScore = epaInfo.EPAId > 0 && epaInfo.Process.Reviewers[0].IsEndProcess ? '' : 'red';

                    var currentRow = {
                        Columns: [
                            { Name: 'No', Value: rowIndex + index + 1 },
                            { Name: 'Code', Value: epaInfo.Employee.Code },
                            { Name: 'NickName', Value: epaInfo.Employee.NickName },
                            { Name: 'FullName', Value: epaInfo.Employee.FullName },
                            { Name: 'Section', Value: epaInfo.Employee.DepartmentName },
                            { Name: 'Team', Value: epaInfo.Employee.GroupName },
                            { Name: 'JobTitle', Value: epaInfo.Employee.JobTitleName },
                            { Name: 'GroupDate', Value: epaInfo.Employee.JoinedDate },
                            { Name: 'TargetTemplate', Value: epaInfo.Period.TargetTemplate },
                            { Name: 'SelfScore', Value: "<span class='" + cssSelfScore + "'>" + score.SelfScore + "</span>" },
                            { Name: 'EvaluatorScore', Value: "<span class='" + cssEvaluatorScore + "'>" + score.EvaluatorScore + "</span>" },
                            { Name: 'ReviewerScore', Value: "<span class='" + cssReviewerScore + "'>" + score.ReviewerScore + "</span>" },
                            { Name: 'FinalScore', Value: score.FinalScore },
                            { Name: 'Grade', Value: score.Grade },
                            { Name: 'ViewDetail', Value: epaInfo.EPAId > 0 ? viewIcon : '' }
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
                var epaId = parseInt($(this).data('epaid'));
                var url = baseUrl + '#' + 'EPA/Detail/' + epaId;
                $(this).attr('href', url);
            });
        }

        return {
            restrict: 'A',
            scope: {
                gridData: '=',
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
                        text: 'Monthly EPA Detail Report',
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
                        if (gridData.EpaInfos) {
                            var columHeaders = generateColumnHeaders($scope),
                                bodyRows = generateBodyRows(gridData, $scope.pageOption);

                            gridOptions.columns = columHeaders;
                            gridOptions.bodyRows = bodyRows;

                            gridOptions.pagerOption = {
                                startPerPage: utilitiesFactory.pageOption.itemsPerPage,
                                totalItem: gridData ? gridData.EpaInfos.length : 0,
                                itemsPerPageArray: utilitiesFactory.pageOption.itemsPerPageArray,
                                itemsPerPage: $scope.pageOption.itemsPerPage,
                                currentPage: $scope.pageOption.pageIndex
                            };

                            $(iElement).fanexGrid(gridOptions);
                            $(iElement).fanexGrid('hideColumns', gridOptions, $scope.hideColumns);

                            registerViewEPADetailEvent();
                        }
                    });
                });
            }
        };
    }
})(angular);
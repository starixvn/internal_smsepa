(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('reportModule').directive('gridEpaDetailReport', gridEpaDetailReport);
    gridEpaDetailReport.$inject = ['$window', '$timeout', 'utilitiesFactory', 'EvaluationModel', 'baseUrl'];

    function gridEpaDetailReport($window, $timeout, utilitiesFactory, EvaluationModel, baseUrl) {
        function generateColumnHeaders(scope) {
            var sortCode = scope.sortColumnName === 'Code' ? scope.sortDirection : '',
                sortTeam = scope.sortColumnName === 'Team' ? scope.sortDirection : '';

            var columnHeaders = [
                { HeaderText: '#', Width: 40, Name: 'No', HeaderAlign: 'center', CellAlign: 'center' },
                {
                    HeaderText: 'Code', Width: 60, Name: 'Code',
                    HeaderAlign: 'center', CellAlign: 'center', AllowTruncate: true,
                    Sort: true, SortDirection: sortCode, SortType: 'string'
                },
                { HeaderText: 'Nickname', Width: 100, Name: 'NickName', HeaderAlign: 'center', CellAlign: 'left', AllowTruncate: true },
                { HeaderText: 'Full Name', Width: 140, Name: 'FullName', HeaderAlign: 'center', CellAlign: 'left' },
                { HeaderText: 'Department', Width: 120, Name: 'Department', HeaderAlign: 'center', CellAlign: 'left' },
                { HeaderText: 'Section', Width: 120, Name: 'Section', HeaderAlign: 'center', CellAlign: 'left' },
                {
                    HeaderText: 'Team', Width: 120, Name: 'Team',
                    HeaderAlign: 'center', CellAlign: 'left',
                    Sort: true, SortDirection: sortTeam, SortType: 'string'
                },
                { HeaderText: 'Job Title', Width: 120, Name: 'JobTitle', HeaderAlign: 'center', CellAlign: 'left' },
                { HeaderText: 'Group Date', Width: 80, Name: 'GroupDate', HeaderAlign: 'center', CellAlign: 'left', FormatDateTime: 'DD MMM, YYYY' },
                { HeaderText: 'Target Type', Width: 120, Name: 'TargetType', HeaderAlign: 'center', CellAlign: 'left' },
                { HeaderText: 'Target Template', Width: 120, Name: 'TargetTemplate', HeaderAlign: 'center', CellAlign: 'left' },
                { HeaderText: 'Self Score', Width: 80, Name: 'SelfScore', HeaderAlign: 'center', CellAlign: 'center' },
                { HeaderText: 'Evaluator Score', Width: 80, Name: 'EvaluatorScore', HeaderAlign: 'center', CellAlign: 'center' },
                { HeaderText: 'Reviewer Score', Width: 80, Name: 'ReviewerScore', HeaderAlign: 'center', CellAlign: 'center' },
                { HeaderText: 'Management Score', Width: 80, Name: 'ManagementScore', HeaderAlign: 'center', CellAlign: 'center' },
                { HeaderText: 'Discipline Score', Width: 80, Name: 'DisciplineScore', HeaderAlign: 'center', CellAlign: 'center' },
                { HeaderText: 'Final Score', Width: 80, Name: 'FinalScore', HeaderAlign: 'center', CellAlign: 'center' },
                { HeaderText: 'Grade', Width: 80, Name: 'Grade', HeaderAlign: 'center', CellAlign: 'center' },
                { HeaderText: 'Export PDF', Width: 80, Name: 'ExportPdf', HeaderAlign: 'center', CellAlign: 'center' },
                { HeaderText: 'View Detail', Width: 80, Name: 'ViewDetail', HeaderAlign: 'center', CellAlign: 'center' }
            ];

            return columnHeaders;
        }

        function generateBodyRows(gridData, pageOption) {
            var rows = [];
            var rowIndex = pageOption.itemsPerPage * pageOption.pageIndex;

            if (gridData.EpaInfos) {
                angular.forEach(gridData.EpaInfos,
                    function (epaInfo, index) {
                        var score = EvaluationModel.calculateScoreAndGradeToDisplay(epaInfo);
                        var cssSelfScore = epaInfo.EPAId > 0 &&
                            !epaInfo.HasMonthlyEPA &&
                            epaInfo.Process.Self.StatusId === utilitiesFactory.processStatus.InProgress
                            ? 'red' : '';
                        var cssEvaluatorScore = epaInfo.EPAId > 0 &&
                            !epaInfo.HasMonthlyEPA &&
                            epaInfo.Process.Evaluators[0].StatusId === utilitiesFactory.processStatus.InProgress
                            ? 'red' : '';
                        var cssReviewerScore = epaInfo.EPAId > 0 && epaInfo.Process.Reviewers[0].StatusId === utilitiesFactory.processStatus.InProgress ? 'red' : '';
                        var cssManagementScore = '';
                        var cssDisciplineScore = epaInfo.EPAId > 0 && epaInfo.DisciplineScore < 0 ? 'red' : '';
                        var cssPendingShowDefaultRedValue = epaInfo.StatusId === utilitiesFactory.epaStatus.Closed ? '' : 'red';
                        var pdfIcon = '<a class="icon-pdf exportPDF" data-epaid="' + epaInfo.EPAId + '"></a>';
                        var viewIcon = '<a class="icon-view viewEPADetail" data-epaid="' + epaInfo.EPAId + '"></a>';

                        if (epaInfo.EPAId > 0 &&
                            epaInfo.Process.Managements &&
                            epaInfo.Process.Managements.length > 0 &&
                            epaInfo.Process.Managements[0].StatusId === utilitiesFactory.processStatus.InProgress) {
                            cssManagementScore = 'red';
                        }

                        var currentRow = {
                            Columns: [
                                { Name: 'No', Value: rowIndex + index + 1 },
                                { Name: 'Code', Value: epaInfo.Employee.Code },
                                { Name: 'NickName', Value: epaInfo.Employee.NickName },
                                { Name: 'FullName', Value: epaInfo.Employee.FullName },
                                { Name: 'Department', Value: epaInfo.Employee.DepartmentName },
                                { Name: 'Section', Value: epaInfo.Employee.Section },
                                { Name: 'Team', Value: epaInfo.Employee.GroupName },
                                { Name: 'JobTitle', Value: epaInfo.Employee.JobTitleName },
                                { Name: 'GroupDate', Value: epaInfo.Employee.JoinedDate },
                                { Name: 'TargetType', Value: epaInfo.Period.TargetTypeName },
                                { Name: 'TargetTemplate', Value: epaInfo.Period.TargetTemplate },
                                { Name: 'SelfScore', Value: "<span class='" + cssSelfScore + "'>" + score.SelfScore + "</span>" },
                                { Name: 'EvaluatorScore', Value: "<span class='" + cssEvaluatorScore + "'>" + score.EvaluatorScore + "</span>" },
                                { Name: 'ReviewerScore', Value: "<span class='" + cssReviewerScore + "'>" + score.ReviewerScore + "</span>" },
                                { Name: 'ManagementScore', Value: "<span class='" + cssManagementScore + "'>" + score.ManagementScore + "</span>" },
                                { Name: 'DisciplineScore', Value: "<span class='" + cssDisciplineScore + "'>" + score.DisciplineScore + "</span>" },
                                { Name: 'FinalScore', Value: "<span class='" + cssPendingShowDefaultRedValue + "'>" + score.FinalScore + "</span>" },
                                { Name: 'Grade', Value: "<span class='" + cssPendingShowDefaultRedValue + "'>" + score.Grade + "</span>" },
                                { Name: 'ExportPdf', Value: epaInfo.EPAId > 0 ? pdfIcon : '' },
                                { Name: 'ViewDetail', Value: epaInfo.EPAId > 0 ? viewIcon : '' }
                            ]
                        };

                        rows.push(currentRow);
                    });
            }

            return rows;
        }

        function registerViewEPADetailEvent(pageOption) {
            $('.freegrid').off('click', '.table-cell a.viewEPADetail');

            $('.freegrid').on('click',
                '.table-cell a.viewEPADetail',
                function (event) {
                    var epaId = parseInt($(this).data('epaid'));
                    var url = baseUrl + '#' +
                        'EPA/Detail/' +
                        epaId +
                        '?periodId=' +
                        pageOption.periodId +
                        '&departmentId=' +
                        pageOption.departmentId +
                        '&phaseIds=' +
                        pageOption.phaseIds +
                        '&isLocal=' +
                        pageOption.isLocal +
                        '&employeeId=' +
                        pageOption.employeeId +
                        '&exportPdf&detailRpt';
                    $(this).attr('href', url);
                });
        }

        function registerExportPdfEvent() {
            $('.freegrid').off('click', '.table-cell a.exportPDF');

            $('.freegrid').on('click',
                '.table-cell a.exportPDF',
                function (event) {
                    event.preventDefault();

                    var epaId = parseInt($(this).data('epaid'));

                    var url = baseUrl + 'ExportPdf/ExportPdf/' + epaId;
                    var iframe = $('body iframe');

                    if (iframe.length === 0) {
                        iframe = $('<iframe width="0" height="0" class="display-none"></iframe>');
                        iframe.appendTo('body');
                    }

                    iframe.attr('src', url);
                });
        }

        function registerExportExcelEvent(scope) {
            var exportRecommendationFunction = function () {
                scope.exportRecommendationReport();
            }

            var exportDetailFunction = function () {
                scope.exportReport();
            }

            var exportCommentFunction = function () {
                scope.exportCommentReport();
            }

            $('#export-detail')
                .off('click.fanex.grid')
                .on('click.fanex.grid', exportDetailFunction);

            $('#export-recommendation')
                .off('click.fanex.grid')
                .on('click.fanex.grid', exportRecommendationFunction);

            $('#export-comment')
                .off('click.fanex.grid')
                .on('click.fanex.grid', exportCommentFunction);
        }

        return {
            restrict: 'A',
            scope: {
                gridData: '=',
                pageOption: '=',
                onPageChanging: '=',
                onSortChanging: '=',
                hideColumns: '=',
                exportReport: '=',
                exportRecommendationReport: '=',
                exportCommentReport: '=',
                sortColumnName: '=',
                sortDirection: '=',
                gridExpandHeight: '@'
            },
            link: function ($scope, iElement) {
                var gridOptions = {
                    gridExpandHeight: $scope.gridExpandHeight,
                    showPager: true,
                    numberOfFrozenColumn: 7,
                    autoFixFrozenRowHeight: true,
                    autoResizeColumn: true,
                    columnsNeedHide: [],
                    enableHiddenColumns: false,
                    showTitle: true,
                    titleInfo: {
                        text: 'EPA Detail Report',
                        customButtons:
                            '<div class="dropdown">'
                            + '<input type="button" class="btnExcel" title="Export to Excel" id="excelMenu" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">'
                            + '<ul class="dropdown-menu dropdown-menu-right" aria-labelledby="dropdownMenuButton">'
                            + '<li><a class="link dropdown-item" id="export-detail">Export Detail</a></li>'
                            + '<li><a class="link dropdown-item" id="export-comment">Export Comment</a></li>'
                            + '<li><a class="link dropdown-item" id="export-recommendation">Export Recommendation</a></li>'
                            + ' </ul>'
                            + ' </div>'
                    },
                    sortOption: {
                        serverSortFunction: function (sortColumnName, sortDirection) {
                            $scope.onSortChanging(sortColumnName, sortDirection);
                        }
                    }
                };

                $scope.$watch('gridData',
                    function (gridData) {
                        $timeout(function () {
                            if (gridData.EpaInfos) {
                                gridOptions.columns = generateColumnHeaders($scope);
                                gridOptions.bodyRows = generateBodyRows(gridData, $scope.pageOption);

                                gridOptions.pagerOption = {
                                    startPerPage: utilitiesFactory.pageOption.itemsPerPage,
                                    totalItem: gridData ? gridData.EpaInfos.length : 0,
                                    itemsPerPageArray: utilitiesFactory.pageOption.itemsPerPageArray,
                                    itemsPerPage: $scope.pageOption.itemsPerPage,
                                    currentPage: $scope.pageOption.pageIndex
                                };

                                $(iElement).fanexGrid(gridOptions);
                                $(iElement).fanexGrid('hideColumns', gridOptions, $scope.hideColumns);

                                registerViewEPADetailEvent($scope.pageOption);
                                registerExportPdfEvent();
                                registerExportExcelEvent($scope);
                            }
                        });
                    });
            }
        };
    }
})(angular);
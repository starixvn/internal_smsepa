(function (angular) {
    'use strict';

    angular.module('feedbackReportModule').directive('feedbackReportGrid', feedbackReportGrid);
    feedbackReportGrid.$inject = ['$timeout', 'utilitiesFactory', 'baseUrl'];

    function feedbackReportGrid($timeout, utilitiesFactory, baseUrl) {
        function generateColumns() {
            var columns = [
                { HeaderText: 'User', Width: 200, Name: 'Nickname', HeaderAlign: 'center', CellAlign: 'left', ShowCollapseExpand: true },
                { HeaderText: 'Average Final Score', Width: 150, Name: 'FinalScore', HeaderAlign: 'center', CellAlign: 'center' },
                { HeaderText: 'Total Comment', Width: 200, Name: 'Comment', HeaderAlign: 'center', CellAlign: 'left', CanResize: true },
                { HeaderText: 'Total Evaluate', Width: 200, Name: 'Evaluator', HeaderAlign: 'center', CellAlign: 'center' },
                { HeaderText: 'Detail', Width: 80, Name: 'Detail', HeaderAlign: 'center', CellAlign: 'center' }
            ];

            return columns;
        }

        function registerViewDetailEvent() {
            $('.freegrid').off('click', '.table-cell a.viewDetail');
            $('.freegrid').on('click', '.table-cell a.viewDetail', function (event) {
                var feedbackId = parseInt($(this).data('feedbackid'));
                var url = baseUrl + '#' + 'feedback/detail/' + feedbackId;
                $(this).attr('href', url);
            });
        }

        function generateSubRows(employee, feedbacks) {
            var rows = [];
            angular.forEach(feedbacks, function (feedback) {
                var viewIcon = '<a class="icon-view viewDetail" data-feedbackid="' + feedback.FeedbackId + '"></a>';
                var row = {
                    Columns: [
                        { Name: 'Nickname', Value: employee.Nickname },
                        { Name: 'FinalScore', Value: feedback.FinalScore },
                        { Name: 'Comment', Value: feedback.Comment },
                        { Name: 'Evaluator', Value: feedback.Evaluator.Nickname },
                        { Name: 'Detail', Value: viewIcon }
                    ],
                    SubRows: ''
                };

                rows.push(row);
            });

            return rows;
        }

        function generateBodyRows(gridData) {
            var rows = [];
            angular.forEach(gridData, function (employee) {
                var currentRow = {
                    Columns: [
                        { Name: 'Nickname', Value: employee.Nickname },
                        { Name: 'FinalScore', Value: employee.AverageFinalScore },
                        { Name: 'Comment', Value: employee.TotalComment, CssClass: 'cell-content-total-comment' },
                        { Name: 'Evaluator', Value: employee.TotalEvaluate },
                        { Name: 'Detail', Value: '' }
                    ],
                    CssClass: 'parent-row',
                    SubRows: generateSubRows(employee, employee.Feedbacks)
                };

                rows.push(currentRow);
            });

            return rows;
        }

        return {
            restrict: 'A',
            scope: {
                gridData: '=',
                onExportExcel: '='
            },
            link: function ($scope, iElement) {
                $scope.$watch('gridData', function (gridData) {
                    $timeout(function () {
                        if (gridData) {
                            var gridOptions =
                            {
                                columns: generateColumns(),
                                bodyRows: generateBodyRows(gridData),
                                gridExpandHeight: 300,
                                showPager: true,
                                autoResizeColumn: true,
                                showTree: true,
                                showTitle: true,
                                titleInfo:
                                {
                                    text: '360 Degree EPA Report',
                                    showExcelButton: true,
                                    exportExcelFunction: function () {
                                        $scope.onExportExcel();
                                    },
                                    showCollapseExpandButton: true
                                },
                                defaultCollapseAllGroups: false,
                                pagerOption: {
                                    startPerPage: utilitiesFactory.pageOption.itemsPerPage,
                                    totalItem: gridData ? gridData.length : 0,
                                    itemsPerPageArray: utilitiesFactory.pageOption.itemsPerPageArray,
                                    itemsPerPage: utilitiesFactory.pageOption.itemsPerPageArray[0],
                                    currentPage: 0
                                }
                            };

                            $(iElement).fanexGrid(gridOptions);

                            registerViewDetailEvent();
                        }
                    });
                });
            }
        };
    }
})(angular);
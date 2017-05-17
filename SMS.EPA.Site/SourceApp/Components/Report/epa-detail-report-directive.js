(function (angular) {
    "use strict";
    /* jshint latedef:nofunc */

    angular.module("reportModule").directive("gridEpaDetailReport", gridEpaDetailReport);
    gridEpaDetailReport.$inject = ["$window", "$timeout", "utilitiesFactory", "EvaluationModel", "CommonModel", "baseUrl"];

    function gridEpaDetailReport($window, $timeout, utilitiesFactory, EvaluationModel, CommonModel, baseUrl) {
        var status = {
            NotAvailable: "N/A"
        };

        CommonModel.getStandardGrade();

        var standardGrades = CommonModel.standardGrades;

        function generateColumnHeaders(scope) {
            var sortCode = scope.sortColumnName === "Code" ? scope.sortDirection : "",
                sortSection = scope.sortColumnName === "Section" ? scope.sortDirection : "";

            var columnHeaders = [
                { HeaderText: "#", Width: 40, Name: "No", HeaderAlign: "center", CellAlign: "center" },
                {
                    HeaderText: "Code",
                    Width: 60,
                    Name: "Code",
                    HeaderAlign: "center",
                    CellAlign: "center",
                    Sort: true,
                    SortDirection: sortCode,
                    SortType: "string"
                },
                { HeaderText: "Full Name", Width: 140, Name: "FullName", HeaderAlign: "center", CellAlign: "left", CanResize: true },
                { HeaderText: "Nick Name", Width: 100, Name: "NickName", HeaderAlign: "center", CellAlign: "left", AllowTruncate: true },
                {
                    HeaderText: "Section",
                    Width: 120,
                    Name: "Section",
                    HeaderAlign: "center",
                    CellAlign: "left",
                    Sort: true,
                    SortDirection: sortSection,
                    SortType: "string"
                },
                { HeaderText: "Job Title", Width: 160, Name: "JobTitle", HeaderAlign: "center", CellAlign: "left", CanResize: true },
                { HeaderText: "Group Date", Width: 80, Name: "GroupDate", HeaderAlign: "center", CellAlign: "left", FormatDateTime: "DD MMM, YYYY" },
                { HeaderText: "Target Type", Width: 120, Name: "TargetType", HeaderAlign: "center", CellAlign: "left" },
                { HeaderText: "Self Score", Width: 80, Name: "SelfScore", HeaderAlign: "center", CellAlign: "center" },
                { HeaderText: "Evaluator Score", Width: 80, Name: "EvaluatorScore", HeaderAlign: "center", CellAlign: "center" },
                { HeaderText: "Reviewer Score", Width: 80, Name: "ReviewerScore", HeaderAlign: "center", CellAlign: "center" },
                { HeaderText: "Management Score", Width: 80, Name: "ManagementScore", HeaderAlign: "center", CellAlign: "center" },
                { HeaderText: "Discipline Score", Width: 80, Name: "DisciplineScore", HeaderAlign: "center", CellAlign: "center" },
                { HeaderText: "Final Score", Width: 80, Name: "FinalScore", HeaderAlign: "center", CellAlign: "center" },
                { HeaderText: "Grade", Width: 80, Name: "Grade", HeaderAlign: "center", CellAlign: "center" },
                { HeaderText: "Export PDF", Width: 80, Name: "ExportPdf", HeaderAlign: "center", CellAlign: "center" },
                { HeaderText: "View Detail", Width: 80, Name: "ViewDetail", HeaderAlign: "center", CellAlign: "center" }
            ];

            return columnHeaders;
        }

        function calculateScore(epaInfo) {
            var result = {
                SelfScore: status.NotAvailable,
                EvaluatorScore: status.NotAvailable,
                ReviewerScore: status.NotAvailable,
                ManagementScore: status.NotAvailable,
                DisciplineScore: status.NotAvailable,
                FinalScore: status.NotAvailable,
                Grade: status.NotAvailable
            };

            if (epaInfo.HasDisciplineScore) {
                result.DisciplineScore = epaInfo.DisciplineScore;
            } else {
                if (epaInfo.DisciplineScore !== 0) {
                    result.DisciplineScore = epaInfo.DisciplineScore;
                } else {
                    result.DisciplineScore = "-";
                }
            }

            debugger;

            if (epaInfo.EPAId > 0) {
                switch (epaInfo.Process.Self.StatusId) {
                    case utilitiesFactory.processStatus.InProgress:
                        result.SelfScore = epaInfo.Process.Self.TotalScore.length > 0
                            ? utilitiesFactory.parseFloat(epaInfo.Process.Self.TotalScore)
                            : status.NotAvailable;
                        break;
                    case utilitiesFactory.processStatus.Closed:
                        result.SelfScore = epaInfo.Process.Self.TotalScore.length > 0
                            ? utilitiesFactory.parseFloat(epaInfo.Process.Self.TotalScore)
                            : status.NotAvailable;
                        break;
                }

                switch (epaInfo.Process.Evaluators[0].StatusId) {
                    case utilitiesFactory.processStatus.InProgress:
                        result.EvaluatorScore = epaInfo.Process.AverageScoreByEvaluators !== null
                            ? epaInfo.Process.AverageScoreByEvaluators
                            : status.NotAvailable;
                        break;
                    case utilitiesFactory.processStatus.Closed:
                        result.EvaluatorScore = epaInfo.Process.AverageScoreByEvaluators !== null
                            ? epaInfo.Process.AverageScoreByEvaluators
                            : status.NotAvailable;
                        break;
                }

                switch (epaInfo.Process.Reviewers[0].StatusId) {
                    case utilitiesFactory.processStatus.InProgress:
                        result.ReviewerScore = epaInfo.Process.AverageScoreByReviewers !== null
                            ? epaInfo.Process.AverageScoreByReviewers
                            : status.NotAvailable;
                        break;
                    case utilitiesFactory.processStatus.Closed:
                        result.ReviewerScore = epaInfo.Process.AverageScoreByReviewers !== null
                            ? epaInfo.Process.AverageScoreByReviewers
                            : status.NotAvailable;
                        break;
                }

                if (epaInfo.Process.Managements.length > 0) {
                    switch (epaInfo.Process.Managements[0].StatusId) {
                        case utilitiesFactory.processStatus.InProgress:
                            result.ManagementScore = epaInfo.Process.AverageScoreByManagements !== null
                                ? epaInfo.Process.AverageScoreByManagements
                                : status.NotAvailable;
                            break;
                        case utilitiesFactory.processStatus.Closed:
                            result.ManagementScore = epaInfo.Process.AverageScoreByManagements !== null
                                ? epaInfo.Process.AverageScoreByManagements
                                : status.NotAvailable;
                            break;
                    }
                }

                if (epaInfo.IsSemiHasMonthly) {
                    result.SelfScore = epaInfo.MonthlyEPA.AverageSelfScoreConverted !== null
                        ? epaInfo.MonthlyEPA.AverageSelfScoreConverted
                        : status.NotAvailable;

                    result.EvaluatorScore = epaInfo.MonthlyEPA.AverageEvaluatorScoreConverted !== null
                        ? epaInfo.MonthlyEPA.AverageEvaluatorScoreConverted
                        : status.NotAvailable;
                }
            }

            var tempDisciplineScore = 0;
            if (result.DisciplineScore !== "-") {
                tempDisciplineScore = result.DisciplineScore;
            }

            if (result.SelfScore !== status.NotAvailable && result.SelfScore !== "" || isShowScoreOfProcess(epaInfo.Process.Self)) {
                result.FinalScore = getPhaseScore(result.SelfScore, tempDisciplineScore);
            }

            if (result.EvaluatorScore !== status.NotAvailable && result.EvaluatorScore !== "" || isShowScoreOfProcess(epaInfo.Process.Evaluators[0])) {
                result.FinalScore = getPhaseScore(result.EvaluatorScore, tempDisciplineScore);
            }

            if (result.ReviewerScore !== status.NotAvailable && result.ReviewerScore !== "" || isShowScoreOfProcess(epaInfo.Process.Reviewers[0])) {
                result.FinalScore = getPhaseScore(result.ReviewerScore, tempDisciplineScore);
            }

            if (result.ManagementScore !== status.NotAvailable && result.ManagementScore !== "" || isShowScoreOfProcess(epaInfo.Process.Managements[0])) {
                result.FinalScore = getPhaseScore(result.ManagementScore, tempDisciplineScore);
            }

            // Get Standard grades if empty
            if (standardGrades.length === 0) {
                CommonModel.getStandardGrade();
                standardGrades = CommonModel.standardGrades;
            }

            if (result.FinalScore !== "") {
                result.Grade = EvaluationModel.convertScoreToGrade(
                    result.FinalScore - tempDisciplineScore,
                    tempDisciplineScore,
                    epaInfo.HasDisciplineScore,
                    100,
                    standardGrades);
            } else {
                // Set default lowest value
                result.FinalScore = status.NotAvailable;
                result.Grade = status.NotAvailable;
            }

            //if (epaInfo.FinalScore.length > 0) {
            //    result.FinalScore = utilitiesFactory.parseFloat(epaInfo.FinalScore);
            //    result.Grade = epaInfo.Grade;
            //} else {
            //    // Set default lowest value
            //    result.FinalScore = status.NotAvailable;
            //    result.Grade = status.NotAvailable;
            //}

            return result;
        }

        function isShowScoreOfProcess(actor) {
            if (actor.StatusId === utilitiesFactory.processStatus.InProgress ||
                actor.StatusId === utilitiesFactory.processStatus.Closed)
                if (actor.Evaluated) {
                    return true;
                }
            return false;
        }

        function getPhaseScore(phaseScore, tempDisciplineScore) {
            if (phaseScore === status.NotAvailable) {
                return status.NotAvailable;
            } else {
                return utilitiesFactory.parseFloat(phaseScore + tempDisciplineScore);
            }
        }

        function generateBodyRows(gridData, pageOption) {
            var rows = [];
            var rowIndex = pageOption.itemsPerPage * pageOption.pageIndex;

            if (gridData.EpaInfos) {
                angular.forEach(gridData.EpaInfos,
                    function (epaInfo, index) {
                        var score = calculateScore(epaInfo);
                        var cssSelfScore = epaInfo.EPAId > 0 &&
                            !epaInfo.HasMonthlyEPA &&
                            epaInfo.Process.Self.StatusId === utilitiesFactory.processStatus.InProgress
                            ? "red"
                            : "";
                        var cssEvaluatorScore = epaInfo.EPAId > 0 &&
                            !epaInfo.HasMonthlyEPA &&
                            epaInfo.Process.Evaluators[0].StatusId === utilitiesFactory.processStatus.InProgress
                            ? "red"
                            : "";
                        var cssReviewerScore = epaInfo.EPAId > 0 && epaInfo.Process.Reviewers[0].StatusId === utilitiesFactory.processStatus.InProgress ? "red" : "";
                        var cssManagementScore = "";
                        var cssDisciplineScore = epaInfo.EPAId > 0 && epaInfo.DisciplineScore < 0 ? "red" : "";
                        var cssPendingShowDefaultRedValue = epaInfo.StatusId === utilitiesFactory.epaStatus.Closed ? "" : "red";
                        var pdfIcon = '<a class="icon-pdf exportPDF" data-epaid="' + epaInfo.EPAId + '"></a>';
                        var viewIcon = '<a class="icon-view viewEPADetail" data-epaid="' + epaInfo.EPAId + '"></a>';

                        if (epaInfo.EPAId > 0 &&
                            epaInfo.Process.Managements &&
                            epaInfo.Process.Managements.length > 0 &&
                            epaInfo.Process.Managements[0].StatusId === utilitiesFactory.processStatus.InProgress) {
                            cssManagementScore = "red";
                        }

                        var currentRow = {
                            Columns: [
                                { Name: "No", Value: rowIndex + index + 1 },
                                { Name: "Code", Value: epaInfo.Employee.Code },
                                { Name: "FullName", Value: epaInfo.Employee.FullName },
                                { Name: "NickName", Value: epaInfo.Employee.NickName },
                                { Name: "Section", Value: epaInfo.Employee.GroupName },
                                { Name: "JobTitle", Value: epaInfo.Employee.JobTitleName },
                                { Name: "GroupDate", Value: epaInfo.Employee.JoinedDate },
                                { Name: "TargetType", Value: epaInfo.Period.TargetTypeName },
                                { Name: "SelfScore", Value: "<span class='" + cssSelfScore + "'>" + score.SelfScore + "</span>" },
                                { Name: "EvaluatorScore", Value: "<span class='" + cssEvaluatorScore + "'>" + score.EvaluatorScore + "</span>" },
                                { Name: "ReviewerScore", Value: "<span class='" + cssReviewerScore + "'>" + score.ReviewerScore + "</span>" },
                                { Name: "ManagementScore", Value: "<span class='" + cssManagementScore + "'>" + score.ManagementScore + "</span>" },
                                { Name: "DisciplineScore", Value: "<span class='" + cssDisciplineScore + "'>" + score.DisciplineScore + "</span>" },
                                { Name: "FinalScore", Value: "<span class='" + cssPendingShowDefaultRedValue + "'>" + score.FinalScore + "</span>" },
                                { Name: "Grade", Value: "<span class='" + cssPendingShowDefaultRedValue + "'>" + score.Grade + "</span>" },
                                { Name: "ExportPdf", Value: epaInfo.EPAId > 0 ? pdfIcon : "" },
                                { Name: "ViewDetail", Value: epaInfo.EPAId > 0 ? viewIcon : "" }
                            ]
                        };

                        rows.push(currentRow);
                    });
            }

            return rows;
        }

        function registerViewEPADetailEvent(pageOption) {
            $(".freegrid").off("click", ".table-cell a.viewEPADetail");

            $(".freegrid").on("click",
                ".table-cell a.viewEPADetail",
                function (event) {
                    event.preventDefault();

                    var epaId = parseInt($(this).data("epaid"));
                    $window.location.href = baseUrl +
                        "EPA/Detail/" +
                        epaId +
                        "?periodId=" +
                        pageOption.periodId +
                        "&departmentId=" +
                        pageOption.departmentId +
                        "&phaseIds=" +
                        pageOption.phaseIds +
                        "&isLocal=" +
                        pageOption.isLocal +
                        "&employeeId=" +
                        pageOption.employeeId +
                        "&exportPdf&detailRpt";
                });
        }

        function registerExportPdfEvent() {
            $(".freegrid").off("click", ".table-cell a.exportPDF");

            $(".freegrid").on("click",
                ".table-cell a.exportPDF",
                function (event) {
                    event.preventDefault();

                    var epaId = parseInt($(this).data("epaid"));

                    var url = baseUrl + "ExportPdf/ExportPdf?epaId=" + epaId;
                    var iframe = $("body iframe");

                    if (iframe.length === 0) {
                        iframe = $('<iframe width="0" height="0" class="display-none"></iframe>');
                        iframe.appendTo("body");
                    }

                    iframe.attr("src", url);
                });
        }

        return {
            restrict: "A",
            scope: {
                gridData: "=",
                pageOption: "=",
                onPageChanging: "=",
                onSortChanging: "=",
                hideColumns: "=",
                exportReport: "=",
                sortColumnName: "=",
                sortDirection: "=",
                gridExpandHeight: "@"
            },
            link: function ($scope, iElement) {
                var gridOptions = {
                    gridExpandHeight: $scope.gridExpandHeight,
                    showPager: true,
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

                $scope.$watch("gridData",
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
                                $(iElement).fanexGrid("hideColumns", gridOptions, $scope.hideColumns);

                                registerViewEPADetailEvent($scope.pageOption);
                                registerExportPdfEvent();
                            }
                        });
                    });
            }
        };
    }
})(angular);
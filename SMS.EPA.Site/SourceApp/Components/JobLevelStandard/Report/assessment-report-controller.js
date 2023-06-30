(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('jobLevelStandardModule').controller('AssessmentReportController', assessmentReportController);
    assessmentReportController.$inject = ['baseUrl', 'AssessmentReportModel', 'CommonModel', 'utilitiesFactory', 'lockScreenService'];

    function assessmentReportController(baseUrl, AssessmentReportModel, CommonModel, utilitiesFactory, lockScreenService) {
        let vm = this;

        vm.periods = [];
        vm.orgChart = [];
        vm.searchKey = '';
        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.filter = { periodId: null, sectionIds: null, isLocal: '', statusId: '' };
        vm.kpis = [];
        vm.assessments = [];
        utilitiesFactory.getDateFormat(vm);
        vm.entries = [10, 20, 50, 100, 200, 500];
        vm.assessmentStatus = utilitiesFactory.jobLevelAssessmentStatus;
        vm.assessmentStatusOptions = utilitiesFactory.assessmentStatusOptions;

        vm.exportReport = exportReport;

        function getAssessments(filter) {
            lockScreenService.start();

            AssessmentReportModel.getAssessments(filter).then(function () {
                vm.kpis = AssessmentReportModel.kpis;
                vm.assessments = AssessmentReportModel.assessments;

                for (let assessment of vm.assessments) {
                    assessment.CssClass = '';
                    if (assessment.SystemPropose < 0) {
                        assessment.CssClass = 'under-standard';
                    } else if (assessment.SystemPropose === 0) {
                        assessment.CssClass = 'maintain';
                    } else {
                        const currentJobLevelId = assessment.CurrentJobLevelId !== null ? assessment.CurrentJobLevelId : 0;
                        if (assessment.NewJobLevelId !== null && currentJobLevelId !== assessment.NewJobLevelId) {
                            assessment.CssClass = 'promotion';
                        } else {
                            assessment.CssClass = 'maintain';
                        }
                    }
                }

                utilitiesFactory.searchOnPage(vm, vm.assessments);

                lockScreenService.stop();
            });
        }

        vm.query = function () {
            vm.searchKey = '';
            vm.currentPage = 1;
            getAssessments(vm.filter);
        };

        vm.cssOldWeight = function (akpi, assessment) {
            if (akpi === null) {
                return !assessment.AssessmentId ? 'blur-string' : ''
            }

            let hasNewValue = akpi.Level !== akpi.OldLevel;
            return !hasNewValue && !assessment.AssessmentId ? 'blur-string' : ''
        };

        vm.showKpiWeight = function (number) {
            return !number ? "" : utilitiesFactory.splitNumber(number, 2);
        }

        function exportReport() {
            vm.filter.periodName = vm.periods.filter(x => x.PeriodId === vm.filter.periodId)[0].Name;
            vm.exportCriteria = angular.copy(vm.filter);

            var url = baseUrl + "ExportAssessment/ExportJoblevelAssessment";
            var data = $.param(vm.exportCriteria);

            var downloadUrl = url + "?" + data;
            var iframe = $('body iframe');

            if (iframe.length === 0) {
                iframe = $('<iframe width="0" height="0" class="display-none"></iframe>');
                iframe.appendTo('body');
            }

            iframe.attr("src", downloadUrl);
        }

        vm.getIndex = function (index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        };

        vm.onSelectSection = function (sectionIds) {
            vm.filter.sectionIds = sectionIds;
        };

        function initialize() {
            AssessmentReportModel.getOrgChart().then(function () {
                vm.orgChart = AssessmentReportModel.orgChart;
            });

            AssessmentReportModel.getPeriods().then(function () {
                vm.periods = AssessmentReportModel.periods;

                if (vm.periods.length > 0) {
                    vm.filter.periodId = vm.periods[0].PeriodId;

                    getAssessments(vm.filter);
                }
            });
        }

        initialize();
    }
})(angular);
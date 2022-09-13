(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('jobLevelStandardModule').controller('AssessmentReportController', assessmentReportController);
    assessmentReportController.$inject = ['AssessmentReportModel', 'CommonModel', 'utilitiesFactory', 'lockScreenService'];

    function assessmentReportController(AssessmentReportModel, CommonModel, utilitiesFactory, lockScreenService) {
        let vm = this;

        vm.periods = [];
        vm.orgChart = [];
        vm.searchKey = '';
        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.filter = { periodId: null, teamIds: '', isLocal: '', statusId: '' };
        vm.kpis = [];
        vm.assessments = [];
        utilitiesFactory.getDateFormat(vm);
        vm.entries = [10, 20, 50, 100, 200, 500];
        vm.assessmentStatus = utilitiesFactory.jobLevelAssessmentStatus;
        vm.assessmentStatusOptions = [
            { StatusId: vm.assessmentStatus.Pending, Name: 'Pending' },
            { StatusId: vm.assessmentStatus.Verified, Name: 'Verified' },
            { StatusId: vm.assessmentStatus.Approved, Name: 'Approved' }
        ];

        function getAssessments(filter) {
            lockScreenService.start();

            AssessmentReportModel.getAssessments(filter).then(function () {
                vm.kpis = AssessmentReportModel.kpis;
                vm.assessments = AssessmentReportModel.assessments;
                utilitiesFactory.searchOnPage(vm, vm.assessments);

                lockScreenService.stop();
            });
        }

        vm.query = function () {
            vm.searchKey = '';
            vm.currentPage = 1;
            getAssessments(vm.filter);
        };

        vm.getIndex = function (index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
        };

        vm.onSelectTeam = function (teamIds) {
            vm.filter.teamIds = teamIds;
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
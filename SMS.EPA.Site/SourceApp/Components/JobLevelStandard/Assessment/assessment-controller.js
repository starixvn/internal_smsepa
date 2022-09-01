(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('jobLevelStandardModule').controller('AssessmentController', assessmentController);
    assessmentController.$inject = ['AssessmentModel', 'utilitiesFactory', 'lockScreenService'];

    function assessmentController(AssessmentModel, utilitiesFactory, lockScreenService) {
        let vm = this;

        vm.periods = [];
        vm.orgChart = [];
        vm.searchKey = '';
        vm.filter = { periodId: null, teamIds: '' };
        vm.kpis = [];
        vm.assessments = [];
        vm.process = utilitiesFactory.jobLevelAssessmentProcess;
        vm.status = utilitiesFactory.jobLevelAssessmentStatus;
        utilitiesFactory.getDateFormat(vm);
        vm.isEditing = false;

        function getAssessments(filter) {
            lockScreenService.start();

            AssessmentModel.getAssessments(filter).then(function () {
                vm.kpis = AssessmentModel.kpis;
                vm.jobLevelSettings = AssessmentModel.jobLevelSettings;
                vm.assessments = AssessmentModel.assessments;
                utilitiesFactory.searchOnPage(vm, vm.assessments);

                lockScreenService.stop();
            });
        }

        vm.query = function () {
            vm.searchKey = '';
            vm.isEditing = false;
            vm.selectedAssessment = null;
            getAssessments(vm.filter);
        };

        vm.edit = function (assessment) {
            vm.isEditing = true;
            vm.selectedAssessment = angular.copy(assessment);
            assessment.IsEdit = true;
        };

        vm.cancel = function (assessment) {
            vm.isEditing = false;

            assessment.IsEdit = false;
            assessment.KPIs = vm.selectedAssessment.KPIs;
            assessment.TotalWeight = vm.selectedAssessment.TotalWeight;
            assessment.SystemPropose = vm.selectedAssessment.SystemPropose;
            assessment.NewJobLevelId = vm.selectedAssessment.NewJobLevelId;
            assessment.NewJobLevel = vm.selectedAssessment.NewJobLevel;
            assessment.Remark = vm.selectedAssessment.Remark;

            vm.selectedAssessment = null;
        };

        vm.save = function (assessment) {
            const isValid = AssessmentModel.validateAssessment(assessment);
            if (isValid) {
                assessment.StatusId = vm.status.Pending;
                AssessmentModel.edit(vm, assessment);
            }
        };

        vm.verify = function (assessment) {
            AssessmentModel.editStatus(vm, { assessmentId: assessment.AssessmentId, statusId: vm.status.Verified });
        };

        vm.approve = function (assessment) {
            AssessmentModel.editStatus(vm, { assessmentId: assessment.AssessmentId, statusId: vm.status.Approved });
        };

        vm.inputWeight = function (assessment) {
            let totalWeight = 0;
            angular.forEach(assessment.KPIs, function (kpi) {
                totalWeight += kpi.Weight ? kpi.Weight : 0;
            });

            assessment.TotalWeight = totalWeight;
            assessment.SystemPropose = parseFloat(parseFloat(totalWeight - (assessment.RequirementMinScore ? assessment.RequirementMinScore : 0)).toFixed(2));

            const setting = vm.jobLevelSettings.find(x => x.SectionId === assessment.SectionId && x.MinWeight <= totalWeight && x.MaxWeight >= totalWeight);
            if (setting) {
                assessment.NewJobLevelId = setting.JobLevelId;
                assessment.NewJobLevel = setting.Name;
            }

            if (assessment.SystemPropose < 0) {
                assessment.Remark = 'Under standard';
            } else {
                if (assessment.CurrentJobLevelId !== assessment.NewJobLevelId) {
                    assessment.Remark = 'Promotion';
                } else {
                    assessment.Remark = 'Maintain';
                }
            }
        };

        vm.onSelectTeam = function (teamIds) {
            vm.filter.teamIds = teamIds;
        };

        function initialize() {
            AssessmentModel.getPeriods().then(function () {
                vm.periods = AssessmentModel.periods;

                if (vm.periods.length > 0) {
                    vm.filter.periodId = vm.periods[0].PeriodId;

                    AssessmentModel.getOrgChart().then(function () {
                        vm.orgChart = AssessmentModel.orgChart;

                        getAssessments(vm.filter);
                    });
                }
            });
        }

        initialize();
    }
})(angular);
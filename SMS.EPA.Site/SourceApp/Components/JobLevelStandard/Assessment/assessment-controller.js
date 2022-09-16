(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('jobLevelStandardModule').controller('AssessmentController', assessmentController);
    assessmentController.$inject = ['CommonModel', 'AssessmentModel', 'utilitiesFactory', 'lockScreenService'];

    function assessmentController(CommonModel, AssessmentModel, utilitiesFactory, lockScreenService) {
        let vm = this;

        vm.periods = null;
        vm.orgChart = [];
        vm.searchKey = '';
        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.filter = { periodId: null, sectionIds: null, statusId: '' };
        vm.kpis = [];
        vm.assessments = [];
        vm.process = utilitiesFactory.jobLevelAssessmentProcess;
        vm.status = utilitiesFactory.jobLevelAssessmentStatus;
        vm.statusOptions = [
            { statusId: vm.status.Pending, name: 'Pending' },
            { statusId: vm.status.Verified, name: 'Verified' },
            { statusId: vm.status.Approved, name: 'Approved' }
        ];
        utilitiesFactory.getDateFormat(vm);
        vm.entries = [10, 20, 50, 100, 200, 500];
        vm.isEditing = false;
        vm.showApproveAll = false;
        vm.selectedAll = false;

        function getAssessments(filter) {
            lockScreenService.start();

            AssessmentModel.getAssessments(filter).then(function () {
                vm.kpis = AssessmentModel.kpis;
                vm.jobLevelSettings = AssessmentModel.jobLevelSettings;
                vm.assessments = AssessmentModel.assessments;

                for (let assessment of vm.assessments) {
                    if (assessment.IsHost && assessment.CurrentProcessId === vm.process.Management && assessment.StatusId === vm.status.Verified) {
                        vm.showApproveAll = true;
                        assessment.ShowCheckbox = true;
                    }
                }

                utilitiesFactory.searchOnPage(vm, vm.assessments);

                lockScreenService.stop();
            });
        }

        vm.query = function () {
            vm.searchKey = '';
            vm.isEditing = false;
            vm.selectedAssessment = null;
            vm.showApproveAll = false;
            vm.selectedAll = false;
            getAssessments(vm.filter);
        };

        vm.getIndex = function (index, currentPage, pageSize) {
            return CommonModel.getIndex(index, currentPage, pageSize);
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
            AssessmentModel.editStatus(vm, { assessmentIds: assessment.AssessmentId, statusId: vm.status.Verified });
        };

        vm.approve = function (assessment) {
            AssessmentModel.editStatus(vm, { assessmentIds: assessment.AssessmentId, statusId: vm.status.Approved });
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

        vm.onSelectSection = function (sectionIds) {
            vm.filter.sectionIds = sectionIds;
        };

        vm.onSellectAll = function () {
            for (let assessment of vm.assessments) {
                if (assessment.ShowCheckbox) {
                    assessment.Selected = vm.selectedAll;
                }
            }
        };

        vm.approveAll = function () {
            const assessmentIds = vm.assessments.filter(x => x.Selected).map(x => x.AssessmentId).join(',');
            const params = { assessmentIds, statusId: vm.status.Approved };
            AssessmentModel.approveAll(vm, params);
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
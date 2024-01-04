(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('jobLevelStandardModule').controller('AssessmentController', assessmentController);
    assessmentController.$inject = ['$location', 'CommonModel', 'AssessmentModel', 'utilitiesFactory', 'lockScreenService'];

    function assessmentController($location, CommonModel, AssessmentModel, utilitiesFactory, lockScreenService) {
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
        vm.statusOptions = utilitiesFactory.assessmentStatusOptions;
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

        vm.inputLevel = function (akpi, assessment) {
            if (akpi.Level === '' || akpi.Level === null) {
                akpi.Weight = null;
            } else {
                const weight = akpi.TemplateWeight / 5 * akpi.Level;
                akpi.Weight = utilitiesFactory.splitNumber(weight, 4);
            }

            vm.inputWeight(assessment);
        };

        vm.getDisplayKpiWeight = function (number) {
            return number === null ? '' : utilitiesFactory.splitNumber(number, 2);
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
            assessment.CssClass = vm.selectedAssessment.CssClass;

            vm.selectedAssessment = null;
        };

        vm.save = function (assessment) {
            const isValid = AssessmentModel.validateAssessment(assessment);
            if (isValid) {
                assessment.StatusId = assessment.TotalWeight !== null ? vm.status.Pending : null;
                AssessmentModel.edit(vm, assessment);
            }
        };

        vm.verify = function (assessment) {
            if (!assessment.AssessmentId) {
                AssessmentModel.editAndVerify(vm, assessment)
            } else {
                AssessmentModel.editStatus(vm, { assessmentIds: assessment.AssessmentId, statusId: vm.status.Verified });
            }
        };

        vm.approve = function (assessment) {
            AssessmentModel.editStatus(vm, { assessmentIds: assessment.AssessmentId, statusId: vm.status.Approved });
        };

        vm.reset = function (assessment) {
            AssessmentModel.editStatus(vm, { assessmentIds: assessment.AssessmentId, statusId: vm.status.Verified });
        };

        vm.inputWeight = function (assessment) {
            let totalWeight = null;
            angular.forEach(assessment.KPIs, function (kpi) {
                if (!isNaN(parseFloat(kpi.Weight))) {
                    totalWeight = !isNaN(parseFloat(totalWeight)) ? totalWeight : 0;
                    totalWeight += kpi.Weight ? kpi.Weight : 0;
                }
            });

            assessment.TotalWeight = null;
            assessment.SystemPropose = null;
            assessment.NewJobLevelId = null;
            assessment.NewJobLevel = '';
            assessment.Remark = '';

            const currentJobLevelId = assessment.CurrentJobLevelId !== null ? assessment.CurrentJobLevelId : 0;
            if (totalWeight !== null) {
                const requirementMinScore = assessment.RequirementMinScore !== null ? assessment.RequirementMinScore : 0;
                assessment.TotalWeight = totalWeight.toFixed(2);
                assessment.SystemPropose = parseFloat(parseFloat(totalWeight - requirementMinScore).toFixed(2));

                if (assessment.SystemPropose > 0) {
                    const settings = vm.jobLevelSettings.filter(x => x.SectionId === assessment.SectionId
                        && x.JobLevelId != currentJobLevelId && x.MinWeight >= requirementMinScore && x.MinWeight <= totalWeight)
                        .sort(function (a, b) {
                            return a.MinWeight - b.MinWeight;
                        });

                    if (settings && settings.length > 0 && assessment.RequirementMinScore !== null && settings[0].JobLevelId !== currentJobLevelId) {
                        assessment.NewJobLevelId = settings[0].JobLevelId;
                        assessment.NewJobLevel = settings[0].Name;

                        if (settings.length > 1) {
                            assessment.NewJobLevel = assessment.NewJobLevel + ' > ' + settings[settings.length - 1].Name;
                        }
                    }
                }
            }

            assessment.CssClass = '';
            if (assessment.SystemPropose < 0) {
                assessment.CssClass = 'under-standard';
            } else if (assessment.SystemPropose === 0) {
                assessment.CssClass = 'maintain';
            } else {
                if (assessment.NewJobLevelId !== null && currentJobLevelId !== assessment.NewJobLevelId) {
                    assessment.CssClass = 'promotion';
                } else {
                    assessment.CssClass = 'maintain';
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
                var periodId = $location.search().periodId;

                if (vm.periods.length > 0) {
                    if (periodId === undefined) {
                        vm.filter.periodId = vm.periods[0].PeriodId;
                    } else {
                        vm.filter.periodId = periodId;
                    }

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
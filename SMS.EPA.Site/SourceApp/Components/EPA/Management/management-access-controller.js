(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('epaModule').controller('ManagementController', managementController);
    managementController.$inject = [
        '$location',
        '$translate',
        '$routeParams',
        'EvaluationModel',
        'ManagementEPAModel',
        'ReviewerManagementModel',
        'CommonModel',
        'utilitiesFactory',
        'notificationFactory',
        'historyFactory',
        'lockScreenService',
        'baseUrl'
    ];

    function managementController(
        $location,
        $translate,
        $routeParams,
        EvaluationModel,
        ManagementEPAModel,
        ReviewerManagementModel,
        CommonModel,
        utilitiesFactory,
        notificationFactory,
        historyFactory,
        lockScreenService,
        baseUrl) {
        var vm = this;

        vm.permission = 0;
        vm.epaInfos = [];
        vm.members = [];
        vm.standardGrades = CommonModel.standardGrades;
        vm.selectedAll = false;
        vm.averageScore = 0;
        vm.isEndProcessAll = false;
        vm.policyEnum = utilitiesFactory.policy;
        vm.targetTypeEnum = utilitiesFactory.targetType;
        vm.filter = {};
        vm.statusProcesses = [
            { Name: 'Finished', Value: true },
            { Name: 'Unfinished', Value: false }
        ];

        function initFilter() {
            var filter = historyFactory.getState(),
                statusProcess = $routeParams.statusProcess !== undefined
                    ? (($routeParams.statusProcess === true || $routeParams.statusProcess === 'true') ? true : false)
                    : '';

            return filter ? filter : {
                groupId: '',
                isLocal: '',
                employeeIds: '',
                statusProcess: statusProcess
            };
        }

        function resetSorting() {
            vm.sortBy = {
                jobTitle: { ascending: false, descending: false },
                selfScore: { ascending: false, descending: false },
                evaluatorScore: { ascending: false, descending: false },
                reviewerScore: { ascending: false, descending: false },
                disciplineScore: { ascending: false, descending: false },
                totalScore: { ascending: false, descending: false }
            };
        }

        function sortByJobTitle(epaInfo1, epaInfo2) {
            if (epaInfo1.Employee.JobTitleName < epaInfo2.Employee.JobTitleName) return -1;
            if (epaInfo1.Employee.JobTitleName > epaInfo2.Employee.JobTitleName) return 1;

            return 0;
        }

        function sortBySelfScore(epaInfo1, epaInfo2) {
            var tempa = epaInfo1.Process.Self.TotalScore ? parseFloat(epaInfo1.Process.Self.TotalScore) : 0,
                tempb = epaInfo2.Process.Self.TotalScore ? parseFloat(epaInfo2.Process.Self.TotalScore) : 0;

            if (epaInfo1.IsSemiHasMonthly) {
                tempa = epaInfo1.MonthlyEPA.AverageSelfScoreConverted;
            }

            if (epaInfo2.IsSemiHasMonthly) {
                tempb = epaInfo2.MonthlyEPA.AverageSelfScoreConverted;
            }

            return tempa - tempb;
        }

        function sortByEvaluatorScore(epaInfo1, epaInfo2) {
            var tempa = epaInfo1.Process.AverageScoreByEvaluators,
                tempb = epaInfo2.Process.AverageScoreByEvaluators;

            if (epaInfo1.IsSemiHasMonthly) {
                tempa = epaInfo1.MonthlyEPA.AverageEvaluatorScoreConverted;
            }

            if (epaInfo2.IsSemiHasMonthly) {
                tempb = epaInfo2.MonthlyEPA.AverageEvaluatorScoreConverted;
            }

            return tempa - tempb;
        }

        function sortByReviewerScore(epaInfo1, epaInfo2) {
            return epaInfo1.Process.AverageScoreByReviewers - epaInfo2.Process.AverageScoreByReviewers;
        }

        function sortByDisciplineScore(epaInfo1, epaInfo2) {
            return epaInfo1.DisciplineScore - epaInfo2.DisciplineScore;
        }

        function sortByTotalScore(epaInfo1, epaInfo2) {
            return epaInfo1.FinalScore - epaInfo2.FinalScore;
        }

        vm.selectedCls = function (field) {
            var cssClass = '';

            switch (field) {
                case 'JobTitle':
                    cssClass = vm.sortBy.jobTitle.ascending ? 'sorting-asc' : (vm.sortBy.jobTitle.descending ? 'sorting-desc' : '');
                    break;

                case 'SelfScore':
                    cssClass = vm.sortBy.selfScore.ascending ? 'sorting-asc' : (vm.sortBy.selfScore.descending ? 'sorting-desc' : '');
                    break;

                case 'EvaluatorScore':
                    cssClass = vm.sortBy.evaluatorScore.ascending ? 'sorting-asc' : (vm.sortBy.evaluatorScore.descending ? 'sorting-desc' : '');
                    break;

                case 'ReviewerScore':
                    cssClass = vm.sortBy.reviewerScore.ascending ? 'sorting-asc' : (vm.sortBy.reviewerScore.descending ? 'sorting-desc' : '');
                    break;

                case 'DisciplineScore':
                    cssClass = vm.sortBy.disciplineScore.ascending ? 'sorting-asc' : (vm.sortBy.disciplineScore.descending ? 'sorting-desc' : '');
                    break;

                case 'TotalScore':
                    cssClass = vm.sortBy.totalScore.ascending ? 'sorting-asc' : (vm.sortBy.totalScore.descending ? 'sorting-desc' : '');
                    break;
            }

            return cssClass;
        };

        vm.changeSorting = function (field) {
            switch (field) {
                case 'JobTitle':
                    var jobTitle = angular.copy(vm.sortBy.jobTitle);
                    resetSorting();
                    vm.sortBy.jobTitle = jobTitle;
                    if (!vm.sortBy.jobTitle.ascending) {
                        vm.sortBy.jobTitle = { ascending: true, descending: false };
                        vm.epaInfos.sort(sortByJobTitle);
                    } else {
                        vm.sortBy.jobTitle = { ascending: false, descending: true };
                        vm.epaInfos.reverse();
                    }

                    break;

                case 'SelfScore':
                    var selfScore = angular.copy(vm.sortBy.selfScore);
                    resetSorting();
                    vm.sortBy.selfScore = selfScore;
                    if (!vm.sortBy.selfScore.ascending) {
                        vm.sortBy.selfScore = { ascending: true, descending: false };
                        vm.epaInfos.sort(sortBySelfScore);
                    } else {
                        vm.sortBy.selfScore = { ascending: false, descending: true };
                        vm.epaInfos.reverse();
                    }

                    break;

                case 'EvaluatorScore':
                    var evaluatorScore = angular.copy(vm.sortBy.evaluatorScore);
                    resetSorting();
                    vm.sortBy.evaluatorScore = evaluatorScore;
                    if (!vm.sortBy.evaluatorScore.ascending) {
                        vm.sortBy.evaluatorScore = { ascending: true, descending: false };
                        vm.epaInfos.sort(sortByEvaluatorScore);
                    } else {
                        vm.sortBy.evaluatorScore = { ascending: false, descending: true };
                        vm.epaInfos.reverse();
                    }

                    break;

                case 'ReviewerScore':
                    var reviewerScore = angular.copy(vm.sortBy.reviewerScore);
                    resetSorting();
                    vm.sortBy.reviewerScore = reviewerScore;
                    if (!vm.sortBy.reviewerScore.ascending) {
                        vm.sortBy.reviewerScore = { ascending: true, descending: false };
                        vm.epaInfos.sort(sortByReviewerScore);
                    } else {
                        vm.sortBy.reviewerScore = { ascending: false, descending: true };
                        vm.epaInfos.reverse();
                    }

                    break;

                case 'DisciplineScore':
                    var disciplineScore = angular.copy(vm.sortBy.disciplineScore);
                    resetSorting();
                    vm.sortBy.disciplineScore = disciplineScore;
                    if (!vm.sortBy.disciplineScore.ascending) {
                        vm.sortBy.disciplineScore = { ascending: true, descending: false };
                        vm.epaInfos.sort(sortByDisciplineScore);
                    } else {
                        vm.sortBy.disciplineScore = { ascending: false, descending: true };
                        vm.epaInfos.reverse();
                    }

                    break;

                case 'TotalScore':
                    var totalScore = angular.copy(vm.sortBy.totalScore);
                    resetSorting();
                    vm.sortBy.totalScore = totalScore;
                    if (!vm.sortBy.totalScore.ascending) {
                        vm.sortBy.totalScore = { ascending: true, descending: false };
                        vm.epaInfos.sort(sortByTotalScore);
                    } else {
                        vm.sortBy.totalScore = { ascending: false, descending: true };
                        vm.epaInfos.reverse();
                    }

                    break;
            }
        };

        vm.getMember = function (groupId, isLocal) {
            ManagementEPAModel.getMemberByManagement(groupId, isLocal).then(function () {
                vm.members = ManagementEPAModel.members;
                utilitiesFactory.setSelectedItem(vm.members, "EmployeeId", vm.filter.employeeIds);
            });
        };

        vm.viewEPADetail = function (epaInfo) {
            if (ReviewerManagementModel.checkAdjustScoreChanged(vm.epaInfos)) {
                ReviewerManagementModel.openPopupRemindSaveChange(vm).then(function () {
                    $location.path(baseUrl + 'Management/Detail/' + epaInfo.EPAId);
                })['finally'](function () {
                    vm.modalInstance = '';
                });
            } else {
                $location.path(baseUrl + 'Management/Detail/' + epaInfo.EPAId);
            }
        };

        vm.viewEPAResult = function (epaId) {
            $location.path(baseUrl + 'EPA/Detail/' + epaId);
        };

        vm.checkAll = function (selectedAll) {
            angular.forEach(vm.epaInfos, function (epaInfo) {
                if (!epaInfo.Process.CurrentProcess.IsEndProcess) {
                    epaInfo.Selected = selectedAll;
                } else {
                    epaInfo.Selected = false;
                }
            });
        };

        vm.getNotificationInputScore = function (epaInfo) {
            var maxSubtractScore = epaInfo.Process.CurrentProcess.MaxSubtractScore,
                maxAdditionScore = epaInfo.Process.CurrentProcess.MaxAdditionScore;

            return EvaluationModel.getNotificationInputScore(maxSubtractScore, maxAdditionScore, vm.scoreInputValidMessage);
        };

        vm.inputScore = function (epaInfo) {
            var maxSubtractScore = epaInfo.Process.CurrentProcess.MaxSubtractScore,
                maxAdditionScore = epaInfo.Process.CurrentProcess.MaxAdditionScore,
                adjustScore = epaInfo.Process.CurrentProcess.AdjustScore;

            if (!EvaluationModel.validateScoreInput(adjustScore, maxSubtractScore, maxAdditionScore, vm.scoreInputValidMessage)) {
                epaInfo.Process.CurrentProcess.AdjustScore = null;
            }

            ReviewerManagementModel.calculateScore(epaInfo, false);
            epaInfo.Grade = EvaluationModel.convertScoreToGrade(
                epaInfo.Process.CurrentProcess.TotalScore,
                epaInfo.DisciplineScore,
                epaInfo.HasDisciplineScore,
                epaInfo.Process.CurrentProcess.MaxPerformanceScore,
                vm.standardGrades);
        };

        vm.adjustScore = function () {
            var epaInfos = ReviewerManagementModel.getEPASelected(vm.epaInfos);

            if (epaInfos.length === 0) {
                notificationFactory.error("SelectEmployeeBeforeAdjustScoreWarning");
            } else {
                ReviewerManagementModel.openPopupAdjustScore(vm, epaInfos, false).then(function (adjust) {
                    ReviewerManagementModel.calculateAdjustScoreMultiple(epaInfos, adjust, false);
                    angular.forEach(epaInfos, function (epaInfo) {
                        epaInfo.Grade = EvaluationModel.convertScoreToGrade(
                            epaInfo.Process.CurrentProcess.TotalScore,
                            epaInfo.DisciplineScore,
                            epaInfo.HasDisciplineScore,
                            epaInfo.Process.CurrentProcess.MaxPerformanceScore,
                            vm.standardGrades);
                    });
                })['finally'](function () {
                    vm.modalInstance = null;
                });
            }
        };

        vm.saveAdjustScore = function () {
            var epaInfos = ReviewerManagementModel.getEPASelected(vm.epaInfos);

            if (epaInfos.length === 0) {
                notificationFactory.error("SelectEmployeeBeforeAdjustScoreWarning");
            } else {
                ReviewerManagementModel.calculateAdjustScore(epaInfos, false);
                ManagementEPAModel.adjustScoreByManagement(vm, epaInfos)['finally'](function () {
                    getEPAByManagement(vm.filter.groupId, vm.filter.isLocal, vm.filter.employeeIds, epaInfos, vm.filter.statusProcess);
                });
            }
        };

        vm.updateComment = function (epaInfo, comment, isEdit) {
            ReviewerManagementModel.openPopupUpdateComment(vm, epaInfo, comment, isEdit, false).then(function (commentUpdate) {
                var index = vm.epaInfos.map(function (e) { return e.EPAId; }).indexOf(epaInfo.EPAId);

                if (index !== -1) {
                    vm.epaInfos[index].Process.CurrentProcess.TotalScore = vm.epaInfos[index].Process.CurrentProcess.TotalScoreOriginal;
                    vm.epaInfos[index].Process.CurrentProcess.AdjustScore = vm.epaInfos[index].Process.CurrentProcess.AdjustScoreOriginal;
                    vm.epaInfos[index].Process.CurrentProcess.Comment = commentUpdate;
                    vm.epaInfos[index].Process.CurrentProcess.Evaluated = true;

                    ReviewerManagementModel.calculateScore(vm.epaInfos[index], false);
                    vm.epaInfos[index].Grade = EvaluationModel.convertScoreToGrade(
                        vm.epaInfos[index].Process.CurrentProcess.TotalScore,
                        vm.epaInfos[index].DisciplineScore,
                        vm.epaInfos[index].HasDisciplineScore,
                        vm.epaInfos[index].Process.CurrentProcess.MaxPerformanceScore,
                        vm.standardGrades);
                }
            })['finally'](function () {
                vm.modalInstance = null;
            });
        };

        function getEPAByManagement(groupId, isLocal, employeeIds, selectedEmployees, statusProcess) {
            lockScreenService.start();
            vm.isEndProcessAll = false;
            resetSorting();

            ManagementEPAModel.getEPAByManagement(groupId, isLocal, employeeIds, statusProcess).then(function () {
                vm.epaInfos = ManagementEPAModel.epaInfos;

                var averageScore = 0, countEndProcess = 0;
                angular.forEach(vm.epaInfos, function (epaInfo) {
                    var maxPerformanceScore = epaInfo.Process.CurrentProcess.MaxPerformanceScore;

                    if (epaInfo.Process.CurrentProcess.TotalScore.length === 0 && epaInfo.Process.Reviewers[0].IsEndProcess) {
                        epaInfo.Process.CurrentProcess.TotalScore = epaInfo.Process.AverageScoreByReviewers;
                    }

                    epaInfo.Process.Self.TotalScore = utilitiesFactory.parseFloat(epaInfo.Process.Self.TotalScore);
                    epaInfo.Process.CurrentProcess.TotalScore = utilitiesFactory.parseFloat(epaInfo.Process.CurrentProcess.TotalScore);
                    epaInfo.FinalScore = epaInfo.Process.CurrentProcess.TotalScore + epaInfo.DisciplineScore;
                    epaInfo.FinalScore = EvaluationModel.checkScore(epaInfo.FinalScore, 0, maxPerformanceScore);
                    epaInfo.Grade = EvaluationModel.convertScoreToGrade(
                        epaInfo.Process.CurrentProcess.TotalScore,
                        epaInfo.DisciplineScore,
                        epaInfo.HasDisciplineScore,
                        maxPerformanceScore,
                        vm.standardGrades);

                    if (epaInfo.LastFinalScore.length > 0) {
                        epaInfo.LastFinalScore = utilitiesFactory.parseFloat(epaInfo.LastFinalScore);
                        epaInfo.LastPerformanceScore = epaInfo.LastFinalScore - epaInfo.LastDisciplineScore;
                    } else {
                        epaInfo.LastFinalScore = null;
                    }

                    if (epaInfo.Process.CurrentProcess.IsEndProcess) {
                        countEndProcess += 1;
                    }

                    averageScore += epaInfo.Process.AverageScoreByReviewers;
                });

                if (countEndProcess === vm.epaInfos.length) {
                    vm.isEndProcessAll = true;
                }
                vm.averageScore = utilitiesFactory.parseFloat(averageScore / vm.epaInfos.length);

                if (selectedEmployees && selectedEmployees.length > 0) {
                    ReviewerManagementModel.updateSelectedEmployees(vm.epaInfos, selectedEmployees);
                }

                lockScreenService.stop();
            });

            historyFactory.saveState(vm.filter);
        }

        vm.query = function (filter) {
            filter.employeeIds = utilitiesFactory.getSelectedData(vm.members, 'EmployeeId');
            getEPAByManagement(filter.groupId, filter.isLocal, filter.employeeIds, null, filter.statusProcess);
        };

        function initialize() {
            vm.filter = initFilter();
            resetSorting();

            $translate('ScoreInputValid').then(function (message) {
                vm.scoreInputValidMessage = message;
            });

            $translate('AdjustScoreInvalid').then(function (message) {
                vm.adjustScoreInvalidMessage = message;
            });

            ManagementEPAModel.getUserPermissionOnManagement().then(function () {
                vm.permission = ManagementEPAModel.permission;
            });

            ManagementEPAModel.getGroupByManagement().then(function () {
                vm.groupOptions = ManagementEPAModel.groups;
            });

            vm.getMember(vm.filter.groupId, vm.filter.isLocal);

            CommonModel.getStandardGrade().then(function () {
                vm.standardGrades = CommonModel.standardGrades;

                getEPAByManagement(vm.filter.groupId, vm.filter.isLocal, vm.filter.employeeIds, vm.filter.statusProcess);
            });
        }

        initialize();
    }
})(angular);
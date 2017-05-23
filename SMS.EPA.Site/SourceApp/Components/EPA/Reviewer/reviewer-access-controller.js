(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('epaModule').controller('ReviewerController', reviewerController);
    reviewerController.$inject = [
        '$location',
        '$translate',
        '$routeParams',
        'EvaluationModel',
        'ReviewerEPAModel',
        'ReviewerManagementModel',
        'CommonModel',
        'utilitiesFactory',
        'notificationFactory',
        'historyFactory',
        'lockScreenService',
        'baseUrl'
    ];

    function reviewerController(
        $location,
        $translate,
        $routeParams,
        EvaluationModel,
        ReviewerEPAModel,
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
        vm.standardGrades = [];
        vm.selectedAll = false;
        vm.averageScore = 0;
        vm.isEndProcessAll = false;
        vm.policyEnum = utilitiesFactory.policy;
        vm.targetTypeEnum = utilitiesFactory.targetType;
        vm.monthlyFilterSelected = false;
        vm.filter = {};

        function initFilter() {
            var filter = historyFactory.getState();

            return filter
                ? filter
                : {
                    groupId: '',
                    isLocal: '',
                    isMonthly: $routeParams.isMonthly ? $routeParams.isMonthly === 'true' : false,
                    employeeIds: '',
                    statusProcess: $routeParams.statusProcess ? $routeParams.statusProcess : ''
                };
        }

        vm.getMember = function () {
            ReviewerEPAModel.getMemberByReviewer(vm.filter.groupId, vm.filter.isLocal, vm.filter.isMonthly).then(
                function () {
                    vm.members = ReviewerEPAModel.members;
                    utilitiesFactory.setSelectedItem(vm.members, "EmployeeId", vm.filter.employeeIds);
                });
        };

        vm.selectedCls = function (field) {
            var cssClass = '';

            switch (field) {
                case 'SelfScore':
                    if (vm.sortBy.selfScore.ascending) {
                        cssClass = 'sorting-asc';
                    } else if (vm.sortBy.selfScore.descending) {
                        cssClass = 'sorting-desc';
                    }
                    break;
                case 'EvaluatorScore':
                    if (vm.sortBy.evaluatorScore.ascending) {
                        cssClass = 'sorting-asc';
                    } else if (vm.sortBy.evaluatorScore.descending) {
                        cssClass = 'sorting-desc';
                    }
                    break;
                case 'DisciplineScore':
                    if (vm.sortBy.disciplineScore.ascending) {
                        cssClass = 'sorting-asc';
                    } else if (vm.sortBy.disciplineScore.descending) {
                        cssClass = 'sorting-desc';
                    }
                    break;
                case 'TotalScore':
                    if (vm.sortBy.totalScore.ascending) {
                        cssClass = 'sorting-asc';
                    } else if (vm.sortBy.totalScore.descending) {
                        cssClass = 'sorting-desc';
                    }
                    break;
            }

            return cssClass;
        };

        function resetSorting() {
            vm.sortBy = {
                selfScore: {
                    ascending: false,
                    descending: false
                },
                evaluatorScore: {
                    ascending: false,
                    descending: false
                },
                disciplineScore: {
                    ascending: false,
                    descending: false
                },
                totalScore: {
                    ascending: false,
                    descending: false
                }
            };
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

        function sortByDisciplineScore(epaInfo1, epaInfo2) {
            return epaInfo1.DisciplineScore - epaInfo2.DisciplineScore;
        }

        function sortByTotalScore(epaInfo1, epaInfo2) {
            return epaInfo1.FinalScore - epaInfo2.FinalScore;
        }

        vm.changeSorting = function (field) {
            switch (field) {
                case 'SelfScore':
                    vm.sortBy.evaluatorScore = { ascending: false, descending: false };
                    vm.sortBy.disciplineScore = { ascending: false, descending: false };
                    vm.sortBy.totalScore = { ascending: false, descending: false };

                    if (!vm.sortBy.selfScore.ascending) {
                        vm.sortBy.selfScore = { ascending: true, descending: false };
                        vm.epaInfos.sort(sortBySelfScore);
                    } else {
                        vm.sortBy.selfScore = { ascending: false, descending: true };
                        vm.epaInfos.reverse();
                    }
                    break;
                case 'EvaluatorScore':
                    vm.sortBy.selfScore = { ascending: false, descending: false };
                    vm.sortBy.disciplineScore = { ascending: false, descending: false };
                    vm.sortBy.totalScore = { ascending: false, descending: false };

                    if (!vm.sortBy.evaluatorScore.ascending) {
                        vm.sortBy.evaluatorScore = { ascending: true, descending: false };
                        vm.epaInfos.sort(sortByEvaluatorScore);
                    } else {
                        vm.sortBy.evaluatorScore = { ascending: false, descending: true };
                        vm.epaInfos.reverse();
                    }
                    break;
                case 'DisciplineScore':
                    vm.sortBy.selfScore = { ascending: false, descending: false };
                    vm.sortBy.evaluatorScore = { ascending: false, descending: false };
                    vm.sortBy.totalScore = { ascending: false, descending: false };

                    if (!vm.sortBy.disciplineScore.ascending) {
                        vm.sortBy.disciplineScore = { ascending: true, descending: false };
                        vm.epaInfos.sort(sortByDisciplineScore);
                    } else {
                        vm.sortBy.disciplineScore = { ascending: false, descending: true };
                        vm.epaInfos.reverse();
                    }
                    break;
                case 'TotalScore':
                    vm.sortBy.selfScore = { ascending: false, descending: false };
                    vm.sortBy.evaluatorScore = { ascending: false, descending: false };
                    vm.sortBy.disciplineScore = { ascending: false, descending: false };

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

        vm.viewEPADetail = function (epaInfo) {
            if (ReviewerManagementModel.checkAdjustScoreChanged(vm.epaInfos)) {
                ReviewerManagementModel.openPopupRemindSaveChange(vm).then(function () {
                    $location.url(baseUrl + 'Reviewer/Detail/' + epaInfo.EPAId);
                })['finally'](function () {
                    vm.modalInstance = '';
                });
            } else {
                $location.url(baseUrl + 'Reviewer/Detail/' + epaInfo.EPAId);
            }
        };

        vm.viewEPAResult = function (epaInfo) {
            $location.url(baseUrl + 'EPA/Detail/' + epaInfo.EPAId);
        };

        vm.viewOtherViolationDetail = function (epaInfo) {
            CommonModel.openPopupViewOtherViolationDetail(epaInfo.EPAId);
        };

        vm.checkAll = function (selectedAll) {
            angular.forEach(vm.epaInfos, function (epaInfo) {
                if (!epaInfo.Process.CurrentProcess.IsEndProcess && (epaInfo.IsSemiAnnual || epaInfo.IsMonthly)) {
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
                addjustScore = epaInfo.Process.CurrentProcess.AdjustScore;

            if (!EvaluationModel.validateScoreInput(addjustScore, maxSubtractScore, maxAdditionScore, vm.scoreInputValidMessage)) {
                epaInfo.Process.CurrentProcess.AdjustScore = null;
            }

            ReviewerManagementModel.calculateScore(epaInfo, true);
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
                ReviewerManagementModel.openPopupAdjustScore(vm, epaInfos, true).then(function (adjust) {
                    ReviewerManagementModel.calculateAdjustScoreMultiple(epaInfos, adjust, true);
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
                notificationFactory.error("SelectEmployeeBeforeSaveWarning");
            } else {
                ReviewerManagementModel.calculateAdjustScore(epaInfos, true);
                ReviewerEPAModel.adjustScoreByReviewer(vm, epaInfos)['finally'](function () {
                    getEPAByReviewer(vm.filter.groupId,
                        vm.filter.isLocal,
                        vm.filter.isMonthly,
                        vm.filter.employeeIds,
                        epaInfos,
                        vm.filter.statusProcess);
                });
            }
        };

        vm.updateComment = function (epaInfo, comment, isEdit) {
            ReviewerManagementModel.openPopupUpdateComment(vm, epaInfo, comment, isEdit, true).then(
                function (commentUpdate) {
                    var index = vm.epaInfos.map(function (e) { return e.EPAId; }).indexOf(epaInfo.EPAId);

                    if (index !== -1) {
                        vm.epaInfos[index].Process.CurrentProcess.TotalScore = vm.epaInfos[index].Process.CurrentProcess.TotalScoreOriginal;
                        vm.epaInfos[index].Process.CurrentProcess.AdjustScore = vm.epaInfos[index].Process.CurrentProcess.AdjustScoreOriginal;
                        vm.epaInfos[index].Process.CurrentProcess.Comment = commentUpdate;
                        vm.epaInfos[index].Process.CurrentProcess.Evaluated = true;

                        ReviewerManagementModel.calculateScore(vm.epaInfos[index], true);
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

        function getEPAByReviewer(groupId, isLocal, isMonthly, employeeIds, selectedEmployees, statusProcess) {
            lockScreenService.start();
            vm.isEndProcessAll = false;
            resetSorting();

            ReviewerEPAModel.getEPAByReviewer(groupId, isLocal, isMonthly, employeeIds, statusProcess).then(function () {
                vm.epaInfos = ReviewerEPAModel.epaInfos;

                var averageScore = 0, countEndProcess = 0;
                angular.forEach(vm.epaInfos, function (epaInfo) {
                    if (epaInfo.IsSemiHasMonthly && !epaInfo.Process.CurrentProcess.TotalScore) {
                        epaInfo.Process.CurrentProcess.TotalScore = epaInfo.MonthlyEPA.AverageReviewerScoreConverted;
                    } else if (!epaInfo.IsSemiHasMonthly) {
                        if (epaInfo.Process.Evaluators[0].IsEndProcess && !epaInfo.Process.CurrentProcess.TotalScore) {
                            epaInfo.Process.CurrentProcess.TotalScore = epaInfo.Process.AverageScoreByEvaluators;
                        }
                    }

                    epaInfo.Process.Self.TotalScore = utilitiesFactory.parseFloat(epaInfo.Process.Self.TotalScore);
                    epaInfo.Process.CurrentProcess.TotalScore = utilitiesFactory.parseFloat(epaInfo.Process.CurrentProcess.TotalScore);
                    epaInfo.FinalScore = epaInfo.Process.CurrentProcess.TotalScore + epaInfo.DisciplineScore;
                    epaInfo.FinalScore = EvaluationModel.checkScore(epaInfo.FinalScore, 0, epaInfo.Process.CurrentProcess.MaxPerformanceScore);
                    epaInfo.Grade = EvaluationModel.convertScoreToGrade(
                        epaInfo.Process.CurrentProcess.TotalScore,
                        epaInfo.DisciplineScore,
                        epaInfo.HasDisciplineScore,
                        epaInfo.Process.CurrentProcess.MaxPerformanceScore,
                        vm.standardGrades);

                    if (epaInfo.LastFinalScore.length > 0) {
                        epaInfo.LastFinalScore = utilitiesFactory.parseFloat(epaInfo.LastFinalScore);
                    } else {
                        epaInfo.LastFinalScore = null;
                    }

                    if (epaInfo.Process.CurrentProcess.IsEndProcess || !(epaInfo.IsSemiAnnual || epaInfo.IsMonthly)) {
                        countEndProcess += 1;
                    }
                    averageScore += epaInfo.Process.AverageScoreByEvaluators;
                });

                if (countEndProcess === vm.epaInfos.length) {
                    vm.isEndProcessAll = true;
                }
                vm.averageScore = utilitiesFactory.parseFloat(averageScore / vm.epaInfos.length);

                if (selectedEmployees && selectedEmployees.length > 0) {
                    ReviewerManagementModel.updateSelectedEmployees(vm.epaInfos, selectedEmployees);
                }

                $(".rating-loading").rating();
                if (isMonthly) {
                    vm.monthlyFilterSelected = true;
                } else {
                    vm.monthlyFilterSelected = false;
                }

                lockScreenService.stop();
            });

            historyFactory.saveState(vm.filter);
        }

        vm.query = function (filter) {
            filter.employeeIds = utilitiesFactory.getSelectedData(vm.members, 'EmployeeId');
            getEPAByReviewer(filter.groupId,
                filter.isLocal,
                filter.isMonthly,
                filter.employeeIds,
                null,
                filter.statusProcess);
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

            ReviewerEPAModel.getUserPermissionOnReviewer().then(function () {
                vm.permission = ReviewerEPAModel.permission;
            });

            ReviewerEPAModel.getGroupByReviewer().then(function () {
                vm.groupOptions = ReviewerEPAModel.groups;
            });

            vm.getMember();

            CommonModel.getStandardGrade().then(function () {
                vm.standardGrades = CommonModel.standardGrades;

                getEPAByReviewer(vm.filter.groupId,
                    vm.filter.isLocal,
                    vm.filter.isMonthly,
                    vm.filter.employeeIds,
                    null,
                    vm.filter.statusProcess);
            });
        }

        initialize();
    }
})(angular);
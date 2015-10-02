'use strict';
/*global epaModule*/

epaModule.controller('ViewEPADetailController',
['$window', '$translate', '$routeParams', 'EmployeeEPAModel', 'utilitiesFactory',
function ($window, $translate, $routeParams, EmployeeEPAModel, utilitiesFactory) {
    var vm = this;

    vm.epaId = parseInt($routeParams.id);

    vm.isViewDetail = true;
    vm.isReadonly = true;
    vm.finalScore = null;

    var calculateFinalScore = function (epaInfo) {
        var finalScore = 0;

        if (epaInfo.ProcessByManagements.length > 0) {
            finalScore = parseFloat(epaInfo.AverageScoreByManagements);
        }
        else if (epaInfo.ProcessByReviewers.length > 0) {
            finalScore = parseFloat(epaInfo.AverageScoreByReviewers);
        }
        else {
            finalScore = parseFloat(epaInfo.AverageScoreByEvaluators);
        }

        return finalScore;
    };

    // Event read more / less text.
    utilitiesFactory.maxLengthDescription(vm);

    vm.readMore = function (item) {
        utilitiesFactory.readMore(item);
    };

    vm.readLess = function (item) {
        utilitiesFactory.readLess(item);
    };

    vm.openPopupViewComment = function (comment) {
        EmployeeEPAModel.openPopupViewComment(vm, comment);
    };

    vm.getClassProgress = function (statusEndProcess) {
        return EmployeeEPAModel.getClassProgress(statusEndProcess);
    };

    vm.range = function (n) {
        return new Array(n);
    };

    vm.back = function () {
        $window.history.back();
    };

    vm.initialize = function () {
        EmployeeEPAModel.getEPAInfoById(vm.epaId).then(function () {
            vm.epaInfo = EmployeeEPAModel.epaInfo;

            if (vm.epaInfo.HasDisciplineScore) {
                vm.finalScore = calculateFinalScore(vm.epaInfo) + vm.epaInfo.DisciplineScore;
            }
            else {
                vm.finalScore = calculateFinalScore(vm.epaInfo);
            }

            angular.forEach(vm.epaInfo.KPIGroups, function (kpiGroup) {
                if (!kpiGroup.HasKPIOptional && kpiGroup.IsSpreadKPI) {
                    var weight = kpiGroup.Weight / kpiGroup.KPIs.length;
                    angular.forEach(kpiGroup.KPIs, function (kpi) {
                        if ((weight % 1) === 0) {
                            kpi.Weight = weight;
                        }
                        else {
                            kpi.Weight = weight.toFixed(2);
                        }
                    });
                }
            });
        });
    };

    vm.initialize();
}]);

epaModule.controller('ModalViewCommentController',
['$modalInstance', 'comment',
function ($modalInstance, comment) {
    var vm = this;

    vm.comment = comment;

    vm.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}]);

epaModule.controller('ModalAdjustScoreController',
['$modalInstance', 'EmployeeEPAModel', 'selectedEmployees', 'isReviewer', 'notificationFactory',
function ($modalInstance, EmployeeEPAModel, selectedEmployees, isReviewer, notificationFactory) {
    var vm = this;

    var calculateAdjustScore = function (memberEPAs) {
        var score = vm.adjust.Score ? parseFloat(vm.adjust.Score) : 0,
            totalScore, totalScoreOriginal, totalScoreAdjust;

        angular.forEach(memberEPAs, function (memberEPA) {
            if (vm.isReviewer) {
                totalScoreOriginal = memberEPA.ProcessByReviewers[0].TotalScoreOriginal ? parseFloat(memberEPA.ProcessByReviewers[0].TotalScoreOriginal) : 0;
                totalScore = memberEPA.ProcessByReviewers[0].TotalScore ? parseFloat(memberEPA.ProcessByReviewers[0].TotalScore) : 0;

                if (totalScore !== totalScoreOriginal) {
                    memberEPA.ProcessByReviewers[0].TotalScore = totalScoreOriginal;
                }

                totalScoreAdjust = totalScoreOriginal + score;

                if (totalScoreAdjust > memberEPA.ProcessByReviewers[0].MaxScore) {
                    memberEPA.ProcessByReviewers[0].TotalScore = memberEPA.ProcessByReviewers[0].MaxScore;
                }
                else if (totalScoreAdjust < 0) {
                    memberEPA.ProcessByReviewers[0].TotalScore = 0;
                }
                else {
                    memberEPA.ProcessByReviewers[0].TotalScore = totalScoreAdjust;
                }

                if (memberEPA.HasDisciplineScore) {
                    memberEPA.FinalScore = memberEPA.ProcessByReviewers[0].TotalScore + memberEPA.DisciplineScore;
                }

                if (memberEPA.ProcessByReviewers[0].Comment && memberEPA.ProcessByReviewers[0].Comment.length > 0) {
                    memberEPA.ProcessByReviewers[0].Comment = memberEPA.ProcessByReviewers[0].Comment + '\n' + vm.adjust.Comment;
                }
                else {
                    memberEPA.ProcessByReviewers[0].Comment = vm.adjust.Comment;
                }
            }
            else {
                totalScoreOriginal = memberEPA.ProcessByManagements[0].TotalScoreOriginal ? parseFloat(memberEPA.ProcessByManagements[0].TotalScoreOriginal) : 0;
                totalScore = memberEPA.ProcessByManagements[0].TotalScore ? parseFloat(memberEPA.ProcessByManagements[0].TotalScore) : 0;

                if (totalScore !== totalScoreOriginal) {
                    memberEPA.ProcessByManagements[0].TotalScore = totalScoreOriginal;
                }

                totalScoreAdjust = totalScoreOriginal + score;

                if (totalScoreAdjust > memberEPA.ProcessByManagements[0].MaxScore) {
                    memberEPA.ProcessByManagements[0].TotalScore = memberEPA.ProcessByManagements[0].MaxScore;
                }
                else if (totalScoreAdjust < 0) {
                    memberEPA.ProcessByManagements[0].TotalScore = 0;
                }
                else {
                    memberEPA.ProcessByManagements[0].TotalScore = totalScoreAdjust;
                }

                if (memberEPA.HasDisciplineScore) {
                    memberEPA.FinalScore = memberEPA.ProcessByManagements[0].TotalScore + memberEPA.DisciplineScore;
                }

                if (memberEPA.ProcessByManagements[0].Comment && memberEPA.ProcessByManagements[0].Comment.length > 0) {
                    memberEPA.ProcessByManagements[0].Comment = memberEPA.ProcessByManagements[0].Comment + '\n' + vm.adjust.Comment;
                }
                else {
                    memberEPA.ProcessByManagements[0].Comment = vm.adjust.Comment;
                }
            }
        });
    };

    vm.adjust = {
        Score: '',
        Comment: ''
    };

    vm.selectedEmployees = selectedEmployees;
    vm.isReviewer = isReviewer;

    vm.ok = function () {
        if (!vm.adjust.Score) {
            notificationFactory.error("ScoreRequire");
        }
        else {
            calculateAdjustScore(vm.selectedEmployees);

            if (vm.isReviewer) {
                EmployeeEPAModel.adjustScoreByReviewer(vm, vm.selectedEmployees);
            }
            else {
                EmployeeEPAModel.adjustScoreByManagement(vm, vm.selectedEmployees);
            }

            $modalInstance.close(vm);
        }
    };

    vm.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}]);

epaModule.controller('ModalUpdateCommentController',
['$modalInstance', 'EmployeeEPAModel', 'memberEPA', 'comment', 'isEdit', 'isReviewer',
function ($modalInstance, EmployeeEPAModel, memberEPA, comment, isEdit, isReviewer) {
    var vm = this;

    vm.comment = comment;
    vm.isEdit = isEdit;
    vm.isReviewer = isReviewer;
    vm.memberEPA = memberEPA;

    vm.ok = function () {
        var model = [], totalScore, totalScoreOriginal;

        if (vm.isReviewer) {
            totalScoreOriginal = vm.memberEPA.ProcessByReviewers[0].TotalScoreOriginal ? parseFloat(vm.memberEPA.ProcessByReviewers[0].TotalScoreOriginal) : 0;
            totalScore = vm.memberEPA.ProcessByReviewers[0].TotalScore ? parseFloat(vm.memberEPA.ProcessByReviewers[0].TotalScore) : 0;

            if (totalScore !== totalScoreOriginal) {
                vm.memberEPA.ProcessByReviewers[0].TotalScore = totalScoreOriginal;
            }
            vm.memberEPA.ProcessByReviewers[0].Comment = vm.comment;
            model.push(vm.memberEPA);
            EmployeeEPAModel.adjustScoreByReviewer(vm, model);
        }
        else {
            totalScoreOriginal = vm.memberEPA.ProcessByManagements[0].TotalScoreOriginal ? parseFloat(vm.memberEPA.ProcessByManagements[0].TotalScoreOriginal) : 0;
            totalScore = vm.memberEPA.ProcessByManagements[0].TotalScore ? parseFloat(vm.memberEPA.ProcessByManagements[0].TotalScore) : 0;

            if (totalScore !== totalScoreOriginal) {
                vm.memberEPA.ProcessByManagements[0].TotalScore = totalScoreOriginal;
            }
            vm.memberEPA.ProcessByManagements[0].Comment = vm.comment;
            model.push(vm.memberEPA);
            EmployeeEPAModel.adjustScoreByManagement(vm, model);
        }

        $modalInstance.close(vm.comment);
    };

    vm.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}]);

epaModule.controller('ModalNotificationController',
['$modalInstance', 'message',
function ($modalInstance, message) {
    var vm = this;

    vm.message = message;

    vm.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}]);
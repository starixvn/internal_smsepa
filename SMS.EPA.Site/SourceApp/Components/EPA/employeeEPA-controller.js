'use strict';
/*global epaModule*/

epaModule.controller('ViewEPADetailController',
['$window', '$translate', '$routeParams', 'EmployeeEPAModel', 'CommonModel', 'utilitiesFactory',
function ($window, $translate, $routeParams, EmployeeEPAModel, CommonModel, utilitiesFactory) {
    var vm = this;

    vm.epaId = parseInt($routeParams.id);

    vm.isViewDetail = true;
    vm.isReadonly = true;
    vm.finalScore = null;

    vm.standardGrades = CommonModel.standardGrades;

    var calculateFinalScore = function (epaInfo) {
        var finalScore = 0;

        if (epaInfo.ProcessByManagements.length > 0) {
            finalScore = epaInfo.AverageScoreByManagements;
        }
        else if (epaInfo.ProcessByReviewers.length > 0) {
            finalScore = epaInfo.AverageScoreByReviewers;
        }
        else {
            finalScore = epaInfo.AverageScoreByEvaluators;
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

    var getEPAInfoById = function () {
        EmployeeEPAModel.getEPAInfoById(vm.epaId).then(function () {
            vm.epaInfo = EmployeeEPAModel.epaInfo;
            var totalScore,
                disciplineScore = vm.epaInfo.MaxDisciplineScore - vm.epaInfo.DisciplineScore;
            if (disciplineScore < 0) {
                disciplineScore = 0;
            }

            vm.epaInfo.DisciplineScore = disciplineScore;
            totalScore = utilitiesFactory.parseFloat(vm.epaInfo.ProcessBySelf.TotalScore);
            vm.finalScore = calculateFinalScore(vm.epaInfo);
            vm.epaInfo.ProcessBySelf.TotalScore = totalScore;

            angular.forEach(vm.epaInfo.ProcessByEvaluators, function (evaluator) {
                evaluator.TotalScore = utilitiesFactory.parseFloat(evaluator.TotalScore);
            });

            angular.forEach(vm.epaInfo.ProcessByReviewers, function (reviewer) {
                reviewer.TotalScore = utilitiesFactory.parseFloat(reviewer.TotalScore);
            });

            angular.forEach(vm.epaInfo.ProcessByManagements, function (management) {
                management.TotalScore = utilitiesFactory.parseFloat(management.TotalScore);
            });

            if (vm.epaInfo.HasDisciplineScore) {
                vm.epaInfo.ProcessBySelf.TotalScore += vm.epaInfo.DisciplineScore;

                angular.forEach(vm.epaInfo.ProcessByEvaluators, function (evaluator) {
                    evaluator.TotalScore += vm.epaInfo.DisciplineScore;
                });
                vm.epaInfo.AverageScoreByEvaluators += vm.epaInfo.DisciplineScore;

                angular.forEach(vm.epaInfo.ProcessByReviewers, function (reviewer) {
                    reviewer.TotalScore += vm.epaInfo.DisciplineScore;
                });
                vm.epaInfo.AverageScoreByReviewers += vm.epaInfo.DisciplineScore;

                angular.forEach(vm.epaInfo.ProcessByManagements, function (management) {
                    management.TotalScore += vm.epaInfo.DisciplineScore;
                });
                vm.epaInfo.AverageScoreByManagements += vm.epaInfo.DisciplineScore;

                vm.finalScore += vm.epaInfo.DisciplineScore;
            }

            vm.epaInfo.Grade = EmployeeEPAModel.convertScoreToGrade(vm.finalScore, vm.standardGrades);

            angular.forEach(vm.epaInfo.KPIGroups, function (kpiGroup) {
                EmployeeEPAModel.scoreSpread(kpiGroup);
            });
        });
    };

    vm.initialize = function () {
        CommonModel.getStandardGrade().then(function () {
            vm.standardGrades = CommonModel.standardGrades;

            getEPAInfoById();
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
['$modalInstance', 'EmployeeEPAModel', 'utilitiesFactory', 'notificationFactory', 'selectedEmployees', 'isReviewer',
function ($modalInstance, EmployeeEPAModel, utilitiesFactory, notificationFactory, selectedEmployees, isReviewer) {
    var vm = this;

    var calculateAdjustScore = function (memberEPAs) {
        var scoreAdjust = utilitiesFactory.parseFloat(vm.adjust.Score),
            totalScoreAdjust;

        angular.forEach(memberEPAs, function (memberEPA) {
            if (vm.isReviewer) {
                totalScoreAdjust = memberEPA.AverageScoreByEvaluators + scoreAdjust;
            }
            else {
                if (memberEPA.ProcessByReviewers.length > 0) {
                    totalScoreAdjust = memberEPA.AverageScoreByReviewers + scoreAdjust;
                }
                else {
                    totalScoreAdjust = memberEPA.AverageScoreByEvaluators + scoreAdjust;
                }
            }

            if (memberEPA.HasDisciplineScore) {
                totalScoreAdjust -= memberEPA.DisciplineScore;
            }

            if (totalScoreAdjust > memberEPA.CurrentProcess.MaxTotalScore) {
                totalScoreAdjust = memberEPA.CurrentProcess.MaxTotalScore;
            }
            else if (totalScoreAdjust < 0) {
                totalScoreAdjust = 0;
            }

            memberEPA.CurrentProcess.AdjustScore = scoreAdjust;
            memberEPA.CurrentProcess.TotalScore = totalScoreAdjust;
            memberEPA.FinalScore = totalScoreAdjust;
            if (memberEPA.HasDisciplineScore) {
                memberEPA.FinalScore += memberEPA.DisciplineScore;
            }

            if (memberEPA.FinalScore > memberEPA.MaxFinalScore) {
                memberEPA.FinalScore = memberEPA.MaxFinalScore;
            }
            else if (memberEPA.FinalScore < 0) {
                memberEPA.FinalScore = 0;
            }

            if (memberEPA.CurrentProcess.Comment && memberEPA.CurrentProcess.Comment.length > 0) {
                memberEPA.CurrentProcess.Comment = memberEPA.CurrentProcess.Comment + '\n' + vm.adjust.Comment;
            }
            else {
                memberEPA.CurrentProcess.Comment = vm.adjust.Comment;
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
['$modalInstance', 'EmployeeEPAModel', 'utilitiesFactory', 'memberEPA', 'comment', 'isEdit', 'isReviewer',
function ($modalInstance, EmployeeEPAModel, utilitiesFactory, memberEPA, comment, isEdit, isReviewer) {
    var vm = this;

    vm.comment = comment;
    vm.isEdit = isEdit;
    vm.isReviewer = isReviewer;
    vm.memberEPA = memberEPA;

    vm.ok = function () {
        var model = [], adjustScore, totalScore;

        adjustScore = utilitiesFactory.parseFloat(memberEPA.CurrentProcess.AdjustScoreOriginal);

        if (vm.isReviewer) {
            totalScore = vm.memberEPA.AverageScoreByEvaluators + adjustScore;
        }
        else {
            if (vm.memberEPA.ProcessByReviewers.length > 0) {
                totalScore = vm.memberEPA.AverageScoreByReviewers + adjustScore;
            }
            else {
                totalScore = vm.memberEPA.AverageScoreByEvaluators + adjustScore;
            }
        }

        if (vm.memberEPA.HasDisciplineScore) {
            totalScore -= vm.memberEPA.DisciplineScore;
        }

        if (totalScore > vm.memberEPA.CurrentProcess.MaxTotalScore) {
            totalScore = vm.memberEPA.CurrentProcess.MaxTotalScore;
        }
        else if (totalScore < 0) {
            totalScore = 0;
        }

        vm.memberEPA.CurrentProcess.TotalScore = totalScore;
        vm.memberEPA.CurrentProcess.AdjustScore = adjustScore;
        vm.memberEPA.FinalScore = totalScore;

        if (vm.memberEPA.HasDisciplineScore) {
            vm.memberEPA.FinalScore += vm.memberEPA.DisciplineScore;
        }

        if (vm.memberEPA.FinalScore > vm.memberEPA.MaxFinalScore) {
            vm.memberEPA.FinalScore = vm.memberEPA.MaxFinalScore;
        }
        else if (vm.memberEPA.FinalScore < 0) {
            vm.memberEPA.FinalScore = 0;
        }

        vm.memberEPA.CurrentProcess.Comment = vm.comment;
        model.push(vm.memberEPA);

        if (vm.isReviewer) {
            EmployeeEPAModel.adjustScoreByReviewer(vm, model);
        }
        else {
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

appModule.controller("ModalRemindController",
['$modalInstance',
 function ($modalInstance) {
     var vm = this;

     vm.ok = function () {
         $modalInstance.close();
     };

     vm.cancel = function () {
         $modalInstance.dismiss("cancel");
     };
 }]);
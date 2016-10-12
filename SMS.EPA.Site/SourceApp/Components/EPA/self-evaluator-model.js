(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('epaModule').factory('SelfEvalutorModel', selfEvalutorModel);
    selfEvalutorModel.$inject = ['$modal', 'EmployeeEPAModel', 'utilitiesFactory'];

    function selfEvalutorModel($modal, EmployeeEPAModel, utilitiesFactory) {
        var model = {
            calculateScore: calculateScore,
            openPopupNotificationInvalidComment: openPopupNotificationInvalidComment,
            spreadScoreForKPIGroupHasKPIOptional: spreadScoreForKPIGroupHasKPIOptional
        };

        return model;

        function calculateScore(vm) {
            var totalScore = 0;

            angular.forEach(vm.epaInfo.KPIGroups, function (kpiGroup) {
                if (kpiGroup.HasKPIOptional) {
                    spreadScoreForKPIGroupHasKPIOptional(kpiGroup, vm.maxStar);
                }

                angular.forEach(kpiGroup.KPIs, function (kpi) {
                    var score = utilitiesFactory.splitNumber(kpi.CurrentEvaluation.Score, 4),
                        kpiWeight = utilitiesFactory.splitNumber(kpi.Weight, 2),
                        scoreRating = utilitiesFactory.splitNumber(kpi.CurrentEvaluation.Score, 2);

                    score = score ? parseFloat(score) : 0;

                    if ((score > 0 && scoreRating === kpiWeight) || (kpi.CurrentEvaluation.Rate === vm.maxStar)) {
                        kpi.CurrentEvaluation.IsMaxScore = true;
                        totalScore += kpi.Weight;
                    } else {
                        kpi.CurrentEvaluation.IsMaxScore = false;
                        totalScore += score;
                    }
                });
            });

            vm.epaInfo.Process.CurrentProcess.TotalScore = utilitiesFactory.parseFloat(totalScore);
            var maxTotalScore = vm.epaInfo.Process.CurrentProcess.MaxTotalScore;
            vm.epaInfo.Process.CurrentProcess.TotalScore = EmployeeEPAModel.checkScore(vm.epaInfo.Process.CurrentProcess.TotalScore, 0, maxTotalScore);

            vm.finalScore = utilitiesFactory.parseFloat(totalScore + vm.epaInfo.DisciplineScore);
            vm.finalScore = EmployeeEPAModel.checkScore(vm.finalScore, 0, vm.epaInfo.Process.CurrentProcess.MaxPerformanceScore);

            vm.epaInfo.Grade = EmployeeEPAModel.convertScoreToGrade(vm.finalScore, vm.standardGrades);
        }

        function openPopupNotificationInvalidComment(vm, message) {
            vm.modalInstance = $modal.open({
                templateUrl: 'SourceApp/Components/EPA/Templates/modalNotification.html',
                controller: "ModalNotificationController as modalNotificationCtrl",
                backdrop: 'static',
                resolve: {
                    message: function () {
                        return message;
                    }
                },
                windowClass: "modal-confirm-center"
            });

            return vm.modalInstance.result;
        }

        function spreadScoreForKPIGroupHasKPIOptional(kpiGroup, maxStar) {
            var score, averageWeight, totalStarEvaluated = 0,
                numberKPIOptional = kpiGroup.NumberKPIOptional;
            var kpis = [];

            angular.forEach(kpiGroup.KPIs, function (kpi) {
                if (kpi.CurrentEvaluation.Rate > 0) {
                    kpis.push(kpi);
                    totalStarEvaluated += kpi.CurrentEvaluation.Rate;
                }

                if (!kpi.IsRequiredEvaluate && kpi.CurrentEvaluation.Rate > 0) {
                    numberKPIOptional -= 1;
                }
            });

            var totalKPIMustEvaluate = kpiGroup.KPIs.length - numberKPIOptional;
            averageWeight = kpiGroup.Weight / totalKPIMustEvaluate;
            averageWeight = utilitiesFactory.splitNumber(averageWeight, 4);

            angular.forEach(kpis, function (kpi) {
                kpi.Weight = averageWeight;
                score = (averageWeight * kpi.CurrentEvaluation.Rate) / maxStar;
                kpi.CurrentEvaluation.Score = utilitiesFactory.splitNumber(score, 4);
            });
        }
    }
})(angular);
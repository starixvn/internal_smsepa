(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */
    /* jshint validthis:true */

    angular.module('feedbackModule').controller('FeedbackDetailController', feedbackDetailController);
    feedbackDetailController.$inject = ['$timeout', '$translate', '$routeParams', 'EvaluationModel', 'SelfEvalutorModel',
        'errorFactory', 'utilitiesFactory', 'historyFactory', 'notificationFactory', 'feedbackFactory', 'lockScreenService', 'baseUrl'];

    function feedbackDetailController($timeout, $translate, $routeParams, EvaluationModel, SelfEvalutorModel,
        errorFactory, utilitiesFactory, historyFactory, notificationFactory, feedbackFactory, lockScreenService, baseUrl) {
        var vm = this;

        vm.feedbackId = parseInt($routeParams.id);
        vm.feedback = {};
        vm.maxStar = utilitiesFactory.maxStar;

        vm.hoveringLeave = null;

        function getFeedbackById() {
            feedbackFactory.getFeedbackById(vm.feedbackId).success(function (feedback) {
                vm.feedback = feedback;
                if (vm.feedback.IsHost) {
                    vm.hoveringLeave = hoveringLeave;
                }

                vm.feedback.Employee.Avatar = baseUrl + vm.feedback.Employee.Avatar;
                vm.feedback.FinalScore = utilitiesFactory.parseFloat(vm.feedback.FinalScore);

                angular.forEach(vm.feedback.KPIGroups, function (kpiGroup) {
                    angular.forEach(kpiGroup.KPIs, function (kpi) {
                        kpi.Rate = kpi.Rate ? parseInt(kpi.Rate) : 0;
                    });
                });

                lockScreenService.stop();
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function spreadScoreForKPIGroupHasKPIOptional(kpiGroup) {
            var kpis = [],
                averageWeight = 0,
                kpiScoreNotEvaluate = 0;

            EvaluationModel.scoreSpread(kpiGroup);

            angular.forEach(kpiGroup.KPIs, function (kpi) {
                if (kpi.IsRequiredEvaluate || kpi.Rate > 0) {
                    kpis.push(kpi);
                }

                if (!kpi.IsRequiredEvaluate && kpi.Rate === 0) {
                    kpiScoreNotEvaluate += kpi.Weight;
                }
            });

            averageWeight = kpiScoreNotEvaluate / kpis.length;
            averageWeight = utilitiesFactory.splitNumber(averageWeight, 4);

            angular.forEach(kpis, function (kpi) {
                var weight = kpi.Weight + averageWeight;
                var score = weight * kpi.CurrentEvaluation.Rate / vm.maxStar;
                kpi.CurrentEvaluation.Score = utilitiesFactory.splitNumber(score, 4);
            });
        }

        function calculateScore() {
            var totalScore = 0;
            angular.forEach(vm.feedback.KPIGroups, function (kpiGroup) {
                angular.forEach(kpiGroup.KPIs, function (kpi) {
                    var score = utilitiesFactory.splitNumber(kpi.Score, 4);
                    score = score ? parseFloat(score) : 0;
                    totalScore += score;
                });
            });

            vm.feedback.FinalScore = utilitiesFactory.parseFloat(totalScore);
            vm.finalScore = EvaluationModel.checkScore(vm.feedback.FinalScore, 0, 100);
        }

        function hoveringLeave(kpiGroup, kpi) {
            if (kpi.Rate > 0) {
                if (!kpiGroup.HasKPIOptional) {
                    var score = kpi.Weight * kpi.Rate / vm.maxStar;
                    score = utilitiesFactory.splitNumber(score, 4);
                    kpi.Score = score;
                } else {
                    spreadScoreForKPIGroupHasKPIOptional(kpiGroup, vm.maxStar);
                }

                calculateScore();
            }
        }

        function validate() {
            var isValid = true;

            angular.forEach(vm.feedback.KPIGroups, function (kpiGroup) {
                angular.forEach(kpiGroup.KPIs, function (kpi) {
                    var score = utilitiesFactory.parseFloat(kpi.Score);

                    if (kpi.IsRequiredEvaluate && score <= 0) {
                        isValid = false;
                        kpi.EvaluateInvalid = true;
                    } else {
                        kpi.EvaluateInvalid = false;
                    }

                    if (kpi.HasComment && kpi.IsRequiredComment && (!kpi.Comment || kpi.Comment.length === 0)) {
                        isValid = false;
                        kpi.CommentInvalid = true;
                        kpi.ExpandedComment = true;
                    } else {
                        kpi.CommentInvalid = false;
                    }
                });
            });

            return isValid;
        }

        function initialize() {
            lockScreenService.start();

            $translate('ScoreInputValid').then(function (message) {
                vm.scoreInputValidMessage = message;
            });

            $translate('EvaluateEPAInvalid').then(function (message) {
                vm.evaluateEPAInvalidMessage = message;
            });

            getFeedbackById();
        }

        vm.cancel = function () {
            historyFactory.back();
        };

        vm.cancelRating = function (kpiGroup, kpi) {
            kpi.Rate = 0;
            $('#star_' + kpi.KPIId).rating('update', kpi.Rate);
            kpi.Score = null;

            if (kpiGroup.HasKPIOptional) {
                spreadScoreForKPIGroupHasKPIOptional(kpiGroup);
            }

            calculateScore();
        };

        vm.generateRatingStar = function () {
            $timeout(function () {
                $(".rating-loading").rating();
            }, 1);
        };

        vm.save = function () {
            if (validate()) {
                feedbackFactory.updateFeedback(vm.feedback).success(function (response) {
                    if (response.IsSuccessful) {
                        notificationFactory.success('UpdatedSuccessfully');
                    } else {
                        notificationFactory.error('', response.Message);
                    }
                }).error(function (response, status) {
                    errorFactory.errorHandle(status);
                });
            } else {
                SelfEvalutorModel.openPopupNotificationInvalidComment(vm, vm.evaluateEPAInvalidMessage);
            }
        };

        initialize();
    }
})(angular);
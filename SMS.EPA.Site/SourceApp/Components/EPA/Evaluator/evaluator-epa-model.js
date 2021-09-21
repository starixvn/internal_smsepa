(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('epaModule').factory('EvaluatorEPAModel', evaluatorEPAModel);
    evaluatorEPAModel.$inject = ['evaluatorEPAFactory', 'utilitiesFactory', 'notificationFactory', 'errorFactory'];

    function evaluatorEPAModel(evaluatorEPAFactory, utilitiesFactory, notificationFactory, errorFactory) {
        var epaInfo = {},
            epaInfos = [],
            periods = [],
            groups = [],
            members = [],
            permission = 0;

        var model = {
            epaInfo: epaInfo,
            epaInfos: epaInfos,
            periods: periods,
            groups: groups,
            members: members,
            permission: permission,
            getEPAByEvaluator: getEPAByEvaluator,
            getEPAInfoByEvaluator: getEPAInfoByEvaluator,
            getPeriodByEvaluator: getPeriodByEvaluator,
            getGroupByEvaluator: getGroupByEvaluator,
            getMemberByEvaluator: getMemberByEvaluator,
            getUserPermissionOnEvaluator: getUserPermissionOnEvaluator,
            updateEPAScoreByEvaluator: updateEPAScoreByEvaluator,
            validateEPA: validateEPA,
            validateRecommendation: validateRecommendation
        };

        return model;

        function getEPAByEvaluator(periodId, groupId, isLocal, isMonthly, employeeIds, statusProcess) {
            return evaluatorEPAFactory.getEPAByEvaluator(periodId, groupId, isLocal, isMonthly, employeeIds, statusProcess).success(function (epaInfos) {
                model.epaInfos = epaInfos;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getEPAInfoByEvaluator(epaId, evaluatorId) {
            return evaluatorEPAFactory.getEPAInfoByEvaluator(epaId, evaluatorId).success(function (epaInfo) {
                model.epaInfo = epaInfo;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getPeriodByEvaluator() {
            return evaluatorEPAFactory.getPeriodByEvaluator().success(function (periods) {
                model.periods = periods;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getGroupByEvaluator(periodId) {
            return evaluatorEPAFactory.getGroupByEvaluator(periodId).success(function (groups) {
                model.groups = groups;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getMemberByEvaluator(periodId, groupId, isLocal, isMonthly) {
            return evaluatorEPAFactory.getMemberByEvaluator(periodId, groupId, isLocal, isMonthly).success(function (members) {
                model.members = members;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getUserPermissionOnEvaluator() {
            return evaluatorEPAFactory.getUserPermissionOnEvaluator().success(function (permission) {
                model.permission = parseInt(permission);
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function updateEPAScoreByEvaluator(vm) {
            var criteria = {
                EPAId: vm.epaInfo.EPAId,
                EmployeeId: vm.epaInfo.Employee.EmployeeId,
                EPAProcess: vm.epaInfo.Process.CurrentProcess,
                KPIGroups: vm.epaInfo.KPIGroups,
                Recommendations: [vm.epaInfo.Recommendation]
            };

            return evaluatorEPAFactory.updateEPAScoreByEvaluator(criteria).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success('UpdatedSuccessfully');
                    vm.dataSaved = true;
                } else {
                    notificationFactory.error('', response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function validateEPA(epaInfo) {
            var isValid = true;

            angular.forEach(epaInfo.KPIGroups, function (kpiGroup) {
                angular.forEach(kpiGroup.KPIs, function (kpi) {
                    var score = kpi.CurrentEvaluation.Score;

                    if (kpi.IsRequiredEvaluate && (score === '' || score === null)) {
                        isValid = false;
                        kpi.CurrentEvaluation.EvaluateInvalid = true;
                    } else {
                        kpi.CurrentEvaluation.EvaluateInvalid = false;
                    }

                    if (kpi.HasComment && (!kpi.CurrentEvaluation.Comment || kpi.CurrentEvaluation.Comment.length === 0)
                        && (kpi.IsRequiredComment || (epaInfo.RequiredCommentOnMaxScoreForEvaluator && !kpi.IsRequiredComment && kpi.CurrentEvaluation.IsMaxScore))) {
                        isValid = false;
                        kpi.CurrentEvaluation.CommentInvalid = true;
                        kpi.ExpandedComment = true;
                    } else {
                        kpi.CurrentEvaluation.CommentInvalid = false;
                    }
                });
            });

            return isValid;
        }

        function validateRecommendation(epaInfo) {
            var isValid = true;
            if (epaInfo.Recommendation && epaInfo.Recommendation.EPAId > 0) {
                if ((epaInfo.IsProbation || epaInfo.IsPromotion) && epaInfo.Recommendation.ProbationStatus === null) {
                    epaInfo.Recommendation.Invalid = true;
                    isValid = false;
                } else if (epaInfo.IsSemiAnnual && epaInfo.Recommendation.JobLevelIncreased && epaInfo.Recommendation.JobLevelId === null) {
                    epaInfo.Recommendation.Invalid = true;
                    isValid = false;
                } else {
                    epaInfo.Recommendation.Invalid = false;
                }
            }

            return isValid;
        }
    }
})(angular);
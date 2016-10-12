(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('epaModule').factory('ReviewerEPAModel', reviewerEPAModel);
    reviewerEPAModel.$inject = ['reviewerEPAFactory', 'utilitiesFactory', 'notificationFactory', 'errorFactory'];

    function reviewerEPAModel(reviewerEPAFactory, utilitiesFactory, notificationFactory, errorFactory) {
        var epaInfo = {},
            epaInfos = [],
            groups = [],
            members = [],
            permission = 0;

        var model = {
            epaInfo: epaInfo,
            epaInfos: epaInfos,
            groups: groups,
            members: members,
            permission: permission,
            adjustScoreByReviewer: adjustScoreByReviewer,
            getEPAByReviewer: getEPAByReviewer,
            getEPAInfoByReviewer: getEPAInfoByReviewer,
            getGroupByReviewer: getGroupByReviewer,
            getMemberByReviewer: getMemberByReviewer,
            getUserPermissionOnReviewer: getUserPermissionOnReviewer,
            validateRecommendation: validateRecommendation
        };

        return model;

        function adjustScoreByReviewer(vm, epaInfos) {
            var criteria = {
                EpaInfos: [],
                Recommendations: []
            };

            angular.forEach(epaInfos, function (epaInfo) {
                var model = {
                    EPAId: epaInfo.EPAId,
                    EmployeeId: epaInfo.Employee.EmployeeId,
                    ProcessId: epaInfo.Process.CurrentProcess.ProcessId,
                    EvaluatorId: epaInfo.Process.CurrentProcess.EvaluatorInfo.EmployeeId,
                    AdjustScore: epaInfo.Process.CurrentProcess.AdjustScore,
                    TotalScore: epaInfo.Process.CurrentProcess.TotalScore,
                    Comment: epaInfo.Process.CurrentProcess.Comment
                };

                if (epaInfo.Recommendation && epaInfo.Recommendation.EPAId > 0) {
                    criteria.Recommendations.push(epaInfo.Recommendation);
                }

                criteria.EpaInfos.push(model);
            });

            return reviewerEPAFactory.adjustScoreByReviewer(criteria).success(function (response) {
                if (response.IsSuccessful) {
                    vm.dataSaved = true;
                    notificationFactory.success('UpdatedSuccessfully');
                } else {
                    notificationFactory.error('', response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getEPAByReviewer(groupId, isLocal, isMonthly, employeeIds, statusProcess) {
            return reviewerEPAFactory.getEPAByReviewer(groupId, isLocal, isMonthly, employeeIds, statusProcess).success(function (epaInfos) {
                model.epaInfos = epaInfos;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getEPAInfoByReviewer(epaId) {
            return reviewerEPAFactory.getEPAInfoByReviewer(epaId).success(function (epaInfo) {
                model.epaInfo = epaInfo;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getGroupByReviewer() {
            return reviewerEPAFactory.getGroupByReviewer().success(function (groups) {
                model.groups = groups;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getMemberByReviewer(groupId, isLocal, isMonthly) {
            return reviewerEPAFactory.getMemberByReviewer(groupId, isLocal, isMonthly).success(function (members) {
                model.members = members;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getUserPermissionOnReviewer() {
            return reviewerEPAFactory.getUserPermissionOnReviewer().success(function (permission) {
                model.permission = parseInt(permission);
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function validateRecommendation(epaInfo) {
            var isValid = true;

            if (epaInfo.Recommendation && epaInfo.Recommendation.EPAId > 0) {
                if ((epaInfo.IsProbation || epaInfo.IsPromotion) && epaInfo.Recommendation.ProbationStatus === null) {
                    epaInfo.Recommendation.Invalid = true;
                    isValid = false;
                } else if (epaInfo.IsSemiAnnual && epaInfo.Recommendation.JobGradeIncreased
                    && (!epaInfo.Recommendation.JobGrade || !epaInfo.Recommendation.JobGrade.JobGradeId)) {
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
(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('epaModule').factory('EvaluatorEPAModel', evaluatorEPAModel);
    evaluatorEPAModel.$inject = ['evaluatorEPAFactory', 'utilitiesFactory', 'notificationFactory', 'errorFactory'];

    function evaluatorEPAModel(evaluatorEPAFactory, utilitiesFactory, notificationFactory, errorFactory) {
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
            getEPAByEvaluator: getEPAByEvaluator,
            getEPAInfoByEvaluator: getEPAInfoByEvaluator,
            getGroupByEvaluator: getGroupByEvaluator,
            getMemberByEvaluator: getMemberByEvaluator,
            getUserPermissionOnEvaluator: getUserPermissionOnEvaluator,
            updateEPAScoreByEvaluator: updateEPAScoreByEvaluator,
            validateEPA: validateEPA
        };

        return model;

        function getEPAByEvaluator(groupId, isLocal, isMonthly, employeeIds, statusProcess) {
            return evaluatorEPAFactory.getEPAByEvaluator(groupId, isLocal, isMonthly, employeeIds, statusProcess).success(function (epaInfos) {
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

        function getGroupByEvaluator() {
            return evaluatorEPAFactory.getGroupByEvaluator().success(function (groups) {
                model.groups = groups;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getMemberByEvaluator(groupId, isLocal, isMonthly) {
            return evaluatorEPAFactory.getMemberByEvaluator(groupId, isLocal, isMonthly).success(function (members) {
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
                KPIGroups: vm.epaInfo.KPIGroups
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
                    var score = utilitiesFactory.parseFloat(kpi.CurrentEvaluation.Score);

                    if (kpi.IsRequiredEvaluate && (score <= 0)) {
                        isValid = false;
                        kpi.CurrentEvaluation.EvaluateInvalid = true;
                    } else {
                        kpi.CurrentEvaluation.EvaluateInvalid = false;
                    }

                    if (kpi.HasComment && (!kpi.CurrentEvaluation.Comment || kpi.CurrentEvaluation.Comment.length === 0)
                        && (kpi.IsRequiredComment || (epaInfo.RequiredCommentOnMaxScore && !kpi.IsRequiredComment && kpi.CurrentEvaluation.IsMaxScore))) {
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
    }
})(angular);
(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('epaModule').factory('SelfEvalutionModel', selfEvalutionModel);
    selfEvalutionModel.$inject = ['selfEvaluationFactory', 'utilitiesFactory', 'notificationFactory', 'errorFactory'];

    function selfEvalutionModel(selfEvaluationFactory, utilitiesFactory, notificationFactory, errorFactory) {
        var epaInfo = {},
            epaInfos = [],
            permission = 0;

        var model = {
            epaInfo: epaInfo,
            epaInfos: epaInfos,
            permission: permission,
            getEPAByEmployee: getEPAByEmployee,
            getEPAInfoByEmployee: getEPAInfoByEmployee,
            getUserPermissionOnEmployee: getUserPermissionOnEmployee,
            updateEPAScoreByEmployee: updateEPAScoreByEmployee,
            validateEPA: validateEPA
        };

        return model;

        function getEPAByEmployee() {
            return selfEvaluationFactory.getEPAByEmployee().success(function (epaInfos) {
                model.epaInfos = epaInfos;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getEPAInfoByEmployee(epaId) {
            return selfEvaluationFactory.getEPAInfoByEmployee(epaId).success(function (epaInfo) {
                model.epaInfo = epaInfo;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getUserPermissionOnEmployee() {
            return selfEvaluationFactory.getUserPermissionOnEmployee().success(function (permission) {
                model.permission = parseInt(permission);
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function updateEPAScoreByEmployee(vm) {
            var criteria = {
                EPAId: vm.epaInfo.EPAId,
                EmployeeId: vm.epaInfo.Employee.EmployeeId,
                EPAProcess: vm.epaInfo.Process.CurrentProcess,
                KPIGroups: vm.epaInfo.KPIGroups,
                ProbationSubQuestions: vm.epaInfo.ExtendedQuestion.SubQuestions
            };

            return selfEvaluationFactory.updateEPAScoreByEmployee(criteria).success(function (response) {
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
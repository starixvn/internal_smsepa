(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('epaModule').factory('ManagementEPAModel', managementEPAModel);
    managementEPAModel.$inject = ['managementEPAFactory', 'utilitiesFactory', 'notificationFactory', 'errorFactory'];

    function managementEPAModel(managementEPAFactory, utilitiesFactory, notificationFactory, errorFactory) {
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
            adjustScoreByManagement: adjustScoreByManagement,
            getEPAByManagement: getEPAByManagement,
            getEPAInfoByManagement: getEPAInfoByManagement,
            getPeriodByManagement: getPeriodByManagement,
            getGroupByManagement: getGroupByManagement,
            getMemberByManagement: getMemberByManagement,
            getUserPermissionOnManagement: getUserPermissionOnManagement
        };

        return model;

        function adjustScoreByManagement(vm, epaInfos) {
            var criteria = {
                EpaInfos: []
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

                criteria.EpaInfos.push(model);
            });

            return managementEPAFactory.adjustScoreByManagement(criteria).success(function (response) {
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

        function getEPAByManagement(periodId, groupId, isLocal, employeeIds, statusProcess) {
            return managementEPAFactory.getEPAByManagement(periodId, groupId, isLocal, employeeIds, statusProcess).success(function (epaInfos) {
                model.epaInfos = epaInfos;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getEPAInfoByManagement(epaId, managementId) {
            return managementEPAFactory.getEPAInfoByManagement(epaId, managementId).success(function (epaInfo) {
                model.epaInfo = epaInfo;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getPeriodByManagement() {
            return managementEPAFactory.getPeriodByManagement().success(function (periods) {
                model.periods = periods;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getGroupByManagement(periodId) {
            return managementEPAFactory.getGroupByManagement(periodId).success(function (groups) {
                model.groups = groups;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getMemberByManagement(periodId, groupId, isLocal) {
            return managementEPAFactory.getMemberByManagement(periodId, groupId, isLocal).success(function (members) {
                model.members = members;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getUserPermissionOnManagement() {
            return managementEPAFactory.getUserPermissionOnManagement().success(function (permission) {
                model.permission = parseInt(permission);
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }
    }
})(angular);
'use strict';
/*global epaModule*/

epaModule.controller('EvaluatorController',
['EmployeeEPAModel', 'CommonModel', 'utilitiesFactory',
function (EmployeeEPAModel, CommonModel, utilitiesFactory) {
    var vm = this;

    vm.memberEPAs = EmployeeEPAModel.memberEPAs;
    vm.targetTypes = CommonModel.targetTypes;
    vm.statusList = CommonModel.statusList;
    vm.members = EmployeeEPAModel.members;

    vm.status = {
        isopen: false
    };
    vm.epaStatus = EmployeeEPAModel.epaStatus;

    vm.filter = {
        fromDate: new Date(),
        toDate: new Date(),
        targetTypeId: '',
        groupId: '',
        employeeIds: '',
        status: ''
    };

    utilitiesFactory.getDateFormat(vm);

    vm.calendar = {
        startDateOpened: false,
        endDateOpened: false
    };

    vm.openPopupStartDate = function ($event) {
        utilitiesFactory.openPopupStartDate($event, vm);
    };

    vm.openPopupEndDate = function ($event) {
        utilitiesFactory.openPopupEndDate($event, vm);
    };

    vm.toggleDropdown = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();
        vm.status.isopen = !vm.status.isopen;
    };

    vm.getMember = function (groupId) {
        EmployeeEPAModel.getMember(3, groupId).then(function () {
            vm.members = EmployeeEPAModel.members;
        });
    };

    vm.updateStatusEPA = function (epa, status) {
        EmployeeEPAModel.openPopupUpdateStatusEPA(epa, epa.EPAId, epa.PeriodId, 3, epa.Employee.EmployeeId, status);
    };

    var getEPAByEvaluator = function (fromDate, toDate, targetTypeId, groupId, employeeIds, status) {
        EmployeeEPAModel.getEPAByEvaluator(fromDate, toDate, targetTypeId, groupId, employeeIds, status).then(function () {
            vm.memberEPAs = EmployeeEPAModel.memberEPAs;
        });
    };

    vm.query = function (filter) {
        if (EmployeeEPAModel.validateFromToDate(filter.fromDate, filter.toDate)) {
            filter.employeeIds = utilitiesFactory.getSelectedData(vm.members, 'EmployeeId');
            getEPAByEvaluator(filter.fromDate, filter.toDate, filter.targetTypeId, filter.groupId, filter.employeeIds, filter.status);
        }
    };

    vm.viewEPAInfoOfMember = function (memberEPA) {
        EmployeeEPAModel.viewEPAInfoOfMember(memberEPA.EPAId, memberEPA.PeriodId, memberEPA.Employee.EmployeeId);
    };

    vm.viewEPADetail = function (memberEPA) {
        vm.viewEPADetailUrl = '/Evaluator/Detail?epaId=' + memberEPA.EPAId + '&periodId=' + memberEPA.PeriodId + '&employeeId=' + memberEPA.Employee.EmployeeId;
    };

    vm.initialize = function () {
        CommonModel.getTargetType().then(function () {
            vm.targetTypes = CommonModel.targetTypes;
        });

        CommonModel.getAllGroup().then(function () {
            vm.groupOptions = CommonModel.groups;
        });

        CommonModel.getAllStatus().then(function () {
            vm.statusList = CommonModel.statusList;
        });

        vm.getMember(vm.filter.groupId);

        vm.query(vm.filter);
    };

    vm.initialize();
}]);

epaModule.controller('MemberEPAInfoController',
['$window', '$translate', '$routeParams', 'EmployeeEPAModel', 'CommonModel', 'utilitiesFactory',
function ($window, $translate, $routeParams, EmployeeEPAModel, CommonModel, utilitiesFactory) {
    var vm = this;

    vm.epaId = parseInt($routeParams.epaId);
    vm.periodId = parseInt($routeParams.periodId);
    vm.employeeId = parseInt($routeParams.employeeId);
    vm.isViewDetail = $routeParams.viewDetail ? true : false;
    vm.evaluatorId = $routeParams.evaluatorId ? parseInt($routeParams.evaluatorId) : null;

    vm.maxStar = 5;
    vm.dataSaved = false;
    vm.isReadonly = false;

    vm.status = {
        isopen: false
    };
    vm.epaStatus = EmployeeEPAModel.epaStatus;

    vm.toggleDropdown = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();
        vm.status.isopen = !vm.status.isopen;
    };

    var calculateScore = function (epaInfo) {
        var totalScore = 0;

        angular.forEach(epaInfo.KPIGroups, function (kpiGroup) {
            angular.forEach(kpiGroup.KPIs, function (kpi) {
                if (kpi.ScoreByEvaluator && parseFloat(kpi.ScoreByEvaluator) > 0) {
                    totalScore += parseFloat(kpi.ScoreByEvaluator);
                }
            });
        });

        epaInfo.TotalScoreByEvaluator = totalScore.toFixed(2);
        vm.finalScore = totalScore + epaInfo.AttendanceScore;
    };

    var checkStatus = function (isEndProcess) {
        if (isEndProcess || vm.isViewDetail) {
            vm.isReadonly = true;
            vm.hoveringOver = null;
            vm.hoveringLeave = null;
        }
        else {
            vm.hoveringOver = function (value, kpi) {
                kpi.PercentByEvaluator = (100 * value) / vm.maxStar;
            };

            vm.hoveringLeave = function (kpiGroup, kpi) {
                kpi.PercentByEvaluator = (100 * kpi.RateByEvaluator) / vm.maxStar;

                if (kpi.PercentByEvaluator > 0) {
                    if (kpiGroup.TotalKPIOptional == 0) {
                        var score = kpi.Weight * (kpi.PercentByEvaluator / 100);

                        kpi.ScoreByEvaluator = (score % 1) == 0 ? score : score.toFixed(2);
                    }
                    else {
                        var weight, totalKPIOptional = angular.copy(kpiGroup.TotalKPIOptional);

                        angular.forEach(kpiGroup.KPIs, function (item) {
                            if (!item.IsRequiredEvaluate && item.RateByEvaluator > 0) {
                                totalKPIOptional -= 1;
                            }
                        });

                        weight = kpiGroup.Weight / (kpiGroup.KPIs.length - totalKPIOptional);

                        angular.forEach(kpiGroup.KPIs, function (item) {
                            if (item.RateByEvaluator > 0) {
                                var score = weight * (item.PercentByEvaluator / 100);

                                item.ScoreByEvaluator = (score % 1) == 0 ? score : score.toFixed(2);
                            }
                        });
                    }

                    calculateScore(vm.epaInfo);
                }
            };
        }
    };

    var validateEPA = function (epaInfo) {
        var isValid = true;

        if (!epaInfo.CommentByEvaluator || epaInfo.CommentByEvaluator.length === 0) {
            isValid = false;
            epaInfo.CommentValid = true;
        }

        angular.forEach(epaInfo.KPIGroups, function (kpiGroup) {
            angular.forEach(kpiGroup.KPIs, function (kpi) {
                if (kpi.IsRequiredEvaluate && (!kpi.ScoreByEvaluator || kpi.ScoreByEvaluator === 0)) {
                    isValid = false;
                    kpi.EvaluateValid = true;
                }

                if (kpi.HasComment && kpi.IsRequiredComment && (!kpi.CommentByEvaluator || kpi.CommentByEvaluator.length === 0)) {
                    isValid = false;
                    kpi.CommentValid = true;
                }
            });
        });

        return isValid;
    };

    vm.cancelRating = function (kpiGroup, kpi) {
        kpi.RateByEvaluator = 0;
        kpi.PercentByEvaluator = 0;
        kpi.ScoreByEvaluator = null;

        if (kpiGroup.TotalKPIOptional > 0) {
            var weight, totalKPIOptional = angular.copy(kpiGroup.TotalKPIOptional);

            angular.forEach(kpiGroup.KPIs, function (item) {
                if (!item.IsRequiredEvaluate && item.RateByEvaluator > 0) {
                    totalKPIOptional -= 1;
                }
            });

            weight = kpiGroup.Weight / (kpiGroup.KPIs.length - totalKPIOptional);

            angular.forEach(kpiGroup.KPIs, function (item) {
                if (item.RateByEvaluator > 0) {
                    var score = weight * (item.PercentByEvaluator / 100);

                    item.ScoreByEvaluator = (score % 1) == 0 ? score : score.toFixed(2);
                }
            });
        }

        calculateScore(vm.epaInfo);
    };

    vm.cancel = function () {
        $window.history.back();
    };

    vm.save = function (epaInfo) {
        if (validateEPA(epaInfo)) {
            EmployeeEPAModel.updateEmployeeEPA(vm.dataSaved, epaInfo);
        }
    };

    vm.initialize = function () {
        EmployeeEPAModel.getEPAInfoByEvaluator(vm.epaId, vm.periodId, vm.employeeId, vm.evaluatorId, vm.isViewDetail).then(function () {
            var data = EmployeeEPAModel.epaInfo;

            vm.employee = data.Employee;
            vm.evaluators = data.Evaluators;
            vm.finalScore = data.TotalScoreBySelf ? (parseFloat(data.TotalScoreBySelf) + data.AttendanceScore) : data.AttendanceScore;
            vm.epaInfo = data;
            checkStatus(vm.epaInfo.IsEndProcess);

            angular.forEach(vm.epaInfo.KPIGroups, function (kpiGroup) {
                if (!EmployeeEPAModel.checkKPIOptional(kpiGroup) && kpiGroup.IsSpreadKPI) {
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

kpiModule.controller('ViewEPAInfoOfMemberController',
['$modalInstance', 'EmployeeEPAModel', 'epaId', 'periodId', 'employeeId',
function ($modalInstance, EmployeeEPAModel, epaId, periodId, employeeId) {
    var vm = this;

    vm.isReadonly = true;
    vm.maxStar = 5;

    EmployeeEPAModel.getEPAInfoByEmployee(epaId, periodId, employeeId).then(function () {
        var data = EmployeeEPAModel.epaInfo;

        vm.employee = data.Employee;
        vm.evaluators = data.Evaluators;
        vm.finalScore = data.TotalScoreBySelf ? (parseFloat(data.TotalScoreBySelf) + data.AttendanceScore) : data.AttendanceScore;
        vm.epaInfo = data;

        angular.forEach(vm.epaInfo.KPIGroups, function (kpiGroup) {
            if (!EmployeeEPAModel.checkKPIOptional(kpiGroup) && kpiGroup.IsSpreadKPI) {
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

    vm.cancel = function () {
        $modalInstance.dismiss("cancel");
    };
}]);
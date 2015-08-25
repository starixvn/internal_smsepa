'use strict';
/*global epaModule*/

epaModule.controller('EmployeeEPAController',
['EmployeeEPAModel', 'CommonModel',
function (EmployeeEPAModel, CommonModel) {
    var vm = this;

    vm.myEPAs = EmployeeEPAModel.myEPAs;
    vm.statusList = CommonModel.statusList;
    vm.targetTypes = CommonModel.targetTypes;

    vm.status = {
        isopen: false
    };
    vm.epaStatus = EmployeeEPAModel.epaStatus;

    vm.filter = {
        targetTypeId: '',
        status: ''
    };

    vm.toggleDropdown = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();
        vm.status.isopen = !vm.status.isopen;
    };

    var getEPAOfCurrentMember = function (targetTypeId, status) {
        EmployeeEPAModel.getEPAOfCurrentMember(targetTypeId, status).then(function () {
            vm.myEPAs = EmployeeEPAModel.myEPAs;
        });
    };

    vm.query = function (filter) {
        getEPAOfCurrentMember(filter.targetTypeId, filter.status);
    };

    vm.updateStatusEPA = function (epa, status) {
        EmployeeEPAModel.openPopupUpdateStatusEPA(epa, epa.EPAId, epa.PeriodId, 2, epa.Employee.EmployeeId, status);
    };

    vm.viewEPADetail = function (epa) {
        vm.viewEPADetailUrl = '/SelfEvaluation/Detail?epaId=' + epa.EPAId + '&periodId=' + epa.PeriodId + '&employeeId=' + epa.Employee.EmployeeId;
    };

    vm.initialize = function () {
        CommonModel.getTargetType().then(function () {
            vm.targetTypes = CommonModel.targetTypes;
        });

        CommonModel.getAllStatus().then(function () {
            vm.statusList = CommonModel.statusList;
        });

        vm.query(vm.filter);
    };

    vm.initialize();
}]);

epaModule.controller('EmployeeEPAInfoController',
['$window', '$translate', '$routeParams', 'EmployeeEPAModel',
function ($window, $translate, $routeParams, EmployeeEPAModel) {
    var vm = this;

    vm.epaId = parseInt($routeParams.epaId);
    vm.periodId = parseInt($routeParams.periodId);
    vm.employeeId = parseInt($routeParams.employeeId);

    vm.maxStar = 5;
    vm.dataSaved = false;
    vm.isReadonly = false;

    vm.epaInfo = EmployeeEPAModel.epaInfo;

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
                if (kpi.ScoreBySelf && parseFloat(kpi.ScoreBySelf) > 0) {
                    totalScore += parseFloat(kpi.ScoreBySelf);
                }
            });
        });

        epaInfo.TotalScoreBySelf = totalScore.toFixed(2);
        vm.finalScore = totalScore + epaInfo.AttendanceScore;
    };

    var checkStatus = function (isEndProcess) {
        if (isEndProcess) {
            vm.isReadonly = true;
            vm.hoveringOver = null;
            vm.hoveringLeave = null;
        }
        else {
            vm.hoveringOver = function (value, kpi) {
                kpi.PercentBySelf = (100 * value) / vm.maxStar;
            };

            vm.hoveringLeave = function (kpiGroup, kpi) {
                kpi.PercentBySelf = (100 * kpi.RateBySelf) / vm.maxStar;

                if (kpi.PercentBySelf > 0) {
                    if (kpiGroup.TotalKPIOptional == 0) {
                        var score = kpi.Weight * (kpi.PercentBySelf / 100);

                        kpi.ScoreBySelf = (score % 1) == 0 ? score : score.toFixed(2);
                    }
                    else {
                        var weight, totalKPIOptional = angular.copy(kpiGroup.TotalKPIOptional);

                        angular.forEach(kpiGroup.KPIs, function (item) {
                            if (!item.IsRequiredEvaluate && item.RateBySelf > 0) {
                                totalKPIOptional -= 1;
                            }
                        });

                        weight = kpiGroup.Weight / (kpiGroup.KPIs.length - totalKPIOptional);

                        angular.forEach(kpiGroup.KPIs, function (item) {
                            if (item.RateBySelf > 0) {
                                var score = weight * (item.PercentBySelf / 100);

                                item.ScoreBySelf = (score % 1) == 0 ? score : score.toFixed(2);
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

        if (!epaInfo.CommentBySelf || epaInfo.CommentBySelf.length === 0) {
            isValid = false;
            epaInfo.CommentValid = true;
        }

        angular.forEach(epaInfo.KPIGroups, function (kpiGroup) {
            angular.forEach(kpiGroup.KPIs, function (kpi) {
                if (kpi.IsRequiredEvaluate && (!kpi.ScoreBySelf || kpi.ScoreBySelf === 0)) {
                    isValid = false;
                    kpi.EvaluateValid = true;
                }

                if (kpi.HasComment && kpi.IsRequiredComment && (!kpi.CommentBySelf || kpi.CommentBySelf.length === 0)) {
                    isValid = false;
                    kpi.CommentValid = true;
                }
            });
        });

        return isValid;
    };

    vm.cancelRating = function (kpiGroup, kpi) {
        kpi.RateBySelf = 0;
        kpi.PercentBySelf = 0;
        kpi.ScoreBySelf = null;

        if (kpiGroup.TotalKPIOptional > 0) {
            var weight, totalKPIOptional = angular.copy(kpiGroup.TotalKPIOptional);

            angular.forEach(kpiGroup.KPIs, function (item) {
                if (!item.IsRequiredEvaluate && item.RateBySelf > 0) {
                    totalKPIOptional -= 1;
                }
            });

            weight = kpiGroup.Weight / (kpiGroup.KPIs.length - totalKPIOptional);

            angular.forEach(kpiGroup.KPIs, function (item) {
                if (item.RateBySelf > 0) {
                    var score = weight * (item.PercentBySelf / 100);

                    item.ScoreBySelf = (score % 1) == 0 ? score : score.toFixed(2);
                }
            });
        }

        calculateScore(vm.epaInfo);
    };

    vm.inputScore = function (kpi) {
        kpi.RateBySelf = 0;
        kpi.PercentBySelf = 0;

        if (!EmployeeEPAModel.validateWeightInput(kpi.ScoreBySelf, kpi.Weight, vm.weightInputValidMessage)) {
            kpi.ScoreBySelf = null;
        }
        else {
            calculateScore(vm, vm.epaInfo);
        }
    };

    vm.updateStatusEPA = function (status) {
        EmployeeEPAModel.openPopupUpdateStatusEPA(vm.epaInfo, vm.epaId, vm.periodId, 2, vm.employeeId, status);
    };

    vm.save = function (epaInfo) {
        if (validateEPA(epaInfo)) {
            EmployeeEPAModel.updateEmployeeEPA(vm.dataSaved, epaInfo);
        }
    };

    vm.cancel = function () {
        $window.history.back();
    };

    vm.initialize = function () {
        $translate('WeightInputValid').then(function (message) {
            vm.weightInputValidMessage = message;
        });

        EmployeeEPAModel.getEPAInfoByEmployee(vm.epaId, vm.periodId, vm.employeeId).then(function () {
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
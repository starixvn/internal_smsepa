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
        EmployeeEPAModel.openPopupUpdateStatusEPA(epa, epa.EPAId, 2, epa.Employee.EmployeeId, status);
    };

    vm.viewEPADetail = function (epa) {
        vm.viewEPADetailUrl = '/SelfEvaluation/Detail?epaId=' + epa.EPAId + '&employeeId=' + epa.Employee.EmployeeId;
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

        angular.forEach(epaInfo.EPAInfoBySelf, function (kpiGroup) {
            angular.forEach(kpiGroup.KPIGroup.KPIs, function (kpi) {
                if (kpi.Score && parseFloat(kpi.Score) > 0) {
                    totalScore += parseFloat(kpi.Score);
                }
            });
        });

        epaInfo.ProcessBySelf.TotalScore = totalScore.toFixed(2);
        vm.finalScore = totalScore + epaInfo.AttendanceScore;
    };

    var checkStatus = function (epaInfo) {
        if (epaInfo.ProcessBySelf.IsEndProcess) {
            vm.isReadonly = true;
            vm.hoveringOver = null;
            vm.hoveringLeave = null;
        }
        else {
            vm.hoveringOver = function (value, kpi) {
                kpi.Percent = (100 * value) / vm.maxStar;
            };

            vm.hoveringLeave = function (kpiGroup, kpi) {
                kpi.Percent = (100 * kpi.Rate) / vm.maxStar;

                if (kpi.Percent > 0) {
                    if (kpiGroup.TotalKPIOptional == 0) {
                        var score = kpi.Weight * (kpi.Percent / 100);

                        kpi.Score = (score % 1) == 0 ? score : score.toFixed(2);
                    }
                    else {
                        var weight, totalKPIOptional = angular.copy(kpiGroup.TotalKPIOptional);

                        angular.forEach(kpiGroup.KPIs, function (item) {
                            if (!item.IsRequiredEvaluate && item.Rate > 0) {
                                totalKPIOptional -= 1;
                            }
                        });

                        weight = kpiGroup.Weight / (kpiGroup.KPIs.length - totalKPIOptional);

                        angular.forEach(kpiGroup.KPIs, function (item) {
                            if (item.Rate > 0) {
                                var score = weight * (item.Percent / 100);

                                item.Score = (score % 1) == 0 ? score : score.toFixed(2);
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

        if (!epaInfo.ProcessBySelf.Comment || epaInfo.ProcessBySelf.Comment.length === 0) {
            isValid = false;
            epaInfo.ProcessBySelf.CommentValid = true;
        }
        else {
            epaInfo.ProcessBySelf.CommentValid = false;
        }

        angular.forEach(epaInfo.EPAInfoBySelf, function (kpiGroup) {
            angular.forEach(kpiGroup.KPIGroup.KPIs, function (kpi) {
                if (kpi.IsRequiredEvaluate && (!kpi.Score || kpi.Score === 0)) {
                    isValid = false;
                    kpi.EvaluateValid = true;
                }
                else {
                    kpi.EvaluateValid = false;
                }

                if (kpi.HasComment && kpi.IsRequiredComment && (!kpi.Comment || kpi.Comment.length === 0)) {
                    isValid = false;
                    kpi.CommentValid = true;
                }
                else {
                    kpi.CommentValid = false;
                }
            });
        });

        return isValid;
    };

    vm.cancelRating = function (kpiGroup, kpi) {
        kpi.Rate = 0;
        kpi.Percent = 0;
        kpi.Score = null;

        if (kpiGroup.TotalKPIOptional > 0) {
            var weight, totalKPIOptional = angular.copy(kpiGroup.TotalKPIOptional);

            angular.forEach(kpiGroup.KPIs, function (item) {
                if (!item.IsRequiredEvaluate && item.Rate > 0) {
                    totalKPIOptional -= 1;
                }
            });

            weight = kpiGroup.Weight / (kpiGroup.KPIs.length - totalKPIOptional);

            angular.forEach(kpiGroup.KPIs, function (item) {
                if (item.Rate > 0) {
                    var score = weight * (item.Percent / 100);

                    item.Score = (score % 1) == 0 ? score : score.toFixed(2);
                }
            });
        }

        calculateScore(vm.epaInfo);
    };

    vm.inputScore = function (kpi) {
        kpi.Rate = 0;
        kpi.Percent = 0;

        if (!EmployeeEPAModel.validateWeightInput(kpi.Score, kpi.Weight, vm.weightInputValidMessage)) {
            kpi.Score = null;
        }
        else {
            calculateScore(vm, vm.epaInfo);
        }
    };

    vm.updateStatusEPA = function (status) {
        EmployeeEPAModel.openPopupUpdateStatusEPA(vm.epaInfo, vm.epaId, 2, vm.employeeId, status);
    };

    vm.save = function (epaInfo) {
        if (validateEPA(epaInfo)) {
            EmployeeEPAModel.updateEPAScore(vm.dataSaved, epaInfo);
        }
    };

    vm.cancel = function () {
        $window.history.back();
    };

    vm.initialize = function () {
        $translate('WeightInputValid').then(function (message) {
            vm.weightInputValidMessage = message;
        });

        EmployeeEPAModel.getEPAInfoByEmployee(vm.epaId, vm.employeeId).then(function () {
            var data = EmployeeEPAModel.epaInfo;
            vm.finalScore = data.ProcessBySelf.TotalScore ? (parseFloat(data.ProcessBySelf.TotalScore) + data.AttendanceScore) : data.AttendanceScore;
            vm.epaInfo = data;
            checkStatus(vm.epaInfo);

            angular.forEach(vm.epaInfo.EPAInfoBySelf, function (kpiGroup) {
                if (!EmployeeEPAModel.checkKPIOptional(kpiGroup.KPIGroup) && kpiGroup.KPIGroup.IsSpreadKPI) {
                    var weight = kpiGroup.KPIGroup.Weight / kpiGroup.KPIGroup.KPIs.length;

                    angular.forEach(kpiGroup.KPIGroup.KPIs, function (kpi) {
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
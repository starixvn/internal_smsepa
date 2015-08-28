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
        targetTypeId: '',
        groupId: '',
        employeeIds: '',
        status: ''
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
        EmployeeEPAModel.openPopupUpdateStatusEPA(epa.ProcessByEvaluators[0], epa.EPAId, 3, epa.Employee.EmployeeId, status);
    };

    var getEPAByEvaluator = function (targetTypeId, groupId, employeeIds, status) {
        EmployeeEPAModel.getEPAByEvaluator(targetTypeId, groupId, employeeIds, status).then(function () {
            vm.memberEPAs = EmployeeEPAModel.memberEPAs;
        });
    };

    vm.query = function (filter) {
        filter.employeeIds = utilitiesFactory.getSelectedData(vm.members, 'EmployeeId');
        getEPAByEvaluator(filter.targetTypeId, filter.groupId, filter.employeeIds, filter.status);
    };

    vm.viewEPAInfoOfMember = function (memberEPA) {
        EmployeeEPAModel.viewEPAInfoOfMember(memberEPA.EPAId, memberEPA.Employee.EmployeeId);
    };

    vm.viewEPADetail = function (memberEPA) {
        vm.viewEPADetailUrl = '/Evaluator/Detail?epaId=' + memberEPA.EPAId + '&employeeId=' + memberEPA.Employee.EmployeeId;
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

epaModule.controller('MemberEPAInfoOfEvaluatorController',
['$window', '$translate', '$routeParams', 'EmployeeEPAModel', 'CommonModel', 'utilitiesFactory',
function ($window, $translate, $routeParams, EmployeeEPAModel, CommonModel, utilitiesFactory) {
    var vm = this;

    vm.epaId = parseInt($routeParams.epaId);
    vm.periodId = parseInt($routeParams.periodId);
    vm.employeeId = parseInt($routeParams.employeeId);
    vm.evaluatorId = $routeParams.evaluatorId ? parseInt($routeParams.evaluatorId) : null;
    vm.isViewDetail = $routeParams.evaluatorId ? true : false;

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
                if (kpi.EvaluateByEvaluators[0].Score && parseFloat(kpi.EvaluateByEvaluators[0].Score) > 0) {
                    totalScore += parseFloat(kpi.EvaluateByEvaluators[0].Score);
                }
            });
        });

        epaInfo.ProcessByEvaluators[0].TotalScore = totalScore.toFixed(2);
        vm.finalScore = totalScore + epaInfo.AttendanceScore;
    };

    var checkStatus = function (epaInfo) {
        if (epaInfo.ProcessByEvaluators[0].IsEndProcess || vm.isViewDetail) {
            vm.isReadonly = true;
            vm.hoveringOver = null;
            vm.hoveringLeave = null;
        }
        else {
            vm.hoveringOver = function (value, kpi) {
                kpi.EvaluateByEvaluators[0].Percent = (100 * value) / vm.maxStar;
            };

            vm.hoveringLeave = function (kpiGroup, kpi) {
                kpi.EvaluateByEvaluators[0].Percent = (100 * kpi.EvaluateByEvaluators[0].Rate) / vm.maxStar;

                if (kpi.EvaluateByEvaluators[0].Percent > 0) {
                    if (kpiGroup.TotalKPIOptional == 0) {
                        var score = kpi.Weight * (kpi.EvaluateByEvaluators[0].Percent / 100);

                        kpi.EvaluateByEvaluators[0].Score = (score % 1) == 0 ? score : score.toFixed(2);
                    }
                    else {
                        var weight, totalKPIOptional = angular.copy(kpiGroup.TotalKPIOptional);

                        angular.forEach(kpiGroup.KPIs, function (item) {
                            if (!item.IsRequiredEvaluate && item.EvaluateByEvaluators[0].Rate > 0) {
                                totalKPIOptional -= 1;
                            }
                        });

                        weight = kpiGroup.Weight / (kpiGroup.KPIs.length - totalKPIOptional);

                        angular.forEach(kpiGroup.KPIs, function (item) {
                            if (item.EvaluateByEvaluators[0].Rate > 0) {
                                var score = weight * (item.EvaluateByEvaluators[0].Percent / 100);

                                item.EvaluateByEvaluators[0].Score = (score % 1) == 0 ? score : score.toFixed(2);
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

        if (!epaInfo.ProcessByEvaluators[0].Comment || epaInfo.ProcessByEvaluators[0].Comment.length === 0) {
            isValid = false;
            epaInfo.ProcessByEvaluators[0].CommentValid = true;
        }
        else {
            epaInfo.ProcessByEvaluators[0].CommentValid = false;
        }

        angular.forEach(epaInfo.KPIGroups, function (kpiGroup) {
            angular.forEach(kpiGroup.KPIs, function (kpi) {
                if (kpi.IsRequiredEvaluate && (!kpi.EvaluateByEvaluators[0].Score || kpi.EvaluateByEvaluators[0].Score === 0)) {
                    isValid = false;
                    kpi.EvaluateByEvaluators[0].EvaluateValid = true;
                }
                else {
                    kpi.EvaluateByEvaluators[0].EvaluateValid = false;
                }

                if (kpi.HasComment && kpi.IsRequiredComment && (!kpi.EvaluateByEvaluators[0].Comment || kpi.EvaluateByEvaluators[0].Comment.length === 0)) {
                    isValid = false;
                    kpi.EvaluateByEvaluators[0].CommentValid = true;
                }
                else {
                    kpi.EvaluateByEvaluators[0].CommentValid = true;
                }
            });
        });

        return isValid;
    };

    vm.updateStatusEPA = function (epa, status) {
        EmployeeEPAModel.openPopupUpdateStatusEPA(epa.ProcessByEvaluators[0], epa.EPAId, 3, epa.Employee.EmployeeId, status);
    };

    vm.inputScore = function (kpi) {
        kpi.EvaluateByEvaluators[0].Rate = 0;
        kpi.EvaluateByEvaluators[0].Percent = null;

        if (!EmployeeEPAModel.validateWeightInput(kpi.EvaluateByEvaluators[0].Score, kpi.Weight, vm.weightInputValidMessage)) {
            kpi.EvaluateByEvaluators[0].Score = null;
        }
        else {
            calculateScore(vm.epaInfo);
        }
    };

    vm.cancelRating = function (kpiGroup, kpi) {
        kpi.EvaluateByEvaluators[0].Rate = 0;
        kpi.EvaluateByEvaluators[0].Percent = 0;
        kpi.EvaluateByEvaluators[0].Score = null;

        if (kpiGroup.TotalKPIOptional > 0) {
            var weight, totalKPIOptional = angular.copy(kpiGroup.TotalKPIOptional);

            angular.forEach(kpiGroup.KPIs, function (item) {
                if (!item.IsRequiredEvaluate && item.EvaluateByEvaluators[0].Rate > 0) {
                    totalKPIOptional -= 1;
                }
            });

            weight = kpiGroup.Weight / (kpiGroup.KPIs.length - totalKPIOptional);

            angular.forEach(kpiGroup.KPIs, function (item) {
                if (item.EvaluateByEvaluators[0].Rate > 0) {
                    var score = weight * (item.EvaluateByEvaluators[0].Percent / 100);

                    item.EvaluateByEvaluators[0].Score = (score % 1) == 0 ? score : score.toFixed(2);
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
            EmployeeEPAModel.updateEPAScoreByEvaluator(vm.dataSaved, epaInfo);
        }
    };

    vm.initialize = function () {
        $translate('WeightInputValid').then(function (message) {
            vm.weightInputValidMessage = message;
        });

        EmployeeEPAModel.getEPAInfoByEvaluator(vm.epaId, vm.employeeId, vm.evaluatorId).then(function () {
            var data = EmployeeEPAModel.epaInfo;

            vm.finalScore = data.ProcessByEvaluators[0].TotalScore ? (parseFloat(data.ProcessByEvaluators[0].TotalScore) + data.AttendanceScore) : data.AttendanceScore;
            vm.epaInfo = data;
            checkStatus(vm.epaInfo);

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
        vm.finalScore = data.ProcessBySelf.TotalScore ? (parseFloat(data.ProcessBySelf.TotalScore) + data.AttendanceScore) : data.AttendanceScore;
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
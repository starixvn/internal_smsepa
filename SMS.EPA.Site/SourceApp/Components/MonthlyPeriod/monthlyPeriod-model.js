'use strict';
/*global monthlyPeriodModule*/

monthlyPeriodModule.factory('MonthlyPeriodModel',
['$modal', 'monthlyPeriodFactory', 'errorFactory', 'notificationFactory', 'utilitiesFactory',
function ($modal, monthlyPeriodFactory, errorFactory, notificationFactory, utilitiesFactory) {
    var monthlyPeriodModel = {};

    monthlyPeriodModel.periods = [];
    monthlyPeriodModel.period = {};
    monthlyPeriodModel.processSettings = [];
    monthlyPeriodModel.permission = 0;

    monthlyPeriodModel.getPermissionOnMonthlyPeriod = function () {
        return monthlyPeriodFactory.getPermissionOnMonthlyPeriod()
        .success(function (permisson) {
            monthlyPeriodModel.permission = parseInt(permisson);
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    monthlyPeriodModel.openPopupStartDate = function (event, vm) {
        event.preventDefault();
        event.stopPropagation();

        vm.calendar.startDateOpened = true;
        vm.calendar.endDateOpened = false;
    };

    monthlyPeriodModel.openPopupEndDate = function (event, vm) {
        event.preventDefault();
        event.stopPropagation();

        vm.calendar.startDateOpened = false;
        vm.calendar.endDateOpened = true;
    };

    monthlyPeriodModel.getMonthlyPeriod = function (fromDate, toDate) {
        return monthlyPeriodFactory.getMonthlyPeriod(fromDate, toDate)
        .success(function (periods) {
            monthlyPeriodModel.periods = periods;
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    monthlyPeriodModel.getMonthlyPeriodById = function (periodId) {
        return monthlyPeriodFactory.getMonthlyPeriodById(periodId)
        .success(function (period) {
            monthlyPeriodModel.period = period;
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    monthlyPeriodModel.openPopupCreateMonthlyPeriod = function (vm) {
        vm.modalInstance = $modal.open({
            templateUrl: 'SourceApp/Components/MonthlyPeriod/Templates/create.html',
            controller: "CreateMonthlyPeriodController as createMonthlyPeriodCtrl",
            backdrop: 'static',
            size: 'lg',
            windowClass: "modal-confirm-center"
        });

        return vm.modalInstance.result;
    };

    monthlyPeriodModel.validateMonthlyPeriod = function (period) {
        if (!period) {
            return false;
        }
        else {
            if (!period.Name) {
                notificationFactory.error("NameRequired");
                return false;
            }
            else if (!period.StartDate) {
                notificationFactory.error("StartDatePeriodRequired");
                return false;
            } else if (!period.EndDate) {
                notificationFactory.error("EndDatePeriodRequired");
                return false;
            }
            else if (new Date(period.StartDate).setHours(0, 0, 0, 0) > new Date(period.EndDate).setHours(0, 0, 0, 0)) {
                notificationFactory.error("StartDateGreaterThanEndDateWarning");
                return false;
            }
        }

        return true;
    };

    monthlyPeriodModel.createMonthlyPeriod = function (period, modalInstance) {
        return monthlyPeriodFactory.createMonthlyPeriod(period)
        .success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("CreatedSuccessfully");
                modalInstance.close();
            }
            else {
                notificationFactory.error("", response.Message);
            }
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    monthlyPeriodModel.updateBasicInfoMonthlyPeriod = function (period) {
        return monthlyPeriodFactory.updateBasicInfoMonthlyPeriod(period).success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("UpdatedSuccessfully");
            }
            else {
                notificationFactory.error("", response.Message);
            }
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    monthlyPeriodModel.getProcessSettingByPeriodId = function (periodId) {
        return monthlyPeriodFactory.getProcessSettingByPeriodId(periodId)
        .success(function (processSettings) {
            monthlyPeriodModel.processSettings = processSettings;
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    monthlyPeriodModel.updateProcessSettingMonthlyPeriod = function (processSetting) {
        return monthlyPeriodFactory.updateProcessSettingMonthlyPeriod(processSetting)
        .success(function (response) {
            if (response.IsSuccessful) {
                notificationFactory.success("UpdatedSuccessfully");
            }
            else {
                notificationFactory.error("UpdatedUnsuccessfully");
            }
        })
        .error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    monthlyPeriodModel.openPopupDeleteMonthlyPeriod = function (vm, period) {
        var modalInstance = $modal.open({
            templateUrl: '/SourceApp/Shared/Templates/ModalConfirmContent.html',
            controller: "ModalConfirmController as modalConfirmCtrl",
            backdrop: 'static',
            resolve: {
                name: function () {
                    return period.Name;
                },
                action: function () {
                    return 'delete';
                }
            },
            windowClass: "modal-confirm-center"
        });

        modalInstance.result.then(function () {
            monthlyPeriodFactory.deleteMonthlyPeriod(period.PeriodId).success(function (response) {
                if (response.IsSuccessful) {
                    var index = vm.periods.indexOf(period);
                    vm.periods.splice(index, 1);
                    utilitiesFactory.searchOnPage(vm, vm.periods);
                    notificationFactory.success("DeletedSuccessfully");
                }
                else {
                    notificationFactory.error("", response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        })['finally'](function () {
            modalInstance = '';
        });
    };

    return monthlyPeriodModel;
}]);
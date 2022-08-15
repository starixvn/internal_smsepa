(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('jobLevelStandardModule').factory('JobLevelStandardPeriodModel', periodModel);
    periodModel.$inject = ['$uibModal', 'jobLevelStandardPeriodFactory', 'errorFactory', 'notificationFactory', 'utilitiesFactory'];

    function periodModel($uibModal, periodFactory, errorFactory, notificationFactory, utilitiesFactory) {
        var model = {};

        model.periods = [];
        model.period = {};

        model.calendar = {
            minDate: new Date(2010, 0, 1),
            fromDateEvaluatorOpened: false,
            toDateEvaluatorOpened: false,
            fromDateEmployeeOpened: false,
            toDateEmployeeOpened: false,
            fromDateManagementOpened: false,
            toDateManagementOpened: false
        };

        function resetCalendar() {
            model.calendar.fromDateEvaluatorOpened = false;
            model.calendar.toDateEvaluatorOpened = false;
            model.calendar.fromDateEmployeeOpened = false;
            model.calendar.toDateEmployeeOpened = false;
            model.calendar.fromDateManagementOpened = false;
            model.calendar.toDateManagementOpened = false;
        }

        model.openFromDate = function (event, processId) {
            event.preventDefault();
            event.stopPropagation();

            resetCalendar();

            switch (processId) {
                case utilitiesFactory.jobLevelStandardProcess.Evaluator:
                    model.calendar.fromDateEvaluatorOpened = true;
                    break;

                case utilitiesFactory.jobLevelStandardProcess.Employee:
                    model.calendar.fromDateEmployeeOpened = true;
                    break;

                case utilitiesFactory.jobLevelStandardProcess.Management:
                    model.calendar.fromDateManagementOpened = true;
                    break;
            }
        };

        model.openToDate = function (event, processId) {
            event.preventDefault();
            event.stopPropagation();

            resetCalendar();

            switch (processId) {
                case utilitiesFactory.jobLevelStandardProcess.Evaluator:
                    model.calendar.toDateEvaluatorOpened = true;
                    break;

                case utilitiesFactory.jobLevelStandardProcess.Employee:
                    model.calendar.toDateEmployeeOpened = true;
                    break;

                case utilitiesFactory.jobLevelStandardProcess.Management:
                    model.calendar.toDateManagementOpened = true;
                    break;
            }
        };

        model.getPeriods = function (isActive) {
            return periodFactory.getPeriods(isActive).success(function (periods) {
                model.periods = periods;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.getPeriodById = function (periodId) {
            return periodFactory.getPeriodById(periodId).success(function (period) {
                model.period = period;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.openPopupCreatePeriod = function (vm) {
            vm.modalInstance = $uibModal.open({
                templateUrl: 'SourceApp/Components/JobLevelStandard/Period/Templates/create.html',
                controller: 'CreateJobLevelStandardPeriodController as createPeriodCtrl',
                backdrop: 'static',
                windowClass: "modal-confirm-center job-level-standard-period-modal"
            });

            return vm.modalInstance.result;
        };

        model.validatePeriod = function (period) {
            var isValid = true;
            if (!period) {
                isValid = false;
            } else {
                if (!period.Name) {
                    notificationFactory.error('NameRequired');
                    isValid = false;
                } else if (!period.Settings[0].FromDate) {
                    notificationFactory.error('', 'Evaluator From Date is required!');
                    isValid = false;
                } else if (!period.Settings[0].ToDate) {
                    notificationFactory.error('', 'Evaluator To Date is required!');
                    isValid = false;
                } else if (new Date(period.Settings[0].FromDate).setHours(0, 0, 0, 0) > new Date(period.Settings[0].ToDate).setHours(0, 0, 0, 0)) {
                    notificationFactory.error('', 'Evaluator From Date must be less than or equal to Evaluator To Date!');
                    isValid = false;
                } else if (!period.Settings[1].FromDate) {
                    notificationFactory.error('', 'Employee From Date is required!');
                    isValid = false;
                } else if (!period.Settings[1].ToDate) {
                    notificationFactory.error('', 'Employee To Date is required!');
                    isValid = false;
                } else if (new Date(period.Settings[1].FromDate).setHours(0, 0, 0, 0) > new Date(period.Settings[1].ToDate).setHours(0, 0, 0, 0)) {
                    notificationFactory.error('', 'Employee From Date must be less than or equal to Employee To Date!');
                    isValid = false;
                } else if (new Date(period.Settings[0].ToDate).setHours(0, 0, 0, 0) >= new Date(period.Settings[1].FromDate).setHours(0, 0, 0, 0)) {
                    notificationFactory.error('', 'Evaluator To Date must be less than Employee From Date!');
                    isValid = false;
                } else if (!period.Settings[2].FromDate) {
                    notificationFactory.error('', 'Management From Date is required!');
                    isValid = false;
                } else if (!period.Settings[2].ToDate) {
                    notificationFactory.error('', 'Management To Date is required!');
                    isValid = false;
                } else if (new Date(period.Settings[2].FromDate).setHours(0, 0, 0, 0) > new Date(period.Settings[2].ToDate).setHours(0, 0, 0, 0)) {
                    notificationFactory.error('', 'Management From Date must be less than or equal to Management To Date!');
                    isValid = false;
                } else if (new Date(period.Settings[1].ToDate).setHours(0, 0, 0, 0) >= new Date(period.Settings[2].FromDate).setHours(0, 0, 0, 0)) {
                    notificationFactory.error('', 'Employee To Date must be less than Management From Date!');
                    isValid = false;
                }
            }

            return isValid;
        };

        model.createPeriod = function (period, modalInstance) {
            return periodFactory.createPeriod(period).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success('CreatedSuccessfully');
                    modalInstance.close();
                } else {
                    notificationFactory.error('', response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.editPeriod = function (period) {
            return periodFactory.editPeriod(period).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success('UpdatedSuccessfully');
                } else {
                    notificationFactory.error('', response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.openPopupEditStatusPeriod = function (period) {
            period.IsActive = !period.IsActive;

            var action = period.IsActive ? 'activate' : 'deactivate',
                newPeriod = {
                PeriodId: period.PeriodId,
                IsActive: period.IsActive
            };

            var modalInstance = $uibModal.open({
                templateUrl: '/SourceApp/Shared/Templates/ModalConfirmContent.html',
                controller: 'ModalConfirmController as modalConfirmCtrl',
                backdrop: 'static',
                resolve: {
                    name: function () {
                        return period.Name;
                    },
                    action: function () {
                        return action;
                    }
                },
                windowClass: 'modal-confirm-center'
            });

            var keyMessage = '';
            modalInstance.result.then(function () {
                periodFactory.editStatusPeriod(newPeriod).success(function (response) {
                    if (response.IsSuccessful) {
                        keyMessage = action === 'activate' ? 'ActivationSuccess' : 'DeactivationSuccess';
                        notificationFactory.success(keyMessage);
                    } else {
                        keyMessage = action === 'activate' ? 'ActivationFail' : 'DeactivationFail';
                        period.IsActive = !newPeriod.IsActive;
                        notificationFactory.error(keyMessage);
                    }
                }).error(function (response, status) {
                    if (status === 405) {
                        errorFactory.errorHandle(status);
                    } else {
                        keyMessage = action === 'activate' ? 'ActivationFail' : 'DeactivationFail';
                        period.IsActive = !newPeriod.IsActive;
                        notificationFactory.error(keyMessage);
                    }
                });
            }, function () {
                period.IsActive = !newPeriod.IsActive;
            })['finally'](function () {
                modalInstance = '';
            });
        };

        model.openPopupDeletePeriod = function (vm, period) {
            var modalInstance = $uibModal.open({
                templateUrl: '/SourceApp/Shared/Templates/ModalConfirmContent.html',
                controller: 'ModalConfirmController as modalConfirmCtrl',
                backdrop: 'static',
                resolve: {
                    name: function () {
                        return period.Name;
                    },
                    action: function () {
                        return 'delete';
                    }
                },
                windowClass: 'modal-confirm-center'
            });

            modalInstance.result.then(function () {
                periodFactory.deletePeriod(period.PeriodId).success(function (response) {
                    if (response.IsSuccessful) {
                        var index = vm.periods.indexOf(period);
                        vm.periods.splice(index, 1);
                        utilitiesFactory.searchOnPage(vm, vm.periods);
                        notificationFactory.success('DeletedSuccessfully');
                    } else {
                        notificationFactory.error('', response.Message);
                    }
                }).error(function (response, status) {
                    errorFactory.errorHandle(status);
                });
            })['finally'](function () {
                modalInstance = '';
            });
        };

        return model;
    }
})(angular);
(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('jobLevelStandardModule').factory('JobLevelStandardPeriodModel', periodModel);
    periodModel.$inject = ['$uibModal', 'jobLevelStandardPeriodFactory', 'errorFactory', 'notificationFactory', 'utilitiesFactory'];

    function periodModel($uibModal, periodFactory, errorFactory, notificationFactory, utilitiesFactory) {
        let model = {};

        model.permission = 0;
        model.periods = [];
        model.period = {};
        model.jobLevelSettings = [];
        model.sectionJobLevelSettings = [];
        model.employeeSettings = [];

        model.calendar = {
            minDate: new Date(2010, 0, 1),
            fromDateEvaluatorOpened: false,
            toDateEvaluatorOpened: false,
            fromDateEmployeeOpened: false,
            toDateEmployeeOpened: false,
            fromDateManagementOpened: false,
            toDateManagementOpened: false
        };

        function resetCalendar(calendar) {
            calendar.fromDateEvaluatorOpened = false;
            calendar.toDateEvaluatorOpened = false;
            calendar.fromDateEmployeeOpened = false;
            calendar.toDateEmployeeOpened = false;
            calendar.fromDateManagementOpened = false;
            calendar.toDateManagementOpened = false;
        }

        model.openFromDate = function (event, calendar, processId) {
            event.preventDefault();
            event.stopPropagation();

            resetCalendar(calendar);

            switch (processId) {
                case utilitiesFactory.jobLevelAssessmentProcess.Evaluator:
                    calendar.fromDateEvaluatorOpened = true;
                    break;

                case utilitiesFactory.jobLevelAssessmentProcess.Employee:
                    calendar.fromDateEmployeeOpened = true;
                    break;

                case utilitiesFactory.jobLevelAssessmentProcess.Management:
                    calendar.fromDateManagementOpened = true;
                    break;
            }
        };

        model.openToDate = function (event, calendar, processId) {
            event.preventDefault();
            event.stopPropagation();

            resetCalendar(calendar);

            switch (processId) {
                case utilitiesFactory.jobLevelAssessmentProcess.Evaluator:
                    calendar.toDateEvaluatorOpened = true;
                    break;

                case utilitiesFactory.jobLevelAssessmentProcess.Employee:
                    calendar.toDateEmployeeOpened = true;
                    break;

                case utilitiesFactory.jobLevelAssessmentProcess.Management:
                    calendar.toDateManagementOpened = true;
                    break;
            }
        };

        model.getPermission = function () {
            return periodFactory.getPermission().success(function (permisson) {
                model.permission = permisson;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
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
                windowClass: 'modal-confirm-center job-level-standard-period-modal'
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
                    notificationFactory.error('', 'Evaluator / Reviewer From Date is required!');
                    isValid = false;
                } else if (!period.Settings[0].ToDate) {
                    notificationFactory.error('', 'Evaluator / Reviewer To Date is required!');
                    isValid = false;
                } else if (new Date(period.Settings[0].FromDate).setHours(0, 0, 0, 0) > new Date(period.Settings[0].ToDate).setHours(0, 0, 0, 0)) {
                    notificationFactory.error('', 'Evaluator / Reviewer From Date must be less than or equal to Evaluator / Reviewer To Date!');
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
                    notificationFactory.error('', 'Evaluator / Reviewer To Date must be less than Employee From Date!');
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

            let action = period.IsActive ? 'activate' : 'deactivate',
                params = {
                PeriodId: period.PeriodId,
                IsActive: period.IsActive
            };

            let modalInstance = $uibModal.open({
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

            modalInstance.result.then(function () {
                periodFactory.editStatusPeriod(params).success(function (response) {
                    if (response.IsSuccessful) {
                        notificationFactory.success('UpdatedSuccessfully');
                    } else {
                        period.IsActive = !params.IsActive;
                        notificationFactory.error('', response.Message);
                    }
                }).error(function (response, status) {
                    if (status === 405) {
                        errorFactory.errorHandle(status);
                    } else {
                        period.IsActive = !params.IsActive;
                        notificationFactory.success('UpdatedSuccessfully');
                    }
                });
            }, function () {
                period.IsActive = !params.IsActive;
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

        model.getEmployeeSettings = function (params) {
            return periodFactory.getEmployeeSettings(params).success(function (employeeSettings) {
                model.employeeSettings = employeeSettings;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.openPopupAddEmployeeSetting = function (vm) {
            vm.modalInstance = $uibModal.open({
                templateUrl: 'SourceApp/Components/JobLevelStandard/Period/Templates/addEmployeeSetting.html',
                controller: 'AddEmployeeSettingController as addEmployeeSettingCtrl',
                backdrop: 'static',
                resolve: {
                    periodId: function () {
                        return vm.filter.periodId;
                    },
                    orgChart: function () {
                        return vm.orgChart;
                    }
                },
                windowClass: 'modal-confirm-center add-employee-setting-modal'
            });

            return vm.modalInstance.result;
        };

        model.validateAddEmployeeSetting = function (params) {
            let isValid = true;
            if (params.employeeId === null && params.teamIds === '') {
                isValid = false;

                notificationFactory.error('', 'Please input employee or select at least a team!');
            }

            return isValid;
        };

        model.addEmployeeSetting = function (params, modalInstance) {
            return periodFactory.addEmployeeSetting(params).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success('', 'Add employee successfully!');
                    modalInstance.close();
                } else {
                    notificationFactory.error('', response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.openPopupEditEmployeeSetting = function (vm, employee) {
            vm.modalInstance = $uibModal.open({
                templateUrl: 'SourceApp/Components/JobLevelStandard/Period/Templates/editEmployeeSetting.html',
                controller: 'EditEmployeeSettingController as editEmployeeSettingCtrl',
                backdrop: 'static',
                resolve: {
                    employee: function () {
                        return angular.copy(employee);
                    }
                },
                windowClass: 'modal-confirm-center edit-employee-setting-modal'
            });

            return vm.modalInstance.result;
        };

        model.validateEditEmployeeSetting = function (employeeSettings) {
            let fromDate, toDate;

            if (employeeSettings.evaluator.ids && employeeSettings.evaluator.ids.length > 0) {
                fromDate = new Date(employeeSettings.evaluator.fromDate).setHours(0, 0, 0, 0);
                toDate = new Date(employeeSettings.evaluator.toDate).setHours(0, 0, 0, 0);

                if (fromDate > toDate) {
                    notificationFactory.error('', 'Evaluator From Date must be less than or equal to Evaluator To Date!');
                    return false;
                }

                fromDate = new Date(employeeSettings.evaluator.toDate).setHours(0, 0, 0, 0);
                toDate = new Date(employeeSettings.employee.fromDate).setHours(0, 0, 0, 0);

                if (fromDate >= toDate) {
                    notificationFactory.error('', 'Evaluator To Date must be less than Employee From Date!');
                    return false;
                }
            }

            if (employeeSettings.employee.ids && employeeSettings.employee.ids.length > 0) {
                fromDate = new Date(employeeSettings.employee.fromDate).setHours(0, 0, 0, 0);
                toDate = new Date(employeeSettings.employee.toDate).setHours(0, 0, 0, 0);

                if (fromDate > toDate) {
                    notificationFactory.error('', 'Employee From Date must be less than or equal to Employee To Date!');
                    return false;
                }

                fromDate = new Date(employeeSettings.employee.toDate).setHours(0, 0, 0, 0);
                toDate = new Date(employeeSettings.management.fromDate).setHours(0, 0, 0, 0);

                if (fromDate >= toDate) {
                    notificationFactory.error('', 'Employee To Date must be less than Management From Date!');
                    return false;
                }
            }

            if (employeeSettings.management.ids && employeeSettings.management.ids.length > 0) {
                fromDate = new Date(employeeSettings.management.fromDate).setHours(0, 0, 0, 0);
                toDate = new Date(employeeSettings.management.toDate).setHours(0, 0, 0, 0);

                if (fromDate > toDate) {
                    notificationFactory.error('', 'Management From Date must be less than or equal to Management To Date!');
                    return false;
                }
            }

            return true;
        };

        model.editEmployeeSetting = function (employeeSettings) {
            return periodFactory.editEmployeeSetting(employeeSettings).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success('UpdatedSuccessfully');
                } else {
                    notificationFactory.error('', response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.openPopupEditStatusEmployeeSetting = function (vm, periodId, employee) {
            employee.IsExcluded = !employee.IsExcluded;

            let action = employee.IsExcluded ? 'exclude' : 'include',
                params = {
                    PeriodId: periodId,
                    EmployeeId: employee.Employee.EmployeeId,
                    IsExcluded: employee.IsExcluded
                };

            let modalInstance = $uibModal.open({
                templateUrl: '/SourceApp/Shared/Templates/ModalConfirmContent.html',
                controller: 'ModalConfirmController as modalConfirmCtrl',
                backdrop: 'static',
                resolve: {
                    name: function () {
                        return employee.FullName + ' (' + employee.Nickname + ')';
                    },
                    action: function () {
                        return action;
                    }
                },
                windowClass: 'modal-confirm-center'
            });

            modalInstance.result.then(function () {
                periodFactory.editStatusEmployeeSetting(params).success(function (response) {
                    if (response.IsSuccessful) {
                        notificationFactory.success('UpdatedSuccessfully');

                        vm.query();
                    } else {
                        employee.IsExcluded = !params.IsExcluded;
                        notificationFactory.error('', response.Message);
                    }
                }).error(function (response, status) {
                    if (status === 405) {
                        errorFactory.errorHandle(status);
                    } else {
                        employee.IsExcluded = !params.IsExcluded;
                        notificationFactory.error('UpdatedUnsuccessfully');
                    }
                });
            }, function () {
                employee.IsExcluded = !params.IsExcluded;
            })['finally'](function () {
                modalInstance = '';
            });
        };

        model.getJobLevelSettings = function (params) {
            return periodFactory.getJobLevelSettings(params).success(function (settings) {
                model.jobLevelSettings = settings;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.getJobLevelSettingBySectionId = function (params) {
            return periodFactory.getJobLevelSettingBySectionId(params).success(function (settings) {
                model.sectionJobLevelSettings = settings;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.openPopupEditJobLevelSetting = function (vm, section) {
            vm.modalInstance = $uibModal.open({
                templateUrl: 'SourceApp/Components/JobLevelStandard/Period/Templates/editJobLevelSetting.html',
                controller: 'EditPeriodJobLevelSettingController as editJobLevelCtrl',
                backdrop: 'static',
                resolve: {
                    permission: function () {
                        return vm.permission;
                    },
                    periodId: function () {
                        return vm.period.PeriodId;
                    },
                    section: function () {
                        return section;
                    }
                },
                windowClass: 'modal-confirm-center modal-edit-job-level-setting'
            });

            return vm.modalInstance.result;
        };

        model.editJobLevelSetting = function (command, modalInstance) {
            return periodFactory.editJobLevelSetting(command).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success('UpdatedSuccessfully');
                    modalInstance.close(true);
                } else {
                    notificationFactory.error('', response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.openPopupCopyJobLevelSetting = function (vm, selectedSection, sections) {
            vm.modalInstance = $uibModal.open({
                templateUrl: 'SourceApp/Components/JobLevelStandard/Period/Templates/copyJobLevelSetting.html',
                controller: 'CopyPeriodJobLevelSettingController as copyJobLevelCtrl',
                backdrop: 'static',
                resolve: {
                    periodId: function () {
                        return vm.period.PeriodId;
                    },
                    selectedSection: function () {
                        return selectedSection;
                    },
                    sections: function () {
                        return sections;
                    }
                },
                windowClass: 'modal-confirm-center modal-copy-job-level-setting'
            });

            return vm.modalInstance.result;
        };

        model.openPopupCopyDefaultJobLevelSetting = function (vm, periodId, sectionIds) {
            if (sectionIds.length === 0) {
                notificationFactory.error('', 'Please select at least a section.');
                return;
            }

            let modalInstance = $uibModal.open({
                templateUrl: '/SourceApp/Shared/Templates/ModalConfirmContent.html',
                controller: 'ModalConfirmController as modalConfirmCtrl',
                backdrop: 'static',
                resolve: {
                    name: function () {
                        return 'Default Setting';
                    },
                    action: function () {
                        return 'copy';
                    }
                },
                windowClass: 'modal-confirm-center'
            });

            modalInstance.result.then(function () {
                periodFactory.copyDefaultJobLevelSetting({ periodId, sectionIds }).success(function (response) {
                    if (response.IsSuccessful) {
                        notificationFactory.success('UpdatedSuccessfully');

                        vm.query(vm.filter);
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
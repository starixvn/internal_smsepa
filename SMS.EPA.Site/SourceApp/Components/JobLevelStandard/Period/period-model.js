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
                case utilitiesFactory.jobLevelAssessmentProcess.Evaluator:
                    model.calendar.fromDateEvaluatorOpened = true;
                    break;

                case utilitiesFactory.jobLevelAssessmentProcess.Employee:
                    model.calendar.fromDateEmployeeOpened = true;
                    break;

                case utilitiesFactory.jobLevelAssessmentProcess.Management:
                    model.calendar.fromDateManagementOpened = true;
                    break;
            }
        };

        model.openToDate = function (event, processId) {
            event.preventDefault();
            event.stopPropagation();

            resetCalendar();

            switch (processId) {
                case utilitiesFactory.jobLevelAssessmentProcess.Evaluator:
                    model.calendar.toDateEvaluatorOpened = true;
                    break;

                case utilitiesFactory.jobLevelAssessmentProcess.Employee:
                    model.calendar.toDateEmployeeOpened = true;
                    break;

                case utilitiesFactory.jobLevelAssessmentProcess.Management:
                    model.calendar.toDateManagementOpened = true;
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

        model.openPopupEditAssessmentStatus = function (vm, employee) {
            vm.modalInstance = $uibModal.open({
                templateUrl: 'SourceApp/Components/JobLevelStandard/Period/Templates/editAssessmentStatus.html',
                controller: 'EditAssessmentStatusController as editAssessmentStatusCtrl',
                backdrop: 'static',
                resolve: {
                    employee: function () {
                        return employee;
                    }
                },
                windowClass: 'modal-confirm-center edit-assessment-status-modal'
            });

            return vm.modalInstance.result;
        };

        model.editAssessmentStatus = function (params, modalInstance) {
            return periodFactory.editAssessmentStatus(params).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success('UpdatedSuccessfully');
                    modalInstance.close();
                } else {
                    notificationFactory.error('', response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
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

        model.openPopupCopyJobLevelSetting = function (vm, selectedSection, sectionHasSettings) {
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
                    sectionHasSettings: function () {
                        return sectionHasSettings;
                    }
                },
                windowClass: 'modal-confirm-center modal-copy-job-level-setting'
            });

            return vm.modalInstance.result;
        };

        model.openPopupCopyDefaultJobLevelSetting = function (vm, periodId) {
            var modalInstance = $uibModal.open({
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
                periodFactory.copyDefaultJobLevelSetting(periodId).success(function (response) {
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
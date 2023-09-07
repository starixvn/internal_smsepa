(function (angular) {
    'use strict';

    angular.module('jobDescriptionModule').factory('JobDescriptionModel', jobDescriptionModel);
    jobDescriptionModel.$inject = ['$uibModal', 'jobDescriptionFactory', 'errorFactory', 'notificationFactory'];

    function jobDescriptionModel($uibModal, jobDescriptionFactory, errorFactory, notificationFactory) {
        var model = {
            permission: 0,
            groupSettingInfos: [],
            groupHasSettings: [],
            groupSettingInfo: {},
            employeeSettingInfos: [],
            employeeHasSettings: [],
            employeeSettingInfo: {},
            getUserPermission: getUserPermission,
            getGroupSettingInfo: getGroupSettingInfo,
            getGroupHasSetting: getGroupHasSetting,
            getGroupSettingById: getGroupSettingById,
            editGroupSetting: editGroupSetting,
            openPopupCopyGroupSetting: openPopupCopyGroupSetting,
            copyGroupSetting: copyGroupSetting,
            getEmployeeSettingInfo: getEmployeeSettingInfo,
            getEmployeeHasSetting: getEmployeeHasSetting,
            getEmployeeSettingById: getEmployeeSettingById,
            editEmployeeSetting: editEmployeeSetting,
            openPopupCopyEmployeeSetting: openPopupCopyEmployeeSetting,
            copyEmployeeSetting: copyEmployeeSetting,
            createDescriptionGroupItem: createDescriptionGroupItem,
            editDescriptionGroupItem: editDescriptionGroupItem,
            createDescriptionItem: createDescriptionItem,
            editDescriptionItem: editDescriptionItem,
            openPopupConfirmRefreshData: openPopupConfirmRefreshData,
            openPopupCreateDescriptionGroupItem: openPopupCreateDescriptionGroupItem,
            openPopupDeleteDescriptionGroupItem: openPopupDeleteDescriptionGroupItem,
            openPopupEditDescriptionGroupItem: openPopupEditDescriptionGroupItem,
            openPopupCreateDescriptionItem: openPopupCreateDescriptionItem,
            openPopupEditDescriptionItem: openPopupEditDescriptionItem,
            openPopupDeleteDescriptionItem: openPopupDeleteDescriptionItem,
            validateDescription: validateDescription
        };

        return model;

        function getUserPermission() {
            return jobDescriptionFactory.getUserPermission().success(function (permission) {
                model.permission = permission;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getGroupSettingInfo() {
            return jobDescriptionFactory.getGroupSettingInfo()
                .success(function (settings) {
                    model.groupSettingInfos = settings;
                }).error(function (response, status) {
                    errorFactory.errorHandle(status);
                });
        }

        function getGroupHasSetting() {
            return jobDescriptionFactory.getGroupHasSetting()
                .success(function (settings) {
                    model.groupHasSettings = settings;
                }).error(function (response, status) {
                    errorFactory.errorHandle(status);
                });
        }

        function getGroupSettingById(groupId, jobLevelId) {
            return jobDescriptionFactory.getGroupSettingById(groupId, jobLevelId)
                .success(function (setting) {
                    model.groupSettingInfo = setting;
                }).error(function (response, status) {
                    errorFactory.errorHandle(status);
                });
        }

        function editGroupSetting(setting) {
            return jobDescriptionFactory.editGroupSetting(setting).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success('UpdatedSuccessfully');
                } else {
                    notificationFactory.error('', response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function openPopupCopyGroupSetting(vm, group) {
            vm.modalInstance = $uibModal.open({
                templateUrl: 'SourceApp/Components/JobDescription/Templates/copy-group-setting.html',
                controller: 'CopyGroupSettingController as copyGroupSettingCtrl',
                backdrop: 'static',
                windowClass: 'modal-confirm-center',
                resolve: {
                    group: function () {
                        return group;
                    }
                }
            });

            return vm.modalInstance.result;
        }

        function copyGroupSetting(setting, modalInstance) {
            return jobDescriptionFactory.copyGroupSetting(setting).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success('UpdatedSuccessfully');
                    modalInstance.close(true);
                } else {
                    notificationFactory.error('', response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getEmployeeSettingInfo(teamIds, employeeId, showAll) {
            return jobDescriptionFactory.getEmployeeSettingInfo(teamIds, employeeId, showAll)
                .success(function (settings) {
                    model.employeeSettingInfos = settings;
                }).error(function (response, status) {
                    errorFactory.errorHandle(status);
                });
        }

        function getEmployeeHasSetting(teamIds, employeeId, showAll) {
            return jobDescriptionFactory.getEmployeeSettingInfo(teamIds, employeeId, showAll)
                .success(function (settings) {
                    model.employeeHasSettings = settings;
                }).error(function (response, status) {
                    errorFactory.errorHandle(status);
                });
        }

        function getEmployeeSettingById(employeeId) {
            return jobDescriptionFactory.getEmployeeSettingById(employeeId)
                .success(function (setting) {
                    model.employeeSettingInfo = setting;
                }).error(function (response, status) {
                    errorFactory.errorHandle(status);
                });
        }

        function editEmployeeSetting(setting) {
            return jobDescriptionFactory.editEmployeeSetting(setting).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success('UpdatedSuccessfully');
                } else {
                    notificationFactory.error('', response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function openPopupCopyEmployeeSetting(vm, employee) {
            vm.modalInstance = $uibModal.open({
                templateUrl: 'SourceApp/Components/JobDescription/Templates/copy-personal-setting.html',
                controller: 'CopyEmployeeSettingController as copyEmployeeSettingCtrl',
                backdrop: 'static',
                windowClass: 'modal-confirm-center',
                resolve: {
                    employee: function () {
                        return employee;
                    }
                }
            });

            return vm.modalInstance.result;
        }

        function copyEmployeeSetting(setting, modalInstance) {
            return jobDescriptionFactory.copyEmployeeSetting(setting).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success('UpdatedSuccessfully');
                    modalInstance.close(true);
                } else {
                    notificationFactory.error('', response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function openPopupConfirmRefreshData(vm) {
            vm.modalInstance = $uibModal.open({
                templateUrl: 'SourceApp/Shared/Templates/modal-confirm-refresh-data.html',
                controller: 'ConfirmRefreshDataController as modalConfirmCtrl',
                backdrop: 'static',
                windowClass: 'modal-confirm-center'
            });

            return vm.modalInstance.result;
        }

        function openPopupCreateDescriptionGroupItem(vm, groupId, employeeId) {
            vm.modalInstance = $uibModal.open({
                templateUrl: 'SourceApp/Components/JobDescription/Templates/create-job-description-group-item.html',
                controller: 'CreateDescriptionGroupItemController as createGroupItemCtrl',
                backdrop: 'static',
                windowClass: 'modal-confirm-center',
                resolve: {
                    groupId: function () {
                        return groupId;
                    },
                    employeeId: function () {
                        return employeeId;
                    }
                }
            });

            return vm.modalInstance.result;
        }

        function openPopupCreateDescriptionItem(vm, groupItemId) {
            vm.modalInstance = $uibModal.open({
                templateUrl: 'SourceApp/Components/JobDescription/Templates/create-job-description-item.html',
                controller: 'CreateDescriptionItemController as createItemCtrl',
                resolve: {
                    groupItemId: function () {
                        return groupItemId;
                    }
                },
                backdrop: 'static',
                windowClass: 'modal-confirm-center'
            });

            return vm.modalInstance.result;
        }

        function openPopupEditDescriptionGroupItem(vm, groupItem) {
            vm.modalInstance = $uibModal.open({
                templateUrl: 'SourceApp/Components/JobDescription/Templates/edit-job-description-group-item.html',
                controller: 'EditDescriptionGroupItemController as editGroupItemCtrl',
                backdrop: 'static',
                resolve: {
                    descriptionGroupItem: function () {
                        return angular.copy(groupItem);
                    }
                },
                windowClass: 'modal-confirm-center'
            });

            return vm.modalInstance.result;
        }

        function openPopupDeleteDescriptionGroupItem(vm, groupItem) {
            let modalInstance = $uibModal.open({
                templateUrl: 'SourceApp/Shared/Templates/ModalConfirmContent.html',
                controller: 'ModalConfirmController as modalConfirmCtrl',
                backdrop: 'static',
                resolve: {
                    name: function () {
                        return groupItem.Name;
                    },
                    action: function () {
                        return 'delete';
                    }
                },
                windowClass: 'modal-confirm-center'
            });

            modalInstance.result.then(function () {
                jobDescriptionFactory.deleteJobDescriptionGroupItem(groupItem.GroupItemId).success(function (response) {
                    if (response.IsSuccessful) {
                        const index = vm.settingInfo.GroupItems.indexOf(groupItem);
                        vm.settingInfo.GroupItems.splice(index, 1);

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
        }

        function createDescriptionGroupItem(groupItem, modalInstance) {
            return jobDescriptionFactory.createJobDescriptionGroupItem(groupItem).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success('CreatedSuccessfully');
                    modalInstance.close(response.IsSuccessful);
                } else {
                    notificationFactory.error('', response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function openPopupEditDescriptionItem(vm, item) {
            vm.modalInstance = $uibModal.open({
                templateUrl: 'SourceApp/Components/JobDescription/Templates/edit-job-description-item.html',
                controller: 'EditDescriptionItemController as editItemCtrl',
                backdrop: 'static',
                resolve: {
                    descriptionItem: function () {
                        return angular.copy(item);
                    }
                },
                windowClass: 'modal-confirm-center'
            });

            return vm.modalInstance.result;
        }

        function createDescriptionItem(item, modalInstance) {
            return jobDescriptionFactory.createJobDescriptionItem(item).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success('CreatedSuccessfully');
                    modalInstance.close(response.IsSuccessful);
                } else {
                    notificationFactory.error('', response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function openPopupDeleteDescriptionItem(vm, item) {
            let modalInstance = $uibModal.open({
                templateUrl: 'SourceApp/Shared/Templates/ModalConfirmContent.html',
                controller: 'ModalConfirmController as modalConfirmCtrl',
                backdrop: 'static',
                resolve: {
                    name: function () {
                        return item.Name;
                    },
                    action: function () {
                        return 'delete';
                    }
                },
                windowClass: 'modal-confirm-center'
            });

            modalInstance.result.then(function () {
                jobDescriptionFactory.deleteJobDescriptionItem(item.ItemId).success(function (response) {
                    if (response.IsSuccessful) {
                        const indexOfGroupItem = vm.settingInfo.GroupItems.map(x => x.GroupItemId).indexOf(item.GroupItemId);
                        const indexOfItem = vm.settingInfo.GroupItems[indexOfGroupItem].Items.map(x => x.ItemId).indexOf(item.ItemId);
                        vm.settingInfo.GroupItems[indexOfGroupItem].Items.splice(indexOfItem, 1);

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
        }

        function editDescriptionGroupItem(groupItem, modalInstance) {
            return jobDescriptionFactory.editJobDescriptionGroupItem(groupItem).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success('UpdatedSuccessfully');
                    modalInstance.close(groupItem);
                } else {
                    notificationFactory.error('', response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function editDescriptionItem(item, modalInstance) {
            return jobDescriptionFactory.editJobDescriptionItem(item).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success('UpdatedSuccessfully');
                    modalInstance.close(item);
                } else {
                    notificationFactory.error('', response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function validateDescription(name) {
            let isValid = true;
            if (!name) {
                notificationFactory.error('NameRequired');
                isValid = false;
            }

            return isValid;
        }
    }
})(angular);
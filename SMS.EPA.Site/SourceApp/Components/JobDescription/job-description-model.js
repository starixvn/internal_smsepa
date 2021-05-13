(function (angular) {
    'use strict';

    angular.module('jobDescriptionModule').factory('JobDescriptionModel', jobDescriptionModel);
    jobDescriptionModel.$inject = ['$uibModal', 'jobDescriptionFactory', 'errorFactory', 'notificationFactory'];

    function jobDescriptionModel($uibModal, jobDescriptionFactory, errorFactory, notificationFactory) {
        var model = {
            permission: 0,
            groupSettingInfos: [],
            groupSettingInfo: {},
            employeeSettingInfos: [],
            employeeSettingInfo: {},
            getUserPermission: getUserPermission,
            getGroupSettingInfo: getGroupSettingInfo,
            getGroupSettingById: getGroupSettingById,
            editGroupSetting: editGroupSetting,
            getEmployeeSettingInfo: getEmployeeSettingInfo,
            getEmployeeSettingById: getEmployeeSettingById,
            createDescriptionGroupItem: createDescriptionGroupItem,
            editDescriptionGroupItem: editDescriptionGroupItem,
            createDescriptionItem: createDescriptionItem,
            editDescriptionItem: editDescriptionItem,
            openPopupCreateDescriptionGroupItem: openPopupCreateDescriptionGroupItem,
            openPopupDeleteDescriptionGroupItem: openPopupDeleteDescriptionGroupItem,
            openPopupEditDescriptionGroupItem: openPopupEditDescriptionGroupItem,
            openPopupCreateDescriptionItem: openPopupCreateDescriptionItem,
            openPopupEditDescriptionItem: openPopupEditDescriptionItem,
            openPopupDeleteDescriptionItem: openPopupDeleteDescriptionItem,
            editEmployeeSetting: editEmployeeSetting,
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
                .success(function (groupSettingInfos) {
                    model.groupSettingInfos = groupSettingInfos;
                }).error(function (response, status) {
                    errorFactory.errorHandle(status);
                });
        }

        function getGroupSettingById(groupId, jobLevelId) {
            return jobDescriptionFactory.getGroupSettingById(groupId, jobLevelId)
                .success(function (groupSettingInfo) {
                    model.groupSettingInfo = groupSettingInfo;
                }).error(function (response, status) {
                    errorFactory.errorHandle(status);
                });
        }

        function editGroupSetting(setting) {
            return jobDescriptionFactory.editGroupSetting(setting).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success("UpdatedSuccessfully");
                } else {
                    notificationFactory.error("", response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function editEmployeeSetting(setting) {
            return jobDescriptionFactory.editEmployeeSetting(setting).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success("UpdatedSuccessfully");
                } else {
                    notificationFactory.error("", response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getEmployeeSettingInfo(groupId, employeeId, showAll) {
            return jobDescriptionFactory.getEmployeeSettingInfo(groupId, employeeId, showAll)
                .success(function (employeeSettingInfos) {
                    model.employeeSettingInfos = employeeSettingInfos;
                }).error(function (response, status) {
                    errorFactory.errorHandle(status);
                });
        }

        function getEmployeeSettingById(employeeId) {
            return jobDescriptionFactory.getEmployeeSettingById(employeeId)
                .success(function (employeeSettingInfo) {
                    model.employeeSettingInfo = employeeSettingInfo;
                }).error(function (response, status) {
                    errorFactory.errorHandle(status);
                });
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
            var modalInstance = $uibModal.open({
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
                        var index = vm.employeeSettingInfo.GroupItems.indexOf(groupItem);
                        vm.employeeSettingInfo.GroupItems.splice(index, 1);

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
            var modalInstance = $uibModal.open({
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
                        var indexOfGroupItem = vm.employeeSettingInfo.GroupItems.map(x => x.GroupItemId).indexOf(item.GroupItemId);
                        var indexOfItem = vm.employeeSettingInfo.GroupItems[indexOfGroupItem].Items.map(x => x.ItemId).indexOf(item.ItemId);
                        vm.employeeSettingInfo.GroupItems[indexOfGroupItem].Items.splice(indexOfItem, 1);
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
            var isValid = true;

            if (!name) {
                notificationFactory.error('NameRequired');
                isValid = false;
            }

            return isValid;
        }
    }
})(angular);
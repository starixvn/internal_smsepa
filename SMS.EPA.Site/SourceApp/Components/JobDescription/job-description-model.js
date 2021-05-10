(function (angular) {
    'use strict';

    angular.module('jobDescriptionModule').factory('JobDescriptionModel', jobDescriptionModel);
    jobDescriptionModel.$inject = ['$uibModal', 'jobDescriptionFactory', 'errorFactory', 'notificationFactory', 'utilitiesFactory'];

    function jobDescriptionModel($uibModal, jobDescriptionFactory, errorFactory, notificationFactory, utilitiesFactory) {
        var model = {
            permission: 0,
            groupSettingInfos: [],
            groupSettingInfo: {},
            employeeSettingInfos: [],
            employeeSettingInfo: {},
            getUserPermission: getUserPermission,
            getGroupSettingInfo: getGroupSettingInfo,
            getGroupSettingById: getGroupSettingById,
            getEmployeeSettingInfo: getEmployeeSettingInfo,
            getEmployeeSettingById: getEmployeeSettingById,
            createDescriptionGroupItem: createDescriptionGroupItem,
            updateDescriptionGroupItem: updateDescriptionGroupItem,
            createDescriptionItem: createDescriptionItem,
            updateDescriptionItem: updateDescriptionItem,
            openPopupCreateDescriptionGroupItem: openPopupCreateDescriptionGroupItem,
            openPopupDeleteDescriptionGroupItem: openPopupDeleteDescriptionGroupItem,
            openPopupUpdateDescriptionGroupItem: openPopupUpdateDescriptionGroupItem,
            openPopupCreateDescriptionItem: openPopupCreateDescriptionItem,
            openPopupUpdateDescriptionItem: openPopupUpdateDescriptionItem,
            openPopupDeleteDescriptionItem: openPopupDeleteDescriptionItem,
            openPopupUpdateStatusDescriptionGroupItem: openPopupUpdateStatusDescriptionGroupItem,
            openPopupUpdateStatusDescriptionItem: openPopupUpdateStatusDescriptionItem,
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

        function openPopupCreateDescriptionGroupItem(vm) {
            vm.modalInstance = $uibModal.open({
                templateUrl: 'SourceApp/Components/JobDescription/Templates/create-job-description-group-item.html',
                controller: 'CreateDescriptionGroupItemController as createGroupItemCtrl',
                backdrop: 'static',
                windowClass: 'modal-confirm-center'
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


        function openPopupUpdateDescriptionGroupItem(vm, groupItem) {
            vm.modalInstance = $uibModal.open({
                templateUrl: 'SourceApp/Components/JobDescription/Templates/update-job-description-group-item.html',
                controller: 'UpdateDescriptionGroupItemController as updateGroupItemCtrl',
                backdrop: 'static',
                resolve: {
                    getGroupItem: function () {
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
                        utilitiesFactory.searchOnPage(vm, vm.employeeSettingInfo.GroupItems);

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
                    modalInstance.close();
                } else {
                    notificationFactory.error('', response.Message);
                }
            })
                .error(function (response, status) {
                    errorFactory.errorHandle(status);
                });
        }

        function openPopupUpdateDescriptionItem(vm, item) {
            vm.modalInstance = $uibModal.open({
                templateUrl: 'SourceApp/Components/JobDescription/Templates/update-job-description-item.html',
                controller: 'UpdateDescriptionItemController as updateItemCtrl',
                backdrop: 'static',
                resolve: {
                    getItem: function () {
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
                    modalInstance.close();
                } else {
                    notificationFactory.error('', response.Message);
                }
            })
                .error(function (response, status) {
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
                        //do it
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

        function updateDescriptionGroupItem(groupItem, modalInstance) {
            return jobDescriptionFactory.updateJobDescriptionGroupItem(groupItem).success(function (response) {
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

        function updateDescriptionItem(item, modalInstance) {
            return jobDescriptionFactory.updateJobDescriptionItem(item).success(function (response) {
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

        function openPopupUpdateStatusDescriptionGroupItem(groupItem) {
            groupItem.IsActive = !groupItem.IsActive;

            var action = groupItem.IsActive ? 'activate' : 'deactivate',
                newGroupItem = {
                    GroupItemId: groupItem.GroupItemId,
                    IsActive: groupItem.IsActive
                };

            var modalInstance = $uibModal.open({
                templateUrl: 'SourceApp/Shared/Templates/ModalConfirmContent.html',
                controller: 'ModalConfirmController as modalConfirmCtrl',
                backdrop: 'static',
                resolve: {
                    name: function () {
                        return groupItem.Name;
                    },
                    action: function () {
                        return action;
                    }
                },
                windowClass: 'modal-confirm-center'
            });

            var keyMessage = '';

            modalInstance.result.then(function () {
                jobDescriptionFactory.updateStatusDescriptionGroupItem(newGroupItem).success(function (response) {
                    if (response.IsSuccessful) {
                        keyMessage = action === 'activate' ? 'ActivationSuccess' : 'DeactivationSuccess';
                        notificationFactory.success(keyMessage);
                    } else {
                        groupItem.IsActive = !newGroupItem.IsActive;
                        notificationFactory.error('', response.Message);
                    }
                }).error(function (response, status) {
                    if (status === 405) {
                        errorFactory.errorHandle(status);
                    } else {
                        keyMessage = action === 'activate' ? 'ActivationFail' : 'DeactivationFail';
                        groupItem.IsActive = !newGroupItem.IsActive;
                        notificationFactory.error(keyMessage);
                    }
                });
            }, function () {
                groupItem.IsActive = !newGroupItem.IsActive;
            })['finally'](function () {
                modalInstance = '';
            });
        }

        function openPopupUpdateStatusDescriptionItem(item) {
            item.IsActive = !item.IsActive;

            var action = item.IsActive ? 'activate' : 'deactivate',
                newItem = {
                    ItemId: item.ItemId,
                    IsActive: item.IsActive
                };

            var modalInstance = $uibModal.open({
                templateUrl: 'SourceApp/Shared/Templates/ModalConfirmContent.html',
                controller: 'ModalConfirmController as modalConfirmCtrl',
                backdrop: 'static',
                resolve: {
                    name: function () {
                        return item.Name;
                    },
                    action: function () {
                        return action;
                    }
                },
                windowClass: 'modal-confirm-center'
            });

            var keyMessage = '';

            modalInstance.result.then(function () {
                jobDescriptionFactory.updateStatusDescriptionItem(newItem).success(function (response) {
                    if (response.IsSuccessful) {
                        keyMessage = action === 'activate' ? 'ActivationSuccess' : 'DeactivationSuccess';
                        notificationFactory.success(keyMessage);
                    } else {
                        item.IsActive = !newItem.IsActive;
                        notificationFactory.error('', response.Message);
                    }
                }).error(function (response, status) {
                    if (status === 405) {
                        errorFactory.errorHandle(status);
                    } else {
                        keyMessage = action === 'activate' ? 'ActivationFail' : 'DeactivationFail';
                        item.IsActive = !newItem.IsActive;
                        notificationFactory.error(keyMessage);
                    }
                });
            }, function () {
                item.IsActive = !newItem.IsActive;
            })['finally'](function () {
                modalInstance = '';
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
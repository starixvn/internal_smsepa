(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('jobLevelModule').factory('JobLevelModel', jobLevelModel);
    jobLevelModel.$inject = ['$uibModal', 'jobLevelFactory', 'utilitiesFactory', 'notificationFactory', 'errorFactory'];

    function jobLevelModel($uibModal, jobLevelFactory, utilitiesFactory, notificationFactory, errorFactory) {
        var permission = 0;

        var model = {
            permission: permission,
            openPopupCreateJobLevel: openPopupCreateJobLevel,
            openPopupEditJobLevel: openPopupEditJobLevel,
            openPopupDeleteJobLevel: openPopupDeleteJobLevel,
            getUserPermission: getUserPermission,
            validateJobLevel: validateJobLevel,
            createJobLevel: createJobLevel,
            editJobLevel: editJobLevel
        };

        return model;

        function createJobLevel(joblevel, modalInstance) {
            return jobLevelFactory.createJobLevel(joblevel).success(function (response) {
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

        function editJobLevel(jobLevel, modalInstance) {
            return jobLevelFactory.editJobLevel(jobLevel).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success('UpdatedSuccessfully');
                    modalInstance.close(jobLevel);
                } else {
                    notificationFactory.error('', response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function validateJobLevel(name) {
            let isValid = true;
            if (!name) {
                notificationFactory.error('NameRequired');
                isValid = false;
            }

            return isValid;
        }

        function getUserPermission() {
            return jobLevelFactory.getUserPermission().success(function (permission) {
                model.permission = permission;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function openPopupCreateJobLevel(vm) {
            vm.modalInstance = $uibModal.open({
                templateUrl: 'SourceApp/Components/JobLevel/Templates/create.html',
                controller: 'CreateJobLevelController as createJobLevelCtrl',
                backdrop: 'static',
                windowClass: 'modal-confirm-center'
            });

            return vm.modalInstance.result;
        }

        function openPopupDeleteJobLevel(vm, jobLevel) {
            var modalInstance = $uibModal.open({
                templateUrl: 'SourceApp/Shared/Templates/ModalConfirmContent.html',
                controller: 'ModalConfirmController as modalConfirmCtrl',
                backdrop: 'static',
                resolve: {
                    name: function () {
                        return jobLevel.Name;
                    },
                    action: function () {
                        return 'delete';
                    }
                },
                windowClass: 'modal-confirm-center'
            });

            modalInstance.result.then(function () {
                jobLevelFactory.deleteJobLevel(jobLevel.JobLevelId).success(function (response) {
                    if (response.IsSuccessful) {
                        var index = vm.jobLevels.indexOf(jobLevel);
                        vm.jobLevels.splice(index, 1);
                        utilitiesFactory.searchOnPage(vm, vm.jobLevels);

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

        function openPopupEditJobLevel(vm, jobLevel) {
            vm.modalInstance = $uibModal.open({
                templateUrl: 'SourceApp/Components/JobLevel/Templates/edit.html',
                controller: 'EditJobLevelController as editJobLevelCtrl',
                backdrop: 'static',
                resolve: {
                    jobLevel: function () {
                        return angular.copy(jobLevel);
                    }
                },
                windowClass: 'modal-confirm-center'
            });

            return vm.modalInstance.result;
        }
    }
})(angular);
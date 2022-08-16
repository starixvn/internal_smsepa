(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('jobLevelStandardModule').factory('JobLevelSettingModel', jobLevelSettingModel);
    jobLevelSettingModel.$inject = ['$uibModal', 'jobLevelSettingFactory', 'errorFactory', 'notificationFactory', 'utilitiesFactory'];

    function jobLevelSettingModel($uibModal, jobLevelSettingFactory, errorFactory, notificationFactory, utilitiesFactory) {
        let model = {};

        model.sections = [];
        model.settings = [];

        model.getSettings = function (params) {
            return jobLevelSettingFactory.getSettings(params).success(function (sections) {
                model.sections = sections;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.getBySectionId = function (sectionId) {
            return jobLevelSettingFactory.getBySectionId(sectionId).success(function (settings) {
                model.settings = settings;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.openPopupEditJobLevelSetting = function (vm, section) {
            vm.modalInstance = $uibModal.open({
                templateUrl: 'SourceApp/Components/JobLevelStandard/JobLevelSetting/Templates/edit.html',
                controller: 'EditJobLevelSettingController as editCtrl',
                backdrop: 'static',
                resolve: {
                    section: function () {
                        return section;
                    }
                },
                windowClass: 'modal-confirm-center modal-edit-job-level-setting'
            });

            return vm.modalInstance.result;
        };

        model.editJobLevelSetting = function (command, modalInstance) {
            return jobLevelSettingFactory.edit(command).success(function (response) {
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
                templateUrl: 'SourceApp/Components/JobLevelStandard/JobLevelSetting/Templates/copy.html',
                controller: 'CopyJobLevelSettingController as copyCtrl',
                backdrop: 'static',
                resolve: {
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

        return model;
    }
})(angular);
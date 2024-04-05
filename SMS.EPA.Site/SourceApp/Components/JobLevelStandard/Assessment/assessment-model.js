(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('jobLevelStandardModule').factory('AssessmentModel', assessmentModel);
    assessmentModel.$inject = ['$uibModal', 'assessmentFactory', 'errorFactory', 'notificationFactory'];

    function assessmentModel($uibModal, assessmentFactory, errorFactory, notificationFactory) {
        let model = {};

        model.periods = [];
        model.orgChart = [];
        model.kpis = [];
        model.comment = "";
        model.jobLevelSettings = [];
        model.assessments = [];

        model.openPopupAddComment = function (vm, assessment) {
            vm.modalInstance = $uibModal.open({
                templateUrl: 'SourceApp/Components/JobLevelStandard/Assessment/Templates/comment.html',
                controller: 'CommentAssessmentController as commentCtrl',
                backdrop: 'static',
                resolve: {
                    comment: function () {
                        return assessment.Comment;
                    }
                },
                windowClass: 'modal-confirm-center job-level-standard-period-modal'
            });

            return vm.modalInstance.result;
        };

        model.getPeriods = function () {
            return assessmentFactory.getPeriods().success(function (periods) {
                model.periods = periods;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.getOrgChart = function (periodId) {
            return assessmentFactory.getOrgChart(periodId).success(function (data) {
                model.orgChart = data;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.getAssessments = function (params) {
            return assessmentFactory.getAssessments(params).success(function (data) {
                model.kpis = data.kpis;
                model.jobLevelSettings = data.jobLevelSettings;
                model.assessments = data.assessments;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.validateAssessment = function (assessment) {
            let isValid = true;
            if (assessment.TotalWeight > 100) {
                isValid = false;
                notificationFactory.error('', 'Total Weight must be less than or equal to 100.');
            }

            return isValid;
        };

        model.edit = function (vm, assessment) {
            return assessmentFactory.edit(assessment).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success('UpdatedSuccessfully');
                    vm.query();
                } else {
                    notificationFactory.error('', response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.editAndVerify = function (vm, assessment) {
            return assessmentFactory.edit(assessment).success(function (response) {
                if (response.IsSuccessful) {
                    model.editStatus(vm, { assessmentIds: response.ObjectId, statusId: vm.status.Verified });
                } else {
                    notificationFactory.error('', response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.editStatus = function (vm, params) {
            return assessmentFactory.editStatus(params).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success('UpdatedSuccessfully');
                    vm.query();
                } else {
                    notificationFactory.error('', response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.approveAll = function (vm, params) {
            if (params.assessmentIds.length === 0) {
                notificationFactory.error('', 'Please select at least an employee.');
            } else {
                model.editStatus(vm, params);
            }
        };

        return model;
    }
})(angular);
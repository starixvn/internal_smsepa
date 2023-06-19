(function(angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('targetTemplateModule').factory('TargetTemplateModel', targetTemplateModel);
    targetTemplateModel.$inject = ['$uibModal', 'targetTemplateFactory', 'errorFactory', 'notificationFactory', 'utilitiesFactory'];

    function targetTemplateModel($uibModal, targetTemplateFactory, errorFactory, notificationFactory, utilitiesFactory) {
        let targetTemplates = [],
            targetTemplate = {},
            kpiGroups = [],
            kpiList = [],
            permission = 0,
            scoreRatingTypes = [],
            ratingTypes = [];

        let model = {
            addKPIGroupWeight: addKPIGroupWeight,
            checkSelectedAllKPI: checkSelectedAllKPI,
            checkSelectedAllKPIGroup: checkSelectedAllKPIGroup,
            createTargetTemplate: createTargetTemplate,
            getKPIGroupOfTargetTemplate: getKPIGroupOfTargetTemplate,
            getKPIOfTargetTemplate: getKPIOfTargetTemplate,
            getPermissionOnTargetTemplate: getPermissionOnTargetTemplate,
            getRatingType: getRatingType,
            getScoreRatingType: getScoreRatingType,
            getTargetTemplate: getTargetTemplate,
            getTargetTemplateById: getTargetTemplateById,
            kpiGroups: kpiGroups,
            kpiList: kpiList,
            openPopupAddKPIGroup: openPopupAddKPIGroup,
            openPopupCreateTargetTemplate: openPopupCreateTargetTemplate,
            openPopupDeleteKPIGroup: openPopupDeleteKPIGroup,
            openPopupDeleteTargetTemplate: openPopupDeleteTargetTemplate,
            openPopupUpdateStatusTargetTemplate: openPopupUpdateStatusTargetTemplate,
            permission: permission,
            ratingTypes: ratingTypes,
            scoreRatingTypes: scoreRatingTypes,
            targetTemplate: targetTemplate,
            targetTemplates: targetTemplates,
            updateTargetTemplate: updateTargetTemplate,
            updateTargetTemplateBasicInfo: updateTargetTemplateBasicInfo,
            validateKPIGroupWeight: validateKPIGroupWeight,
            validateTargetTemplate: validateTargetTemplate
        };

        return model;

        function addKPIGroupWeight(kpiGroupWeightModel) {
            return targetTemplateFactory.addKPIGroupWeight(kpiGroupWeightModel).success(function(response) {
                if (response.IsSuccessful) {
                    notificationFactory.success('UpdatedSuccessfully');
                } else {
                    notificationFactory.error('', response.Message);
                }
            }).error(function(response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function checkSelectedAllKPI(vm, kpiList) {
            vm.SelectedAll = true;

            if (kpiList && kpiList.length > 0) {
                angular.forEach(kpiList,
                    function(group) {
                        angular.forEach(group.KPIs,
                            function(kpi) {
                                if (!kpi.Selected) {
                                    vm.SelectedAll = false;
                                }
                            });
                    });
            } else {
                vm.SelectedAll = false;
            }
        }

        function checkSelectedAllKPIGroup(vm, kpiGroups) {
            vm.selectedAll = true;

            if (kpiGroups && kpiGroups.length > 0) {
                angular.forEach(kpiGroups,
                    function(group) {
                        if (!group.Selected) {
                            vm.selectedAll = false;
                        }
                    });
            } else {
                vm.selectedAll = false;
            }
        }

        function createTargetTemplate(targetTemplate, modalInstance) {
            return targetTemplateFactory.createTargetTemplate(targetTemplate).success(function(response) {
                if (response.IsSuccessful) {
                    notificationFactory.success('CreatedSuccessfully');
                    modalInstance.close();
                } else {
                    notificationFactory.error('', response.Message);
                }
            }).error(function() {
                errorFactory.errorHandle(status);
            });
        }

        function getKPIGroupOfTargetTemplate(targetTemplateId) {
            return targetTemplateFactory.getKPIGroupOfTargetTemplate(targetTemplateId).success(function(groups) {
                model.kpiGroups = groups;
            }).error(function(response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getKPIOfTargetTemplate(targetTemplateId, onlySelected) {
            return targetTemplateFactory.getKPIOfTargetTemplate(targetTemplateId, onlySelected).success(
                function(kpiGroups) {
                    model.kpiList = kpiGroups;
                }).error(function(response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getPermissionOnTargetTemplate() {
            return targetTemplateFactory.getPermissonOnTargetTemplate().success(function(permission) {
                model.permission = permission;
            }).error(function(response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getRatingType() {
            return targetTemplateFactory.getRatingType().success(function(ratingTypes) {
                model.ratingTypes = ratingTypes;
            }).error(function(response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getScoreRatingType(scoreRatingTypeId, isActive) {
            return targetTemplateFactory.getScoreRatingType(scoreRatingTypeId, isActive).success(
                function(scoreRatingTypes) {
                    model.scoreRatingTypes = scoreRatingTypes;
                }).error(function(response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getTargetTemplate(targetTypeId, isActive, departmentId, isGroup, scoreRatingTypeId) {
            return targetTemplateFactory.getTargetTemplate(targetTypeId, isActive, departmentId, isGroup, scoreRatingTypeId).success(function(targetTemplates) {
                model.targetTemplates = targetTemplates;
            }).error(function(response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getTargetTemplateById(targetTemplateId) {
            return targetTemplateFactory.getTargetTemplateById(targetTemplateId).success(function(targetTemplate) {
                model.targetTemplate = targetTemplate;
            }).error(function(response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function openPopupAddKPIGroup(vm, targetTemplateId, maxWeight) {
            vm.modalInstance = $uibModal.open({
                templateUrl: 'SourceApp/Components/TargetTemplate/Templates/addKPIGroup.html',
                controller: 'AddedKPIGroupController as addKPIGroupCtrl',
                backdrop: 'static',
                resolve: {
                    targetTemplateId: function() {
                        return targetTemplateId;
                    },
                    maxWeight: function() {
                        return maxWeight;
                    }
                },
                windowClass: 'modal-confirm-center kpi-group-popup'
            });

            return vm.modalInstance.result;
        }

        function openPopupCreateTargetTemplate(vm) {
            vm.modalInstance = $uibModal.open({
                templateUrl: '/SourceApp/Components/TargetTemplate/Templates/create.html',
                controller: 'CreateTargetTemplateController as createTargetTemplateCtrl',
                backdrop: 'static',
                windowClass: 'modal-confirm-center'
            });

            return vm.modalInstance.result;
        }

        function openPopupDeleteKPIGroup(targetTemplateId, kpiGroup) {
            var modalInstance = $uibModal.open({
                templateUrl: '/SourceApp/Shared/Templates/ModalConfirmContent.html',
                controller: 'ModalConfirmController as modalConfirmCtrl',
                backdrop: 'static',
                resolve: {
                    name: function() {
                        return kpiGroup.Name;
                    },
                    action: function() {
                        return 'delete';
                    }
                },
                windowClass: 'modal-confirm-center'
            });

            return modalInstance.result.then(function() {
                targetTemplateFactory.deleteKPIGroupOfTargetTemplate(targetTemplateId, kpiGroup.KPIGroupId).success(
                    function(response) {
                        if (response.IsSuccessful) {
                            notificationFactory.success('DeletedSuccessfully');
                        } else {
                            notificationFactory.error('', response.Message);
                        }
                    }).error(function(response, status) {
                    errorFactory.errorHandle(status);
                });
            })['finally'](function() {
                modalInstance = '';
            });
        }

        function openPopupDeleteTargetTemplate(vm, targetTemplate) {
            var modalInstance = $uibModal.open({
                templateUrl: '/SourceApp/Shared/Templates/ModalConfirmContent.html',
                controller: 'ModalConfirmController as modalConfirmCtrl',
                backdrop: 'static',
                resolve: {
                    name: function() {
                        return targetTemplate.Name;
                    },
                    action: function() {
                        return 'delete';
                    }
                },
                windowClass: 'modal-confirm-center'
            });

            modalInstance.result.then(function() {
                targetTemplateFactory.deleteTargetTemplate(targetTemplate.TargetTemplateId).success(function(response) {
                    if (response.IsSuccessful) {
                        var index = vm.targetTemplates.indexOf(targetTemplate);
                        vm.targetTemplates.splice(index, 1);
                        utilitiesFactory.searchOnPage(vm, vm.targetTemplates);
                        notificationFactory.success('DeletedSuccessfully');
                    } else {
                        notificationFactory.error('', response.Message);
                    }
                }).error(function(response, status) {
                    errorFactory.errorHandle(status);
                });
            })['finally'](function() {
                modalInstance = '';
            });
        }

        function openPopupUpdateStatusTargetTemplate(targetTemplate) {
            targetTemplate.IsActive = !targetTemplate.IsActive;

            var action = targetTemplate.IsActive ? 'activate' : 'deactivate',
                newTargetTemplate = {
                    TargetTemplateId: targetTemplate.TargetTemplateId,
                    IsActive: targetTemplate.IsActive
                };

            var modalInstance = $uibModal.open({
                templateUrl: '/SourceApp/Shared/Templates/ModalConfirmContent.html',
                controller: 'ModalConfirmController as modalConfirmCtrl',
                backdrop: 'static',
                resolve: {
                    name: function() {
                        return targetTemplate.Name;
                    },
                    action: function() {
                        return action;
                    }
                },
                windowClass: 'modal-confirm-center'
            });

            var keyMessage = '';

            modalInstance.result.then(function() {
                    targetTemplateFactory.updateStatusTargetTemplate(newTargetTemplate).success(function(response) {
                        if (response.IsSuccessful) {
                            keyMessage = action === 'activate' ? 'ActivationSuccess' : 'DeactivationSuccess';
                            notificationFactory.success(keyMessage);
                        } else {
                            keyMessage = action === 'activate' ? 'ActivationFail' : 'DeactivationFail';
                            targetTemplate.IsActive = !newTargetTemplate.IsActive;
                            notificationFactory.error(keyMessage);
                        }
                    }).error(function(response, status) {
                        if (status === 405) {
                            errorFactory.errorHandle(status);
                        } else {
                            keyMessage = action === 'activate' ? 'ActivationFail' : 'DeactivationFail';
                            targetTemplate.IsActive = !newTargetTemplate.IsActive;
                            notificationFactory.error(keyMessage);
                        }
                    });
                },
                function() {
                    targetTemplate.IsActive = !newTargetTemplate.IsActive;
                })['finally'](function() {
                modalInstance = '';
            });
        }

        function updateTargetTemplate(targetTemplate, totalWeight, vm) {
            return targetTemplateFactory.updateTargetTemplate(targetTemplate).success(function(response) {
                if (response.IsSuccessful) {
                    if (totalWeight !== vm.maxWeight) {
                        var message =
                            utilitiesFactory.formatString(vm.totalWeightWarningMessage, vm.maxWeight);
                        notificationFactory.error('', message);
                    } else {
                        notificationFactory.success('UpdatedSuccessfully');
                    }
                } else {
                    notificationFactory.error('', response.Message);
                }
            }).error(function(response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function updateTargetTemplateBasicInfo(targetTemplate, originalScoreTypeId) {
            return targetTemplateFactory.updateTargetTemplateBasicInfo(targetTemplate).success(function(response) {
                if (response.IsSuccessful) {
                    if (targetTemplate.ScoreRatingTypeId !== originalScoreTypeId) {
                        notificationFactory.success('ChangeScoreRatingTypeWarning');
                    } else {
                        notificationFactory.success('UpdatedSuccessfully');
                    }
                } else {
                    notificationFactory.error('', response.Message);
                }
            }).error(function(response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function validateKPIGroupWeight(kpiGroups, maxWeight, totalWeightWarningMessage) {
            var hasWeight = true,
                totalWeight = 0,
                isValid = true;

            angular.forEach(kpiGroups,
                function(group) {
                    if (!group.Weight) {
                        hasWeight = false;
                    }

                    totalWeight += group.Weight;
                });

            if (!hasWeight) {
                notificationFactory.error('WeightKPIGroupRequired');
                isValid = false;
            } else if (utilitiesFactory.parseFloat(totalWeight) !== maxWeight) {
                var message = utilitiesFactory.formatString(totalWeightWarningMessage, maxWeight);
                notificationFactory.error('', message);
                isValid = false;
            }

            return isValid;
        }

        function validateTargetTemplate(targetTemplate) {
            var isValid = true;

            if (!targetTemplate) {
                isValid = false;
            } else if (!targetTemplate.Name) {
                notificationFactory.error('NameRequired');
                isValid = false;
            } else if (!targetTemplate.TargetTypeId) {
                notificationFactory.error('TargetTypeRequired');
                isValid = false;
            } else if (!targetTemplate.ScoreRatingTypeId) {
                notificationFactory.error('ScoreRatingTypeRequired');
                isValid = false;
            } else if (!targetTemplate.RatingTypeId) {
                notificationFactory.error('RatingTypeRequired');
                isValid = false;
            }

            return isValid;
        }
    }
})(angular);
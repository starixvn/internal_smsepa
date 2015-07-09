'use strict';
/*global commonModule*/

commonModule.factory('CommonModel', ['commonFactory', 'errorFactory',
function (commonFactory, errorFactory) {
    var commonModel = {};

    commonModel.targetTypes = [];
    commonModel.systemRoles = [];
 
    /// Get target type.
    commonModel.getTargetType = function (isActive) {
        return commonFactory.getTargetType(isActive)
        .success(function (targetTypes) {
            commonModel.targetTypes = targetTypes;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    /// Get index of row in table.
    commonModel.getIndex = function (index, currentPage, pageSize) {
        var indexOfRow = pageSize * (currentPage - 1) + index + 1;
        return indexOfRow;
    };

    commonModel.getSystemRoles = function (exceptNone) {
        return commonFactory.getSystemRoles(exceptNone).success(function (systemRoles) {
            commonModel.systemRoles = systemRoles;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    return commonModel;
} ]);
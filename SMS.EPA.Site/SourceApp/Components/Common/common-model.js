'use strict';
/*global commonModule*/

commonModule.factory('CommonModel', ['commonFactory', 'errorFactory',
function (commonFactory, errorFactory) {
    var commonModel = {};

    commonModel.targetTypes = [];

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

    return commonModel;
} ]);
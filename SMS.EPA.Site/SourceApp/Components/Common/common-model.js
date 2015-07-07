'use strict';
/*global commonModule*/

commonModule.factory('CommonModel', [function () {
    var commonModel = {};

    /// Get index of row in table.
    commonModel.getIndex = function (index, currentPage, pageSize) {
        var indexOfRow = pageSize * (currentPage - 1) + index + 1;
        return indexOfRow;
    };

    return commonModel;
} ]);
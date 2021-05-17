(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('jobDescriptionModule').directive('moveItemRow', moveItemRow);
    moveItemRow.$inject = [];

    function moveItemRow() {
        return {
            link: function (scope, iElement) {
                iElement.on('click', function () {
                    var row = $(this).parents("table#tblItem tbody tr.row-data:first");
                    var firstItem = $(this).parents('table#tblItem tbody').find('tr.row-data:first');
                    var lastItem = $(this).parents('table#tblItem tbody').find('tr:last');

                    if ($(this).is(".itemUp")) {
                        if ($(this).parents('tr')[0] !== firstItem[0]) {
                            row.insertBefore(row.prev());
                        }
                    } else if ($(this).is(".itemDown")) {
                        row.insertAfter(row.next());
                    } else if ($(this).is(".itemBottom")) {
                        if ($(this).parents('tr')[0] !== lastItem[0]) {
                            row.insertAfter(lastItem);
                        }
                    } else if ($(this).is(".itemTop")) {
                        if ($(this).parents('tr')[0] !== firstItem[0]) {
                            row.insertBefore(firstItem);
                        }
                    }
                });
            }
        };
    }
})(angular);
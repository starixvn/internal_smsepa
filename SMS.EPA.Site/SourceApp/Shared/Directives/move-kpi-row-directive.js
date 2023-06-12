(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('appModule').directive('moveKpiRow', moveKpiRow);
    moveKpiRow.$inject = [];

    function moveKpiRow() {
        return {
            link: function (scope, iElement) {
                iElement.on('click', function () {
                    var row = $(this).parents("table#tblKPI tbody tr.row-data:first");
                    var firstItem = $(this).parents('table#tblKPI tbody').find('tr.row-data:first');
                    var lastItem = $(this).parents('table#tblKPI tbody').find('tr:last');

                    if ($(this).is(".kpiUp")) {
                        if ($(this).parents('tr')[0] !== firstItem[0]) {
                            row.insertBefore(row.prev());
                        }
                    } else if ($(this).is(".kpiDown")) {
                        row.insertAfter(row.next());
                    } else if ($(this).is(".kpiBottom")) {
                        if ($(this).parents('tr')[0] !== lastItem[0]) {
                            row.insertAfter(lastItem);
                        }
                    } else if ($(this).is(".kpiTop")) {
                        if ($(this).parents('tr')[0] !== firstItem[0]) {
                            row.insertBefore(firstItem);
                        }
                    }
                });
            }
        };
    }
})(angular);
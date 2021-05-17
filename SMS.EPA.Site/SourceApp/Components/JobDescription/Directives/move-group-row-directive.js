(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('jobDescriptionModule').directive('moveGroupRow', moveGroupRow);
    moveGroupRow.$inject = [];

    function moveGroupRow() {
        return {
            link: function (scope, iElement) {
                iElement.on('click', function () {
                    var row = $(this).parents("table#tblGroup tbody tr.row-data:first");
                    var firstItem = $(this).parents('table#tblGroup tbody').find('tr.row-data:first');
                    var lastItem = $(this).parents('table#tblGroup tbody').find('tr:last');

                    if ($(this).is(".groupUp")) {
                        if ($(this).parents('tr')[0] !== firstItem[0]) {
                            row.insertBefore(row.prev());
                        }
                    } else if ($(this).is(".groupDown")) {
                        row.insertAfter(row.next());
                    } else if ($(this).is(".groupBottom")) {
                        if ($(this).parents('tr')[0] !== lastItem[0]) {
                            row.insertAfter(lastItem);
                        }
                    } else if ($(this).is(".groupTop")) {
                        if ($(this).parents('tr')[0] !== firstItem[0]) {
                            row.insertBefore(firstItem);
                        }
                    }
                });
            }
        };
    }
})(angular);
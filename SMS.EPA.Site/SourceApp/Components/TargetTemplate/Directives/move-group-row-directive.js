(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('targetTemplateModule').directive('moveGroupRow', moveGroupRow);
    moveGroupRow.$inject = [];

    function moveGroupRow() {
        return {
            link: function (scope, iElement) {
                iElement.on('click', function () {
                    var row = $(this).parents("tr:first");
                    if ($(this).is(".up")) {
                        row.insertBefore(row.prev());
                    } else if ($(this).is(".down")) {
                        row.insertAfter(row.next());
                    } else if ($(this).is(".bottom")) {
                        row.insertAfter($("table#tblKPIGroup tbody tr:last"));
                    } else if ($(this).is(".top")) {
                        row.insertBefore($("table#tblKPIGroup tbody tr:first"));
                    }
                });
            }
        };
    }
})(angular);
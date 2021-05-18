(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('jobDescriptionModule').directive('listSortable', listSortable);
    listSortable.$inject = [];

    function listSortable() {
        return {
            link: function (scope, el) {
                el.closest('.table').sortable(
                    {
                        revert: true,
                        items: '> tbody',
                        cursor: 'move'
                    });

                el.sortable({
                    revert: true,
                    items: 'tr:not(.unsortable)',
                    cursor: 'move'
                });
            }
        };
    }
})(angular);
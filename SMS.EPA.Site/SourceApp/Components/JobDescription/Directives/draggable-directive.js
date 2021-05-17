(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('jobDescriptionModule').directive('listDraggable', listDraggable);
    listDraggable.$inject = [];

    function listDraggable() {
        return {
            link: function (scope, el, attrs) {
                el.draggable({
                    connectToSortable: attrs.listDraggable,
                    helper: "clone",
                    revert: "invalid"
                });
                el.disableSelection();
            }
        };
    }
})(angular);
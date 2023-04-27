(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('targetTemplateModule').directive('listDraggable', listDraggable);
    listDraggable.$inject = [];

    function listDraggable() {
        return {
            link: function (scope, el, attrs) {
                el.draggable({
                    connectToSortable: attrs.listDraggable,
                    helper: "clone", //drag
                    revert: "invalid"
                });
                el.disableSelection();
            }
        };
    }
})(angular);
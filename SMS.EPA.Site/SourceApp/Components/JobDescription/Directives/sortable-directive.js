(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('jobDescriptionModule').directive('listSortable', listSortable);
    listSortable.$inject = [];

    function listSortable() {
        return {
            link: function (scope, el) {
                var currentUserId;
                el.sortable({
                    revert: true,
                    receive: function (event, ui) {
                        currentUserId = ui.sender.attr("id");
                    },
                    items: "tr:not(.unsortable), li:not(.unsortable)"
                });

                el.on("sortdeactivate", function (event, ui) {
                    var from = angular.element(ui.item).scope().$index;
                    var to = el.children().index(ui.item);
                    if (to >= 0) {
                        scope.$apply(function () {
                            if (from >= 0) {
                                scope.$emit('list-sorted', { from: from, to: to, sender: ui.sender.attr("id") });
                            } else {
                                scope.$emit('list-created', { to: to, name: ui.item.text(), id: currentUserId, sender: ui.sender.attr("id") });
                                ui.item.remove();
                            }
                        });
                    }
                });
            }
        };
    }
})(angular);
'use strict';
/*global targetTemplateModule*/

targetTemplateModule.directive('listSortable', function () {
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
});

targetTemplateModule.directive('listDraggable', function () {
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
});

targetTemplateModule.directive('getGroupWeight', function () {
    return {
        restrict: 'A',
        link: function (scope, iElement) {
            iElement.on('click', function () {
                var selectedGroups = [],
                    groupTable = $('#tblKPIGroup'),
                    selectedRows = groupTable.find('tr').has('.check-group:checked');

                angular.forEach(selectedRows, function (selectedRow, index) {
                    var selectedRowElement = $(selectedRow),
                        group = {
                            KPIGRoupId: parseInt(selectedRowElement.find('td').attr('data-id')),
                            Weight: parseInt(selectedRowElement.find("[name='weight']").val()),
                            IsSpreadKPI: selectedRowElement.find("[name='spread']").is(':checked'),
                            DisplayOrder: index + 1
                        };

                    selectedGroups.push(group);
                });

                scope.addKPIGroupCtrl.save.call(this, selectedGroups);
            });
        }
    };
});
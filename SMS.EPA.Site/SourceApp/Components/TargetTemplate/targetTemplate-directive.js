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

targetTemplateModule.directive('moveGroupRow', function () {
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
});

targetTemplateModule.directive('moveKpiRow', function () {
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
                            KPIGroupId: parseInt(selectedRowElement.find('td').attr('data-id')),
                            Weight: parseFloat(selectedRowElement.find("[name='weight']").val()),
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

targetTemplateModule.directive('getWeight', ['notificationFactory', function (notificationFactory) {

    var validateKPIList = function (groups) {
        var isValid = true;

        angular.forEach(groups, function (group) {
            if (!group.IsSpreadKPI) {
                var totalWeightOfGroup = 0,
                    kpis = group.KPIs;

                for (var i = 0; i < kpis.length; i++) {
                    var kpiItem = kpis[i];

                    if (kpiItem.Selected) {
                        if (!kpiItem.Weight || parseFloat(kpiItem.Weight) === 0) {
                            notificationFactory.error("WeightKPIRequired");
                            isValid = false;
                            break;
                        }
                        else {
                            totalWeightOfGroup += parseFloat(kpiItem.Weight);
                        }
                    }
                }

                if (isValid && totalWeightOfGroup !== group.Weight) {
                    isValid = false;
                    notificationFactory.error("TotalKPIWeightWarning");
                }
            }
            
        });

        return isValid;
    };

    return {
        restrict: 'A',
        link: function (scope, iElement) {
            iElement.on('click', function () {
                if (!scope.kpiListCtrl || validateKPIList(scope.kpiListCtrl.kpiList)) {
                    var KPIs = [],
                        groupTable = $('#tblKPI'),
                        selectedRows = groupTable.find('tr.row-data').has('.check-group:checked');

                    angular.forEach(selectedRows, function (selectedRow, index) {
                        var selectedRowElement = $(selectedRow),
                            weight = selectedRowElement.find("[name='weight']").val(),
                            kpi = {
                                KPIId: parseInt(selectedRowElement.find('td').attr('item-id')),
                                KPIGroupId: parseInt(selectedRowElement.find('td').attr('group-id')),
                                Weight: weight ? parseFloat(weight) : 0,
                                DisplayOrder: index + 1
                            };

                        KPIs.push(kpi);
                    });

                    scope.updateCtrl.targetTemplate.KPIIds = KPIs;
                    scope.updateCtrl.save.call(this, scope.updateCtrl.targetTemplate);
                }
            });
        }
    };
}]);
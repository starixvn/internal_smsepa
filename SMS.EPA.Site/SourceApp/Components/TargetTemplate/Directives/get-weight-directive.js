(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('targetTemplateModule').directive('getWeight', getWeight);
    getWeight.$inject = ['utilitiesFactory', 'notificationFactory'];

    function getWeight(utilitiesFactory, notificationFactory) {
        function validateKPIList(groups) {
            var isValid = true,
                hasSelected = false;

            angular.forEach(groups, function (group) {
                if (!group.IsSpreadKPI) {
                    var totalWeightOfGroup = 0, weight,
                        kpis = group.KPIs;

                    for (var i = 0; i < kpis.length; i++) {
                        var kpiItem = kpis[i];

                        if (kpiItem.Selected) {
                            hasSelected = true;

                            weight = parseFloat(kpiItem.Weight);
                            if (!weight) {
                                notificationFactory.error("WeightKPIRequired");
                                isValid = false;
                                break;
                            } else {
                                totalWeightOfGroup += weight;
                            }
                        }
                    }

                    if (hasSelected && isValid
                        && utilitiesFactory.parseFloat(totalWeightOfGroup) !== utilitiesFactory.parseFloat(group.Weight)) {
                        isValid = false;
                        notificationFactory.error("TotalKPIWeightWarning");
                    }
                }
            });

            return isValid;
        }

        return {
            restrict: 'A',
            link: function (scope, iElement) {
                iElement.on('click', function () {
                    if (scope.kpiListCtrl && validateKPIList(scope.kpiListCtrl.kpiList)) {
                        var KPIs = [],
                            groupTable = $('#tblKPI'),
                            selectedRows = groupTable.find('tr.row-data').has('.check-group:checked');

                        angular.forEach(selectedRows, function (selectedRow, index) {
                            var selectedRowElement = $(selectedRow),
                                kpiGroupId = parseInt(selectedRowElement.find('td').attr('group-id')),
                                indexKPIGroup = scope.kpiListCtrl.kpiList.map(function (e) { return e.KPIGroupId; }).indexOf(kpiGroupId),
                                isSpreadKPI = scope.kpiListCtrl.kpiList[indexKPIGroup].IsSpreadKPI,
                                weight = selectedRowElement.find("[name='weight']").val(),
                                isRequiredEvaluate = selectedRowElement.find("[name='IsRequiredEvaluate']").prop('checked'),
                                hasComment = selectedRowElement.find("[name='HasComment']").prop('checked'),
                                isRequiredComment = selectedRowElement.find("[name='IsRequiredComment']").prop('checked'),
                                kpi = {
                                    KPIId: parseInt(selectedRowElement.find('td').attr('item-id')),
                                    KPIGroupId: kpiGroupId,
                                    IsSpreadKPI: isSpreadKPI,
                                    Weight: weight ? parseFloat(weight) : 0,
                                    DisplayOrder: index + 1,
                                    IsRequiredEvaluate: isRequiredEvaluate,
                                    HasComment: hasComment,
                                    IsRequiredComment: isRequiredComment
                                };

                            KPIs.push(kpi);
                        });

                        scope.updateCtrl.targetTemplate.KPIIds = KPIs;
                        scope.kpiListCtrl.save.call(this, scope.updateCtrl.targetTemplate);
                    }
                });
            }
        };
    }
})(angular);
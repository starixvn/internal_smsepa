(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('jobLevelStandardModule').directive('getData', getData);
    getData.$inject = ['utilitiesFactory', 'notificationFactory'];

    function getData(utilitiesFactory, notificationFactory) {
        function validateKPIList(listKPI) {
            var isValid = true,
                totalWeightOfGroup = 0,
                weight;

            for (var i = 0; i < listKPI.length; i++) {
                var kpi = listKPI[i];
                if (kpi.IsSelected) {
                    weight = parseFloat(kpi.Weight);
                    if (!weight) {
                        notificationFactory.error("WeightKPIRequired");
                        isValid = false;
                        break;
                    } else {
                        totalWeightOfGroup += weight;
                    }
                }
            }

            if (isValid && utilitiesFactory.parseFloat(totalWeightOfGroup) !== utilitiesFactory.parseFloat(100)) {
                isValid = false;
                notificationFactory.error("TotalKPIWeightOfJLSWarning");
            }

            return isValid;
        }

        return {
            restrict: 'A',
            link: function (scope, iElement) {
                iElement.on('click', function () {
                    if (scope.kpiListCtrl && validateKPIList(scope.kpiListCtrl.kpis)) {
                        var KPIs = [],
                            groupTable = $('#tblKPI'),
                            selectedRows = groupTable.find('tr.row-data').has('.check-group:checked');

                        angular.forEach(selectedRows, function (selectedRow, index) {
                            var selectedRowElement = $(selectedRow),
                                weight = selectedRowElement.find("[name='weight']").val(),
                                kpi = {
                                    KPIId: parseInt(selectedRowElement.find('td').attr('item-id')),
                                    DisplayOrder: index + 1,
                                    Weight: weight ? parseFloat(weight) : 0,
                                };

                            KPIs.push(kpi);
                        });

                        scope.updateCtrl.template.KPIIds = KPIs;
                        scope.kpiListCtrl.save.call(this, scope.updateCtrl.template);
                    }
                });
            }
        };
    }
})(angular);
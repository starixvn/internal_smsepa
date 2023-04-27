(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('jobLevelStandardModule').directive('getData', getData);
    getData.$inject = ['utilitiesFactory', 'notificationFactory'];

    function getData(utilitiesFactory, notificationFactory) {
        return {
            restrict: 'A',
            link: function (scope, iElement) {
                iElement.on('click', function () {
                    if (scope.kpiListCtrl) {
                        var KPIs = [],
                            groupTable = $('#tblKPI'),
                            selectedRows = groupTable.find('tr.row-data').has('.check-group:checked');

                        angular.forEach(selectedRows, function (selectedRow, index) {
                            var selectedRowElement = $(selectedRow),
                                kpi = {
                                    KPIId: parseInt(selectedRowElement.find('td').attr('item-id')),
                                    DisplayOrder: index + 1,
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
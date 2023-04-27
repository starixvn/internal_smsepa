(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('targetTemplateModule').directive('getGroupWeight', getGroupWeight);
    getGroupWeight.$inject = [];

    function getGroupWeight() {
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
    }
})(angular);
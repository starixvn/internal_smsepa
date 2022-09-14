(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('appModule').directive('multiSelectTreeView', multiSelectTreeView);
    multiSelectTreeView.$inject = ['$timeout'];

    function multiSelectTreeView($timeout) {
        return {
            restrict: 'E',
            scope: {
                options: '=',
                selectedItems: '=',
                onSelect: '='
            },
            link: function (scope, iElement, iAttr) {
                scope.$watch('options', function (options) {
                    $timeout(function () {
                        $(iElement).multiSelectTreeView({
                            data: options,
                            singleSelection: iAttr.singleSelection !== undefined ? JSON.parse(iAttr.singleSelection) : false,
                            selectedItems: scope.selectedItems,
                            isSelectedAll: iAttr.isSelectedAll !== undefined ? JSON.parse(iAttr.isSelectedAll) : false,
                            onSelect: function () {
                                if (typeof scope.onSelect === 'function') {
                                    scope.onSelect($(iElement).multiSelectTreeView('getSelectedItems'));
                                }
                            },
                            onSelectAll: function () {
                                if (typeof scope.onSelect === 'function') {
                                    scope.onSelect($(iElement).multiSelectTreeView('getSelectedItems'));
                                }
                            }
                        });
                    });
                });
            }
        };
    }
})(angular);
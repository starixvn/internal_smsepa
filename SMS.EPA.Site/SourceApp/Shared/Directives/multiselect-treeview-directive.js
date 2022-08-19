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
                selectedItems: '='
            },
            link: function (scope, iElement, iAttr) {
                scope.$watch('options', function (options) {
                    $timeout(function () {
                        $(iElement).multiSelectTreeView({
                            data: options,
                            selectedItems: scope.selectedItems,
                            isSelectedAll: JSON.parse(iAttr.isSelectedAll),
                            onClose: function () {
                                $timeout(function () {
                                    scope.selectedItems = $(iElement).multiSelectTreeView('getSelectedItems');
                                });
                            }
                        });
                    });
                });
            }
        };
    }
})(angular);
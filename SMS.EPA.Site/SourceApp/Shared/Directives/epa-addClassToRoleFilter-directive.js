(function (angular) {
    'use strict';

    angular.module('appModule').directive('epaAddClassToRoleFilter', addClassToRoleFilter);
    addClassToRoleFilter.$inject = ['$compile'];

    // remove class button, add class form-control to multi-select
    function addClassToRoleFilter($compile) {
        return {
            restrict: 'E',
            link: function (scope, iElement) {
                var button = iElement.find('button.multiSelectButton');

                button.removeClass('button').addClass('form-control');
                $compile(button)(scope);
            }
        };
    }
})(angular);
(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('appModule').directive('searchAutoComplete', searchAutoComplete);
    searchAutoComplete.$inject = ['$timeout', 'errorFactory'];

    function searchAutoComplete($timeout, errorFactory) {
        return {
            scope: {
                employee: '=',
                searchEmployees: '&',
                employeeStyle: '@',
                onSelect: '=',
                sender: '@'
            },
            link: function (scope, iElement) {
                iElement.autocomplete({
                    source: function (request, response) {
                        scope.searchEmployees().success(function (employees) {
                            var data = [];
                            angular.forEach(employees, function (employee) {
                                data.push(
                                {
                                    value: employee.NickName + ' (' + employee.FullName + ')',
                                    id: employee.EmployeeId,
                                    employee: {
                                        EmployeeId: employee.EmployeeId,
                                        Name: employee.NickName,
                                        HasMonthlyEPA: employee.HasMonthlyEPA,
                                        Avatar: employee.Avatar,
                                        NickName: employee.NickName
                                    }
                                });
                            });

                            response(data);

                            if (scope.employeeStyle) {
                                if (data.length > 0) {
                                    angular.element('.ui-autocomplete').attr('style', scope.employeeStyle);
                                } else {
                                    angular.element('.ui-autocomplete').attr('style', 'display: none;');
                                }
                            }
                        }).error(function (response, status) {
                            errorFactory.errorHandle(status);
                        });
                    },
                    select: function (event, ui) {
                        scope.$apply();
                        scope.employee = ui.item.employee;
                        if (scope.sender !== undefined && typeof scope.onSelect === 'function') {
                            scope.onSelect(scope.sender, ui.item.employee);
                        }
                        $timeout(function () {
                            //iElement.trigger('input');
                        }, 0);
                    },
                    appendTo: iElement.parent(),
                    delay: 500,
                    minLength: 2
                });
            }
        };
    }
})(angular);
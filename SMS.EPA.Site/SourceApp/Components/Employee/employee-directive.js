'use strict';
/*global employeeModule*/

// get user permission when click save button
employeeModule.directive('getEmployeePermission', function () {
    return {
        restrict: "A",
        link: function (scope, iElement) {
            iElement.on("click", function () {
                var roleDiv = $("[name='employeeRoles']"),
                roleItems = $(roleDiv).find("[name='role']:checked"),
                roles = '';

                if (roleItems) {
                    for (var i = 0; i < roleItems.length; i++) {
                        var item = roleItems[i],
                            roleId = $(item).val();
                        roles = roles !== '' ? roles + ',' + roleId : roleId;
                    }
                }

                // call save function
                scope.assignRoleCtrl.save.call(this, roles);
            });
        }
    };
});
(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    // Assign Permission to Role
    angular.module('roleModule').directive('assignPermission', assignPermission);
    // get role permission when click save button
    angular.module('roleModule').directive('getRolePermission', getRolePermission);

    function assignPermission() {
        var updateParent = function (checked, type) {
            var pid = $(this).attr('pid');

            if (pid && pid.length > 0) {
                var parent = $("#func" + type + pid);

                if (checked && !parent.is(':checked')) {
                    parent.prop('checked', true);
                }

                if (!checked) {
                    var siblings = $("[name='func" + type + "'][pid='" + parent.val() + "']:checked");

                    if (!siblings || (siblings && siblings.length === 0)) {
                        parent.prop('checked', false);
                    }
                }

                updateParent.call(parent.get(0), checked, type);
            }
        };

        return {
            restrict: "A",
            link: function (scope, iElement) {
                iElement.on("change", function () {
                    var permissionType = $(this).attr('name'),
                    currentId = $(this).val(),
                    checked = $(this).is(':checked');

                    if (permissionType === 'funcHost') {
                        if (checked) {
                            $("#funcRead" + currentId).prop('checked', true).trigger('change');
                        }

                        // parent changed -> children changed
                        $("[name='funcHost'][pid='" + currentId + "']").prop('checked', checked).trigger('change');

                        // update parent
                        updateParent.call(this, checked, "Host");
                    } else {
                        if (!checked) {
                            $("#funcHost" + currentId).prop('checked', false).trigger('change');
                        }

                        // parent changed -> children changed
                        $("[name='funcRead'][pid='" + currentId + "']").prop('checked', checked).trigger('change');

                        // update parent
                        updateParent.call(this, checked, "Read");
                    }
                });
            }
        };
    }

    function getRolePermission() {
        return {
            restrict: "A",
            link: function (scope, iElement) {
                iElement.on("click", function () {
                    var permissions = [],
                        table = $("[name='tableFunction']"),
                        roleFunction = $(table).find('[type=checkbox][name=funcRead]');

                    if (roleFunction) {
                        for (var i = 0; i < roleFunction.length; i++) {
                            var element = roleFunction[i];
                            var functionId = $(element).val();
                            permissions.push({
                                functionId: parseInt(functionId, 10),
                                isRead: $(element).is(':checked'),
                                isHost: $('#funcHost' + functionId).is(':checked')
                            });
                        }

                        // call save function
                        scope.roleCtrl.save.call(this, {
                            permissions,
                            roleId: scope.roleCtrl.filter.roleId
                        });
                    }
                });
            }
        };
    }
})(angular);
(function (angular) {
    "use strict";

    angular.module('commonModule').directive("menuTab", menuTabDirective);
    menuTabDirective.$inject = ["$location"];

    function menuTabDirective($location) {
        return {
            restrict: "A",
            link: function (scope, element, attrs) {
                var selfPath = attrs.link;
                scope.location = $location;

                element.bind("click", function () {
                    $location.path(selfPath).search({});

                    scope.$apply();
                });
            }
        };
    }
})(angular);
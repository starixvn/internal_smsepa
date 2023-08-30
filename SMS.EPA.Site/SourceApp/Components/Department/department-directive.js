(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('departmentModule').directive('treeEvent', treeEvent);

    function treeEvent() {
        return {
            link: function ($scope, iElement) {
                $scope.$watch(function () {
                    let parents = $(iElement).find('.tree ul'),
                        childs = parents.find('> li');

                    childs.each(function () {
                        let li = $(this).find('ul > li');
                        if (li.length > 0) {
                            $(this).addClass('parent_li').find('> span').attr('title', 'Collapse this branch');
                        }
                    });
                });
            }
        };
    }
})(angular);
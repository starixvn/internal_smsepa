'use strict';
/*global departmentModule*/

departmentModule.directive('treeEvent', function () {
    return {
        link: function ($scope, iElement) {
            $scope.$watch(function () {
                var parents = $(iElement).find('.tree ul'),
                    childs = parents.find('> li');

                childs.each(function () {
                    var li = $(this).find('ul > li');
                    if (li.length > 0) {
                        $(this).addClass('parent_li').find('> span').attr('title', 'Collapse this branch');
                    };
                });
            });
        }
    };
});
(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('disciplineModule').factory('DisciplineSummaryModel', disciplineSummaryModel);
    disciplineSummaryModel.$inject = ['$uibModal', 'disciplineSummaryFactory', 'errorFactory'];

    function disciplineSummaryModel($uibModal, disciplineSummaryFactory, errorFactory) {
        var date = new Date(),
            summaryPaging = {
                currentPage: 1,
                pageSize: 20
            },
            rangeDateFilter = {
                firstRangeStartDate: new Date(date.getFullYear(), 0, 1), // 1-1
                firstRangeEndDate: new Date(date.getFullYear(), 6, 0), // 31-06
                firstStartDate: new Date(date.getFullYear() - 1, 11, 1), // 1-12-last year
                firstEndDate: new Date(date.getFullYear(), 5, 0), // 31-05
                secondRangeStartDate: new Date(date.getFullYear(), 6, 1), // 1-7
                secondRangeEndDate: new Date(date.getFullYear(), 12, 0), // 31-12
                secondStartDate: new Date(date.getFullYear(), 5, 1), // 1-6
                secondEndDate: new Date(date.getFullYear(), 11, 0), // 30-11
            };

        var model = {
            employeeDisciplines: [],
            getDisciplineSummary: getDisciplineSummary,
            getDisciplineSummaryForOtherSystem: getDisciplineSummaryForOtherSystem,
            getRangeDateFilter: getRangeDateFilter,
            permission: 0,
            rangeDateFilter: rangeDateFilter,
            resetSummaryPaging: resetSummaryPaging,
            setSummaryPaging: setSummaryPaging,
            summaryPaging: summaryPaging
        };

        return model;

        function getDisciplineSummary(startDate, endDate, teamIds, isLocal, employeeId, reasonId, isShowAll) {
            return disciplineSummaryFactory.getDisciplineSummary(startDate, endDate, teamIds, isLocal, employeeId, reasonId, isShowAll)
            .success(function (employeeDisciplines) {
                model.employeeDisciplines = employeeDisciplines;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getDisciplineSummaryForOtherSystem(startDate, endDate, teamIds, isLocal, employeeId, reasonId) {
            return disciplineSummaryFactory.getDisciplineSummaryForOtherSystem(startDate, endDate, teamIds, isLocal, employeeId, reasonId)
            .success(function (employeeDisciplines) {
                model.employeeDisciplines = employeeDisciplines;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getRangeDateFilter() {
            var startDate = new Date(),
                endDate = new Date(),
                dateNoTime = new Date(date.getFullYear(), date.getMonth(), date.getDate(), 0, 0, 0);

            if (dateNoTime >= model.rangeDateFilter.firstRangeStartDate && dateNoTime <= model.rangeDateFilter.firstRangeEndDate) {
                startDate = model.rangeDateFilter.firstStartDate;
                endDate = model.rangeDateFilter.firstEndDate;
            } else if (dateNoTime >= model.rangeDateFilter.secondRangeStartDate && dateNoTime <= model.rangeDateFilter.secondRangeEndDate) {
                startDate = model.rangeDateFilter.secondStartDate;
                endDate = model.rangeDateFilter.secondEndDate;
            }

            return {
                startDate: startDate,
                endDate: endDate
            };
        }

        function resetSummaryPaging() {
            model.summaryPaging.currentPage = 1;
            model.summaryPaging.pageSize = 20;
        }

        function setSummaryPaging(currentPage, pageSize) {
            model.summaryPaging.currentPage = currentPage;
            model.summaryPaging.pageSize = pageSize;
        }
    }
})(angular);
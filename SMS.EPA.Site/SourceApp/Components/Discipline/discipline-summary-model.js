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
                firstRangeStartDate: new Date(date.getFullYear(), 1, 1), // 1-2
                firstRangeEndDate: new Date(date.getFullYear(), 7, 0), // 31-07
                firstStartDate: new Date(date.getFullYear(), 0, 1), // 1-1
                firstEndDate: new Date(date.getFullYear(), 6, 0), // 30-06
                secondRangeStartDate: new Date(date.getFullYear(), 7, 1), // 1-8
                secondRangeEndDate: new Date(date.getFullYear() + 1, 1, 0), // 31-01-next year
                secondStartDate: new Date(date.getFullYear(), 6, 1), // 1-7
                secondEndDate: new Date(date.getFullYear(), 12, 0), // 31-12
                thirdRangeStartDate: new Date(date.getFullYear() - 1, 7, 1), // 1-8-last year
                thirdRangeEndDate: new Date(date.getFullYear(), 1, 0), // 31-1
                thirdStartDate: new Date(date.getFullYear() - 1, 6, 1), // 1-7-last year
                thirdEndDate: new Date(date.getFullYear() - 1, 12, 0) // 31-12-last year
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

        function getDisciplineSummary(startDate, endDate, departmentId, isLocal, employeeId, reasonId, isShowAll) {
            return disciplineSummaryFactory.getDisciplineSummary(startDate, endDate, departmentId, isLocal, employeeId, reasonId, isShowAll)
            .success(function (employeeDisciplines) {
                model.employeeDisciplines = employeeDisciplines;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getDisciplineSummaryForOtherSystem(periodId, disciplineTypeId, departmentId, isLocal, employeeId) {
            return disciplineSummaryFactory.getDisciplineSummaryForOtherSystem(periodId, disciplineTypeId, departmentId, isLocal, employeeId)
            .success(function (employeeDisciplines) {
                model.employeeDisciplines = employeeDisciplines;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getRangeDateFilter() {
            var startDate = new Date(),
                endDate = new Date(),
                date = new Date(),
                dateNoTime = new Date(date.getFullYear(), date.getMonth(), date.getDate(), 0, 0, 0);

            if (dateNoTime >= model.rangeDateFilter.firstRangeStartDate && dateNoTime <= model.rangeDateFilter.firstRangeEndDate) {
                startDate = model.rangeDateFilter.firstStartDate;
                endDate = model.rangeDateFilter.firstEndDate;
            } else if (dateNoTime >= model.rangeDateFilter.secondRangeStartDate && dateNoTime <= model.rangeDateFilter.secondRangeEndDate) {
                startDate = model.rangeDateFilter.secondStartDate;
                endDate = model.rangeDateFilter.secondEndDate;
            } else if (dateNoTime >= model.rangeDateFilter.thirdRangeStartDate && dateNoTime <= model.rangeDateFilter.thirdRangeEndDate) {
                startDate = model.rangeDateFilter.thirdStartDate;
                endDate = model.rangeDateFilter.thirdEndDate;
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
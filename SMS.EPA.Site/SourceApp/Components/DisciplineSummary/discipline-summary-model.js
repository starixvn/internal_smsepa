(function (angular) {
    'use strict';

    angular.module('disciplineModule').factory('DisciplineSummaryModel', disciplineSummaryModel);
    disciplineSummaryModel.$inject = ['disciplineSummaryFactory', 'errorFactory'];

    function disciplineSummaryModel(disciplineSummaryFactory, errorFactory) {
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
            disciplineTypes: [],
            employeeDisciplines: [],
            getDepartmentName: getDepartmentName,
            getDisciplineDetail: getDisciplineDetail,
            getDisciplineSummary: getDisciplineSummary,
            getDisciplineSummaryByPeriod: getDisciplineSummaryByPeriod,
            getDisciplineType: getDisciplineType,
            getOtherViolations: getOtherViolations,
            getRangeDateFilter: getRangeDateFilter,
            getUserPermissionOnDisciplineSummary: getUserPermissionOnDisciplineSummary,
            openPopupOtherViolation: openPopupOtherViolation,
            otherViolations: [],
            permission: 0,
            rangeDateFilter: rangeDateFilter,
            resetSummaryPaging: resetSummaryPaging,
            setSummaryPaging: setSummaryPaging,
            summaryPaging: summaryPaging
        };

        return model;

        function getDepartmentName(departmentId, departments) {
            var departmentName = '';

            angular.forEach(departments, function (department) {
                if (department.DepartmentId === departmentId) {
                    departmentName = department.Name;
                }
            });

            return departmentName;
        }

        function getDisciplineDetail(startDate, endDate, employeeId, departmentId, isLocal, reasonId, hasIgnored) {
            return disciplineSummaryFactory.getDisciplineDetail(startDate, endDate, employeeId, departmentId, isLocal, reasonId, hasIgnored)
            .success(function (employeeDisciplines) {
                model.employeeDisciplines = employeeDisciplines;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getDisciplineSummary(startDate, endDate, departmentId, isLocal, employeeId, reasonId, isShowAll) {
            return disciplineSummaryFactory.getDisciplineSummary(startDate, endDate, departmentId, isLocal, employeeId, reasonId, isShowAll)
            .success(function (employeeDisciplines) {
                model.employeeDisciplines = employeeDisciplines;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getDisciplineSummaryByPeriod(periodId, disciplineTypeId, departmentId, isLocal, employeeId) {
            return disciplineSummaryFactory.getDisciplineSummaryByPeriod(periodId, disciplineTypeId, departmentId, isLocal, employeeId)
            .success(function (employeeDisciplines) {
                model.employeeDisciplines = employeeDisciplines;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getDisciplineType(includeOtherViolate, isActive) {
            return disciplineSummaryFactory.getDisciplineType(includeOtherViolate, isActive).success(function (disciplineTypes) {
                model.disciplineTypes = disciplineTypes;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function getOtherViolations(employeeId, startDate, endDate, workDate) {
            return disciplineSummaryFactory.getOtherViolations(employeeId, startDate, endDate, workDate).success(function (violations) {
                model.otherViolations = violations;
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

        function getUserPermissionOnDisciplineSummary() {
            return disciplineSummaryFactory.getUserPermissionOnDisciplineSummary().success(function (permission) {
                model.permission = permission;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        }

        function openPopupOtherViolation(vm, employee, startDate, endDate) {
            vm.modalInstance = $modal.open({
                templateUrl: 'SourceApp/Components/DisciplineSummary/Templates/otherViolation.html',
                controller: "OtherViolationsController as violationCtrl",
                resolve: {
                    employee: function () {
                        return employee;
                    },
                    startDate: function () {
                        return startDate;
                    },
                    endDate: function () {
                        return endDate;
                    }
                },
                backdrop: 'static',
                windowClass: "modal-confirm-center period-create-popup"
            });

            return vm.modalInstance.result;
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
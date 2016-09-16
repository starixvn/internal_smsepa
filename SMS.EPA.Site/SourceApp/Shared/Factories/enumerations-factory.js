(function (angular) {
    'use strict';

    angular.module('appModule').factory('enumerationsFactory', enumerationsFactory);
    enumerationsFactory.$inject = [];

    function enumerationsFactory() {
        var enumerations = {};

        enumerations.policy = {
            None: 0,
            Read: 1,
            Host: 2
        };

        enumerations.epaStatus = {
            Open: 0,
            Closed: 1,
            Rejected: 2
        };

        enumerations.processType = {
            HumanResource: 1,
            Self: 2,
            Evaluator: 3,
            Reviewer: 4,
            Management: 5
        };

        enumerations.targetType = {
            Probation: 1,
            Monthly: 2,
            SemiAnnual: 3,
            Yearly: 4,
            Promotion: 5
        };

        enumerations.processStatusType = {
            Open: 1,
            InProgress: 2,
            Closed: 3
        };

        enumerations.statusEndProcess = {
            InProgress: 0,
            EndProcess: 1,
            Open: 2
        };

        enumerations.ratingType = {
            Star: 1,
            Score: 2,
            HalfStar: 3
        };

        return enumerations;
    }
})(angular);
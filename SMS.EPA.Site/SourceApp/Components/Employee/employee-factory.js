'use strict';
/*global employeeModule*/

employeeModule.factory('employeeFactory', ['$http', function ($http) {
    return {
        getAllEmployee: function (query, groupId) {
            return $http.get('/api/Employee/GetAllEmployee', { params: { query: query, groupId: groupId} });
        },
        getAllGroup: function () {
            return $http.get('/api/Employee/GetAllGroup');
        },
        insertSpecialEmployee: function (employee) {
            return $http.post('/api/Employee/InsertSpecialEmployee', employee);
        },
        updateSpecialEmployee: function (employee) {
            return $http.put('/api/Employee/UpdateSpecialEmployee', employee);
        }
    };
} ]);
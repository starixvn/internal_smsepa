'use strict';
/*global employeeModule*/

employeeModule.factory('employeeFactory', ['$http', function ($http) {
    return {
        getAllEmployee: function (query, groupId, isSpecialUser) {
            return $http.get('/api/Employee/GetAllEmployee',
                { params: { query: query, groupId: groupId, isSpecialUser: isSpecialUser} });
        },
        getAllGroup: function () {
            return $http.get('/api/Employee/GetAllGroup');
        },
        insertSpecialEmployee: function (employee) {
            return $http.post('/api/Employee/InsertSpecialEmployee', employee);
        },
        updateSpecialEmployee: function (employee) {
            return $http.put('/api/Employee/UpdateSpecialEmployee', employee);
        },
        getAllJobTitle: function () {
            return $http.get('/api/Employee/GetAllJobTitle');
        },
        changePassword: function (employee) {
            return $http.put('/api/Employee/ChangePassword', employee);
        }
    };
} ]);
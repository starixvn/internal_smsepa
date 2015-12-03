'use strict';
/*global appModule*/

appModule.factory('utilitiesFactory',
['$filter', function ($filter) {
    var utilities = {};
    var limitNumber = 100,
        dateTimeFormatString = "MM/DD/YYYY HH:mm",
        dateFormat = "dd MMM, yyyy",
        timeFormat = 'HH:mm';

    utilities.numberEntryList = function (vm) {
        vm.entries = [5, 10, 15, 20, 50, 100, 200];
    };

    utilities.getDateFormat = function (vm) {
        vm.dateFormat = dateFormat;
    };

    utilities.getTimeFormat = function (vm) {
        vm.timeFormat = timeFormat;
    };

    /*
    ** event read more/less text.
    */
    utilities.maxLengthDescription = function (vm) {
        vm.maxLengthDescription = limitNumber;
    };

    utilities.readMore = function (item) {
        item.numLimit = 10000;
        item.showReadMore = true;
    };

    utilities.readLess = function (item) {
        item.numLimit = limitNumber;
        item.showReadMore = false;
    };

    utilities.pageOption = {
        pageIndex: 0,
        itemsPerPage: 50,
        itemsPerPageArray: [50, 100, 200, 500]
    };

    utilities.dateTimeFormatString = function () {
        return dateTimeFormatString;
    };

    /*jshint ignore:start */
    /*moment is third party*/
    utilities.getDateTime = function (dateObject) {
        var resultDate = '';

        if (dateObject) {
            resultDate = moment(dateObject).format(dateTimeFormatString);
        }

        return resultDate;
    };
    /*jshint ignore:end */

    utilities.getInt = function (number) {
        var result = number;

        if (number) {
            result = parseInt(number);
        }

        return result;
    };

    utilities.openPopupStartDate = function ($event, vm) {
        $event.preventDefault();
        $event.stopPropagation();

        vm.calendar.startDateOpened = true;
        vm.calendar.endDateOpened = false;
    };

    utilities.openPopupEndDate = function ($event, vm) {
        $event.preventDefault();
        $event.stopPropagation();

        vm.calendar.endDateOpened = true;
        vm.calendar.startDateOpened = false;
    };

    utilities.searchOnPage = function (vm, originalData) {
        /*
        * Check if item match search text. 
        */
        var searchMatch = function (haystack, needle) {
            if (!needle) {
                return true;
            }

            return haystack.toLowerCase().indexOf(needle.toLowerCase()) !== -1;
        };

        /*
        * Check all item recursion.
        */
        var checkAllItemRecursion = function (item, searchKey) {
            for (var attr in item) {
                var childItem = item[attr];

                if (childItem != null && /^[a-zA-Z0-9]*$/.test(attr) &&
                        attr.toLowerCase().search('id') === -1 && attr.toLowerCase().search('length') === -1 &&
                        attr.toLowerCase().search('index') === -1 && attr.toLowerCase().search('hash') === -1 &&
                        attr.toLowerCase().search('time') === -1 && attr.toLowerCase().search('date') === -1 &&
                        attr.toLowerCase().search('modifiedby') === -1 && attr.toLowerCase().search('createdby') === -1) {

                    if (typeof (childItem) === 'object') {
                        var exist = checkAllItemRecursion(childItem, searchKey);

                        if (exist) {
                            return exist;
                        }
                    }
                    else {
                        if (searchMatch(item[attr].toString(), searchKey)) {
                            return true;
                        }
                    }
                }
            }

            return false;
        };

        vm.search = function () {
            vm.filteredItems = [];

            if (originalData && originalData.length > 0) {
                vm.filteredItems = $filter('filter')(originalData, function (item) {
                    return checkAllItemRecursion(item, vm.searchKey);
                });
            }
        };

        vm.search();
    };


    utilities.formatString = function () {
        var theString = arguments[0];
        for (var i = 1; i < arguments.length; i++) {
            var regEx = new RegExp("\\{" + (i - 1) + "\\}", "gm");
            theString = theString.replace(regEx, arguments[i]);
        }

        return theString;
    };

    utilities.getSelectedData = function (data, propName) {
        var selectedProperties = [],
            allProperties = [];

        angular.forEach(data, function (item) {
            var selectedValue = propName ? item[propName] : item;

            if (item.ticked) {
                selectedProperties.push(selectedValue);
            }

            allProperties.push(selectedValue);
        });

        var result = null;

        if (selectedProperties.length > 0) {
            result = propName ? selectedProperties.join(',') : selectedProperties;
        } else if (allProperties.length > 0) {
            result = propName ? allProperties.join(',') : allProperties;
        }

        return result;
    };

    utilities.parseFloat = function (input) {
        var result = input ? parseFloat(input).toFixed(2) : 0;

        return parseFloat(result);
    };

    utilities.splitNumber = function (number, numberDecimal) {
        number = number ? parseFloat(number) : 0;
        var result = number;

        if (!numberDecimal) {
            numberDecimal = 2;
        }

        if ((number % 1) !== 0) {
            var index = number.toString().indexOf('.'),
                decimal = number.toString().slice(index + 1);

            number = number.toString().slice(0, index) + '.' + decimal.substring(0, numberDecimal);
            result = parseFloat(number);
        }

        return result;
    };

    return utilities;
}]);
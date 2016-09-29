(function (angular) {
    'use strict';
    /* jshint latedef:nofunc */

    angular.module('appModule').factory('utilitiesFactory', utilitiesFactory);
    utilitiesFactory.$inject = ['$filter'];

    function utilitiesFactory($filter) {
        var limitNumber = 100,
            dateFormat = "dd MMM, yyyy",
            timeFormat = 'HH:mm',
            maxStar = 5,
            epaStatus = {
                Open: 0,
                Closed: 1,
                Rejected: 2
            },
            pageOption = {
                pageIndex: 0,
                itemsPerPage: 50,
                itemsPerPageArray: [50, 100, 200, 500]
            },
            policy = {
                None: 0,
                Read: 1,
                Host: 2
            },
            processStatusType = {
                Open: 1,
                InProgress: 2,
                Closed: 3
            },
            processType = {
                HumanResource: 1,
                Self: 2,
                Evaluator: 3,
                Reviewer: 4,
                Management: 5
            },
            ratingType = {
                Star: 1,
                Score: 2,
                HalfStar: 3
            },
            statusEndProcess = {
                InProgress: 0,
                EndProcess: 1,
                Open: 2
            },
            targetType = {
                Probation: 1,
                Monthly: 2,
                SemiAnnual: 3,
                Yearly: 4,
                Promotion: 5
            };

        var utilities = {
            epaStatus: epaStatus,
            formatString: formatString,
            getDateFormat: getDateFormat,
            getDateTime: getDateTime,
            getSelectedData: getSelectedData,
            getTimeFormat: getTimeFormat,
            maxLengthDescription: maxLengthDescription,
            maxStar: maxStar,
            numberEntryList: numberEntryList,
            openPopupEndDate: openPopupEndDate,
            openPopupStartDate: openPopupStartDate,
            pageOption: pageOption,
            policy: policy,
            processStatusType: processStatusType,
            processType: processType,
            ratingType: ratingType,
            readLess: readLess,
            readMore: readMore,
            searchOnPage: searchOnPage,
            setSelectedItem: setSelectedItem,
            splitNumber: splitNumber,
            statusEndProcess: statusEndProcess,
            targetType: targetType
        };

        utilities.parseFloat = function (input) {
            var result = input ? parseFloat(input).toFixed(2) : 0;

            return parseFloat(result);
        };

        return utilities;

        function formatString() {
            var theString = arguments[0];
            for (var i = 1; i < arguments.length; i++) {
                var regEx = new RegExp("\\{" + (i - 1) + "\\}", "gm");
                theString = theString.replace(regEx, arguments[i]);
            }

            return theString;
        }

        function getDateFormat(vm) {
            vm.dateFormat = dateFormat;
        }

        function getDateTime(dateObject) {
            var resultDate = '';

            if (dateObject) {
                resultDate = moment(dateObject).format(dateTimeFormatString);
            }

            return resultDate;
        }

        function getSelectedData(data, propName) {
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
        }

        function getTimeFormat(vm) {
            vm.timeFormat = timeFormat;
        }

        function maxLengthDescription(vm) {
            vm.maxLengthDescription = limitNumber;
        }

        function numberEntryList(vm) {
            vm.entries = [5, 10, 15, 20, 50, 100, 200];
        }

        function openPopupEndDate($event, vm) {
            $event.preventDefault();
            $event.stopPropagation();

            vm.calendar.endDateOpened = true;
            vm.calendar.startDateOpened = false;
        }

        function openPopupStartDate($event, vm) {
            $event.preventDefault();
            $event.stopPropagation();

            vm.calendar.startDateOpened = true;
            vm.calendar.endDateOpened = false;
        }

        function readLess(item) {
            item.numLimit = limitNumber;
            item.showReadMore = false;
        }

        function readMore(item) {
            item.numLimit = 10000;
            item.showReadMore = true;
        }

        function searchOnPage(vm, originalData) {
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
        }

        function setSelectedItem(data, selectedName, selectedItems) {
            if (selectedItems && selectedItems.length > 0) {
                var array = selectedItems.split(',');
                angular.forEach(array, function (id) {
                    id = parseInt(id);
                    angular.forEach(data, function (item) {
                        if (item[selectedName] === id) {
                            item.ticked = true;
                        }
                    });
                });
            }
        }

        function splitNumber(number, numberDecimal) {
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
        }
    }
})(angular);
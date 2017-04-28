namespace SMS.EPA.Core.Utilities
{
    using System;
    using Constants;

    public static class ExcelRecordDataConverter
    {
        public static string NullToEmptyStringConverter(string data)
        {
            return string.IsNullOrEmpty(data) ? string.Empty : data;
        }

        public static string NullabeDateTimeToStringConverter(DateTime? dateTime)
        {
            return dateTime?.ToString(Constants.TimeFormat) ?? string.Empty;
        }

        public static string ZeroValueToEmptyStringConverter(int value)
        {
            return value > 0 ? value.ToString() : string.Empty;
        }

        public static string ValueToOneStringConverter(int value)
        {
            return value > 0 ? "1" : string.Empty;
        }

        //ExcelRecordDataConverter.ZeroValueToEmptyStringConverter(ChangeShift.Value)
    }
}

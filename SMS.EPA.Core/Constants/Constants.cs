// ----------------------------------------------------------------------
// <copyright file="Constants.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/6/2015 11:21:39 AM">Created.</change>
// </history>

namespace SMS.EPA.Core.Constants
{
    /// <summary>
    /// The constants.
    /// </summary>
    public sealed class Constants
    {
        public const string CurrentSessionKey = "SMS.EPA.CurrentSession";

        public const string DefaultPassword = "123456";

        public const string KeyCryptography = "SMSEPA02072015";

        public const string EmployeeFlowSettingReport = "SMS.EPA.Report.EmployeeFlowSettingReport";

        public const string DepartmentFlowSettingReport = "SMS.EPA.Report.DepartmentFlowSettingReport";

        public const string FormatDateTimeEmail = "MMM dd, yyyy HH:mm";

        public const string DateFormat = "dd MMM, yyyy";

        public const string TimeFormant = "HH:mm";

        public const int DepartmentRoot = 1;

        public const int PercentPerStar = 20;

        public const int MaxFinalScore = 100;

        private Constants()
        {
        }
    }
}

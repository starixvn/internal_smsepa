// ----------------------------------------------------------------------
// <copyright file="HSSFCellRangeAddressExtensions.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="11/27/2014 3:27:25 PM">Created.</change>
// </history>

namespace NexFB.Core.Utilities.NPOIExtensions
{
    using NPOI.SS.Util;

    /// <summary>
    /// Cell Range Address extensions.
    /// </summary>
    public static class HSSFCellRangeAddressExtensions
    {
        /// <summary>
        /// Clones cell range address.
        /// </summary>
        /// <param name="range">Current cell range address.</param>
        /// <returns>New cell range address instance.</returns>
        public static CellRangeAddress Clone(this CellRangeAddressBase range)
        {
            var result = new CellRangeAddress(range.FirstRow, range.LastRow, range.FirstColumn, range.LastColumn);
            return result;
        }
    }
}

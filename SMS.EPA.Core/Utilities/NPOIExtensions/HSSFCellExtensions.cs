namespace SMS.EPA.Core.Utilities.NPOIExtensions
{
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;
    using System;

    public static class HSSFCellExtensions
    {
        public static void SetCellValue(this ICell cell, object value)
        {
            if (cell != null)
            {
                InferCellValue(cell, value);
            }
        }

#pragma warning disable S3776 // Cognitive Complexity of methods should not be too high
        private static void InferCellValue(ICell cell, object value)
#pragma warning restore S3776 // Cognitive Complexity of methods should not be too high
        {
            if (value == null)
            {
                cell.SetCellValue(string.Empty);
            }
            else
            {
                var valueString = value.ToString();

                if (TrySetIntCell(cell, valueString))
                {
                    return;
                }

                if (TrySetLongCell(cell, valueString))
                {
                    return;
                }

                if (TrySetDoubleCell(cell, valueString))
                {
                    return;
                }

                if (TrySetDecimalCell(cell, valueString))
                {
                    return;
                }

                if (TrySetBoolCell(cell, valueString))
                {
                    return;
                }

                if (TrySetDateTimeCell(cell, valueString))
                {
                    return;
                }

                var textString = value as IRichTextString;
                if (textString != null)
                {
                    cell.SetCellValue(textString);
                }

                cell.SetCellValue(valueString);
            }
        }

        private static bool TrySetDateTimeCell(ICell cell, string valueString)
        {
            DateTime dateTimeValue;
            if (DateTime.TryParse(valueString, out dateTimeValue))
            {
                cell.SetCellValue(dateTimeValue);
                return true;
            }

            return false;
        }

        private static bool TrySetBoolCell(ICell cell, string valueString)
        {
            bool boolValue;
            if (bool.TryParse(valueString, out boolValue))
            {
                cell.SetCellValue(boolValue);
                return true;
            }

            return false;
        }

        private static bool TrySetDecimalCell(ICell cell, string valueString)
        {
            decimal decimalValue;
            if (decimal.TryParse(valueString, out decimalValue))
            {
                cell.SetCellValue(decimalValue);
                return true;
            }

            return false;
        }

        private static bool TrySetDoubleCell(ICell cell, string valueString)
        {
            double doubleValue;
            if (double.TryParse(valueString, out doubleValue))
            {
                cell.SetCellValue(doubleValue);
                return true;
            }

            return false;
        }

        private static bool TrySetLongCell(ICell cell, string valueString)
        {
            long longValue;
            if (long.TryParse(valueString, out longValue))
            {
                cell.SetCellValue(longValue);
                return true;
            }

            return false;
        }

        private static bool TrySetIntCell(ICell cell, string valueString)
        {
            int intValue;
            if (int.TryParse(valueString, out intValue))
            {
                cell.SetCellValue(intValue);
                return true;
            }

            return false;
        }

        public static void SetCellValue(this ICell cell, decimal value)
        {
            cell?.SetCellValue(Convert.ToDouble(value));
        }

        private static bool IsSourceRangeValid(ICell sourceCell, int index)
        {
            CellRangeAddress range = sourceCell.Sheet.GetMergedRegion(index);
            if ((range.FirstColumn <= sourceCell.ColumnIndex)
                && (range.LastColumn >= sourceCell.ColumnIndex)
                && (range.FirstRow <= sourceCell.RowIndex)
                && (range.LastRow >= sourceCell.RowIndex))
            {
                return true;
            }

            return false;
        }

        public static void CopyFrom(this ICell cell, ICell sourceCell)
        {
            // Copy merged cells
            if (cell != null && sourceCell != null)
            {
                int index = 0;
                while (index < sourceCell.Sheet.NumMergedRegions && !IsSourceRangeValid(sourceCell, index))
                {
                    index++;
                }

                CellRangeAddress range = sourceCell.Sheet.GetMergedRegion(index);
                var sourceRange = range;

                if (sourceRange != null)
                {
                    cell.Sheet.AddMergedRegion(sourceRange.Clone());
                }

                // Copy style
                cell.CellStyle = sourceCell.CellStyle;
            }
        }
    }
}
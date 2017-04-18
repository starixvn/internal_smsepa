namespace SMS.EPA.Model.Report
{
    using System;
    using System.Linq.Expressions;

    public interface IExcelRecord
    {
        object ConvertToExcelRecord();
    }

    public class ConditionalCellStyle
    {
        public int ColumnIndex { get; set; }

        public Func<object, bool> Condition { get; set; }

        public int StyleIndex { get; set; }

        public Type DataType { get; set; }
    }
}
